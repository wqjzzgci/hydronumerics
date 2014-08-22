using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Core.Time;
using HydroNumerics.Geometry;

namespace HydroNumerics.Nitrate.Model
{
  public class DMUStation:BaseViewModel
  {

    private XYPoint _Location;
    public XYPoint Location
    {
      get { return _Location; }
      set
      {
        if (_Location != value)
        {
          _Location = value;
          RaisePropertyChanged("Location");
        }
      }
    }
    

    private string _StreamName;
    public string StreamName
    {
      get { return _StreamName; }
      set
      {
        if (_StreamName != value)
        {
          _StreamName = value;
          RaisePropertyChanged("StreamName");
        }
      }
    }
    

    private int _ODANummer;
    public int ODANummer
    {
      get { return _ODANummer; }
      set
      {
        if (_ODANummer != value)
        {
          _ODANummer = value;
          RaisePropertyChanged("ODANummer");
        }
      }
    }

    private TimeStampSeries _Nitrate = new TimeStampSeries();
    public TimeStampSeries Nitrate
    {
      get { return _Nitrate; }
      set
      {
        if (_Nitrate != value)
        {
          _Nitrate = value;
          RaisePropertyChanged("Nitrate");
        }
      }
    }


    private TimeStampSeries _Flow = new TimeStampSeries();
    public TimeStampSeries Flow
    {
      get { return _Flow; }
      set
      {
        if (_Flow != value)
        {
          _Flow = value;
          RaisePropertyChanged("Flow");
        }
      }
    }
    

  }
}
