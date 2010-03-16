using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpMap.Geometries;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public class Stream:BaseWaterBody 
  {

    private Queue<IWaterPacket> _incomingWater = new Queue<IWaterPacket>();
    private Queue<IWaterPacket> _waterInStream = new Queue<IWaterPacket>();
    private TimeSpan CurrentTravelTime;
    

    #region Constructors


    /// <summary>
    /// Use this constructor to create a WaterBody with a volume. The volume will correspond to the volume of the initialwater
    /// </summary>
    /// <param name="InitialWater"></param>
    public Stream(IWaterPacket InitialWater):base(InitialWater.Volume )
    {
      _waterInStream.Enqueue(InitialWater);


    }
    #endregion


    /// <summary>
    /// Gets the water that will be routed in the current timestep
    /// This property is only to be used for storage. Do not alter the water.
    /// </summary>
    public IWaterPacket CurrentStoredWater
    {
      get
      {
        return _waterInStream.Peek();
      }
    }

    public override void MoveInTime(TimeSpan TimeStep)
    {
      CurrentStartTime += TimeStep;

      #region Sum of Sinks and sources

      //Sum the sources
      IWaterPacket InFlow = WaterMixer.Mix(Sources.Select(var => var.GetSourceWater(CurrentStartTime, TimeStep)));
      double InflowVolume = 0;
      if (InFlow!=null)
        InflowVolume = InFlow.Volume; 

      //Sum the Evaporation boundaries
      double EvapoVolume = EvapoBoundaries.Sum(var => var.GetEvaporationVolume(CurrentStartTime, TimeStep));
      
      //Sum the sinks
      double SinkVolume = Sinks.Sum(var => var.GetSinkVolume(CurrentStartTime, TimeStep));
      
      //Calculate the surplus  
      double WaterToRoute = _waterInStream.Sum(var => var.Volume) + InflowVolume - EvapoVolume - SinkVolume - _volume + _incomingWater.Sum(var => var.Volume);
      
      //If the loss is bigger than available water, reduce Evaporation and Sinks
      if (WaterToRoute + _volume < 0)
      {
        double reductionfactor = 1 + (WaterToRoute + _volume) / (EvapoVolume + SinkVolume);
        EvapoVolume *= reductionfactor;
        SinkVolume *= reductionfactor;
        WaterToRoute = 0;
      }

      //Convert to rates
      double qin = (InflowVolume - EvapoVolume - SinkVolume) / TimeStep.TotalSeconds;
      double qu = (InflowVolume - EvapoVolume - SinkVolume) / TimeStep.TotalSeconds/_volume;
      double qop = _incomingWater.Sum(var => var.Volume) / TimeStep.TotalSeconds;
      double qout = qin + qop;

      //Create a mixer class
      Mixer M = new Mixer(InFlow, EvapoVolume, SinkVolume);

      #endregion

      double t = 0;

      //Calculate the time a water package uses to travel through the stream
      if (qin != 0 & qop!=0)
        CurrentTravelTime = TimeSpan.FromSeconds(_volume / qin * Math.Log(qin / qop + 1));
      else if (qop != 0 | qin!=0)
        CurrentTravelTime = TimeSpan.FromSeconds(_volume / (qop+qin));
      else
        CurrentTravelTime = TimeStep;

      #region Stored water
      //Send stored water out
      if (WaterToRoute > 0)
      {
        //The volume that needs to flow out to meet the watertotroute
        double VToSend = WaterToRoute;
        if (qin != 0)
        {
          VToSend = _volume -qout / (Math.Exp(qu * TimeStep.TotalSeconds));
                  VToSend = _volume * qout / qin * (1 - 1 / Math.Exp(WaterToRoute * qin / (_volume * qout)));
        }
        //There is water in the stream that should be routed
        while (VToSend > 0 & _waterInStream.Count > 0)
        {
          IWaterPacket IW;
          //Mixing during flow towards end of stream
          double dv = _waterInStream.Peek().Volume * (Math.Exp(qin * t / _volume) - 1);
          
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
          if (qin == 0)
            dt = IW.Volume / qop;
          else
            dt = _volume / qin * Math.Log(1 / (1 - IW.Volume * qin / (_volume * qout)));

          //Mixing during outflow
          M.Mix(qout * dt - IW.Volume, IW);

          SendWaterDownstream(TimeSpan.FromSeconds(t + dt / 2), IW);
          t += dt;
        }
      }

      double vol= _waterInStream.Sum(var=>var.Volume);
      //Now move the remaining packets to their final destination and time
      foreach (IWaterPacket IWP in _waterInStream)
      {

        if (qin > 0 |qop > 0)
          M.Mix(IWP.Volume * (Math.Exp(qin * TimeStep.TotalSeconds / _volume) - 1), IWP);
        else
          M.Mix(IWP.Volume / vol * qin * TimeStep.TotalSeconds, IWP);
        IWP.MoveInTime(TimeStep);
      }
      #endregion

      #region Incoming Water
      if (_incomingWater.Count > 0)
      {
        t = 0;
        //Calculate how much incoming water is required to fill stream volume; 
        double VToStore = _volume - _waterInStream.Sum(var => var.Volume);
        if (qin != 0)
          VToStore = qop/qu* Math.Log(VToStore * qu / qop + 1);

        //Now send water through
        double incomingVolume = _incomingWater.Sum(var => var.Volume);
        //Send packets through until the remaining will just fill the stream
        while (incomingVolume > VToStore & _incomingWater.Count != 0)
        {
          IWaterPacket WP;
          if (incomingVolume - _incomingWater.Peek().Volume > VToStore)
            WP = _incomingWater.Dequeue();
          else
            WP = _incomingWater.Peek().Substract(incomingVolume - VToStore);

          incomingVolume -= WP.Volume;

          //Keep track of time. No use yet
          t += WP.Volume / qop;
          if (qin != 0)
          {
            double dvr = WP.Volume * qin / qop;
            M.Mix(dvr, WP);
          }
          SendWaterDownstream(CurrentTravelTime, WP);
        }

        while (_incomingWater.Count > 0)
        {
          IWaterPacket WP = _incomingWater.Dequeue();

          double dt = WP.Volume / qop;
          t += dt;
          double dt2 = TimeStep.TotalSeconds - t;

          if (qin != 0)
          {
            double Vnew = qop * _volume / qin * (Math.Exp(qin * dt / _volume) - 1);
            Vnew *= Math.Exp(qin * dt2 / _volume);
            M.Mix(Vnew - WP.Volume, WP);
          }

          WP.MoveInTime(TimeSpan.FromSeconds(dt2 + dt / 2));
          _waterInStream.Enqueue(WP);
        }
      }
      #endregion

      #region Only boundaries
      else if (_waterInStream.Count==0 &InFlow != null)
      {
          InFlow.Evaporate(EvapoVolume);
          InFlow.Substract(SinkVolume);
          SendWaterDownstream(CurrentTravelTime, InFlow.Substract(WaterToRoute));
          InFlow.MoveInTime(CurrentTravelTime);
          _waterInStream.Enqueue(InFlow);
      }

      #endregion

      Output.TimeSeriesList[0].AddTimeValueRecord(new TimeValue(CurrentStartTime, VolumeSend));
      VolumeSend = 0;
    }

    private double VolumeSend = 0;
    /// <summary>
    /// Distributes water on the downstream connections
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="water"></param>
    private void SendWaterDownstream(TimeSpan TimeStep, IWaterPacket water)
    {
      VolumeSend += water.Volume;
      if (water.Volume != 0)
      {
        water.MoveInTime(TimeStep);
        if (DownStreamConnections.Count > 0)
          DownStreamConnections.First().ReceiveWater(TimeStep, water);
      }
    }

    /// <summary>
    /// Receives water and adds it to the storage. 
    /// This method is to be used by upstream connections.
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Water"></param>
    public override void ReceiveWater(TimeSpan TimeStep, IWaterPacket Water)
    {
      if (Water.Volume !=0)
        _incomingWater.Enqueue(Water);
    }




    /// <summary>
    /// Small internal class that handles the mixing between boundaries and water packets
    /// </summary>
    private class Mixer
    {
      private IWaterPacket _inflow;
      private double _inFlowFactor;
      private double _evapoFactor;
      private double _sinkFactor;
      private bool _anythingToMix = false;

      internal Mixer(IWaterPacket Inflow, double EvapoVolume, double SinkVolume)
      {
        _inflow = Inflow;
        double totalVolume;
        if (Inflow != null)
        {
          totalVolume = Inflow.Volume - EvapoVolume - SinkVolume;
          _inFlowFactor = Inflow.Volume / totalVolume;
        }
        else
          totalVolume = -EvapoVolume - SinkVolume;

        if (totalVolume != 0)
        {
          _evapoFactor = EvapoVolume / totalVolume;
          _sinkFactor = SinkVolume / totalVolume;
          _anythingToMix = true;
        }
      }

      public void Mix(double Volume, IWaterPacket WP)
      {
        if (_anythingToMix)
        {
          if (_inflow != null)
            WP.Add(_inflow.Substract(Volume * _inFlowFactor));
          WP.Evaporate(Volume * _evapoFactor);
          WP.Substract(Volume * _sinkFactor);
        }
      }
    }


  }
}
