using HydroNumerics.Time2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TSToolsTest and is intended
    ///to contain all TSToolsTest Unit Tests
    ///</summary>
  [TestClass()]
  public class TSToolsTest
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
    ///A test for Substract
    ///</summary>
    [TestMethod()]
    public void SubstractTest()
    {
      FixedTimeStepSeries ts1 = new FixedTimeStepSeries();
      FixedTimeStepSeries ts2 = new FixedTimeStepSeries();

      ts1.StartTime = new DateTime(2010, 1, 1);
      ts1.AddRange(new double[] { 2, 3, 4 });
      ts1.TimeStepSize = TimeStepUnit.Month;

      ts2.StartTime = new DateTime(2010, 1, 1);
      ts2.AddRange(new double[] { 1, 1, 1 });
      ts2.TimeStepSize = TimeStepUnit.Month;


      FixedTimeStepSeries actual;
      actual = TSTools.Substract(ts1, ts2);
      Assert.AreEqual(1, actual.GetValue(new DateTime(2010,1,1)));
      Assert.AreEqual(2, actual.GetValue(new DateTime(2010, 2, 1)));
    }
  }
}
