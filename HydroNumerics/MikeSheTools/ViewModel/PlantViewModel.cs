using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.JupiterTools;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class PlantViewModel:BaseViewModel
  {

    private Plant plant;

    public PlantViewModel(Plant plant)
    {
      this.plant = plant;
    }


    public string DisplayName
    {
      get
      {
        return plant.Name;
      }
    }

    public int IDNumber
    {
      get
      {
        return plant.IDNumber;
      }
    }

    public string Address
    {
      get
      {
        return plant.Address;
      }
    }

    public int PostalCode
    {
      get
      {
        return plant.PostalCode;
      }
    }

    public double Permit
    {
      get
      {
        return plant.Permit;
      }
    }


    /// <summary>
    /// Gets the subplants
    /// </summary>
    public IEnumerable<PlantViewModel> SubPlants
    {
      get
      {
        foreach (Plant p in plant.SubPlants)
        {
          yield return new PlantViewModel(p);
        }
      }
    }


    public IEnumerable<PumpingIntake> PumpingIntakes
    {
      get
      {
        return plant.PumpingIntakes;
      }
    }

    /// <summary>
    /// Returns the wells that the plant pumps from
    /// Carefull: these are not the actual wells since they only contain the intakes used by the plant.
    /// </summary>
    public IEnumerable<WellViewModel> Wells
    {
      get
      {
        IWellCollection iwc = new IWellCollection();

        foreach(PumpingIntake P in plant.PumpingIntakes)
        {
          IWell CurrentWell;
          if (!iwc.TryGetValue(P.Intake.well.ID, out CurrentWell))
          {
            CurrentWell = new Well(P.Intake.well.ID, P.Intake.well.X, P.Intake.well.Y);
            iwc.Add(CurrentWell);
          }

          CurrentWell.AddNewIntake(P.Intake.IDNumber);
        }

        foreach (IWell w in iwc)
          yield return new WellViewModel(w, null);
      }
    }


    /// <summary>
    /// Returns true if the plant has extractions but no intakes attached
    /// </summary>
    public bool MissingData
    {
      get
      {
        if (plant.Extractions.Items.Count > 0 & plant.PumpingIntakes.Count == 0 | Wells.Any(var=>var.MissingData==true))
          return true;
        else
          return false;
      }
    }

  }
}
