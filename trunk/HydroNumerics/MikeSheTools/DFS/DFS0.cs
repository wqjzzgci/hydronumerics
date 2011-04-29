using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero.DFS;


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
      //Create the header
      _headerPointer = DfsDLLWrapper.dfsHeaderCreate(FileType.NeqtimeFixedspaceAllitems, "Title", "HydroNumerics", 1, NumberOfItems, StatType.RegularStat);
      _timeAxis = TimeAxisType.CalendarNonEquidistant;
      _spaceAxis = SpaceAxisType.EqD0;
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


    /// <summary>
    /// Sets the time of a time step
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Time"></param>
    public void SetTime(int TimeStep, DateTime Time)
    {
      if (TimeSteps.Count > TimeStep)
      {
        TimeSteps[TimeStep] = Time;
        double d = GetData(TimeStep, 1);
        SetData(TimeStep, 1, d);
      }
      else if (TimeSteps.Count == TimeStep)
        TimeSteps.Add(Time);

      if (TimeStep == 0)
        WriteTime();
    }
  }
}
