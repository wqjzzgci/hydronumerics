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

    private double _CurrentNMass=0;
    public double CurrentNMass
    {
      get { return _CurrentNMass; }
      set
      {
        if (_CurrentNMass != value)
        {
          _CurrentNMass = value;
          NotifyPropertyChanged("CurrentNMass");
        }
      }
    }

    private double _RetentionTime;
    /// <summary>
    /// Gets and sets the retention time in years
    /// </summary>
    public double RetentionTime
    {
      get { return _RetentionTime; }
      set
      {
        if (_RetentionTime != value)
        {
          _RetentionTime = value;
          NotifyPropertyChanged("RetentionTime");
        }
      }
    }

    private double _Volume;
    public double Volume  
    {
      get { return _Volume; }
      set
      {
        if (_Volume != value)
        {
          _Volume = value;
          NotifyPropertyChanged("Volume");
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

    private Time2.TimeStampSeries _Temperature;
    public Time2.TimeStampSeries Temperature
    {
      get {
        if (_Temperature == null)
          _Temperature = new Time2.TimeStampSeries();
        return _Temperature; }
      set
      {
        if (_Temperature != value)
        {
          _Temperature = value;
          NotifyPropertyChanged("Temperature");
        }
      }
    }

    private Time2.TimeStampSeries _NitrateConcentration;
    public Time2.TimeStampSeries NitrateConcentration
    {
      get {
        if (_NitrateConcentration == null)
          _NitrateConcentration = new Time2.TimeStampSeries();
        
        return _NitrateConcentration; }
      set
      {
        if (_NitrateConcentration != value)
        {
          _NitrateConcentration = value;
          NotifyPropertyChanged("NitrateConcentration");
        }
      }
    }

    private Time2.TimeStampSeries _NitrateReduction;
    public Time2.TimeStampSeries NitrateReduction
    {
      get {

        if (_NitrateReduction == null)
          _NitrateReduction = new Time2.TimeStampSeries();
        return _NitrateReduction; }
      set
      {
        if (_NitrateReduction != value)
        {
          _NitrateReduction = value;
          NotifyPropertyChanged("NitrateReduction");
        }
      }
    }

    private Time2.TimeStampSeries _FlushingRatio;
    public Time2.TimeStampSeries FlushingRatio
    {
      get {
        if (_FlushingRatio == null)
          _FlushingRatio = new Time2.TimeStampSeries();
        return _FlushingRatio; }
      set
      {
        if (_FlushingRatio != value)
        {
          _FlushingRatio = value;
          NotifyPropertyChanged("FlushingRatio");
        }
      }
    }
    
    
    
    
    

  }
}
