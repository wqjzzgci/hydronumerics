using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  public class NetworkBuilder
  {
    static int UniqueIDs = 0;

    public static List<Stream> CreateBranch(int numberofWbs)
    {
      List<Stream> Branch = new List<Stream>();
      for (int i = 0; i < numberofWbs; i++)
      {
        XYPolyline line = new XYPolyline();
        line.Points.Add(new XYPoint(i, i));
        line.Points.Add(new XYPoint(i+1, i+1));
        Branch.Add(new Stream(line, 1, 1));
        if (i > 0)
        {
          Branch[i - 1].AddDownStreamWaterBody(Branch[i]);
        }
      }
      return Branch;
    }

    public static List<IWaterBody> CreateCombo(int númberofWbs, double vol)
    {
      List<IWaterBody> wbs = new List<IWaterBody>();

      XYPolyline line = new XYPolyline();
      line.Points.Add(new XYPoint(0, 0));
      line.Points.Add(new XYPoint(1, 1));

      wbs.Add(new Stream(line, 1, 1));
      for (int i = 1; i < númberofWbs; )
      {
        wbs.Add(new Lake(vol));
        wbs[i - 1].AddDownStreamWaterBody(wbs[i]);
        line = new XYPolyline();
        line.Points.Add(new XYPoint(i, i));
        line.Points.Add(new XYPoint(i+1, i+1));
        wbs.Add(new Stream(line, 1, 1));
        wbs[i].AddDownStreamWaterBody(wbs[i+1]);
        i = i + 2;
      }
      return wbs;
    }


    public static List<Lake> CreateConnectedLakes(int numberofWbs)
    {
      List<Lake> Branch = new List<Lake>();
      for (int i = 0; i < numberofWbs; i++)
      {
        Branch.Add(new Lake(100));
        if (i > 0)
        {
          Branch[i - 1].AddDownStreamWaterBody(Branch[i]);
        }
      }
      return Branch;
    }


     public static List<Stream> CreateSortedYBranch(int NumberOWBsInEachBranch, IWaterSinkSource LeftUpperBoundary, IWaterSinkSource RightUpperBoundary)
    {

      List<Stream> YBranch = new List<Stream>();
      var B1 = CreateBranch(NumberOWBsInEachBranch);
      var B2 = CreateBranch(NumberOWBsInEachBranch);
      var B3 = CreateBranch(NumberOWBsInEachBranch);
      B1.Last().AddDownStreamWaterBody(B3.First());
      B2.Last().AddDownStreamWaterBody(B3.First());

      B1.First().SinkSources.Add(LeftUpperBoundary);
      B2.First().SinkSources.Add(RightUpperBoundary);
      YBranch.AddRange(B1);
      YBranch.AddRange(B2);
      YBranch.AddRange(B3);

      return YBranch;

    }
  }
}
