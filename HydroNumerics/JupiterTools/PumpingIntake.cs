using System;
using System.Collections.Generic;
using System.ComponentModel;
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
  public class PumpingIntake:INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private Plant _plant;

    public PumpingIntake(IIntake intake, Plant plant)
    {
      Intake = intake;
      _plant = plant;
    }

    /// <summary>
    /// Gets the real intake
    /// </summary>
    public IIntake Intake { get; private set; }


    private DateTime? start;
    /// <summary>
    /// Gets and sets the start time for the pumping in this intake
    /// </summary>
    public DateTime? StartNullable 
    {
      get
      {
        return start;
      }
      set
      {
        if (start != value)
        {
          start = value;
          NotifyPropertyChanged("StartNullable");
        }
      }
    }

    private DateTime? end;
    /// <summary>
    /// Gets and sets the end time for the pumping in this intake
    /// </summary>
    public DateTime? EndNullable
    {
      get
      {
        return end;
      }
      set
      {
        if (end != value)
        {
          end = value;
          NotifyPropertyChanged("EndNullable");
        }
      }
    }

    /// <summary>
    /// returns the distance to the plant in meters
    /// </summary>
    public double DistanceToPlant
    {
      get
      {
        return Geometry.XYGeometryTools.CalculatePointToPointDistance(_plant, Intake.well);
      }
    }

    protected void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }


  }
}
