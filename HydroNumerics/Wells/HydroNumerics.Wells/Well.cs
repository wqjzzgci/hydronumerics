using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using HydroNumerics.Geometry;

namespace HydroNumerics.Wells
{
  /// <summary>
  /// A small class holding typical data to describe a well
  /// </summary>
  [DataContract]
  public class Well : IWell,IEquatable<IWell>
  {

    protected string _id;
    protected string _description;
    protected double _terrain;
    public XYPoint Location { get; set; }

    /// <summary>
    /// Gets and sets the Depth
    /// </summary>
    public double? Depth { get; set; }
    public bool UsedForExtraction { get; set; }

    public IEnumerable<IIntake> Intakes
    {
      get
      {
        return _intakes;
        
      }
    }
    protected List<IIntake> _intakes = new List<IIntake>();

    #region Constructors

    public Well()
    {
      Location = new XYPoint();
    }

    public Well(string ID):this()
    {
      _id = ID;
    }

    public Well(string ID, double X, double Y):this(ID)
    {
      Location.X = X;
      Location.Y = Y;
    }
    #endregion

    /// <summary>
    /// Adds a new intake to the well
    /// </summary>
    /// <param name="IDNumber"></param>
    /// <returns></returns>
    public virtual IIntake AddNewIntake(int IDNumber)
    {
      Intake I = new Intake(this, IDNumber);
      _intakes.Add(I);
      return I;
    }

    public override string ToString()
    {
      return _id;
    }

    #region Properties

    /// <summary>
    /// Gets and sets the x-coodinate. Deprecated, use Location property instead
    /// </summary>
    [DataMember]
    public double X
    {
      get { return Location.X; }
      set { Location.X = value; }
    }

    /// <summary>
    /// Gets and sets the y-coodinate. Deprecated, use Location property instead
    /// </summary>
    [DataMember]
    public double Y
    {
      get { return Location.Y;}
      set { Location.Y = value; }
    }

    /// <summary>
    /// Gets and sets the ID of the well
    /// </summary>
    [DataMember]
    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    /// <summary>
    /// Gets and sets a description
    /// </summary>
    [DataMember]
    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }

    /// <summary>
    /// Gets and sets the terrain in meters above mean sea level
    /// </summary>
    [DataMember]
    public double Terrain
    {
      get { return _terrain; }
      set { _terrain = value; }
    }

    #endregion

    #region IEquatable<IWell> Members

    public bool Equals(IWell other)
    {
      return ID.Equals(other.ID);
    }

    #endregion
  }
}
