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


    [TestMethod]
    public void DownstreamCorrections()
    {
      Assert.AreEqual(143, target.network.Branches.Count);

      var ends = target.network.Branches.Where(b => b.IsEndPoint).ToList();


      Assert.AreEqual(81, ends.Count);


      foreach (var v in ends)
      {
        Assert.IsTrue(v.CrossSections.Last().MaxHeightMrk1and3 < 2);
      }


    }

    /// <summary>
    ///A test for ReadCrossSections
    ///</summary>
    [TestMethod()]
    public void ReadCrossSectionsTest()
    {
      M11Branch hygum = target.network.Branches.First(var => var.Name == "HYGUM_NOR_KANAL");
      CrossSection cs = hygum.CrossSections.First(var => var.Chainage == 1926);
      Assert.AreEqual(4.8, cs.MaxHeightMrk1and3, 0.000001);

      double d = cs.MaxHeightMrk1and3;
      cs.MaxHeightMrk1and3 = d + 1;
      Assert.AreEqual(d + 1, cs.MaxHeightMrk1and3);

    }
  }
}
