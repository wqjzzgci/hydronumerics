using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {
      DFS3 df = new DFS3(@"C:\Users\Jacob\Documents\MIKE Zero Projects\SHEPar1.she - Result Files\SHEPar1_3DSZflow.dfs3");

      for (int i = 0; i < 30; i++)
      {
        var mat = df.GetData(i, 1);
        mat[0][1, 1] = -0.5;
        mat[1][1, 1] = -0.5;

        df.SetData(i, 1, mat);
      }
      df.Dispose();

    }
  }
}
