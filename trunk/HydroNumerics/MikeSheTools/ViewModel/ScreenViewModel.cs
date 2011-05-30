using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScreenViewModel:BaseViewModel,IDataErrorInfo
  {
    public Screen _screen;
    public ChangesViewModel CVM;

    public ScreenViewModel(Screen screen, ChangesViewModel cvm)
    {
      _screen = screen;
      CVM = cvm;
    }

    public int MsheTopLayer { get; private set; }
    public int MsheBottomLayer { get; private set; }

    /// <summary>
    /// Returns true if the screen is entirely above the terrain
    /// </summary>
    public bool AboveModelTerrain
    {
      get
      {
        return MsheBottomLayer == -1;
      }
    }

    /// <summary>
    /// Returns true if the screen is entirely below the bottom
    /// </summary>
    public bool BelowModelBottom
    {
      get
      {
        return MsheTopLayer == -2;
      }
    }

    /// <summary>
    /// Sets the top and bottom layer
    /// </summary>
    /// <param name="mshe"></param>
    /// <param name="thiswell"></param>
    public void LinkToMshe(Model mshe, WellViewModel thiswell)
    {
      if (thiswell.Column >= 0 & thiswell.Row >= 0 )
      {
        if (_screen.TopAsKote.HasValue)
          MsheTopLayer = mshe.GridInfo.GetLayer(thiswell.Column, thiswell.Row, _screen.TopAsKote.Value);
        if (_screen.BottomAsKote.HasValue)
          MsheBottomLayer = mshe.GridInfo.GetLayer(thiswell.Column, thiswell.Row, _screen.BottomAsKote.Value);
      }
    }


    public double? DepthToTop
    {
      get
      {
        return _screen.DepthToTop;
      }
      set
      {
        if (_screen.DepthToTop != value)
        {
          ChangeDescription c = CVM.ChangeController.ChangeTopOnScreen(_screen, value.Value);
          _screen.DepthToTop = value;
          NotifyPropertyChanged("DepthToTop");
          var cv = new ChangeDescriptionViewModel(c);
          cv.IsApplied = true;
          CVM.AddChange(cv, true);          
        }
      }
    }



    public IIntake Intake
    {
      get
      {
        return _screen.Intake;
      }
      set
      {
        if (_screen.Intake != value)
        {
          _screen.Intake.Screens.Remove(_screen);
          Screen NewScreen = new Screen(value);
          NewScreen.Number = _screen.Number;
          NewScreen.TopAsKote = _screen.TopAsKote;
          NewScreen.BottomAsKote = _screen.BottomAsKote;
          _screen = NewScreen;
          NotifyPropertyChanged("Intake");
        }
      }
    }


    public double? DepthToBottom
    {
      get
      {
        return _screen.DepthToBottom;
      }
      set
      {
        if (_screen.DepthToBottom != value)
        {
          ChangeDescription c = CVM.ChangeController.ChangeBottomOnScreen(_screen, value.Value);
          _screen.DepthToBottom = value;
          NotifyPropertyChanged("DepthToBottom");
          var cv = new ChangeDescriptionViewModel(c);
          cv.IsApplied = true;
          CVM.AddChange(cv, true);
        }
      }
    }

    public string Error
    {
      get
      {
        return null;
      }
    }

    public string this[string name]
    {
      get
      {
        string result = null;

        if (name == "DepthToBottom" || name =="DepthToTop")
        {
          if (DepthToBottom< DepthToTop)
          {
            result = "The bottom of the screen must be below the top";
          }
          if (DepthToBottom > Intake.well.Depth)
          {
            result = "The bottom of the screen must be above the bottom of the well";
          }
          if (DepthToBottom < 0 || DepthToTop < 0)
          {
            result = "Depths must be positive";
          }
        }
        return result;
      }
    }

  }
}
