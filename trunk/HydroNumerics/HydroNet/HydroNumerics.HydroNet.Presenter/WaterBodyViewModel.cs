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
    private ObservableCollection<IWaterBody> _downstreamconnections;

    public WaterBodyViewModel(IWaterBody WB)
    {
      _waterBody = WB;
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
    /// Gets the downstream connections
    /// </summary>
    public ObservableCollection<IWaterBody> DownStreamConnections
    {
      get
      {
        if (_downstreamconnections == null)
          _downstreamconnections = new ObservableCollection<IWaterBody>(_waterBody.DownStreamConnections);
        return _downstreamconnections;
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

