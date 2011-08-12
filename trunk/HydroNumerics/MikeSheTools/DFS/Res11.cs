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

      Points = new List<Res11Point>();
      foreach (Item I in Items)
      {
        int unit;
        int j;
        float x;
        float dx;
        string name;
        var coords = new Coords[1];


        DfsDLLWrapper.dfsGetItemAxisNeqD1(I.ItemPointer, out unit, out name, out j, out coords);
        int dim =DfsDLLWrapper.dfsGetItemDim(I.ItemPointer);
        int el = DfsDLLWrapper.dfsGetItemElements(I.ItemPointer);

        for (int i = 0; i< el;i++)
        {
          Points.Add(new Res11Point(this, I, i, coords[i].X));
        }
        
      }
    }

    internal float[] GetData(int TimeStep, int Item)
    {
      dfsdata = new float[Items[Item - 1].NumberOfElements];

      return ReadItemTimeStep(TimeStep, Item);

    }





    public class Res11Point
    {
      private Res11 dfs;
      private Item I;
      private int ElementNumber;

      public double Chainage { get; private set; }
      public string BranchName { get; private set; }


      internal Res11Point(Res11 dfs, Item I, int ElementNumber, double chainage)
      {
        Chainage = chainage;
        this.dfs = dfs;
        this.I = I;
        this.ElementNumber = ElementNumber;

      }

      public double GetData(int TimeStep)
      {
        return dfs.GetData(TimeStep, I.ItemNumber)[ElementNumber];
      }


    }
  }
}
