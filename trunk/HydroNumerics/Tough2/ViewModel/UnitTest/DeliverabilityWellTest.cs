using HydroNumerics.Tough2.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Tough2.ViewModel.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for DeliverabilityWellTest and is intended
    ///to contain all DeliverabilityWellTest Unit Tests
    ///</summary>
  [TestClass()]
  public class DeliverabilityWellTest
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
    ///A test for ToString
    ///</summary>
    [TestMethod()]
    public void ToStringTest()
    {
      DeliverabilityWell target = new DeliverabilityWell(); // TODO: Initialize to an appropriate value
      target.Eleme = new Element("A1  1", 1, 1);
      target.Pressure = 100014;
      target.Resistance = 2e-10;
      string actual = target.ToString();
      Assert.AreEqual("A1  1                              DELV 2.0000E-101.0001E+05", actual);
    }
  }
}
