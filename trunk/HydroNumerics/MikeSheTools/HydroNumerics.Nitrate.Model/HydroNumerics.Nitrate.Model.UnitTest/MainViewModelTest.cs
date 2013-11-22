using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Diagnostics;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MainViewModelTest and is intended
    ///to contain all MainViewModelTest Unit Tests
    ///</summary>
  [TestClass()]
  public class MainViewModelTest
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
    ///A test for CurrentCatchment
    ///</summary>
    [TestMethod()]
    public void CurrentCatchmentTest()
    {
      MainViewModel target = new MainViewModel();
      target.LoadCatchments(@"D:\DK_information\id15_NSTmodel\id15_NSTmodel.shp");
      target.CurrentCatchment = target.AllCatchments.Values.First();
      target.LoadParticles(@"D:\DK_information\DK_data\Data from MIKE SHE WQ\PTReg_Extraction_1_20131016_dk4.shp");
      Stopwatch sw = new Stopwatch();
      sw.Start();
      target.CombineParticlesAndCatchments();
      sw.Stop();

      int k = 0;

    }

    /// <summary>
    ///A test for LoadGridPoints
    ///</summary>
    [TestMethod()]
    public void LoadGridPointsTest()
    {
      MainViewModel target = new MainViewModel(); // TODO: Initialize to an appropriate value
      string ShapeFileName = @"D:\DK_information\DKDomainNodes_LU_Soil_codes.shp";
      target.LoadGridPoints(ShapeFileName);
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }
  }
}
