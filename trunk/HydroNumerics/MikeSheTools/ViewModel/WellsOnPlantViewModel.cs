using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

using HydroNumerics.Core.WPF;
using HydroNumerics.Geometry;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class WellsOnPlantViewModel:BaseViewModel
  {

    public IEnumerable<WellViewModel> Wells { get; private set; }
    public PlantViewModel CurrentPlant { get; private set; }
    private ChangesViewModel CVM;
    private IEnumerable<WellViewModel> AllWells;

    public event Action RequestClose;

    PumpingIntake IntakeAdded = null;
    PumpingIntake IntakeRemoved = null;
    ChangeDescription StartDateChange = null;
    ChangeDescription EndDateChange = null;

    public WellsOnPlantViewModel(IEnumerable<WellViewModel> wells, PlantViewModel plant, ChangesViewModel cvm)
    {
      CVM = cvm;
      AllWells = wells;
      CurrentPlant = plant;
      BuildWellList();
      CanApply = false;
      DisplayName = plant.DisplayName;
      CurrentChange = new ChangeDescriptionViewModel(cvm.ChangeController.GetGenericPlantIntake());
    }

    private void BuildWellList()
    {
      double distanceInMeters = SearchDistance * 1000;
      Wells = AllWells.Where(var => XYGeometryTools.CalculatePointToPointDistance(var, CurrentPlant.plant) <= distanceInMeters);
        NotifyPropertyChanged("Wells");
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
        bool toreturn = CurrentIntake != null & IntakeRemoved == null;
        if (IntakeAdded!=null)
        {
          toreturn =false;
          if (IntakeAdded == CurrentIntake)
            toreturn = true;
          }
        return toreturn;
      }
    }

    private void RemoveIntake()
    {
      if (IntakeAdded == null)
      {
        IntakeRemoved = CurrentIntake;
        CanApply = true;
      }
      else //This was an intake added previously
      {
        IntakeAdded = null;
        CanApply = false;
      }
      CurrentPlant.PumpingIntakes.Remove(CurrentIntake);
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
        return SelectedIntake != null & IntakeAdded==null;
      }
    }

    private void AddIntake()
    {
      IntakeAdded = new PumpingIntake(SelectedIntake, CurrentPlant.plant);
      CurrentPlant.PumpingIntakes.Add(IntakeAdded);
      CurrentIntake = IntakeAdded;

      CanApply = true;
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

    private bool CanApply { get; set; }

    private void Apply()
    {
      if (IntakeAdded != null)
      {
        ChangeDescription cd = CVM.ChangeController.AddIntakeToPlant(IntakeAdded, CurrentPlant.plant);
        CurrentChange.changeDescription.Action = cd.Action;
        CurrentChange.changeDescription.ChangeValues = cd.ChangeValues;
        CurrentChange.changeDescription.PrimaryKeys = cd.PrimaryKeys;
      }
      else if (IntakeRemoved != null)
      {
        ChangeDescription cd = CVM.ChangeController.RemoveIntakeFromPlant(IntakeRemoved, CurrentPlant.plant);
        CurrentChange.changeDescription.Action = cd.Action;
        CurrentChange.changeDescription.ChangeValues = cd.ChangeValues;
        CurrentChange.changeDescription.PrimaryKeys = cd.PrimaryKeys;
      }
      else //Only the dates have been changed
      {

        if (StartDateChange != null)
        {
          if (StartDateChange.PrimaryKeys.Values.First() == "") //It is a change of change
          {
            ChangeDescription cd = CVM.ChangeController.AddIntakeToPlant(CurrentIntake, CurrentPlant.plant);
            CurrentChange.changeDescription.Action = TableAction.EditValue;
            CurrentChange.changeDescription.ChangeValues = cd.ChangeValues;
            CurrentChange.changeDescription.PrimaryKeys = cd.PrimaryKeys;
          }
          else
            CurrentChange.changeDescription = StartDateChange;
          if (EndDateChange != null)
            CurrentChange.changeDescription.ChangeValues.Add(EndDateChange.ChangeValues[0]);
        }
        else if (EndDateChange != null)
        {
          if (EndDateChange.PrimaryKeys.Values.First() == "") //It is a change of change
          {
            ChangeDescription cd = CVM.ChangeController.AddIntakeToPlant(CurrentIntake, CurrentPlant.plant);
            CurrentChange.changeDescription.Action = TableAction.EditValue;
            CurrentChange.changeDescription.ChangeValues = cd.ChangeValues;
            CurrentChange.changeDescription.PrimaryKeys = cd.PrimaryKeys;
          }
          else
            CurrentChange.changeDescription = EndDateChange;
        }
      }
      //call distribute extraction again.
      CurrentPlant.plant.DistributeExtraction(true);

        CurrentChange.IsApplied = true;
        CVM.AddChange(CurrentChange, false);
      CurrentChange = new ChangeDescriptionViewModel(CVM.ChangeController.GetGenericPlantIntake());
      IntakeRemoved = null;
      IntakeAdded = null;
      EndDateChange = null;
      StartDateChange = null;
      CanApply = false;
    }

    RelayCommand okCommand;
    public ICommand OkCommand
    {
      get
      {
        if (okCommand == null)
          okCommand = new RelayCommand(param => RequestCloseAndSave(), param=>true);
        return okCommand;
      }
    }

    private void RequestCloseAndSave()
    {
      if (CanApply)
        Apply();

      if (RequestClose != null)
        RequestClose();
    }

    public void Cancel()
    {
      if (IntakeAdded != null)
        CurrentPlant.PumpingIntakes.Remove(IntakeAdded);
      if (IntakeRemoved != null)
        CurrentPlant.PumpingIntakes.Add(IntakeRemoved);
      if (StartDateChange != null)
      {

      }
      if (EndDateChange!= null)
      {

      }

    }


    private ChangeDescriptionViewModel changeViewModel = null;
    public ChangeDescriptionViewModel CurrentChange
    {
      get
      {
        return changeViewModel;
      }
      private set
      {
        if (changeViewModel != value)
        {
          changeViewModel = value;
          NotifyPropertyChanged("CurrentChange");
        }
      }
    }


    private double searchDistance = 5;
    /// <summary>
    /// Gets and sets the search distance for the wells. In kilometers!
    /// </summary>
    public double SearchDistance
    {
      get { return searchDistance; }
      set {
        if (searchDistance != value)
        {
          searchDistance = value;
          NotifyPropertyChanged("SearchDistance");
          BuildWellList();
        }
      }
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
          CanApply = true;

         StartDateChange =  CVM.ChangeController.ChangeStartDateOnPumpingIntake(CurrentIntake, CurrentPlant.plant, value.Value);
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
          CanApply = true;
          EndDateChange = CVM.ChangeController.ChangeEndDateOnPumpingIntake(CurrentIntake, CurrentPlant.plant, value.Value);
          CurrentIntake.EndNullable = value;
          NotifyPropertyChanged("EndDate");
        }
      }
    }
  }
}
