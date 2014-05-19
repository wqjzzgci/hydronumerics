using HydroNumerics.Time2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for FixedTimeStepSeriesTest and is intended
    ///to contain all FixedTimeStepSeriesTest Unit Tests
    ///</summary>
  [TestClass()]
  public class FixedTimeStepSeriesTest
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
    ///A test for GetIndex
    ///</summary>
    [TestMethod()]
    [DeploymentItem("HydroNumerics.Time2.dll")]
    public void GetIndexTest()
    {
      FixedTimeStepSeries_Accessor target = new FixedTimeStepSeries_Accessor(); // TODO: Initialize to an appropriate value
      
      target.TimeStepSize = TimeStepUnit.Month;
      target.AddRange(new DateTime(2010, 1, 1), new double[] { 1, 2, 3, 4, 5 });
      
      int expected = 0; 
      int actual;
      actual = target.GetIndex(new DateTime(2010,1,1));
      Assert.AreEqual(expected, actual);

      Assert.AreEqual(-1, target.GetIndex(new DateTime(2009,12,1)));
      Assert.AreEqual(-2, target.GetIndex(new DateTime(2009, 11, 1)));

      Assert.AreEqual(11, target.GetIndex(new DateTime(2010, 12, 1)));
    }
  }
}
