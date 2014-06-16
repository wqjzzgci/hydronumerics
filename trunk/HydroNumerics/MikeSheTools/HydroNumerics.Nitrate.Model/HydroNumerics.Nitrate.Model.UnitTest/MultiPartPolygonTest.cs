using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HydroNumerics.Geometry;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MultiPartPolygonTest and is intended
    ///to contain all MultiPartPolygonTest Unit Tests
    ///</summary>
  [TestClass()]
  public class MultiPartPolygonTest
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
    [Ignore]
    public void ContainsTest2()
    {
      IXYPolygon stevns;
      using (Geometry.Shapes.ShapeReader sr = new Geometry.Shapes.ShapeReader(@"D:\NitrateModel\Overfladevand\oplande\Stevns.shp"))
      {

        stevns = (IXYPolygon)sr.GeoData.First().Geometry;
        Assert.IsTrue(stevns.Contains(709422, 6129109));
      }

      List<int> parts = new List<int>();
      using (Geometry.Shapes.ShapeReader sr = new Geometry.Shapes.ShapeReader(@"E:\dhi\data\dkm\dk1\result\dbf results\PTReg_dk1_R201401_normal.shp"))
      {
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
                      int id = sr.Data.ReadInt(i, "ID");

          double XStart = sr.Data.ReadDouble(i, "X-Birth");
          double YStart = sr.Data.ReadDouble(i, "Y-Birth");
          int Registration = sr.Data.ReadInt(i, "Registrati");
          string sink = sr.Data.ReadString(i, "SinkType");

          if (stevns.Contains(XStart, YStart))
            parts.Add(id);
        }
      }

      int k = parts.Count;





    }

    /// <summary>
    ///A test for Contains
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void ContainsTest()
    {
      List<IXYPolygon> pols = new List<IXYPolygon>();
      using (Geometry.Shapes.ShapeReader sr = new Geometry.Shapes.ShapeReader(@"D:\NitrateModel\Overfladevand\Punktkilder\kystzone.shp"))
      {
        foreach (var pol in sr.GeoData)
        {
          if (pol.Data[1].ToString().Trim().ToLower() == "land")
            pols.Add(pol.Geometry as IXYPolygon);
        }
      }

      List<XYPoint> points = new List<XYPoint>();
      using (Geometry.Shapes.ShapeReader sr = new Geometry.Shapes.ShapeReader(@"D:\NitrateModel\Overfladevand\Punktkilder\spredt_pkt.shp"))
      {
        for(int i =0;i<sr.Data.NoOfEntries;i++)
        {
          XYPoint p = sr.ReadNext() as XYPoint;

          Parallel.ForEach(pols, (pol, state) =>
          {
            if (pol.Contains(p))
            {
              points.Add(p);
              state.Break();
            }
          });
        }
      }

      int k = points.Count;
      
    }
  }
}
