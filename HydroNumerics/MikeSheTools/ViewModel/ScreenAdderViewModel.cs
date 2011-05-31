using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScreenAdderViewModel : BaseViewModel, IDataErrorInfo
  {
    private WellViewModel well;
    private ScreenViewModel svm;
    public ScreenAdderViewModel(WellViewModel well)
    {
      this.well = well;
      svm = well.AddScreen();
      CurrentChange = new ChangeDescriptionViewModel(svm.CVM.ChangeController.NewScreen(svm._screen));
      NotifyPropertyChanged("CurrentChange");
      NotifyPropertyChanged("Intakes");
    }


    public IEnumerable<IIntake> Intakes
    {
      get
      {
        return well.Intakes;
      }
    }

    public IIntake Intake
    {
      get
      {
        return svm.Intake;
      }
      set
      {
        if (Intake != value)
        {
          Intake.Screens.Remove(svm._screen);
          Intake = value;
          Intake.Screens.Add(svm._screen);
          svm.Intake = Intake;
        }
      }
    }

    public double? DepthToTop
    {
      get
      {
        return svm._screen.DepthToTop;
      }
      set
      {
        svm._screen.DepthToTop = value;
        NotifyPropertyChanged("DepthToTop");
      }
    }

    public double? DepthToBottom
    {
      get
      {
        return svm._screen.DepthToBottom;
      }
      set
      {
        svm._screen.DepthToBottom = value;
        NotifyPropertyChanged("DepthToBottom");

      }
    }

    #region OKCommand
    RelayCommand okCommand;

    /// <summary>
    /// Gets the command that loads the Mike she
    /// </summary>
    public ICommand OkCommand
    {
      get
      {
        if (okCommand == null)
        {
          okCommand = new RelayCommand(param => this.OK(), param => this.CanOK);
        }
        return okCommand;
      }
    }

    private bool CanOK 
    { 
      get
      {
        return svm._screen.DepthToBottom.HasValue & svm._screen.DepthToTop.HasValue;
      }
    }

    private void OK()
    {
      var cd = svm.CVM.ChangeController.NewScreen(svm._screen);
      CurrentChange.changeDescription.ChangeValues.Clear();
      CurrentChange.changeDescription.ChangeValues.AddRange(cd.ChangeValues);
      CurrentChange.IsApplied = true;
      svm.CVM.AddChange(CurrentChange, false);
      svm.FireEvents();


      if (RequestClose != null)
        RequestClose();
    }

    #endregion

    #region CancelCommand
    RelayCommand cancelCommand;

    /// <summary>
    /// Gets the command that loads the Mike she
    /// </summary>
    public ICommand CancelCommand
    {
      get
      {
        if (cancelCommand == null)
        {
          cancelCommand = new RelayCommand(param => this.Cancel(), param => true);
        }
        return cancelCommand;
      }
    }


    private void Cancel()
    {

      well.RemoveScreen(svm);
      if (RequestClose != null)
        RequestClose();
    }

    #endregion

    
    public ChangeDescriptionViewModel CurrentChange { get; private set; }

    public event Action RequestClose;

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

        if (name == "DepthToBottom" || name == "DepthToTop")
        {
          if (DepthToBottom < DepthToTop)
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
