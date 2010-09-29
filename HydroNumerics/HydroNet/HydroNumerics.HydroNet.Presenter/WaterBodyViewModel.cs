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
    private IWaterBody _waterBody;
    private ObservableCollection<ISink> _sinks;
    private ObservableCollection<ISource> _sources;
    private ObservableCollection<ISink> _evaporationBoundaries;
    private ObservableCollection<IGroundwaterBoundary> _groundwaterBoundaries;



    public WaterBodyViewModel(IWaterBody WB)
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
    /// Gets the volume
    /// </summary>
    public double Volume
    {
      get
      {
        return _waterBody.Volume;
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
    /// Gets the groundwater boundaries
    /// </summary>
    public ObservableCollection<IGroundwaterBoundary> GroundwaterBoundaries
    {
      get
      {
        if (_groundwaterBoundaries == null)
          _groundwaterBoundaries = new ObservableCollection<IGroundwaterBoundary>();
        return _groundwaterBoundaries;
      }
    }

  }
}

