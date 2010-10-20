using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using System.Windows.Controls.DataVisualization.Charting;

using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class WaterBodyViewModel : IDObjectViewModel
  {
    protected AbstractWaterBody _waterBody;
    protected ObservableCollection<SourceBoundaryViewModel> _sinks;
    protected ObservableCollection<SourceBoundaryViewModel> _sources;
    protected ObservableCollection<GroundWaterBoundary> _groundwaterBoundaries;

    protected ObservableCollection<KeyValuePair<string, double>> _waterBalance = new ObservableCollection<KeyValuePair<string, double>>();
    protected DateTime _storageTimeStart;
    protected DateTime _storageTimeEnd;
    protected int _storageTime;


    public WaterBodyViewModel(AbstractWaterBody WB):base(WB)
    {
      _waterBody = WB;
      _storageTimeStart = _waterBody.Output.StartTime;
      _storageTimeEnd = _waterBody.Output.EndTime;
      UpdateWB();

      //Build the groundwater list
      _groundwaterBoundaries = new ObservableCollection<GroundWaterBoundary>();
      foreach (var GWB in _waterBody.GroundwaterBoundaries)
      {
        if (GWB is GroundWaterBoundary)
          _groundwaterBoundaries.Add((GroundWaterBoundary)GWB);
      }

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

      //Build the chemical lists
      Chemicals = new ObservableCollection<LineSeries>();
      foreach (var ts in _waterBody.Output.ChemicalsToLog.Values)
      {
        LineSeries ls = new LineSeries();
        ls.ItemsSource = ts.Items;
        ls.DependentValuePath = "Value";
        ls.IndependentValuePath = "Time";
        ls.Title = ts.Name;
        Chemicals.Add(ls);
      }

      //Build composition lists
      Compositions = new ObservableCollection<LineSeries>();
      foreach (var ts in _waterBody.Output.CompositionLog.Values)
      {
        LineSeries ls = new LineSeries();
        ls.ItemsSource = ts.Items;
        ls.DependentValuePath = "Value";
        ls.IndependentValuePath = "Time";
        ls.Title = ts.Name;
        Compositions.Add(ls);
      }

      ChemicalMeasurements = new ObservableCollection<LineSeries>();
      {
        foreach (var cts in _waterBody.RealData.ChemicalConcentrations.Values)
        {

          ScatterSeries ls = new ScatterSeries();
          ls.ItemsSource = cts.Items;
          ls.DependentValuePath = "Value";
          ls.IndependentValuePath = "Time";
          ls.Title = cts.Name;
           
        }
      }

    }

    public ObservableCollection<LineSeries> ChemicalMeasurements { get; private set; }


    public ObservableCollection<LineSeries> Compositions { get; private set; }

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
          NotifyPropertyChanged("StorageTimeString");
        }
      }
    }

    public string StorageTimeString
    {
      get
      {
        return "The mean water storage time in the selected time period is " + _storageTime + " years.";
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

    /// <summary>
    /// Gets line series with chemical concentrations in water
    /// </summary>
    public ObservableCollection<LineSeries> Chemicals { get; private set; }


    private void UpdateWB()
    {
      WaterBalanceComponents.Clear();

      for (int i = 0; i < 8; i++)
      {
        string name = _waterBody.Output.Items[i].Name;
        if (_waterBody.Output.Items[i].Values.Count() > 0)
          WaterBalanceComponents.Add(new KeyValuePair<string, double>(name, _waterBody.Output.Items[i].GetSiValue(StorageTimeStart, StorageTimeEnd)));
        else
          WaterBalanceComponents.Add(new KeyValuePair<string, double>(name, 0));

        StorageTime = (int)_waterBody.GetStorageTime(StorageTimeStart, StorageTimeEnd).TotalDays / 365;
      }
    }


  }
}

