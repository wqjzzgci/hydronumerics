using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
  [TestClass]
  public class MultiPolygonReader
  {
    [TestMethod]
    public void TestMethod1()
    {

      using (ShapeReader sr = new ShapeReader(@"D:\DK_information\TestData\FileStructure\temp.shp"))
      {
        var data = sr.GeoData.ToList();
      }
    }
  }
}
