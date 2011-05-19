using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScreenAdderViewModel:BaseViewModel
  {
    private WellViewModel well;
    public ScreenAdderViewModel(WellViewModel well)
    {
      this.well = well;
      well.AddScreen();
      screen = well.Screens.Last();
      CurrentChange = new ChangeDescriptionViewModel(screen.CVM.ChangeController.NewScreen(screen._screen));
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

    public ScreenViewModel screen { get; private set; }

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
        return screen._screen.DepthToBottom.HasValue & screen._screen.DepthToTop.HasValue;
      }
    }

    ChangeDescriptionViewModel CurrentChange;

    private void OK()
    {
      var cd = screen.CVM.ChangeController.NewScreen(screen._screen);
      CurrentChange.changeDescription.ChangeValues.Clear();
      CurrentChange.changeDescription.ChangeValues.AddRange(cd.ChangeValues);
      CurrentChange.IsApplied = true;
      screen.CVM.AddChange(CurrentChange, false);
    }

  }
}
