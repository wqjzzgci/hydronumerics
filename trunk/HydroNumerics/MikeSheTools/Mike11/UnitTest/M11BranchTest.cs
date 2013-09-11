using HydroNumerics.MikeSheTools.Mike11;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HydroNumerics.MikeSheTools.PFS.NWK11;
using System.Collections.Generic;

namespace HydroNumerics.MikeSheTools.Mike11.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for M11BranchTest and is intended
    ///to contain all M11BranchTest Unit Tests
    ///</summary>
  [TestClass()]
  public class M11BranchTest
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
    ///A test for EndPointElevation
    ///</summary>
    [TestMethod()]
    public void EndPointElevationTest()
    {



      branch BranchFromPFS = null; // TODO: Initialize to an appropriate value
      SortedDictionary<int, point> Points = null; // TODO: Initialize to an appropriate value
      M11Branch target = new M11Branch(BranchFromPFS, Points); // TODO: Initialize to an appropriate value
      double expected = 0F; // TODO: Initialize to an appropriate value
      double actual;
      target.EndPointElevation = expected;
      actual = target.EndPointElevation;
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }
  }
}
