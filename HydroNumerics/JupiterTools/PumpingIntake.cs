using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Wells;

namespace HydroNumerics.JupiterTools
{
  /// <summary>
  /// Small class. Only holds information about when the intake is active
  /// The class is necessary because the same intake can have different active periods on different plants
  /// </summary>
  public class PumpingIntake
  {
    private Plant _plant;
    private TimespanSeries Fractions { get; set; }

    public PumpingIntake(IIntake intake, Plant plant)
    {
      Intake = intake;
      _plant = plant;
      Fractions = new TimespanSeries();
      Fractions.AllowExtrapolation = false;

      Start = DateTime.MinValue;
      End = DateTime.MaxValue;
    }


    public void SetFraction(int year, double Fraction)
    {
      double d;

      DateTime start = new DateTime(year, 1, 1);
      DateTime end = new DateTime(year, 12, 31);

      //If a value has already been provided remove it first. Both from the fractions and the actual extraction on the intake
      if (Fractions.TryGetValue(start, out d))
      {
        Intake.Extractions.AddValue(start, end, -d * _plant.Extractions.GetValue(start, end));
        Fractions.AddValue(start, end, -d);
      }

      Fractions.AddValue(start, end, Fraction);
      Intake.Extractions.AddValue(start, end, -d * _plant.Extractions.GetValue(start, end));
    }


    /// <summary>
    /// Gets the real intake
    /// </summary>
    public IIntake Intake { get; private set; }

    /// <summary>
    /// Deprecated! Use StartNullable
    /// </summary>
    public DateTime Start {get;set;}

    /// <summary>
    /// Deprecated! Use EndNullable
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    /// Gets and sets the start time for the pumping in this intake
    /// </summary>
    public DateTime? StartNullable 
    {
      get
      {
        if (Start == DateTime.MinValue)
          return null;
        else
          return Start;
      }
      set
      {
        if (value.HasValue)
          Start = value.Value;
        else
          Start = DateTime.MinValue;
      }
    }

    /// <summary>
    /// Gets and sets the end time for the pumping in this intake
    /// </summary>
    public DateTime? EndNullable
    {
      get
      {
        if (End == DateTime.MaxValue)
          return null;
        else
          return End;
      }
      set
      {
        if (value.HasValue)
          End = value.Value;
        else
          End = DateTime.MaxValue;
      }
    }

    /// <summary>
    /// returns the distance to the plant in meters
    /// </summary>
    public double DistanceToPlant
    {
      get
      {
        return Geometry.XYGeometryTools.CalculatePointToPointDistance(_plant.Location, Intake.well);
      }
    }

  }
}
