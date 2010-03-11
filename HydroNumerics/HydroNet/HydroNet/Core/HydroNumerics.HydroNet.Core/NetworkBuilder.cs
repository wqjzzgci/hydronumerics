using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public class NetworkBuilder
  {
    static int UniqueIDs = 0;

    public static List<IWaterBody> CreateBranch(int numberofWbs)
    {
      List<IWaterBody> Branch = new List<IWaterBody>();
      for (int i = 0; i < numberofWbs; i++)
      {
        Stream S = new Stream(new WaterPacket(100));
        S.ID = UniqueIDs++;
        Branch.Add(S);
        if (i > 0)
        {
          Branch[i - 1].AddDownstreamConnection(Branch[i]);
        }

      }
      return Branch;
    }


    public static List<IWaterBody> CreateSortedYBranch(int NumberOWBsInEachBranch, IWaterSinkSource LeftUpperBoundary, IWaterSinkSource RightUpperBoundary)
    {

      List<IWaterBody> YBranch = new List<IWaterBody>();
      var B1 = CreateBranch(NumberOWBsInEachBranch);
      var B2 = CreateBranch(NumberOWBsInEachBranch);
      var B3 = CreateBranch(NumberOWBsInEachBranch);
      B1.Last().AddDownstreamConnection(B3.First());
      B2.Last().AddDownstreamConnection(B3.First());

      B1.First().AddWaterSinkSource(LeftUpperBoundary);
      B2.First().AddWaterSinkSource(RightUpperBoundary);
      YBranch.AddRange(B1);
      YBranch.AddRange(B2);
      YBranch.AddRange(B3);

      return YBranch;

    }
  }
}
