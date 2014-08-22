using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using HydroNumerics.Core.Time;
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

    private DateTime _Start;
    public DateTime Start
    {
      get { return _Start; }
      set
      {
        if (_Start != value)
        {
          _Start = value;
          RaisePropertyChanged("Start");
        }
      }
    }

    private DateTime _End;
    public DateTime End
    {
      get { return _End; }
      set
      {
        if (_End != value)
        {
          _End = value;
          RaisePropertyChanged("End");
        }
      }
    }
    
    

    private bool _IsSmallLake=true;
    public bool IsSmallLake
    {
      get { return _IsSmallLake; }
      set
      {
        if (_IsSmallLake != value)
        {
          _IsSmallLake = value;
          RaisePropertyChanged("IsSmallLake");
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
          RaisePropertyChanged("DegreeOfCultivation");
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
          RaisePropertyChanged("SoilType");
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
          RaisePropertyChanged("HasDischarge");
        }
      }
    }

    private int _BigLakeID =-1;
    public int BigLakeID
    {
      get { return _BigLakeID; }
      set
      {
        if (_BigLakeID != value)
        {
          _BigLakeID = value;
          RaisePropertyChanged("BigLakeID");
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
          RaisePropertyChanged("CurrentNMass");
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
          RaisePropertyChanged("RetentionTime");
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
          RaisePropertyChanged("Volume");
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
          RaisePropertyChanged("Geometry");
        }
      }
    }

    private TimeStampSeries _Temperature;
    public TimeStampSeries Temperature
    {
      get {
        if (_Temperature == null)
          _Temperature = new TimeStampSeries();
        return _Temperature; }
      set
      {
        if (_Temperature != value)
        {
          _Temperature = value;
          RaisePropertyChanged("Temperature");
        }
      }
    }

    private TimeStampSeries _NitrateConcentration;
    public TimeStampSeries NitrateConcentration
    {
      get {
        if (_NitrateConcentration == null)
          _NitrateConcentration = new TimeStampSeries();
        
        return _NitrateConcentration; }
      set
      {
        if (_NitrateConcentration != value)
        {
          _NitrateConcentration = value;
          RaisePropertyChanged("NitrateConcentration");
        }
      }
    }

    private TimeStampSeries _NitrateReduction;
    public TimeStampSeries NitrateReduction
    {
      get {

        if (_NitrateReduction == null)
          _NitrateReduction = new TimeStampSeries();
        return _NitrateReduction; }
      set
      {
        if (_NitrateReduction != value)
        {
          _NitrateReduction = value;
          RaisePropertyChanged("NitrateReduction");
        }
      }
    }

    private TimeStampSeries _FlushingRatio;
    public TimeStampSeries FlushingRatio
    {
      get {
        if (_FlushingRatio == null)
          _FlushingRatio = new TimeStampSeries();
        return _FlushingRatio; }
      set
      {
        if (_FlushingRatio != value)
        {
          _FlushingRatio = value;
          RaisePropertyChanged("FlushingRatio");
        }
      }
    }
    
    
    
    
    

  }
}
