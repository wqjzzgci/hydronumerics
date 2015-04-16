using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Geometry.UnitTests
{
  [TestClass]
  public class PointShapeReaderTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      string File = @"..\..\..\..\MikeSheTools\TestData\CommandAreas.Shp";
      ShapeReader target = new ShapeReader(File);
      var geo = target.ReadNext(0);
      double d = ((XYPolygon)geo).GetArea();
      Assert.IsTrue(0 < d);

    }

    [TestMethod]
    public void ReadXYPoint()
    {
      string file = @"..\..\..\Testdata\kontinuitet.shp";
      ShapeReader target = new ShapeReader(file);
      foreach (GeoRefData grd in target.GeoData)
      {
        Assert.IsTrue(grd.Geometry is XYPoint);
      }
    }

    [TestMethod]
    public void GeoData()
    {
      string File = @"..\..\..\..\MikeSheTools\TestData\CommandAreas.Shp";
      ShapeReader target = new ShapeReader(File);

      foreach (GeoRefData grd in target.GeoData)
      {
        Assert.IsTrue(grd.Geometry is XYPolygon);
      }

    }

    [TestMethod]
    public void ReadProjectionFile()
    {
      string File = @"..\..\..\Testdata\kontinuitet.prj";


      ShapeReader sr = new ShapeReader(File);
      var proj = sr.Projection;




    }

  }
}
