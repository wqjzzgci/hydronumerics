using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  public class MixingStream:Stream
  {

    private List<IWaterBody> _wbs = new List<IWaterBody>();


    public MixingStream(string name, XYPolyline length,double width, double depth, int mixfactor):base(name, length,width,depth)
    {

      for (int i = 0; i < mixfactor; i++)
      {
        _wbs.Add(new Stream(name + i.ToString(),Length/mixfactor,Width,Depth)); 
      }
    }


    #region IWaterBody Members


    public double Area
    {
      get { throw new NotImplementedException(); }
    }

    public double Volume
    {
      get { throw new NotImplementedException(); }
    }

    public IGeometry Geometry
    {
      get { throw new NotImplementedException(); }
    }

    public void MoveInTime(TimeSpan TimeStep)
    {
      throw new NotImplementedException();
    }

    public void ReceiveWater(DateTime Start, DateTime End, IWaterPacket Water)
    {
      throw new NotImplementedException();
    }

    public IWaterPacket CurrentStoredWater
    {
      get { throw new NotImplementedException(); }
    }

    public void SetState(string StateName, DateTime Time, IWaterPacket WaterInStream)
    {
      throw new NotImplementedException();
    }

    public void KeepCurrentState(string StateName)
    {
      throw new NotImplementedException();
    }

    public void RestoreState(string StateName)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}
