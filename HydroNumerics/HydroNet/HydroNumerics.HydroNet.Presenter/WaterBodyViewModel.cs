using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class WaterBodyViewModel : BaseViewModel
  {
    protected AbstractWaterBody _waterBody;
    protected ObservableCollection<ISink> _sinks;
    protected ObservableCollection<ISource> _sources;
    protected ObservableCollection<ISource> _precipitation;
    protected ObservableCollection<ISink> _evaporationBoundaries;
    protected ObservableCollection<IGroundwaterBoundary> _groundwaterBoundaries;

    protected ObservableCollection<KeyValuePair<string, double>> _waterBalance = new ObservableCollection<KeyValuePair<string, double>>();
    protected DateTime _storageTimeStart;
    protected DateTime _storageTimeEnd;
    protected int _storageTime;


    public WaterBodyViewModel(AbstractWaterBody WB)
    {
      _waterBody = WB;
      
    }


    /// <summary>
    /// Gets and sets the Name
    /// </summary>
    public string Name
    {
      get
      {
        return _waterBody.Name;
      }
      set
      {
        if (value != _waterBody.Name)
        {
          _waterBody.Name = value;
          NotifyPropertyChanged("Name");
        }
      }
    }

    /// <summary>
    /// Gets and sets the Water level
    /// </summary>
    public double WaterLevel
    {
      get
      {
        return _waterBody.WaterLevel;
      }
      set
      {
        if (value != _waterBody.WaterLevel)
        {
          _waterBody.WaterLevel = value;
          NotifyPropertyChanged("WaterLevel");
        }
      }
    }



    /// <summary>
    /// Gets the sinks
    /// </summary>
    public ObservableCollection<ISink> Sinks
    {
      get
      {
        if (_sinks == null)
          _sinks = new ObservableCollection<ISink>(_waterBody.Sinks);
        return _sinks;
      }
    }

    /// <summary>
    /// Gets the evaporation boundaries
    /// </summary>
    public ObservableCollection<ISink> EvaporationBoundaries
    {
      get
      {
        if (_evaporationBoundaries == null)
          _evaporationBoundaries = new ObservableCollection<ISink>(_waterBody.EvaporationBoundaries);
        return _evaporationBoundaries;
      }
    }

    /// <summary>
    /// Gets the Sources
    /// </summary>
    public ObservableCollection<ISource> Sources
    {
      get
      {
        if (_sources == null)
          _sources = new ObservableCollection<ISource>(_waterBody.Sources);
        return _sources;
      }
    }

    /// <summary>
    /// Gets the Sources
    /// </summary>
    public ObservableCollection<ISource> Precipitation
    {
      get
      {
        if (_precipitation == null)
          _precipitation = new ObservableCollection<ISource>(_waterBody.Precipitation);
        return _precipitation;
      }
    }


    /// <summary>
    /// Gets the groundwater boundaries
    /// </summary>
    public ObservableCollection<IGroundwaterBoundary> GroundwaterBoundaries
    {
      get
      {
        if (_groundwaterBoundaries == null)
          _groundwaterBoundaries = new ObservableCollection<IGroundwaterBoundary>(_waterBody.GroundwaterBoundaries);
        return _groundwaterBoundaries;
      }
    }

    /// <summary>
    /// Gets and sets the start time for the calculation of water balance
    /// </summary>
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

    /// <summary>
    /// Gets and sets the end time for the water balance calculation
    /// </summary>
    public DateTime StorageTimeEnd
    {
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

    /// <summary>
    /// Gets the water balance
    /// </summary>
    public ObservableCollection<KeyValuePair<string, double>> WaterBalanceComponents
    {
      get
      {
        return _waterBalance;
      }
    }

    /// <summary>
    /// Gets the storage time
    /// </summary>
    public int StorageTime
    {
      get
      {
        return _storageTime;
      }
      private set
      {
        if (_storageTime != value)
        {
          _storageTime = value;
          NotifyPropertyChanged("StorageTime");
        }
      }
    }

    /// <summary>
    /// Gets the output generated during the simulation
    /// </summary>
    public WaterBodyOutput Output
    {
      get
      {
        return _waterBody.Output;
      }
    }


    private void UpdateWB()
    {
      WaterBalanceComponents.Clear();
      WaterBalanceComponents.Add(new KeyValuePair<string, double>("Outflow", _waterBody.Output.Outflow.GetSiValue(StorageTimeStart, StorageTimeEnd)));
      WaterBalanceComponents.Add(new KeyValuePair<string, double>("Evaporation", -_waterBody.Output.Evaporation.GetSiValue(StorageTimeStart, StorageTimeEnd)));
      double sources = _waterBody.Output.Sources.GetSiValue(StorageTimeStart, StorageTimeEnd);
      double groundwater = ((GroundWaterBoundary)_waterBody.GroundwaterBoundaries.First()).Output.Items[0].GetSiValue(StorageTimeStart, StorageTimeEnd);

      if (_waterBody.Output.Inflow.Items.Count > 0)
      {
        double inflow = _waterBody.Output.Inflow.GetSiValue(StorageTimeStart, StorageTimeEnd);
        WaterBalanceComponents.Add(new KeyValuePair<string, double>("Inflow", inflow));
      }

      WaterBalanceComponents.Add(new KeyValuePair<string, double>("Precipitation", sources - groundwater));
      WaterBalanceComponents.Add(new KeyValuePair<string, double>("Groundwater", groundwater));

      StorageTime = (int)_waterBody.GetStorageTime(StorageTimeStart, StorageTimeEnd).TotalDays / 365;

    }
  }
}

