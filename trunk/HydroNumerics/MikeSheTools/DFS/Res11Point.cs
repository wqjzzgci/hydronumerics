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

    /// <summary>
    /// Gets the chainage
    /// </summary>
    public double Chainage { get; private set; }
    
    /// <summary>
    /// Gets the Branchname
    /// </summary>
    public string BranchName { get; private set; }
    
    /// <summary>
    /// Gets the TopoID
    /// </summary>
    public string TopoID { get; private set; }
    
    /// <summary>
    /// Gets the type of point, i.e: Waterlevel (H) or discharge (Q)
    /// </summary>
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

    /// <summary>
    /// Gets the data for the timestep. Counts from zero
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public double GetData(int TimeStep)
    {
      return dfs.GetData(TimeStep, I.ItemNumber)[ElementNumber];
    }


    #region IXYPoint Members

    public double X { get; set; }

    public double Y { get; set; }

    #endregion
  }

}
