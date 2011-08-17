using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{
  [TestClass]
  public class Res11Test
  {

    [TestMethod]
    public void ReadRes11()
    {

      Res11 df = new Res11(@"..\..\..\TestData\Model Inputs\MIKE11\karup.sim11 - Result Files\KARUP.RES11");
      Assert.AreEqual(73.06, df.Points[0].GetData(2), 0.01);
      Assert.AreEqual(PointType.WaterLevel, df.Points[0].pointType);

      
      Assert.AreEqual(0.01183, df.Points[1].GetData(2), 0.001);
      Assert.AreEqual(PointType.Discharge, df.Points[1].pointType);


      Assert.AreEqual("BORDING CREEK", df.Points.First().BranchName);
      Assert.AreEqual("model cross".ToUpper(), df.Points.First().TopoID);

      Assert.AreEqual(17555, df.Points.Last().X,1);
      Assert.AreEqual(17500, df.Points.Last().Y,1);


    }
  }
}
