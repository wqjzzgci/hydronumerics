using HydroNumerics.MikeSheTools.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ResultsTest and is intended
    ///to contain all ResultsTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ResultsTest
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
    ///A test for Mike11Observations
    ///</summary>
    [Ignore]
    [TestMethod()]
    public void Mike11ObservationsTest()
    {
      Model Mshe = new Model(@"D:\DK_information\M11_omr4-7\XY_M11_detailed\Mike11\DK5_2013_inv.she");
      List<DetailedMike11> actual;
      actual = Mshe.Results.Mike11Observations;

      System.Data.DataTable dt = new System.Data.DataTable();

      dt.Columns.Add("Name", typeof(string));
      dt.Columns.Add("Branch", typeof(string));
      dt.Columns.Add("Chainage", typeof(double));

      using (HydroNumerics.Geometry.Shapes.ShapeWriter sw = new Geometry.Shapes.ShapeWriter(@"d:\temp\detailedm11Points_DK5.shp"))
      {
        foreach (var m11 in actual)
        {
          var data = dt.NewRow();
          data[0]= m11.Name;
          data[1] = m11.Branch;
          data[2]=m11.Chainage;
          sw.Write(new Geometry.GeoRefData() { Geometry = m11.Location, Data = data });
        }
      }
    }
  }
}
