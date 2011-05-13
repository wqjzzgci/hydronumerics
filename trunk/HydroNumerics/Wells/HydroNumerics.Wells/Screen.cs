using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace HydroNumerics.Wells
{
  [DataContract]
  public class Screen
  {
    private IIntake _intake;

    /// <summary>
    /// Gets the intake where the screen is located
    /// </summary>
    public IIntake Intake
    {
      get
      {
        return _intake;
      }
    }

    [DataMember]
    public int Number { get; set; }

    public Screen(IIntake Intake)
    {
      _intake = Intake;
      _intake.Screens.Add(this);
    }


    /// <summary>
    /// Gets and sets the depth to the top in meters below surface
    /// </summary>
    [DataMember]
    public double? DepthToTop{get;set;}

    /// <summary>
    /// Gets and sets the depth to the bottom in meters below surface
    /// </summary>
    [DataMember]
    public double? DepthToBottom { get; set; }

    /// <summary>
    /// Gets and sets the top in meters above sea level
    /// This property requires that the terrain level of the well is set.
    /// </summary>
    public double? TopAsKote
    {
      get 
      {
        if (!DepthToTop.HasValue)
          return null;
        return _intake.well.Terrain - DepthToTop.Value; 
      }
      set
      {
        DepthToTop = _intake.well.Terrain - value;
      }
    }

    /// <summary>
    /// Gets and sets the bottom in meters above sea level.
    /// This property requires that the terrain level of the well is set.
    /// </summary>
    public double? BottomAsKote
    {
      get 
      {
        if (!DepthToBottom.HasValue)
          return null;
        
        return _intake.well.Terrain - DepthToBottom.Value; 
      }
      set
      {
        DepthToBottom = _intake.well.Terrain - value;
      }
    }

    public override string ToString()
    {
      return _intake.ToString() + "_" + Number;
    }

  }
}
