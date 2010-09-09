using HydroNumerics.MikeSheTools.Mike11;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.MikeSheTools.DFS;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace HydroNumerics.MikeSheTools.Mike11.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for M11SetupTest and is intended
    ///to contain all M11SetupTest Unit Tests
    ///</summary>
  [TestClass()]
  public class M11SetupTest
  {
    static M11Setup target;

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
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
      target = new M11Setup();
      target.ReadNetwork(@"..\..\..\testdata\mike11\novomr6.nwk11");
      target.ReadCrossSections(@"..\..\..\testdata\mike11\novomr6.xns11");
    }
    //
    //Use ClassCleanup to run code after all tests in a class have run
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
      target.WriteToShape(@"..\..\..\testdata\mike11\output");
    }
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
    ///A test for ReadCrossSections
    ///</summary>
    [TestMethod()]
    public void ReadCrossSectionsTest()
    {
      M11Branch hygum = target.network.Branches.First(var => var.Name == "HYGUM_NOR_KANAL");
      CrossSection cs = hygum.CrossSections.First(var => var.Chainage == 1926);
      Assert.AreEqual(3.7, cs.HeigthAtMidstream, 0.000001);
      

    }
  }
}
