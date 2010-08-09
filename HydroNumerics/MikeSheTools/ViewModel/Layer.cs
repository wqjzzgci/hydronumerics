using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class Layer : INotifyPropertyChanged
  {

    private bool _moveUp;

    private DFS2 _gridCodes;

    public ObservableCollection<IWell> Wells = new ObservableCollection<IWell>();


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


    /// <summary>
    /// Gets and sets the filename with grid codes
    /// </summary>
    public string GridCodesFileName
    {
      get
      {
        return _gridCodes.FileName;
      }
      set
      {
        if(_gridCodes!=null)
          _gridCodes.Dispose();
          
        _gridCodes = new DFS2(value);
        NotifyPropertyChanged("GridCodesFileName");
      }
    }

    /// <summary>
    /// Gets and sets a boolean to tell whether wells should be moved up or down
    /// </summary>
    public bool MoveUp
    {
      get { return _moveUp; }
      set
      {
        if (value != _moveUp)
        {
          _moveUp = value;
          NotifyPropertyChanged("MoveUp");

        }
      }
    }
  }

}
