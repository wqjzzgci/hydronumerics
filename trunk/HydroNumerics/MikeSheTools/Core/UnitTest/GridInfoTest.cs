using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;


namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
  [TestClass]
  public class GridInfoTest
  {

    [TestMethod]
    public void GetIndexTest()
    {
      Model mshe = new Model(@"..\..\..\TestData\TestModelDemo.she");

      int Column;
      int Row;

      Assert.IsTrue(mshe.GridInfo.TryGetIndex(11,11,out Column, out Row));
      Assert.AreEqual(1, Column);
      Assert.AreEqual(1, Row);

      Assert.IsTrue(mshe.GridInfo.TryGetIndex(19, 19, out Column, out Row));
      Assert.AreEqual(1, Column);
      Assert.AreEqual(1, Row);

      mshe.Dispose();

      DFS3 heads = new DFS3(@"..\..\..\TestData\TestModelDemo.she - Result Files\TestModelDemo_3DSZ.dfs3");

      Assert.AreEqual(1, heads.GetColumnIndex(11));

      Assert.AreEqual(2, heads.GetColumnIndex(19));

      heads.Dispose();
      

    }

    [TestMethod]
    public void EqualsTest()
    {

      MikeSheGridInfo M1 = new MikeSheGridInfo(@"..\..\TestData\TestModel.she - Result Files\TestModel_PreProcessed_3DSZ.DFS3", @"..\..\TestData\TestModel.she - Result Files\TestModel_PreProcessed.DFS2");
      MikeSheGridInfo M2 = new MikeSheGridInfo(@"..\..\TestData\TestModel.she - Result Files\TestModel_PreProcessed_3DSZ.DFS3", @"..\..\TestData\TestModel.she - Result Files\TestModel_PreProcessed.DFS2");

      Assert.IsTrue(M1.Equals(M2));
      M1.Dispose();
      M2.Dispose();
    }

  }
}
