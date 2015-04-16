using HydroNumerics.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for IXYPolygonTest and is intended
    ///to contain all IXYPolygonTest Unit Tests
    ///</summary>
  [TestClass()]
  public class IXYPolygonTest
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
    ///A test for Contains
    ///</summary>
    [TestMethod()]
    public void ContainsTest()
    {
      using (Geometry.Shapes.ShapeReader sr = new Geometry.Shapes.ShapeReader(@"f:\NitrateModel\Overfladevand\oplande\mors.shp"))
      {
        IXYPolygon geo = (IXYPolygon)sr.ReadNext(0);

        Assert.IsTrue(geo.Contains(494131.00000000000, 6309279.00000000000));


      }
    }
  }
}
