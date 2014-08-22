using HydroNumerics.MikeSheTools.PFS.Well;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.MikeSheTools.PFS.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for WellFileTest and is intended
    ///to contain all WellFileTest Unit Tests
    ///</summary>
  [TestClass()]
  public class WellFileTest
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
    ///A test for WellFile Constructor
    ///</summary>
    [TestMethod()]
    public void WellFileConstructorTest()
    {
      HydroNumerics.MikeSheTools.PFS.Well.WellFile target = new Well.WellFile(@"..\..\..\testdata\demowells.wel");
      Assert.AreEqual(2, target.WEL_CFG.WELLDATA.NoWells);

      target.WEL_CFG.WELLDATA.AddWell();
      Assert.AreEqual(3, target.WEL_CFG.WELLDATA.NoWells);
      Assert.AreEqual(3, target.WEL_CFG.WELLDATA.WELLNO_1s.Count);

      target.SaveAs(@"..\..\..\testdata\demowells_test.wel");




    
    }

  }
}
