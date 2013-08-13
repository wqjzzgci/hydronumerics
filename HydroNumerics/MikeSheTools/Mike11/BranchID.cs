using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class BranchID:IEquatable<BranchID>,IComparable<BranchID>
  {

    public string Branchname { get; set; }
    public double StartChainage { get; set; }

    public override string ToString()
    {
      return Branchname + " " + StartChainage;
    }

    #region IEquatable<BranchID> Members

    public bool Equals(BranchID other)
    {
      return other.Branchname == Branchname & other.StartChainage == StartChainage;
    }

    #endregion

    #region IComparable<BranchID> Members

    public int CompareTo(BranchID other)
    {

      int v = Branchname.CompareTo(other.Branchname);
      if (v == 0)
        return StartChainage.CompareTo(other.StartChainage);
      else
        return v;
    }

    #endregion
  }
}
