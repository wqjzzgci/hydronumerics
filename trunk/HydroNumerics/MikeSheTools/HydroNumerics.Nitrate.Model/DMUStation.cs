using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time2;
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
          NotifyPropertyChanged("Location");
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
          NotifyPropertyChanged("StreamName");
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
          NotifyPropertyChanged("ODANummer");
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
          NotifyPropertyChanged("Nitrate");
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
          NotifyPropertyChanged("Flow");
        }
      }
    }
    

  }
}
