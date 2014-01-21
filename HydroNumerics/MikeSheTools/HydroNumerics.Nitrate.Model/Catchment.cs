using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time2;
using HydroNumerics.Core;
using HydroNumerics.Geometry;

using DotSpatial.Data;

namespace HydroNumerics.Nitrate.Model
{
  public class Catchment:BaseViewModel
  {


    public TimeSpanSeries GWInput { get; set; }

    private TimeSpanSeries _GWFlow;
    public TimeSpanSeries GWFlow
    {
      get { return _GWFlow; }
      set
      {
        if (_GWFlow != value)
        {
          _GWFlow = value;
          NotifyPropertyChanged("GWFlow");
        }
      }
    }



    public Catchment(int ID)
    {
      ID15 = ID;
      UpstreamConnections = new List<Catchment>();
      Particles = new List<Particle>();
      GWInput = new TimeSpanSeries();
    }

    public int ID15 { get; private set; }

    private Catchment downstreamConnection;

    public Catchment DownstreamConnection
    {
      get
      { return downstreamConnection;}
      set
      {
        if (value != downstreamConnection)
        {
          downstreamConnection = value;
          NotifyPropertyChanged("DownstreamConnection");
        }
      }
    }

    public List<Catchment> UpstreamConnections{get;private set;}

    public List<Particle> Particles { get; set; }


    public XYPolygon Geometry { get; set; }

    public void MoveInTime(DateTime Endtime)
    {

      double input = 0;
      foreach (var ups in UpstreamConnections)
        input += ups.GetOutput(Endtime);


      //Get groundwater from particles

      //Get atmospheric

      //Get point sources

      //Run reduction models

      output = input;

      CurrentTime = Endtime;
    }


    private double output;

    public double GetOutput(DateTime EndTime)
    {

      MoveInTime(EndTime);
      return output;
    }

    public DateTime CurrentTime { get; private set; }



    public override bool Equals(object obj)
    {
      if (!(obj is Catchment))
        return false;
      return ID15.Equals(((Catchment)obj).ID15);
    }

    public override int GetHashCode()
    {
      return ID15.GetHashCode();
    }
  }
}
