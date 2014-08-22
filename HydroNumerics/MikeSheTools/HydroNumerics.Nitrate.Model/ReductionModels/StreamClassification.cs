using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class StreamClassification:BaseViewModel
  {


    private string _StreamType;
    public string StreamType
    {
      get { return _StreamType; }
      set
      {
        if (_StreamType != value)
        {
          _StreamType = value;
          RaisePropertyChanged("Width");
        }
      }
    }

    private double _StreamDepthSummer;
    public double StreamDepthSummer
    {
      get { return _StreamDepthSummer; }
      set
      {
        if (_StreamDepthSummer != value)
        {
          _StreamDepthSummer = value;
          RaisePropertyChanged("StreamDepthSummer");
        }
      }
    }

    private double _StreamDepthWinter;
    public double StreamDepthWinter
    {
      get { return _StreamDepthWinter; }
      set
      {
        if (_StreamDepthWinter != value)
        {
          _StreamDepthWinter = value;
          RaisePropertyChanged("StreamDepthWinter");
        }
      }
    }

    private double _StreamVelocitySummer;
    public double StreamVelocitySummer
    {
      get { return _StreamVelocitySummer; }
      set
      {
        if (_StreamVelocitySummer != value)
        {
          _StreamVelocitySummer = value;
          RaisePropertyChanged("StreamVelocitySummer");
        }
      }
    }


    private double _StreamVelocityWinter;
    public double StreamVelocityWinter
    {
      get { return _StreamVelocityWinter; }
      set
      {
        if (_StreamVelocityWinter != value)
        {
          _StreamVelocityWinter = value;
          RaisePropertyChanged("StreamVelocityWinter");
        }
      }
    }
    



  }
}
