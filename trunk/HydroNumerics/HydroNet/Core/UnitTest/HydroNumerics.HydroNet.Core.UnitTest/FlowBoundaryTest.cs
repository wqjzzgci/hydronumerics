using HydroNumerics.HydroNet.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroInform.TimeSeries.Core;
using System;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for FlowBoundaryTest and is intended
    ///to contain all FlowBoundaryTest Unit Tests
    ///</summary>
  [TestClass()]
  public class FlowBoundaryTest
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
    ///A test for Source
    ///</summary>
    [TestMethod()]
    public void SourceTest()
    {
      FlowBoundary target = new FlowBoundary(200);
      Assert.IsTrue(target.Source(DateTime.Now));

      target = new FlowBoundary(-200);
      Assert.IsFalse(target.Source(DateTime.Now));
    }

    /// <summary>
    ///A test for GetSinkVolume
    ///</summary>
    [TestMethod()]
    public void GetSinkVolumesTest()
    {
      FlowBoundary target = new FlowBoundary(-200);

      double actual = target.GetSinkVolume(DateTime.Now, new TimeSpan(1,0,0));
      Assert.AreEqual(200*3600, actual,0.000001);
    }

    /// <summary>
    ///A test for GetSourceWater
    ///</summary>
    [TestMethod()]
    public void GetSourceWaterTest()
    {
      FlowBoundary target = new FlowBoundary(200);
      IWaterPacket actual = target.GetSourceWater(DateTime.Now, new TimeSpan(1,0,0));
      Assert.AreEqual(200*3600, actual.Volume);
    }
  }
}
