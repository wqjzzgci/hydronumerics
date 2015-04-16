using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Geometry.Shapes;


namespace HydroNumerics.Geometry.UnitTests
{
  [TestClass]
  public class PointShapeWriterTest
  {
    [TestMethod]
    public void WritePolyLineTest()
    {
      string File = @"..\..\..\TestData\PolyLineTest.Shp";
      XYPolyline line = new XYPolyline();
      line.Points.Add(new XYPoint(0, 0));
      line.Points.Add(new XYPoint(2, 2));
      line.Points.Add(new XYPoint(4, 5));

      DataTable dt = new DataTable();
      dt.Columns.Add("tekst", typeof(string));

      GeoRefData grf = new GeoRefData();
      grf.Geometry = line;
      grf.Data = dt.NewRow();
      grf.Data[0] = "Her er værdien";

      ShapeWriter sp = new ShapeWriter(File);
      sp.Write(grf);
      sp.Dispose();
    }


    /// <summary>
    ///A test for WritePointShape
    ///</summary>
    [TestMethod()]
    public void WritePointShapeTest()
    {
      string File = @"..\..\..\TestData\WriteTest.Shp";

      ShapeWriter PSW = new ShapeWriter(File);

      PSW.WritePointShape(10, 20);
      PSW.WritePointShape(20, 30);
      PSW.WritePointShape(30, 40);

      DataTable DT = new DataTable();
      DT.Columns.Add("Name", typeof(string));
      DT.Rows.Add(new object[] { "point1" });
      DT.Rows.Add(new object[] { "point2" });
      DT.Rows.Add(new object[] { "point3" });

      PSW.Data.WriteDate(DT);
      PSW.Dispose();


      ShapeReader PSR = new ShapeReader(File);

      IXYPoint p;
      DataTable DTread = PSR.Data.Read();
      int i = 0;
      foreach (DataRow dr in DTread.Rows)
      {
        Console.WriteLine(dr[0].ToString());
        p = (IXYPoint)PSR.ReadNext(i);
        Console.WriteLine(p.X.ToString() + "   " + p.Y.ToString());
        i++;

      }
    }
  }
}
