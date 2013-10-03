using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class BranchID:IComparable
  {

    public string Branchname { get; set; }
    public double StartChainage { get; set; }

    public override string ToString()
    {
      return Branchname + " " + StartChainage;
    }

    public override bool Equals(object obj)
    {
      BranchID other = obj as BranchID;
      if (obj == null)
        return false;
      return other.Branchname == Branchname & other.StartChainage == StartChainage;
    }


    public override int GetHashCode()
    {
      return Branchname.GetHashCode() ^ StartChainage.GetHashCode();
    }



    #region IComparable Members

    public int CompareTo(object obj)
    {
      int v = string.Compare(Branchname, ((BranchID)obj).Branchname, StringComparison.Ordinal);
      if (v == 0)
        return StartChainage.CompareTo(((BranchID)obj).StartChainage);
      else 
        return v;

    }

    #endregion
  }
}
