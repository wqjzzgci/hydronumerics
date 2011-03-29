using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.DFS
{
  public class DFS0 : DFSBase
  {
    public DFS0(string DFSFileName)
      : base(DFSFileName)
    {

    }

    public DFS0(string DFSFileName, int NumberOfItems)
      : base(DFSFileName, NumberOfItems)
    {

    }

    /// <summary>
    /// Gets the value for the Time step and Item
    /// TimeStep counts from zero, Item counts from 1
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Item"></param>
    /// <returns></returns>
    public double GetData(int TimeStep, int Item)
    {
      ReadItemTimeStep(TimeStep, Item);
      return dfsdata[0];
    }

    /// <summary>
    /// Gets the value for the Time step and Item
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Item"></param>
    /// <returns></returns>
    public double GetData(DateTime TimeStep, int Item)
    {
      return GetData(GetTimeStep(TimeStep), Item);
    }

    /// <summary>
    /// Sets the value for the Time step and Item
    /// TimeStep counts from zero, Item counts from 1
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Item"></param>
    /// <param name="Value"></param>
    public void SetData(int TimeStep, int Item, double Value)
    {
      WriteItemTimeStep(TimeStep, Item, new float[]{(float) Value});
    }
  }
}
