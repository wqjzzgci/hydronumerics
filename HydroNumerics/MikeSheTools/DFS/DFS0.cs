using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero.DFS;


namespace HydroNumerics.MikeSheTools.DFS
{
  public class DFS0 : DFSBase
  {

    private Dictionary<int, List<double>> ItemValues { get;  set; }

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

      ItemValues = new Dictionary<int, List<double>>();
      for (int i = 1; i <= NumberOfItems; i++)
        ItemValues.Add(i, new List<double>());

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
      if (ItemValues == null)
        ReadData();
        return ItemValues[Item][TimeStep];
    }

    private void ReadData()
    {
      ItemValues = new Dictionary<int,List<double>>();
      for(int i = 0; i< NumberOfTimeSteps;i++)
        for (int j = 1; j <= NumberOfItems; j++)
        {
          if (i == 0)
            ItemValues.Add(j, new List<double>());
          var dfsdata = ReadItemTimeStep(i, j);
          ItemValues[j].Add(dfsdata[0]);
        }
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
      IsDirty = true;
      if (ItemValues==null) 
        ReadData();

      while (TimeStep>=ItemValues[Item].Count)
        for(int i=1;i<=NumberOfItems;i++)
          ItemValues[i].Add(0);
      ItemValues[Item][TimeStep] = Value;
    }

    private bool IsDirty = false;

    /// <summary>
    /// Sets the time of a time step
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Time"></param>
    public void SetTime(int TimeStep, DateTime Time)
    {
      IsDirty = true;
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

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (IsDirty)
        {
          for (int i = 0; i < NumberOfTimeSteps; i++)
            for (int j = 1; j <= NumberOfItems; j++)
              WriteItemTimeStep(i,j,new float[]{(float)ItemValues[j][i]});
        }
      }
      base.Dispose(disposing);
    }
  }
}
