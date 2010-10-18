using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class Stream:AbstractWaterBody,IWaterBody
  {
    #region Persisted Data

    [DataMember]
    private double _width = 0;
    [DataMember]
    private double _depth = 0;
    
    [DataMember]
    public XYPolyline Line { get; set; }

    [DataMember]
    private Dictionary<string, Tuple<DateTime, Queue<IWaterPacket>>> _states = new Dictionary<string, Tuple<DateTime, Queue<IWaterPacket>>>();

    /// <summary>
    /// Gets and sets the width of the stream
    /// </summary>   
    [DataMember]
    public double Width { get; set; }


    #endregion

    #region Non-persisted Properties



    /// <summary>
    /// Gets and sets the length of the stream
    /// </summary>
    public double Length
    {
      get
      {
        return Line.GetLength();
      }
    }


    /// <summary>
    /// Gets the area of this waterbody
    /// </summary>
    public double Area
    {
      get
      {
        return Width * Length;
      }
    }

    /// <summary>
    /// Gets the volume of the stream
    /// </summary>
    public double Volume
    {
      get
      {
        return Area * Depth;
      }
    }

    /// <summary>
    /// Gets the geometry
    /// </summary>
    public IGeometry Geometry
    {
      get
      {
        return Line;
      }
    }

    /// <summary>
    /// Gets the water that will be routed in the current timestep
    /// This property is only to be used for storage. Do not alter the water.
    /// </summary>
    public IWaterPacket CurrentStoredWater
    {
      get
      {
        WaterWithChemicals Wc = new WaterWithChemicals(0);
        foreach (IWaterPacket wp in _waterInStream)
          Wc.Add(wp);
        return Wc;
      }
    }
    #endregion

    #region Private variables
      [DataMember]
    private Queue<IWaterPacket> _incomingWater = new Queue<IWaterPacket>();
      [DataMember]
    private Queue<IWaterPacket> _waterInStream = new Queue<IWaterPacket>();
      [DataMember]
    private List<Treple<DateTime, DateTime, IWaterPacket>> Incoming = new List<Treple<DateTime, DateTime, IWaterPacket>>();
      [DataMember]
    private TimeSpan CurrentTimeStep;
      [DataMember]
    private DateTime StartofFlowperiod;
      [DataMember]
    private double WaterToRoute;

    #endregion

    #region Constructors


    public Stream(XYPolyline Line, double Width, double Depth):base()
    {
      this.Line = Line;
      this.Width = Width;
      this.Depth = Depth;

      //The volume of the stream cannot change
      Output.StoredVolume.AddSiValue(DateTime.MinValue, Volume);
      Output.StoredVolume.AddSiValue(DateTime.MaxValue, Volume);
    }

    public Stream(double Length, double Width, double Depth)
      : base()
    {
      Line = new XYPolyline();
      Line.Points.Add(new XYPoint(0, 0));
      Line.Points.Add(new XYPoint(Length, 0));
      this.Width = Width;
      this.Depth = Depth;

      //The volume of the stream cannot change
      Output.StoredVolume.AddSiValue(DateTime.MinValue, Volume);
      Output.StoredVolume.AddSiValue(DateTime.MaxValue, Volume);

    }


    #endregion


    /// <summary>
    /// Sets the state. Also stores the state. I
    /// </summary>
    /// <param name="StateName"></param>
    /// <param name="Time"></param>
    /// <param name="WaterInStream"></param>
    public void SetState(string StateName, DateTime Time, IWaterPacket WaterInStream)
    {
      Queue<IWaterPacket> _water = new Queue<IWaterPacket>();
      _water.Enqueue(WaterInStream.DeepClone());

      Tuple<DateTime, Queue<IWaterPacket>> state = new Tuple<DateTime, Queue<IWaterPacket>>(Time, _water);

      if (_states.ContainsKey(StateName))
        _states[StateName] = state;
      else
        _states.Add(StateName, state);
      
      RestoreState(StateName);
    }

    /// <summary>
    /// Saves the current state for future use
    /// </summary>
    /// <param name="StateName"></param>
    public void KeepCurrentState(string StateName)
    {
      Queue<IWaterPacket> _water = new Queue<IWaterPacket>();

      foreach (IWaterPacket iw in _waterInStream)
        _water.Enqueue(iw.DeepClone());
      
      Tuple<DateTime, Queue<IWaterPacket>> state = new Tuple<DateTime, Queue<IWaterPacket>>(CurrentTime, _water);

      _states.Add(StateName, state);
    }

    /// <summary>
    /// Restores to the state stored under the StateName
    /// </summary>
    /// <param name="StateName"></param>
    public void RestoreState(string StateName)
    {
      _waterInStream.Clear();
      
      foreach (IWaterPacket iw in _states[StateName].Second)
        _waterInStream.Enqueue(iw.DeepClone());

      CurrentTime = _states[StateName].First;
      StartofFlowperiod = CurrentTime;
      ResetToTime(CurrentTime);
    }


    /// <summary>
    /// This is the timestepping procedure
    /// </summary>
    /// <param name="TimeStep"></param>
    public void Update(DateTime NewTime)
    {
      CurrentTimeStep = NewTime.Subtract(CurrentTime);
      #region Sum of Sinks and sources

      //Sum the sources
      var GWFlow = GroundwaterBoundaries.Where(var => var.IsSource(CurrentTime)).Select(var => var.GetSourceWater(CurrentTime, CurrentTimeStep));
      var SourceFlow = Sources.Select(var => var.GetSourceWater(CurrentTime, CurrentTimeStep));
      IWaterPacket InFlow = WaterMixer.Mix(GWFlow.Concat(SourceFlow));
      double InflowVolume = 0;
      if (InFlow != null)
        InflowVolume = InFlow.Volume;

      //Sum the Evaporation boundaries
      double EvapoVolume = _evapoBoundaries.Sum(var => var.GetSinkVolume(CurrentTime, CurrentTimeStep));

      //Sum the sinks
      double SinkVolume = Sinks.Sum(var => var.GetSinkVolume(CurrentTime, CurrentTimeStep));
      //Add the sinking groundwater boundaries
      SinkVolume += GroundwaterBoundaries.Where(var => !var.IsSource(CurrentTime)).Sum(var => var.GetSinkVolume(CurrentTime, CurrentTimeStep));
      double sumSinkSources = InflowVolume - EvapoVolume - SinkVolume;

      //If we have no water from upstream but Inflow, remove water from inflow to fill stream
      if (sumSinkSources / Volume > 5)
      {
        AddWaterPacket(CurrentTime, NewTime, InFlow.Substract(sumSinkSources - Volume * 5));
        InflowVolume = InFlow.Volume;
        sumSinkSources = InflowVolume - EvapoVolume - SinkVolume;
      }

      //Sort the incoming water an put in to queue
      PrePareIncomingWater();

      //Calculate the surplus  
      WaterToRoute = _waterInStream.Sum(var => var.Volume) + InflowVolume - EvapoVolume - SinkVolume - Volume + _incomingWater.Sum(var => var.Volume);

      //If the loss is bigger than available water, reduce Evaporation and Sinks
      if (WaterToRoute + Volume < 0)
      {
        double reductionfactor = 1 + (WaterToRoute + Volume) / (EvapoVolume + SinkVolume);
        EvapoVolume *= reductionfactor;
        SinkVolume *= reductionfactor;
        WaterToRoute = 0;
      }

      //Convert to rates
      double qu = sumSinkSources / CurrentTimeStep.TotalSeconds / Volume;
      double qop = _incomingWater.Sum(var => var.Volume) / CurrentTimeStep.TotalSeconds;
      double qout = qu * Volume + qop;

      //Create a mixer class
      Mixer M = new Mixer(InFlow, EvapoVolume, SinkVolume);

      #endregion

      #region Stored water
      //Send stored water out
      if (WaterToRoute > 0)
      {
        double OutflowTime = 0;

        //The volume that needs to flow out to meet the watertotroute
        double VToSend = WaterToRoute;
        if (qu != 0)
        {
          VToSend = qout / qu * (1 - 1 / (Math.Exp(qu * CurrentTimeStep.TotalSeconds)));
        }
        //There is water in the stream that should be routed
        while (VToSend > 0 & _waterInStream.Count > 0)
        {
          IWaterPacket IW;
          //Mixing during flow towards end of stream
          double dv = _waterInStream.Peek().Volume * (Math.Exp(qu * OutflowTime) - 1);

          //Check if the entire water packet should flow out or it should be split
          if (_waterInStream.Peek().Volume + dv < VToSend)
            IW = _waterInStream.Dequeue();
          else
            IW = _waterInStream.Peek().Substract(VToSend);

          //Update how mush water is yet to be routed
          VToSend -= IW.Volume;
          //Now do the mix
          M.Mix(dv, IW);

          //Calculate outflow time
          double dt;
          if (qu == 0)
            dt = IW.Volume / qop;
          else
          {
            dt = Math.Log(qout / (qout - qu * IW.Volume)) / qu;
          }
          //Mixing during outflow
          M.Mix(qout * dt - IW.Volume, IW);

          IW.MoveInTime(TimeSpan.FromSeconds(OutflowTime + dt / 2));
          SendWaterDownstream(IW);
          OutflowTime += dt;
        }
      }

      //Now move the remaining packets to their final destination and time
      foreach (IWaterPacket IWP in _waterInStream)
      {
        if (qu != 0)
          M.Mix(IWP.Volume * (Math.Exp(qu * CurrentTimeStep.TotalSeconds) - 1), IWP);
        IWP.MoveInTime(CurrentTimeStep);
      }
      #endregion

      #region Water packets traveling right through
      double inflowtime = 0;

      //No water in stream and incoming water. Just pass through
      if (_waterInStream.Count == 0 & _incomingWater.Count > 0)
      {
        //Calculate how much incoming water is required to fill stream volume; 
        double VToStore = Volume;
        if (qu != 0)
          VToStore = qop / qu * Math.Log(Volume * qu / qop + 1);

        //Now send water through
        double incomingVolume = _incomingWater.Sum(var => var.Volume);
        //Send packets through until the remaining will just fill the stream
        while (incomingVolume > VToStore + 1e-12 & _incomingWater.Count != 0)
        {
          IWaterPacket WP;
          if (incomingVolume - _incomingWater.Peek().Volume > VToStore)
            WP = _incomingWater.Dequeue();
          else
            WP = _incomingWater.Peek().Substract(incomingVolume - VToStore);

          incomingVolume -= WP.Volume;

          //Keep track of inflow time.
          inflowtime += WP.Volume / qop;
          if (qu != 0)
          {
            double dvr = WP.Volume * qu * Volume / qop;
            M.Mix(dvr, WP);
          }

          //Calculate the time a water package uses to travel through the stream
          TimeSpan CurrentTravelTime;
          if (qu != 0)
            CurrentTravelTime = TimeSpan.FromSeconds(1 / qu * Math.Log(Volume * qu / qop + 1));
          else
            CurrentTravelTime = TimeSpan.FromSeconds(Volume / qout);

          //Moves right through
          WP.MoveInTime(CurrentTravelTime);
          SendWaterDownstream(WP);
        }
      }

      #endregion

      #region Fill the stream 
      //The remaining incoming water is moved forward and stays in the stream.
      while (_incomingWater.Count > 0)
      {
        IWaterPacket WP = _incomingWater.Dequeue();

        double dt = WP.Volume / qop;
        inflowtime += dt;
        double dt2 = CurrentTimeStep.TotalSeconds - inflowtime; //How much of the timestep is left when this packet has moved in.

        if (qu != 0)
        {
          //Volume change during inflow
          double Vnew = qop * (Math.Exp(qu * dt) - 1) / qu;
          //Volume change while in stream
          Vnew *= Math.Exp(qu * dt2);
          M.Mix(Vnew - WP.Volume, WP);
        }
        //The time is half the inflowtime + the time in stream
        WP.MoveInTime(TimeSpan.FromSeconds(dt2 + dt / 2));
        _waterInStream.Enqueue(WP);
      }
      #endregion

      Output.Outflow.AddSiValue(CurrentTime, NewTime, WaterToRoute / CurrentTimeStep.TotalSeconds);
      CurrentTime =NewTime;
    }

    /// <summary>
    /// Adds a water packet to the stream. 
    /// This method is to be used by upstream connections.
    /// The time period is used sort the incoming packets when the stream has multiple upstream connections
    /// </summary>
    /// <param name="Start">Start of inflow period</param>
    /// <param name="End">End of inflow period</param>
    /// <param name="Water"></param>
    public void AddWaterPacket(DateTime Start, DateTime End, IWaterPacket Water)
    {
      Water.Tag(ID);
      if (Water.Volume !=0)
        Incoming.Add(new Treple<DateTime, DateTime, IWaterPacket>(Start, End, Water));
    }

    #region Private methods
    /// <summary>
    /// Distributes water on the downstream connections. 
    /// Should be called chronologically!
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="water"></param>
    private void SendWaterDownstream(IWaterPacket water)
    {
      if (water.Volume != 0)
      {
        DateTime EndOfFlow = StartofFlowperiod.AddSeconds(water.Volume / WaterToRoute * CurrentTimeStep.TotalSeconds);
        SendWaterDownstream(water, StartofFlowperiod, EndOfFlow);
        //Call the logger
        Output.Log(water, StartofFlowperiod, EndOfFlow);
        StartofFlowperiod = EndOfFlow;
      }
    }

    /// <summary>
    /// Mixes the incoming water and sorts it in queue according to time
    /// </summary>
    private void PrePareIncomingWater()
    {
      double ControlVolume = Incoming.Sum(var => var.Third.Volume);
      Queue<DateTime> Starts = new Queue<DateTime>();
      Queue<DateTime> Ends = new Queue<DateTime>();

      List<Tuple<DateTime, DateTime>> TimeSpans = new List<Tuple<DateTime, DateTime>>();

      foreach (DateTime D in Incoming.Select(var => var.First).OrderBy(var => var).Distinct())
        Starts.Enqueue(D);

      foreach (DateTime D in Incoming.Select(var => var.Second).OrderBy(var => var).Distinct())
        Ends.Enqueue(D);

      while (Starts.Count > 0 & Ends.Count>0)
      {
        DateTime d = Starts.Dequeue();

        if (Starts.Count > 0)
        {
          if (Starts.Peek() < Ends.Peek())
            TimeSpans.Add(new Tuple<DateTime, DateTime>(d, Starts.Peek()));
          else
          {
            DateTime e = Ends.Dequeue();
            TimeSpans.Add(new Tuple<DateTime, DateTime>(d, e));
            Starts.Enqueue(e);
            var p = Starts.OrderBy(var => var).ToList();
            Starts.Clear();
            foreach (DateTime D in p)
              Starts.Enqueue(D);
          }
        }
        else
        {
          DateTime e = Ends.Dequeue();
          TimeSpans.Add(new Tuple<DateTime, DateTime>(d, e));
          Starts.Enqueue(e);
        }
      }

      //Store the volumes before substracting anything
      Dictionary<IWaterPacket,double> _vols = new Dictionary<IWaterPacket,double>();
      foreach(var i in Incoming)
      {
        _vols.Add(i.Third,i.Third.Volume);
      }

      foreach (var v in TimeSpans)
      {
        var l = Incoming.Where(var => var.First < v.Second & var.Second > v.First).ToList();

        if(l.Count>0)
        {
          double d = v.Second.Subtract(v.First).TotalSeconds;
          IWaterPacket wp = l[0].Third.Substract(d /(l[0].Second.Subtract(l[0].First).TotalSeconds) * _vols[l[0].Third]);
          for (int i = 1; i < l.Count; i++)
          {
            wp.Add(l[i].Third.Substract( d /(l[i].Second.Subtract(l[i].First).TotalSeconds) * _vols[l[i].Third]));
          }
          _incomingWater.Enqueue(wp);
        }
      }

      //Check the mass balance
      if (_incomingWater.Sum(var=> var.Volume) - ControlVolume > 1E-4)
        throw new Exception("Error in algorithm to mix incoming water");

      Incoming.Clear();
    }
    #endregion


    /// <summary>
    /// Small internal class that handles the mixing between boundaries and water packets
    /// </summary>
    private class Mixer
    {
      private IWaterPacket _inflow;
      private bool _anythingToMix = false;
      private double _evapoVolume;
      private double _sinkVolume;
      private double _inflowVolume;
      private double _totalVolume;

      internal Mixer(IWaterPacket Inflow, double EvapoVolume, double SinkVolume)
      {
        _evapoVolume = EvapoVolume;
        _sinkVolume = SinkVolume;
        _inflow = Inflow;
        double totalVolume;
        if (Inflow != null)
        {
          totalVolume = Inflow.Volume - EvapoVolume - SinkVolume;
          _inflowVolume = Inflow.Volume;
        }
        else
          totalVolume = -EvapoVolume - SinkVolume;

        if (totalVolume != 0)
        {
          _anythingToMix = true;
        }

        _totalVolume = totalVolume;
      }

      public void Mix(double Volume, IWaterPacket WP)
      {
        if (_anythingToMix)
        {
          double d = Volume / _totalVolume;

          if (_inflow != null)
            WP.Add(_inflow.Substract(d*_inflowVolume));
          WP.Evaporate(d*_evapoVolume );
          WP.Substract(d*_sinkVolume);
        }
      }
    }
  }
}
