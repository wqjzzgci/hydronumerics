using HydroNumerics.MikeSheTools.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ModelTest and is intended
    ///to contain all ModelTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ModelTest
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

    private static Model mshe;
    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
      mshe = new Model(@"..\..\..\TestData\Karup_Example_DemoMode.SHE");
    }
    //
    //Use ClassCleanup to run code after all tests in a class have run
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
      mshe.Dispose();
    }
    
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
    ///A test for ExtractionWells
    ///</summary>
    [TestMethod()]
    public void ExtractionWellsTest()
    {
      
      var actual = mshe.ExtractionWells;
      Assert.AreEqual(6, mshe.ExtractionWells.Count);
      Assert.AreEqual(10000, mshe.ExtractionWells[0].X);
      Assert.AreEqual(24111, mshe.ExtractionWells[1].X);
      Assert.AreEqual(5, mshe.ExtractionWells[0].Intakes.First().Screens[0].TopAsKote);
      Assert.AreEqual(-2, mshe.ExtractionWells[0].Intakes.First().Screens[0].BottomAsKote);
    }
  }
}
