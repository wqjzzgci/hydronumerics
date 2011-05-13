using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScreenViewModel:BaseViewModel,IDataErrorInfo
  {
    public Screen _screen;
   

    public ScreenViewModel(Screen screen)
    {
      _screen = screen;
  
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
          _screen.DepthToTop = value;
          NotifyPropertyChanged("DepthToTop");
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
          _screen.DepthToBottom = value;

          NotifyPropertyChanged("DepthToBottom");
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
        }
        return result;
      }
    }

  }
}
