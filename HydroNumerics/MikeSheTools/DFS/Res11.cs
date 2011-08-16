using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero.DFS;

namespace HydroNumerics.MikeSheTools.DFS
{
  public class Res11 : DFSBase
  {

    public List<Res11Point> Points { get; private set; }

    public Res11(string Res11FileName):base(Res11FileName)
    {

      int offset = 4;

      int nitems=Items.Count()/2;

      Points = new List<Res11Point>();
      for (int j = 0; j < nitems; j++)
      {
        
        string name = System.Text.Encoding.ASCII.GetString((byte[])StaticData[offset].Data);
        string topo = System.Text.Encoding.ASCII.GetString((byte[])StaticData[offset + 1].Data);

        PointType pt = PointType.WaterLevel;
        int waterlevelcounter = 0;
        int dischargecounter = 0;
        int itemcounter;
        Item CurrentItem;
        for (int i = 0; i < StaticData[offset + 2].ElementCount; i++)
        {
          if (pt == PointType.WaterLevel)
          {
            itemcounter = waterlevelcounter;
            CurrentItem = Items[j];
            waterlevelcounter++;
            pt = PointType.Discharge;
          }
          else
          {
            itemcounter = dischargecounter;
            CurrentItem = Items[j+nitems];
            dischargecounter++;
            pt = PointType.WaterLevel;
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
      df.Close();
    }

    internal float[] GetData(int TimeStep, int Item)
    {
      dfsdata = new float[Items[Item - 1].NumberOfElements];

      return ReadItemTimeStep(TimeStep, Item);

    }


    public enum PointType
    {
      WaterLevel,
      Discharge
    }


    public class Res11Point:HydroNumerics.Geometry.IXYPoint
    {
      private Res11 dfs;
      private Item I;
      private int ElementNumber;

      public double Chainage { get; private set; }
      public string BranchName { get; private set; }
      public string TopoID { get; private set; }
      public PointType pointType { get; set; }
      


      internal Res11Point(Res11 dfs, Item I, int ElementNumber, double chainage, string BranchName, string TopoId, double X, double Y, PointType pt)
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
        return dfs.GetData(TimeStep, I.ItemNumber)[ElementNumber];
      }



      #region IXYPoint Members


      public double X{get;set;}

      public double Y {get;set;}

      #endregion
    }
  }
}
