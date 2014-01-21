using HydroNumerics.Time2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Time2.UnitTest
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
    ///A test for GetTimeStep
    ///</summary>
    [TestMethod()]
    public void GetTimeStepTest()
    {
      DateTime Start = new DateTime(2000,1,1);

      Assert.IsTrue(TimeStepUnit.Second == TSTools.GetTimeStep(Start, new DateTime(2000,1,1,0,0,1)));
      Assert.IsTrue(TimeStepUnit.Minute == TSTools.GetTimeStep(Start, new DateTime(2000, 1, 1, 0, 1, 0)));
      Assert.IsTrue(TimeStepUnit.Hour == TSTools.GetTimeStep(Start, new DateTime(2000, 1, 1, 1, 0, 0)));
      Assert.IsTrue(TimeStepUnit.Day == TSTools.GetTimeStep(Start, new DateTime(2000, 1, 2, 0, 0, 0)));
      Assert.IsTrue(TimeStepUnit.Month == TSTools.GetTimeStep(Start, new DateTime(2000, 2, 1, 0, 0, 0)));
      Assert.IsTrue(TimeStepUnit.Year == TSTools.GetTimeStep(Start, new DateTime(2001, 1, 1, 0, 0, 0)));

    }
  }
}
