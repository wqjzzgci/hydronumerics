using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero.DFS;

namespace HydroNumerics.MikeSheTools.DFS
{
  public class Res11
  {

    private List<IDfsStaticItem> staticData;

    protected IDfsFile df;


    

    /// <summary>
    /// Gets a list with the static data. Only known use is in res11 files
    /// </summary>
    protected List<IDfsStaticItem> StaticData
    {
      get
      {
        if (staticData == null)
        {
          staticData = new List<IDfsStaticItem>();
          IDfsStaticItem s;
          while ((s = df.ReadStaticItemNext()) != null)
            staticData.Add(s);
        }
        return staticData;
      }
    }

    private string AbsoluteFileName;

    public List<Res11Point> Points { get; private set; }

    public Res11(string Res11FileName)
    {
      AbsoluteFileName = System.IO.Path.GetFullPath(Res11FileName);
      df = DHI.Generic.MikeZero.DFS.DfsFileFactory.DfsGenericOpen(AbsoluteFileName);
      TimeSteps = df.FileInfo.TimeAxis.GetDateTimes();

      for (int i = 0; i < df.FileInfo.TimeAxis.NumberOfTimeSteps; i++)
      {
        TimeSteps[i] = TimeSteps[0].AddSeconds(df.ReadItemTimeStep(1, i).TimeInSeconds(df.FileInfo.TimeAxis));
      }
        int offset = 4;
      int nitems=df.ItemInfo.Count()/2;

      Points = new List<Res11Point>();
      for (int j = 0; j < nitems; j++)
      {
        string name = System.Text.Encoding.ASCII.GetString((byte[])StaticData[offset].Data);
        string topo = System.Text.Encoding.ASCII.GetString((byte[])StaticData[offset + 1].Data);

        PointType pt = PointType.Discharge;
        int waterlevelcounter = 0;
        int dischargecounter = 0;
        int itemcounter;
        IDfsDynamicItemInfo CurrentItem;
        for (int i = 0; i < StaticData[offset + 2].ElementCount; i++)
        {
          if (pt == PointType.Discharge)
          {
            itemcounter = waterlevelcounter;
            CurrentItem = df.ItemInfo[j];
            waterlevelcounter++;
            pt = PointType.WaterLevel;
          }
          else
          {
            itemcounter = dischargecounter;
            CurrentItem = df.ItemInfo[j + nitems];
            dischargecounter++;
            pt = PointType.Discharge;
          }
          
          double chain = (double)(float)StaticData[offset + 2].Data.GetValue(i);
          double x = (double)(float)StaticData[offset + 3].Data.GetValue(i);
          double y = (double)(float)StaticData[offset + 4].Data.GetValue(i);
          Points.Add(new Res11Point(this, CurrentItem, itemcounter, chain, name, topo, x, y, pt));
        }

        int ncross = ((int[])StaticData[offset + 13].Data).Count(var => var != 0);
        offset = offset + 23 + 4 * ncross;
      }

      StaticData.Clear();
    }

    Dictionary<int, Dictionary<int, float[]>> data;


    private void ReadData()
    {
      data = new Dictionary<int, Dictionary<int, float[]>>();
      for (int i=0;i< TimeSteps.Length;i++)
        for (int j = 1; j <= df.ItemInfo.Count; j++)
        {
          if (i == 0)
            data.Add(j, new Dictionary<int,float[]>());
          data[j][i] = (float[])df.ReadItemTimeStepNext().Data;
        }
    }

    internal float[] GetData(int TimeStep, int Item)
    {
      if (data==null)
        ReadData();
      return data[Item][TimeStep];
    //  dfsdata = new float[Items[Item - 1].NumberOfElements];
      //return ReadItemTimeStep(TimeStep, Item);
    }

    public DateTime[] TimeSteps { get; private set; }

    public int GetTimeStep(DateTime TimeStamp)
    {
      if (TimeStamp < TimeSteps.First() || TimeSteps.Length == 1)
        return 0;
      
      //fixed timestep

      //Last timestep is known
        if (TimeStamp >= TimeSteps[TimeSteps.Length - 1])
          return TimeSteps.Length - 1;

        int i = 1;
        //Loop the timesteps
        while (TimeStamp > TimeSteps[i])
        {
          i++;
        }
        //Check if last one was actually closer
        if (TimeSteps[i].Subtract(TimeStamp) < TimeStamp.Subtract(TimeSteps[i - 1]))
          return i;
        else
          return i - 1;
    }

    public void Dispose()
    {
      df.Close();
      
    }
  }
}
