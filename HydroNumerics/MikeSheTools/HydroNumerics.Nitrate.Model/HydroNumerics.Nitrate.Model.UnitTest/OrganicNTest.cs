using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for OrganicNTest and is intended
    ///to contain all OrganicNTest Unit Tests
    ///</summary>
  [TestClass()]
  public class OrganicNTest
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
    ///A test for EvaluateEquation
    ///</summary>
    [TestMethod()]
    public void EvaluateEquationTest()
    {
      OrganicN target = new OrganicN(); // TODO: Initialize to an appropriate value
      double CoarseSandPercentage = 0F; // TODO: Initialize to an appropriate value
      double FineSandPercentage = 8.71; // TODO: Initialize to an appropriate value
      double HumusPercentage = 7.3; // TODO: Initialize to an appropriate value
      double Precipitation = 800; // TODO: Initialize to an appropriate value
      double Slope = 3.23; // TODO: Initialize to an appropriate value
      double expected = 0F; // TODO: Initialize to an appropriate value
      double actual;
      actual = target.EvaluateEquation(CoarseSandPercentage, FineSandPercentage, HumusPercentage, Precipitation, Slope);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }
  }
}
