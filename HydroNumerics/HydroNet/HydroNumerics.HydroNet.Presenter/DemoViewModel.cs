using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class DemoViewModel:INotifyPropertyChanged
  {

    private Lake _lake;
    Model Engine;
    private ObservableCollection<KeyValuePair<string, double>> _waterBalance = new ObservableCollection<KeyValuePair<string, double>>();
    private DateTime _storageTimeStart;
    private DateTime _storageTimeEnd;
    private int _storageTime;

    public TimestampSeries Discharge { get; private set; }


    public DemoViewModel(string Name, XYPolygon SurfaceArea, TimespanSeries Evaporation, TimespanSeries Precipitation)
    {
      Calibration = 1;
      _lake = new Lake(Name, SurfaceArea);
      _lake.Depth = 5;
      _lake.WaterLevel = 45.7;

      //Create and add precipitation boundary
      SinkSourceBoundary Precip = new SinkSourceBoundary(Precipitation);
      Precip.ContactGeometry = _lake.SurfaceArea;
      _lake.Sources.Add(Precip);

      //Create and add evaporation boundary
      EvaporationRateBoundary eva = new EvaporationRateBoundary(Evaporation);
      eva.ContactGeometry = _lake.SurfaceArea;
      _lake.EvaporationBoundaries.Add(eva);

      //Create and add a discharge boundary
      Discharge = new TimestampSeries();
      Discharge.AddSiValue(new DateTime(2007, 3, 12), 6986 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.AddSiValue(new DateTime(2007, 4, 3), 5894 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.AddSiValue(new DateTime(2007, 4, 25), 1205 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.RelaxationFactor = 1;
      Discharge.AllowExtrapolation = true;
      Discharge.Name = "Inflow";
      SinkSourceBoundary Kilde = new SinkSourceBoundary(Discharge);
      _lake.Sources.Add(Kilde);

      //Add a groundwater boundary
      GroundWaterBoundary gwb = new GroundWaterBoundary(_lake, 1e-7, 1, 46, (XYPolygon) _lake.Geometry);
      _lake.GroundwaterBoundaries.Add(gwb);

      DateTime Start = new DateTime(2007, 1, 1);
      //Add to an engine
      Engine = new Model();
      Engine._waterBodies.Add(_lake);

      //Set initial state
      Engine.SetState("Initial", Start, new WaterPacket(1));

      //Add the chemicals
      Chemical cl = ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl);

      //Tell the lake to log the chemicals
      _lake.Output.LogChemicalConcentration(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction));
      _lake.Output.LogChemicalConcentration(cl);

      IsotopeWater Iw = new IsotopeWater(1);
      Iw.SetIsotopeRatio(0.2);
      Iw.AddChemical(cl, 0.1);
      Precip.WaterSample = Iw.DeepClone();

      //Evaporate some of the water to get realistic initial conditions
      Iw.Evaporate(Iw.Volume / 2);
      _lake.SetState("Initial", Start, Iw.DeepClone(_lake.Volume));
      Kilde.WaterSample = Iw.DeepClone();

      Iw.Evaporate(Iw.Volume / 2);
      gwb.WaterSample = Iw.DeepClone();
    
    }

    public string Name
    {
      get { return _lake.Name; }
    }

    public double Calibration { get; set; }

    public double Area
    {
      get { return _lake.Area; }
    }

    public double Depth
    {
      get { return _lake.Depth; }
      set
      {
        if (_lake.Depth != value)
        {
          _lake.Depth = value;
          NotifyPropertyChanged("Depth");
        }
      }
    }

    public double WaterLevel
    {
      get { return _lake.WaterLevel; }
      set
      {
        if (_lake.WaterLevel != value)
        {
          _lake.WaterLevel = value;
          NotifyPropertyChanged("WaterLevel");
        }
      }
    }


    public TimespanSeries Evaporation
    {
      get
      {
        var evap = _lake.EvaporationBoundaries.FirstOrDefault();
        if (evap != null)
        {
          return ((EvaporationRateBoundary)evap).TimeValues;
        }
        return null;
      }
    }

    public TimespanSeries Precipitation
    {
      get
      {
        var precip = _lake.Sources[0];
          return ((SinkSourceBoundary)precip).TimeValues;
      }
    }

    public TimespanSeries Outflow
    {
      get
      {
        return _lake.Output.Outflow;
      }
    }

    public TimestampSeries IsotopeConc
    {
      get 
      {return _lake.Output.ChemicalsToLog[ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction)];}
    }

    public TimestampSeries ChlorideConc
    {
      get
      { return _lake.Output.ChemicalsToLog[ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl)]; }
    }

    public double GWIsotopeConc
    {
      get
      {
       return ((WaterPacket)_lake.GroundwaterBoundaries.First().WaterSample).GetConcentration(ChemicalNames.IsotopeFraction);
      }
      set
      {
        if (value != GWIsotopeConc)
        {
          ((WaterPacket)_lake.GroundwaterBoundaries.First().WaterSample).SetConcentration(ChemicalNames.IsotopeFraction, value);
          NotifyPropertyChanged("GWIsotopeConc");
        }
      }
    }

    public double GWChloridConc
    {
      get
      {
        return ((WaterPacket)_lake.GroundwaterBoundaries.First().WaterSample).GetConcentration(ChemicalNames.Cl);
      }
      set
      {
        if (value != GWChloridConc)
        {
          ((WaterPacket)_lake.GroundwaterBoundaries.First().WaterSample).SetConcentration(ChemicalNames.Cl, value);
          NotifyPropertyChanged("GWChloridConc");
        }
      }
    }

    public double HydraulicConductivity
    {
      get { return ((GroundWaterBoundary)_lake.GroundwaterBoundaries.First()).HydraulicConductivity; }
      set
      {
        if (HydraulicConductivity != value)
        {
          ((GroundWaterBoundary)_lake.GroundwaterBoundaries.First()).HydraulicConductivity = value;
          NotifyPropertyChanged("HydraulicConductivity");
        }
      }
    }

    public double GroundwaterHead
    {
      get { return ((GroundWaterBoundary)_lake.GroundwaterBoundaries.First()).GroundwaterHead; }
      set
      {
        if (GroundwaterHead != value)
        {
          ((GroundWaterBoundary)_lake.GroundwaterBoundaries.First()).GroundwaterHead = value;
          NotifyPropertyChanged("GroundwaterHead");
        }
      }
    }


    public DateTime StorageTimeStart
    {
      get { return _storageTimeStart; }
      set
      {
        if (value != _storageTimeStart)
        {
          _storageTimeStart = value;
          NotifyPropertyChanged("StorageTimeStart");
          UpdateWB();
        }
      }
    }
    
    public DateTime StorageTimeEnd{
      get { return _storageTimeEnd; }
      set
      {
        if (value != _storageTimeEnd)
        {
          _storageTimeEnd = value;
          NotifyPropertyChanged("StorageTimeEnd");
          UpdateWB();
        }
      }
    }

    private void UpdateWB()
    {
      WaterBalanceComponents.Clear();
      WaterBalanceComponents.Add(new KeyValuePair<string, double>("Outflow", _lake.Output.Outflow.GetSiValue(StorageTimeStart, StorageTimeEnd)));
      WaterBalanceComponents.Add(new KeyValuePair<string, double>("Evaporation", -_lake.Output.Evaporation.GetSiValue(StorageTimeStart, StorageTimeEnd)));
      double sources = _lake.Output.Sources.GetSiValue(StorageTimeStart, StorageTimeEnd);
      double groundwater = ((GroundWaterBoundary)_lake.GroundwaterBoundaries.First()).Output.Items[0].GetSiValue(StorageTimeStart, StorageTimeEnd);

      if (_lake.Output.Inflow.Items.Count > 0)
      {
        double inflow = _lake.Output.Inflow.GetSiValue(StorageTimeStart, StorageTimeEnd);
        WaterBalanceComponents.Add(new KeyValuePair<string, double>("Inflow", inflow));
      }

      WaterBalanceComponents.Add(new KeyValuePair<string, double>("Precipitation", sources - groundwater));
      WaterBalanceComponents.Add(new KeyValuePair<string, double>("Groundwater", groundwater));

      StorageTime = (int)_lake.Output.GetStorageTime(StorageTimeStart, StorageTimeEnd).TotalDays / 365;

    }

    public int StorageTime
    {
      get
      {
        return _storageTime;
      }
      set
      {
        if (_storageTime != value)
        {
          _storageTime = value;
          NotifyPropertyChanged("StorageTime");
        }
      }
    }
    
    public void Run()
    {
      Engine.RestoreState("Initial");


      if (Calibration != 1)
      {
        GroundWaterBoundary gwb = ((GroundWaterBoundary)_lake.GroundwaterBoundaries.First());
        double WaterVolume = ((XYPolygon)gwb.ContactGeometry).GetArea() * gwb.HydraulicConductivity * (gwb.GroundwaterHead - WaterLevel) / gwb.Distance;

        HydraulicConductivity *= Calibration;

        EvaporationRateBoundary er = new EvaporationRateBoundary((Calibration - 1) * WaterVolume);
        _lake.EvaporationBoundaries.Add(er);
      }
     

      Engine.MoveInTime(new DateTime(2007, 12, 31), TimeSpan.FromDays(30));
      _storageTimeEnd = new DateTime(2007, 12, 23);
      StorageTimeStart = new DateTime(2007, 1, 1);
      StorageTimeEnd = new DateTime(2007, 12, 24);

      if (Calibration != 1)
      {
        HydraulicConductivity /= Calibration;
        _lake.EvaporationBoundaries.RemoveAt(1);
      }
    }

    public ObservableCollection<KeyValuePair<string, double>> WaterBalanceComponents
    {
      get
      {
        return _waterBalance;
      }
    }
    



    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }


    #endregion
  }
}
