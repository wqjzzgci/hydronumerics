using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class WellsOnPlantViewModel:BaseViewModel
  {

    public IEnumerable<WellViewModel> Wells { get; private set; }
    public Plant CurrentPlant { get; private set; }
    public ChangeDescription CurrentChange { get; private set; }

    public WellsOnPlantViewModel(IEnumerable<WellViewModel> wells, Plant plant)
    {
      Wells = wells;
      CurrentPlant = plant;
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
        }

      }
    }


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





  }
}
