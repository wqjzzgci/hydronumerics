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

    private ChangeDescription GetScreenChange()
    {
      ChangeDescription xchange = new ChangeDescription(JupiterTables.SCREEN);

      xchange.Action = TableAction.EditValue;
      xchange.PrimaryKeys["BOREHOLENO"]= _screen.Intake.well.ID;
      xchange.PrimaryKeys["SCREENNO"] = _screen.Number.ToString();
      return xchange;
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
          ChangeDescription c = GetScreenChange();
          c.ChangeValues.Add(new Change("TOP", value.ToString(), _screen.DepthToTop.ToString()));
          _jvm.Changes.Add(c);
          _screen.DepthToTop = value;
          NotifyPropertyChanged("DepthToTop");

        }
      }
    }

    public bool MissingData
    {
      get
      {
        return _screen.MissingData;
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
          ChangeDescription c = GetScreenChange();
          c.ChangeValues.Add(new Change("TOP", value.ToString(), _screen.DepthToTop.ToString()));
          _jvm.Changes.Add(c);
          _screen.DepthToBottom = value;
          NotifyPropertyChanged("DepthToBottom");
        }
      }
    }
  }
}
