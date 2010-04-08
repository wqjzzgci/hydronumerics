using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using SharpMap.Geometries;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class Stream:AbstractWaterBody,IWaterBody  
  {
    private Queue<IWaterPacket> _incomingWater = new Queue<IWaterPacket>();
    private Queue<IWaterPacket> _waterInStream = new Queue<IWaterPacket>();
    private List<Treple<DateTime, DateTime, IWaterPacket>> Incoming = new List<Treple<DateTime, DateTime, IWaterPacket>>();

    private TimeSpan CurrentTimeStep;
    private DateTime StartofFlowperiod;
    
    private double WaterToRoute;

    [DataMember]
    public double Width { get; set; }
    [DataMember]
    public double Depth { get; set; }
    public LineString Line { get; set; }

    #region Constructors


    /// <summary>
    /// Use this constructor to create a WaterBody with a volume. The volume will correspond to the volume of the initialwater
    /// </summary>
    /// <param name="InitialWater"></param>
    public Stream(IWaterPacket InitialWater)
      : base(InitialWater)
    {
      _waterInStream.Enqueue(InitialWater);
    }

    public Stream(double Length, double Width, double Depth):base(Length * Width * Depth)
    {
      Volume = Length * Width * Depth;
      this.Width = Width;
      this.Depth = Depth;
      Line = new LineString();
      Line.Vertices.Add(new Point(0, 0));
      Line.Vertices.Add(new Point(Length, 0));
    }

    #endregion

    /// <summary>
    /// Gets the Geometry of this waterbody
    /// </summary>
    public IGeometry Geometry
    {
      get
      {
        return Line;
      }
    }

    /// <summary>
    /// Gets the area of this waterbody
    /// </summary>
    public double Area
    {
      get
      {
        return Width * Line.Length;
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

    /// <summary>
    /// This is the timestepping procedure
    /// </summary>
    /// <param name="TimeStep"></param>
    public void MoveInTime(TimeSpan TimeStep)
    {
      CurrentTimeStep = TimeStep;
      #region Sum of Sinks and sources

      //Sum the sources
      IWaterPacket InFlow = WaterMixer.Mix(Sources.Select(var => var.GetSourceWater(CurrentStartTime, TimeStep)));
      double InflowVolume = 0;
      if (InFlow != null)
        InflowVolume = InFlow.Volume;

      //Sum the Evaporation boundaries
      double EvapoVolume = EvapoBoundaries.Sum(var => var.GetEvaporationVolume(CurrentStartTime, TimeStep));

      //Sum the sinks
      double SinkVolume = Sinks.Sum(var => var.GetSinkVolume(CurrentStartTime, TimeStep));
      double sumSinkSources = InflowVolume - EvapoVolume - SinkVolume;

      //If we have no water from upstream but Inflow, remove water from inflow to fill stream
      if (sumSinkSources / Volume > 5)
      {
        ReceiveWater(CurrentStartTime, CurrentStartTime.Add(TimeStep), InFlow.Substract(sumSinkSources - Volume * 5));
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
      double qu = sumSinkSources / TimeStep.TotalSeconds / Volume;
      double qop = _incomingWater.Sum(var => var.Volume) / TimeStep.TotalSeconds;
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
          VToSend = qout / qu * (1 - 1 / (Math.Exp(qu * TimeStep.TotalSeconds)));
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
          M.Mix(IWP.Volume * (Math.Exp(qu * TimeStep.TotalSeconds) - 1), IWP);
        IWP.MoveInTime(TimeStep);
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
        double dt2 = TimeStep.TotalSeconds - inflowtime; //How much of the timestep is left when this packet has moved in.

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

      Output.Outflow.AddTimeValueRecord(new TimeValue(CurrentStartTime, WaterToRoute / TimeStep.TotalSeconds));
      CurrentStartTime += TimeStep;
    }

    public void Reset()
    {
      _waterInStream.Clear();
      _waterInStream.Enqueue(InitialWater.DeepClone(1));
    }


    /// <summary>
    /// Receives water and adds it to the storage. 
    /// This method is to be used by upstream connections.
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Water"></param>
    public void ReceiveWater(DateTime Start, DateTime End, IWaterPacket Water)
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
        if (DownStreamConnections.Count > 0)
          DownStreamConnections.First().ReceiveWater(StartofFlowperiod, EndOfFlow, water);
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
