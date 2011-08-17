using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero.DFS;

namespace HydroNumerics.MikeSheTools.DFS
{
  public enum PointType
  {
    WaterLevel,
    Discharge
  }


  public class Res11Point : HydroNumerics.Geometry.IXYPoint
  {
    private Res11 dfs;
    private IDfsDynamicItemInfo I;
    private int ElementNumber;

    public double Chainage { get; private set; }
    public string BranchName { get; private set; }
    public string TopoID { get; private set; }
    public PointType pointType { get; set; }


    internal Res11Point(Res11 dfs, IDfsDynamicItemInfo I, int ElementNumber, double chainage, string BranchName, string TopoId, double X, double Y, PointType pt)
    {
      Chainage = chainage;
      this.dfs = dfs;
      this.I = I;
      this.ElementNumber = ElementNumber;
      this.BranchName = BranchName;
      this.TopoID = TopoId;
      this.X = X;
      this.Y = Y;
      this.pointType = pt;
    }

    public double GetData(int TimeStep)
    {
      //return dfs.GetData(TimeStep, I.ItemNumber)[0];
      return dfs.GetData(TimeStep, I.ItemNumber)[ElementNumber];
    }



    #region IXYPoint Members

    public double X { get; set; }

    public double Y { get; set; }

    #endregion
  }

}
