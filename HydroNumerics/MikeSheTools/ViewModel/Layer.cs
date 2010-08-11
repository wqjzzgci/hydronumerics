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

    public int LayerNumber { get; private set; }

    private bool _moveUp = false;
    private bool _intakesAllowed = true;

    private DFS2 _gridCodes;

    public ObservableCollection<IIntake> Intakes { get;  set; }

    public Layer(int Number)
    {
      LayerNumber = Number;
      Intakes = new ObservableCollection<IIntake>();
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


    /// <summary>
    /// Gets and sets the filename with grid codes
    /// </summary>
    public string GridCodesFileName
    {
      get
      {
        if (_gridCodes == null)
          return "";
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

    /// <summary>
    /// Gets and sets a boolean to indicate if intakes are allowed in this layer
    /// </summary>
    public bool IntakesAllowed
    {
      get { return _intakesAllowed; }
      set
      {
        if (value != _intakesAllowed)
        {
          _intakesAllowed = value;
          NotifyPropertyChanged("IntakesAllowed");
        }
      }
    }



  }

}
