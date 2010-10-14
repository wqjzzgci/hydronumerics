using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HydroNumerics.JupiterTools
{
  /// <summary>
  /// A small class holding the data from a Jupiter lithology sample. Sorts by depth.
  /// </summary>
  public class Lithology:IComparable<Lithology>
  {
    [DataMember]
    public double Top { get; set; }
    [DataMember]
    public double Bottom { get; set; }
    [DataMember]
    public string RockType { get; set; }
    [DataMember]
    public string RockSymbol { get; set; }
    [DataMember]
    public string TotalDescription { get; set; }

    public Lithology()
    { 
    }

    public override string ToString()
    {
      return Top + " - " + Bottom + ": "+ RockSymbol;
    }


    #region IComparable<Lithology> Members

    public int CompareTo(Lithology other)
    {
      return Top.CompareTo(other.Top);
    }

    #endregion
  }
}
