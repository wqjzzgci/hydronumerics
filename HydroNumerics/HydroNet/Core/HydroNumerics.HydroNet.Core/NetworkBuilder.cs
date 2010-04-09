using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Branch.Add( new Stream(100,1,1));
        if (i > 0)
        {
          Branch[i - 1].AddDownstreamConnection(Branch[i]);
        }
      }
      return Branch;
    }


     public static List<Lake> CreateConnectedLakes(int numberofWbs)
    {
      List<Lake> Branch = new List<Lake>();
      for (int i = 0; i < numberofWbs; i++)
      {
        Branch.Add(new Lake(100));
        if (i > 0)
        {
          Branch[i - 1].AddDownstreamConnection(Branch[i]);
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
