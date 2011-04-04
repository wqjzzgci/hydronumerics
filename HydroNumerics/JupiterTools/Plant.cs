using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Wells;
using HydroNumerics.Time.Core;

namespace HydroNumerics.JupiterTools
{
  /// <summary>
  /// A class representing a water production plant. Holds the historical pumping time series and the associated wells
  /// </summary>
  public class Plant:IComparable<Plant>
  {

    #region Properties

    /// <summary>
    /// Unique ID number
    /// </summary>
    public int IDNumber { get; private set; }

    /// <summary>
    /// Timeseries with extraction rates. 
    /// </summary>
    public TimespanSeries Extractions { get; private set; }

    /// <summary>
    /// Time series with extraction from surface water.
    /// </summary>
    public TimespanSeries SurfaceWaterExtrations { get; private set; }
    
    /// <summary>
    /// The intakes associated to this plant
    /// </summary>
    public BindingList<PumpingIntake> PumpingIntakes { get; private set; }
    private bool PumpingIntakesChanged = true;
    private IWellCollection wells;

    public IWellCollection PumpingWells
    {
      get
      {
        if (PumpingIntakesChanged)
        {
          wells = new IWellCollection();
          foreach (PumpingIntake PI in PumpingIntakes)
            if (!wells.Contains(PI.Intake.well))
              wells.Add(PI.Intake.well);
        }
        return wells;
      }
    }

    /// <summary>
    /// Returns true if the plant has extractions but no intakes attached
    /// </summary>
    public bool MissingData
    {
      get
      {
        if (this.Extractions.Items.Count > 0 & PumpingIntakes.Count == 0)
          return true;
        else
          return false;
      }
    }

    public List<Plant> SubPlants { get; private set; }

    /// <summary>
    /// The name of the plant
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The street where the plant is located
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// The postal code
    /// </summary>
    public int PostalCode { get; set; }

    /// <summary>
    /// The permit was given at this date
    /// </summary>
    public DateTime PermitDate { get; set; }

    /// <summary>
    /// The date at which this permit expires
    /// </summary>
    public DateTime PermitExpiryDate { get; set; }

    /// <summary>
    /// The yearly permit in m3
    /// </summary>
    public double Permit { get; set; }

    #endregion

    public Plant(int IDNumber)
    {
      Extractions = new TimespanSeries();

      PumpingIntakes = new BindingList<PumpingIntake>();

      PumpingIntakes.ListChanged += new ListChangedEventHandler(PumpingIntakes_ListChanged);
      
      SurfaceWaterExtrations = new TimespanSeries();
      SubPlants = new List<Plant>();
      this.IDNumber = IDNumber;
    }


    /// <summary>
    /// Distributes the extractions evenly on the active intakes
    /// </summary>
    public void DistributeExtraction()
    {
      Extractions.Sort();

      //The function to determine if an intake is active
      //The well should be a pumping well and start and end date should cover the year
      Func<PumpingIntake, int, bool> IsActive = new Func<PumpingIntake, int, bool>((var, var2) => var.Intake.well.UsedForExtraction & var.Start.Year <= var2 & var.End.Year >= var2);

      double[] fractions = new double[Extractions.Items.Count()];

      //Calculate the fractions based on how many intakes are active for a particular year.
      for (int i = 0; i < Extractions.Items.Count(); i++)
      {
        int CurrentYear = Extractions.Items[i].StartTime.Year;
        fractions[i] = 1.0 / PumpingIntakes.Count(var => IsActive(var, CurrentYear));
      }

      //Now loop the extraction values
      for (int i = 0; i < Extractions.Items.Count(); i++)
      {
        TimespanValue tsv = new TimespanValue(Extractions.Items[i].StartTime, Extractions.Items[i].EndTime, Extractions.Items[i].Value * fractions[i]);

        //Now loop the intakes
        foreach (PumpingIntake PI in PumpingIntakes)
        {
          IIntake I = PI.Intake;
          //Is it an extraction well?
          if (IsActive(PI, Extractions.Items[i].StartTime.Year))
          {
            I.Extractions.AddValue(tsv.StartTime, tsv.EndTime, tsv.Value);

          }
        }
      }
    }

    void PumpingIntakes_ListChanged(object sender, ListChangedEventArgs e)
    {
      PumpingIntakesChanged = true;
    }


    public override string ToString()
    {
      if (Name != null)
        return Name;
      else
        return IDNumber.ToString();
    }

    #region IComparable<Plant> Members

    /// <summary>
    /// Compares the name
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Plant other)
    {
      return Name.CompareTo(other.Name);
    }

    #endregion
  }
}
