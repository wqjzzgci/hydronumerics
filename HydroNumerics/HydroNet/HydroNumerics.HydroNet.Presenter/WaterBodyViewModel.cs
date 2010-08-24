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
  public class WaterBodyViewModel : INotifyPropertyChanged
  {
    private IWaterBody _waterBody;
    private ObservableCollection<IWaterSinkSource> _sinkSources;
    private ObservableCollection<IEvaporationBoundary> _evaporationBoundaries;



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
    /// Gets the sinks and sources
    /// </summary>
    public ObservableCollection<IWaterSinkSource> SinkSources
    {
      get
      {
        if (_sinkSources == null)
          _sinkSources = new ObservableCollection<IWaterSinkSource>(_waterBody.SinkSources);
        return _sinkSources;
      }
    }

    /// <summary>
    /// Gets the evaporation boundaries
    /// </summary>
    public ObservableCollection<IEvaporationBoundary> EvaporationBoundaries
    {
      get
      {
        if (_evaporationBoundaries == null)
          _evaporationBoundaries = new ObservableCollection<IEvaporationBoundary>(_waterBody.EvaporationBoundaries);
        return _evaporationBoundaries;
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

