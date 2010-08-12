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

    /// <summary>
    /// Mike She style Layer number. Upper layer is number 1
    /// </summary>
    public int LayerNumber { get; private set; }

    private bool _moveUp = false;
    private bool _intakesAllowed = true;

    public Layer _above { get; set; }
    public Layer _below { get; set; }

    private DFS2 _gridCodes;

    public ObservableCollection<IIntake> Intakes { get;  set; }
    public ObservableCollection<IIntake> OriginalIntakes = new ObservableCollection<IIntake>();
    public ObservableCollection<IIntake> IntakesMoved = new ObservableCollection<IIntake>();

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
        //Top and bottom layers can only move in one direction
        if (_below == null)
          value = true;
        if (_above == null)
          value = false;

        if (value != _moveUp)
        {
          ReClaimIntakes();
          _moveUp = value;
          MoveIntakes();
          NotifyPropertyChanged("MoveUp");
        }
      }
    }

    private void MoveIntakes()
    {
      Layer ToMoveTo;
      if (_moveUp)
        ToMoveTo = _above;
      else
        ToMoveTo = _below;

      foreach (IIntake I in Intakes)
      {
        ToMoveTo.Intakes.Add(I);
        IntakesMoved.Add(I);
      }
      Intakes.Clear();
    }

    private void ReClaimIntakes()
    {
      Layer ToMoveFrom;
      if (_moveUp)
        ToMoveFrom = _above;
      else
        ToMoveFrom = _below;

      foreach (IIntake I in IntakesMoved)
      {
        Intakes.Add(I);
        ToMoveFrom.Intakes.Remove(I);
      }
      IntakesMoved.Clear();

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
          
          if (_intakesAllowed)
            ReClaimIntakes();
          else
            MoveIntakes();

          NotifyPropertyChanged("IntakesAllowed");
        }
      }
    }



  }

}
