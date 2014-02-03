using HydroNumerics.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for XYPolygonTest and is intended
    ///to contain all XYPolygonTest Unit Tests
    ///</summary>
  [TestClass()]
  public class XYPolygonTest
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


    /// <summary>
    ///A test for GetPolygons
    ///</summary>
    [Ignore]
    [TestMethod()]
    public void GetPolygonsTest()
    {

      var m = new HydroNumerics.MikeSheTools.Core.Model(@"E:\dhi\data\dkm\dk2\result\DK2_v3_gvf_PT_100p_24hr.she");

      var precip = new HydroNumerics.MikeSheTools.DFS.DFS2(m.Input.MIKESHE_FLOWMODEL.Climate.PrecipitationRate.FULLY_DISTRIBUTED.DFS_2D_DATA_FILE.FILE_NAME);

      var actual = XYPolygon.GetPolygons(precip);

      System.Data.DataTable dt = new System.Data.DataTable();
      dt.Columns.Add("ID", typeof(int));

      int k = 0;
      using (HydroNumerics.Geometry.Shapes.ShapeWriter sw = new Geometry.Shapes.ShapeWriter(@"d:\temp\out.shp"))
      {
        for (int i = 0; i < precip.NumberOfColumns; i++)
          for (int j = 0; j < precip.NumberOfRows; j++)
          {
            var drow =  dt.NewRow();
            drow[0] = k;
            sw.Write(new GeoRefData() { Geometry = actual[i, j],Data =drow  });
            k++;
          }
      }
    }
  }
}
