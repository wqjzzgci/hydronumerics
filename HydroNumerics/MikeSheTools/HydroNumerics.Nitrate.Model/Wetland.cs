using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.Nitrate.Model
{
  public class Wetland:BaseViewModel
  {

    private DateTime _StartTime;
    public DateTime StartTime
    {
      get { return _StartTime; }
      set
      {
        if (_StartTime != value)
        {
          _StartTime = value;
          RaisePropertyChanged("StartTime");
        }
      }
    }
    


    private string _SoilString;
    public string SoilString
    {
      get { return _SoilString; }
      set
      {
        if (_SoilString != value)
        {
          _SoilString = value;
          RaisePropertyChanged("SoilString");
        }
      }
    }


    private IXYPolygon _Geometry;
    public IXYPolygon Geometry
    {
      get { return _Geometry; }
      set
      {
        if (_Geometry != value)
        {
          _Geometry = value;
          RaisePropertyChanged("Geometry");
        }
      }
    }
    
    


  }
}
