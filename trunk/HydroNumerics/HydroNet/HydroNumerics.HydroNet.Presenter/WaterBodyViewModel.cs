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
  public class WaterBodyViewModel : IDObjectViewModel
  {
    protected AbstractWaterBody _waterBody;
    protected ObservableCollection<SourceBoundaryViewModel> _sinks;
    protected ObservableCollection<SourceBoundaryViewModel> _sources;
    protected ObservableCollection<ISource> _precipitation;
    protected ObservableCollection<ISink> _evaporationBoundaries;
    protected ObservableCollection<GroundWaterBoundary> _groundwaterBoundaries;

    protected ObservableCollection<KeyValuePair<string, double>> _waterBalance = new ObservableCollection<KeyValuePair<string, double>>();
    protected DateTime _storageTimeStart;
    protected DateTime _storageTimeEnd;
    protected int _storageTime;


    public WaterBodyViewModel(AbstractWaterBody WB):base(WB)
    {
      _waterBody = WB;

      

      //Build the sources list
      _sources = new ObservableCollection<SourceBoundaryViewModel>();
      foreach (var S in _waterBody.Sources)
      {
        _sources.Add(new SourceBoundaryViewModel((AbstractBoundary)S, "Source"));
      }
      foreach (var S in _waterBody.Precipitation)
      {
        _sources.Add(new SourceBoundaryViewModel((AbstractBoundary)S, "Precipitation"));
      }

      //Build the sinks list
      _sinks = new ObservableCollection<SourceBoundaryViewModel>();
      foreach (var S in _waterBody.EvaporationBoundaries)
      {
        _sinks.Add(new SourceBoundaryViewModel((AbstractBoundary)S, "Evaporation"));
      }
      foreach (var S in _waterBody.Sinks)
      {
        _sinks.Add(new SourceBoundaryViewModel((AbstractBoundary)S, "Sinks"));
      }
    }

    /// <summary>
    /// Gets the area
    /// </summary>
    public double Area
    {
      get
      {
        if (_waterBody.GetType().Equals(typeof(Lake)))
        {
          return ((Lake)_waterBody).Area;
        }
        else
          return ((Stream)_waterBody).Area;
      }
    }

    /// <summary>
    /// Gets the area
    /// </summary>
    public double Depth
    {
      get
      {
        if (_waterBody.GetType().Equals(typeof(Lake)))
        {
          return ((Lake)_waterBody).Depth;
        }
        else
          return ((Stream)_waterBody).Depth;
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
    /// Gets the Sources
    /// </summary>
    public ObservableCollection<SourceBoundaryViewModel> Sources
    {
      get
      {
        return _sources;
      }
    }

    /// <summary>
    /// Gets the Sources
    /// </summary>
    public ObservableCollection<SourceBoundaryViewModel> Sinks
    {
      get
      {
        return _sinks;
      }
    }


    /// <summary>
    /// Gets the groundwater boundaries
    /// </summary>
    public ObservableCollection<GroundWaterBoundary> GroundwaterBoundaries
    {
      get
      {
        if (_groundwaterBoundaries == null)
          _groundwaterBoundaries = new ObservableCollection<GroundWaterBoundary>(_waterBody.GroundwaterBoundaries.Cast<GroundWaterBoundary>() );
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

