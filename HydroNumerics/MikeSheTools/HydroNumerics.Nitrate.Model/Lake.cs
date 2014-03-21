using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;


namespace HydroNumerics.Nitrate.Model
{
  public enum CultivationClass
  {
    Low,
    Intermediate,
    High
  }

  public class Lake:BaseViewModel
  {
    private bool _IsSmallLake=true;
    public bool IsSmallLake
    {
      get { return _IsSmallLake; }
      set
      {
        if (_IsSmallLake != value)
        {
          _IsSmallLake = value;
          NotifyPropertyChanged("IsSmallLake");
        }
      }
    }
    

    private CultivationClass  _DegreeOfCultivation;
    public CultivationClass  DegreeOfCultivation
    {
      get { return _DegreeOfCultivation; }
      set
      {
        if (_DegreeOfCultivation != value)
        {
          _DegreeOfCultivation = value;
          NotifyPropertyChanged("DegreeOfCultivation");
        }
      }
    }
    

    private string _SoilType;
    public string SoilType
    {
      get { return _SoilType; }
      set
      {
        if (_SoilType != value)
        {
          _SoilType = value;
          NotifyPropertyChanged("SoilType");
        }
      }
    }
    

    private bool _HasDischarge;
    public bool HasDischarge
    {
      get { return _HasDischarge; }
      set
      {
        if (_HasDischarge != value)
        {
          _HasDischarge = value;
          NotifyPropertyChanged("HasDischarge");
        }
      }
    }
    

    private XYPolygon  _Geometry;
    public XYPolygon  Geometry
    {
      get { return _Geometry; }
      set
      {
        if (_Geometry != value)
        {
          _Geometry = value;
          NotifyPropertyChanged("Geometry");
        }
      }
    }
    

  }
}
