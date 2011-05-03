using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;


using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class WellsOnPlantViewModel:BaseViewModel
  {

    public IEnumerable<WellViewModel> Wells { get; private set; }
    public PlantViewModel CurrentPlant { get; private set; }

    private ChangeDescriptionViewModel changeViewModel = null;
    public ChangeDescriptionViewModel CurrentChange
    {
      get
      {
        return changeViewModel;
      }
      set
      {
        if (changeViewModel != value)
        {
          changeViewModel = value;
          NotifyPropertyChanged("CurrentChange");
        }
      }
    }
  

    private ChangesViewModel CVM;

    public WellsOnPlantViewModel(IEnumerable<WellViewModel> wells, PlantViewModel plant, ChangesViewModel cvm)
    {
      CVM = cvm;
      Wells = wells;
      CurrentPlant = plant;
    }


    RelayCommand removeIntake;
    public ICommand RemoveIntakeCommand
    {
      get
      {
        if (removeIntake == null)
          removeIntake = new RelayCommand(param => RemoveIntake(), param => CanRemoveIntake);
        return removeIntake;
      }
    }

    private bool CanRemoveIntake
    {
      get
      {
        return CurrentIntake != null & !CanApply;
      }
    }

    private void RemoveIntake()
    {
      ChangeDescription cd = CVM.ChangeController.RemoveIntakeFromPlant(CurrentIntake, CurrentPlant.plant);
     CurrentChange = new ChangeDescriptionViewModel(cd);
     CurrentChange.Description = "Removing intake: " + CurrentIntake.Intake.ToString() + " from " + CurrentPlant.DisplayName;
    }

    RelayCommand addIntake;
    public ICommand AddIntakeCommand
    {
      get
      {
        if (addIntake == null)
          addIntake = new RelayCommand(param => AddIntake(), param => CanAddIntake);
        return addIntake;
      }
    }

    private bool CanAddIntake
    {
      get
      {
        return SelectedIntake != null & !CanApply;
      }
    }

    private void AddIntake()
    {
      PumpingIntake p = new PumpingIntake(SelectedIntake, CurrentPlant.plant);
      ChangeDescription cd = CVM.ChangeController.AddIntakeToPlant(p, CurrentPlant.plant);
      CurrentChange = new ChangeDescriptionViewModel(cd);
      CurrentChange.Description = "Adding intake: " + SelectedIntake.ToString() + " to " + CurrentPlant.DisplayName;
    }

    RelayCommand applyCommand;
    public ICommand ApplyCommand
    {
      get
      {
        if (applyCommand == null)
          applyCommand = new RelayCommand(param => Apply(), param => CanApply);
        return applyCommand;
      }
    }

    private bool CanApply
    {
      get
      {
        return CurrentChange != null;
      }
    }

    private void Apply()
    {
      switch (CurrentChange.changeDescription.Action)
      {
        case TableAction.EditValue:
          break;
        case TableAction.DeleteRow:
          CurrentPlant.PumpingIntakes.Remove(CurrentIntake);
          break;
        case TableAction.InsertRow:
          PumpingIntake P = new PumpingIntake(SelectedIntake, CurrentPlant.plant);
          CurrentPlant.PumpingIntakes.Add(P);
          break;
        default:
          break;
      }

      CurrentChange.IsApplied = true;
      CVM.AddChange(CurrentChange);
      CurrentChange = null;
    }

    private IIntake selectedIntake;
    public IIntake SelectedIntake
    {
      get
      {
        return selectedIntake;
      }
      set
      {
        if (selectedIntake != value)
        {
          selectedIntake = value;
          NotifyPropertyChanged("SelectedIntake");
        }
      }
    }


    private PumpingIntake currentIntake;
    public PumpingIntake CurrentIntake
    {
      get
      {
        return currentIntake;
      }
      set
      {
        if (currentIntake != value)
        {
          currentIntake = value;
          NotifyPropertyChanged("CurrentIntake");
          NotifyPropertyChanged("StartDate");
          NotifyPropertyChanged("EndDate");
        }

      }
    }

    /// <summary>
    /// Gets and sets the start date of the selected intake
    /// </summary>
    public DateTime? StartDate
    {
      get
      {
        if (CurrentIntake == null)
          return null;
        return CurrentIntake.StartNullable;
      }
      set
      {
        if (CurrentIntake.StartNullable != value)
        {
          CurrentIntake.StartNullable = value;
          NotifyPropertyChanged("StartDate");
        }
      }
    }

    /// <summary>
    /// Gets and sets the end date of the selected intake
    /// </summary>
    public DateTime? EndDate
    {
      get
      {
        if (CurrentIntake == null)
          return null;
        return CurrentIntake.EndNullable;
      }
      set
      {
        if (CurrentIntake.EndNullable != value)
        {
          CurrentIntake.EndNullable = value;
          NotifyPropertyChanged("EndDate");
        }
      }
    }




  }
}
