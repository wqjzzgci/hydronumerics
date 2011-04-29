using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScreenViewModel:BaseViewModel
  {
    private Screen _screen;
    private JupiterViewModel _jvm;

    public ScreenViewModel(Screen screen, JupiterViewModel jvm)
    {
      _screen = screen;
      _jvm = jvm;
    }

    public double DepthToTop
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

    /// <summary>
    /// Returns true if one of the depths are below -990
    /// </summary>
    public bool MissingData
    {
      get
      {
        return _screen.TopAsKote < -990 || _screen.BottomAsKote < -990 || _screen.DepthToBottom < -990 || _screen.DepthToTop < -990;
      }
    }

    public int IntakeNo
    {
      get
      {
        return _screen.Intake.IDNumber;
      }
    }


    public double DepthToBottom
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
  }
}
