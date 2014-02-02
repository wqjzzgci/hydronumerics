using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using HydroNumerics.Geometry;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for SoilCodesGridTest and is intended
    ///to contain all SoilCodesGridTest Unit Tests
    ///</summary>
  [TestClass()]
  public class SoilCodesGridTest
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
    ///A test for BuildGrid
    ///</summary>
    [TestMethod()]
    public void BuildGridTest()
    {
      SoilCodesGrid target = new SoilCodesGrid(); // TODO: Initialize to an appropriate value
      string ShapeSoilCodes = @"D:\DK_information\DKDomainNodes_LU_Soil_codes.shp";
      target.BuildGrid(ShapeSoilCodes);
    }

    /// <summary>
    ///A test for GetID
    ///</summary>
    [TestMethod()]
    public void GetIDTest()
    {
      SoilCodesGrid target = new SoilCodesGrid(); // TODO: Initialize to an appropriate value
      string ShapeSoilCodes = @"D:\DK_information\DKDomainNodes_LU_Soil_codes.shp";
      target.BuildGrid(ShapeSoilCodes);
      int ec=0;
      List<int> errors = new List<int>();

      using (HydroNumerics.Geometry.Shapes.ShapeReader sr = new Geometry.Shapes.ShapeReader(ShapeSoilCodes))
      {
        foreach (var c in sr.GeoData)
        {
          if ((int)c.Data["GRIDID"] != target.GetID((IXYPoint)c.Geometry))
            errors.Add((int)c.Data["GRIDID"]);

        }
      }
         
        Assert.AreEqual(0,ec);
    
    }


  }
}
