using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class StreamClassification:BaseViewModel
  {


    private double _Width;
    public double Width
    {
      get { return _Width; }
      set
      {
        if (_Width != value)
        {
          _Width = value;
          NotifyPropertyChanged("Width");
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
          NotifyPropertyChanged("StreamDepthSummer");
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
          NotifyPropertyChanged("StreamDepthWinter");
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
          NotifyPropertyChanged("StreamVelocitySummer");
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
          NotifyPropertyChanged("StreamVelocityWinter");
        }
      }
    }
    



  }
}
