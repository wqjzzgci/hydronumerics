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
    private double _depthToBottom;
    private double _depthToTop;
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
    /// Returns true if one of the depths are below -990
    /// </summary>
    public bool MissingData
    {
      get
      {
        return TopAsKote < -990 || BottomAsKote < -990 || DepthToBottom < -990 || DepthToTop < -990;
      }
    }

    /// <summary>
    /// Gets and sets the depth to the top in meters below surface
    /// </summary>
    [DataMember]
    public double DepthToTop
    {
      get { return _depthToTop; }
      set { _depthToTop = value; }
    }

    /// <summary>
    /// Gets and sets the depth to the bottom in meters below surface
    /// </summary>
    [DataMember]
    public double DepthToBottom
    {
      get { return _depthToBottom; }
      set { _depthToBottom = value; }
    }

    /// <summary>
    /// Gets and sets the top in meters above sea level
    /// This property requires that the terrain level of the well is set.
    /// </summary>
    public double TopAsKote
    {
      get { return _intake.well.Terrain - _depthToTop; }
      set
      {
        _depthToTop = _intake.well.Terrain - value;
      }
    }

    /// <summary>
    /// Gets and sets the bottom in meters above sea level.
    /// This property requires that the terrain level of the well is set.
    /// </summary>
    public double BottomAsKote
    {
      get { return _intake.well.Terrain - _depthToBottom; }
      set
      {
        _depthToBottom = _intake.well.Terrain - value;
      }
    }

    public override string ToString()
    {
      return _intake.ToString() + "_" + Number;
    }

  }
}
