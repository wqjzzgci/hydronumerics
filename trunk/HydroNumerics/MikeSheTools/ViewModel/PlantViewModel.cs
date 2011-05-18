using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.JupiterTools;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class PlantViewModel:BaseViewModel
  {

    public Plant plant { get; private set; }

    private JupiterViewModel jVM;

    public PlantViewModel(Plant plant, JupiterViewModel JVM)
    {
      jVM = JVM;
      this.plant = plant;
      DisplayName = plant.Name;
      PumpingIntakes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PumpingIntakes_CollectionChanged);
    }

    void PumpingIntakes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (var V in e.NewItems)
        {
          PumpingIntake PI = V as PumpingIntake;
          plant.PumpingIntakes.Add(PI);
        }
      }

      if (e.OldItems != null)
      {
        foreach (var V in e.OldItems)
        {
          PumpingIntake PI = V as PumpingIntake;
          plant.PumpingIntakes.Remove(PI);
        }
      }
      NotifyPropertyChanged("MissingData");
      wells = null;
      NotifyPropertyChanged("Wells");
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



    private ObservableCollection<PlantViewModel> subPlants;
    /// <summary>
    /// Gets the subplants
    /// </summary>
    public ObservableCollection<PlantViewModel> SubPlants
    {
      get
      {
        if (subPlants == null)
        {
          subPlants = new ObservableCollection<PlantViewModel>();
          foreach (Plant p in plant.SubPlants)
          {
            subPlants.Add(new PlantViewModel(p, jVM));
          }
        }
        return subPlants;
      }
    }

    /// <summary>
    /// Returns true if the plant has subplants
    /// </summary>
    public bool HasSubPlants
    {
      get
      {
        return plant.SubPlants.Count > 0;
      }
    }

    /// <summary>
    /// Returns true if there are no active wells
    /// </summary>
    public bool NoActiveWells
    {
      get
      {
        return plant.PumpingWells.Count(var => var.UsedForExtraction) == 0;
      }
    }

    public string ActiveWellsString
    {
      get
      {
        return plant.PumpingWells.Count(var => var.UsedForExtraction).ToString() + " out of " + plant.PumpingWells.Count.ToString() + " wells active";
      }
    }

   

    private ObservableCollection<PumpingIntake> pumpingIntakes;

    /// <summary>
    /// Gets the collection of pumping intakes.
    /// </summary>
    public ObservableCollection<PumpingIntake> PumpingIntakes
    {
      get
      {
        if (pumpingIntakes == null)
          pumpingIntakes = new ObservableCollection<PumpingIntake>(plant.PumpingIntakes);
        return pumpingIntakes;
      }
    }

    /// <summary>
    /// Gets the active pumping intakes. Removes wells not used for extraction and wells with missing data
    /// </summary>
    public IEnumerable<PumpingIntake> ActivePumpingIntakes
    {
      get
      {
        return PumpingIntakes.Where(var=>var.Intake.well.UsedForExtraction & !var.Intake.well.HasMissingData());
      }
    }



    private ObservableCollection<WellViewModel> wells;
    /// <summary>
    /// Returns the wells that the plant pumps from. The wells will include all intakes!
    /// </summary>
    public ObservableCollection<WellViewModel> Wells
    {
      get
      {
        if (wells == null)
        {
          wells = new ObservableCollection<WellViewModel>();
          foreach (PumpingIntake P in plant.PumpingIntakes)
          {
            WellViewModel w = new WellViewModel(P.Intake.well, jVM.CVM);

            if (!wells.Contains(w))
              wells.Add(w);
          }
        }
        return wells;
      }
    }


    public string URL
    {
      get
      {
        return "http://jupiter.geus.dk/JupiterWWW/boreServlet?redel=AnlaegRapport&anlaegid=" + IDNumber.ToString();
      }
    }

    /// <summary>
    /// Returns true if the plant has extractions but no intakes attached
    /// </summary>
    public bool MissingData
    {
      get
      {
        if (plant.Extractions.Items.Count > 0 & plant.PumpingWells.Count(var=>var.UsedForExtraction) == 0 | Wells.Any(var=>var.MissingData==true))
          return true;
        else
          return false;
      }
    }
  }
}
