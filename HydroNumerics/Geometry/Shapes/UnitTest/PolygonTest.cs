using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.Geometry.Shapes.UnitTest
{
  [TestClass]
  public class PolygonTest
  {
    [TestMethod]
    public void GetArea()
      
    {
      double area = 0;

      using (ShapeReader sr = new ShapeReader(@"D:/Dropbox/ProjektData/Øvre Suså Ålav/lavb2.shp"))
      {
        foreach(var gd in sr.GeoData)
        {
          area += ((IXYPolygon)sr.GeoData).GetArea();


        }


      }

      double ha = area / 10000;
      Console.WriteLine(ha);

    }
  }
}
