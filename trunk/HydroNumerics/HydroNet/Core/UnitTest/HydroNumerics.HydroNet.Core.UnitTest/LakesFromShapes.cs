using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
  [TestClass()]
  public class LakesFromShapes
  {

    [TestMethod]
    public void ReadAndWriteLakes()
    {

      PointShapeReader psp = new PointShapeReader(@"..\..\..\..\..\TestData\soervp1.shp");

      List<IWaterBody> _lakes = new List<IWaterBody>();

      foreach (var l in psp.GeoData)
      {
        Lake L = new Lake(1);
        L.SurfaceArea = (XYPolygon)l.Geometry;
        L.Name = (string)l.Data[0];
        _lakes.Add(L);
        //Assert.AreEqual((double)l.Data[10], L.SurfaceArea.GetArea()/10000, 2);
      }



      psp.Dispose();

      Model M = new Model();
      M._waterBodies.AddRange(_lakes);
      M.Save(@"..\..\..\..\..\TestData\VPLakes.xml");
    }


  }
}
