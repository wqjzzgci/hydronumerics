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
    public PlantViewModel CurrentPlant { get; private set; }
    public ChangeDescriptionViewModel CurrentChange { get; private set; }

    public WellsOnPlantViewModel(IEnumerable<WellViewModel> wells, PlantViewModel plant)
    {
      Wells = wells;
      CurrentPlant = plant;

      ChangeDescription cd = new ChangeDescription(JupiterTables.DRWPLANTINTAKE);
      cd.Date = DateTime.Now;

      List<ICollection<string>> vals = new List<ICollection<string>>();
      
      List<string> first = new List<string>();
      first.Add("FirstChoice");
      first.Add("SecondChoice");
      vals.Add(first);

      CurrentChange = new ChangeDescriptionViewModel(cd, vals);

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
