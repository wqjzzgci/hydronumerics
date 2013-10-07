using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.Core.WPF;

namespace HydroNumerics.Nitrate.Model
{
  public class Catchment:BaseViewModel
  {

    public Catchment(int ID)
    {
      ID15 = ID;
      UpstreamConnections = new List<Catchment>();
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
      return ID15.Equals(((Catchment)obj).ID15);
    }

    public override int GetHashCode()
    {
      return ID15.GetHashCode();
    }
  }
}
