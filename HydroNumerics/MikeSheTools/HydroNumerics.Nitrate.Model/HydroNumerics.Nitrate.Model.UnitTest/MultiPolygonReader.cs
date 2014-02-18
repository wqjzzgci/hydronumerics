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
      List<Geometry.GeoRefData> data;
      using (ShapeReader sr = new ShapeReader(@"D:\DK_information\TestData\FileStructure\temp.shp"))
      {
        data = sr.GeoData.ToList();
      }
      using (ShapeWriter sr = new ShapeWriter(@"D:\DK_information\TestData\FileStructure\temp_write.shp"))
      {
        foreach (var gd in data)
          sr.Write(gd);
      }
      using (ShapeReader sr = new ShapeReader(@"D:\DK_information\TestData\FileStructure\temp_write.shp"))
      {
       var data2 = sr.GeoData.ToList();
      }


    }
  }
}
