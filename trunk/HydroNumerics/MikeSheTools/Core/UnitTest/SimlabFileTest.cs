using HydroNumerics.MikeSheTools.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for SimlabFileTest and is intended
    ///to contain all SimlabFileTest Unit Tests
    ///</summary>
  [TestClass()]
  public class SimlabFileTest
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
    ///A test for Load
    ///</summary>
    [TestMethod()]
    public void LoadTest()
    {
      SimlabFile target = new SimlabFile(); // TODO: Initialize to an appropriate value
      string FileName = @"C:\Jacob\Work\HydroNumerics\MikeSheTools\ScenarioController\test1NormalD100.sam";
      target.Load(FileName);

      Assert.AreEqual(-9.537879729, target.Samples.Values[2][0]);
    }
  }
}
