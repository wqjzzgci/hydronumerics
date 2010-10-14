using System;
using System.Data;

using HydroNumerics.Geometry.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.Geometry.Shapes.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for PointShapeWriterTest and is intended
    ///to contain all PointShapeWriterTest Unit Tests
    ///</summary>
  [TestClass()]
  public class PointShapeWriterTest
  {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    //[ClassInitialize()]
    //public static void MyClassInitialize(TestContext testContext)
    //{
    //}
    //
    //Use ClassCleanup to run code after all tests in a class have run
    //[ClassCleanup()]
    //public static void MyClassCleanup()
    //{
    //}
    //
    //Use TestInitialize to run code before running each test
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{
    //}
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    [TestMethod]
    public void WritePolyLineTest()
    {
      string File = @"..\..\TestData\PolyLineTest.Shp";
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
      string File = @"..\..\TestData\WriteTest.Shp";

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

      foreach (DataRow dr in DTread.Rows)
      {
        Console.WriteLine(dr[0].ToString());
        p = (IXYPoint)PSR.ReadNext();
        Console.WriteLine(p.X.ToString() + "   " + p.Y.ToString());

      }
    }
  }
}
