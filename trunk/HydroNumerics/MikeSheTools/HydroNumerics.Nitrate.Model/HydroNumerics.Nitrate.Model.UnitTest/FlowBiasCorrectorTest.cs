using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for FlowBiasCorrectorTest and is intended
    ///to contain all FlowBiasCorrectorTest Unit Tests
    ///</summary>
  [TestClass()]
  public class FlowBiasCorrectorTest
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
    ///A test for Open
    ///</summary>
    [TestMethod()]
    public void OpenTest()
    {
      FlowBiasCorrector target = new FlowBiasCorrector(); // TODO: Initialize to an appropriate value
      string MSHEFileName = @"E:\dhi\data\dkm\dk7\result\DK7_2014_m11_produktion.she"; // TODO: Initialize to an appropriate value
      target.Open(MSHEFileName);
    }
  }
}
