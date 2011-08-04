using HydroNumerics.MikeSheTools.Mike11;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HydroNumerics.Geometry;

namespace HydroNumerics.MikeSheTools.Mike11.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for CrossSectionTest and is intended
    ///to contain all CrossSectionTest Unit Tests
    ///</summary>
  [TestClass()]
  public class CrossSectionTest
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

    //Nedenstående virker test med private accessor virker ikke efter skift til 2010.
    /// <summary>
    ///A test for SetPoints
    ///</summary>
    [TestMethod()]
    [Ignore]
    [DeploymentItem("HydroNumerics.MikeSheTools.Mike11.dll")]
    public void SetPointsTest()
    {
 
      CrossSection_Accessor target = new CrossSection_Accessor();
      HydroNumerics.Geometry.IXYPoint p1 = new HydroNumerics.Geometry.XYPoint(0, 0);
      HydroNumerics.Geometry.IXYPoint p2 = new HydroNumerics.Geometry.XYPoint(10, 2);
      target.SetPoints(p1, p2, 0, 10, 5);
      Assert.AreEqual(5, target.MidStreamLocation.X);
      Assert.AreEqual(1, target.MidStreamLocation.Y);


    }

    /// <summary>
    ///A test for SetPoints
    ///</summary>
    [TestMethod()]
    public void SetPointsTest1()
    {
      CrossSection target = new CrossSection();
      IXYPoint p1 = new HydroNumerics.Geometry.XYPoint(0, 0);
      IXYPoint p2 = new HydroNumerics.Geometry.XYPoint(10, 2);
      target.SetPoints(p1, p2, 0, 10, 5);
      Assert.AreEqual(5, target.MidStreamLocation.X);
      Assert.AreEqual(1, target.MidStreamLocation.Y);
    }


    [TestMethod]
    public void ReadTest()
    {
      M11Setup m11 = new M11Setup();
      m11.ReadSetup(@"C:\Users\Jacob\Dropbox\FunFirm\MIKEFile\VivedeMølleåStrækning1.sim11");

    }
  }
}
