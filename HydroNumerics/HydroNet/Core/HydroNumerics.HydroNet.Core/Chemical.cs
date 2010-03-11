using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public class Chemical
  {

    public Chemical(ChemicalType Type)
    {
      this.Type = Type;
    }

    public Chemical(ChemicalType Type, double Moles):this(Type)
    {
      this.Moles = Moles;
    }

    public Chemical Split(double factor)
    {
      Chemical C = new Chemical(this.Type, factor * Moles);
      this.Moles *= (1 - factor);
      return C;
    }

    #region Properties
    /// <summary>
    /// Gets the type of chemical
    /// </summary>
    public ChemicalType Type { get; private set; }

    /// <summary>
    /// Gets and sets the moles of chemicals
    /// </summary>
    public double Moles { get; set; }

    /// <summary>
    /// Gets and sets the mass of chemical
    /// </summary>
    public double Mass
    {
      get
      {
        return Moles * Type.MolarWeight;
      }
      set
      {
        if (Mass < 0)
          throw new ArgumentOutOfRangeException("Mass of chemical cannot be negative");
        Moles = value / Type.MolarWeight;
      }
    }

    #endregion
  }
}
