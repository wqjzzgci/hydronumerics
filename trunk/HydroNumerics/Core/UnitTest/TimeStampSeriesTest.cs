using HydroNumerics.Core.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

using HydroNumerics.Core.Time;

namespace HydroNumerics.Core.Time.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TimeStampSeriesTest and is intended
    ///to contain all TimeStampSeriesTest Unit Tests
    ///</summary>
  [TestClass()]
  public class TimeStampSeriesTest
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


    [TestMethod]
    public void GetSubSeriesTest()
    {
      TimeStampSeries ts = new TimeStampSeries();

      for (int i = 1; i < 16; i++)
        ts.Items.Add(new TimeStampValue(new DateTime(2015, 1, i), i));


      Assert.AreEqual(15, ts.GetSubSeries(new DateTimeSize(new DateTime(2015, 1, 1), DateTime.MaxValue)).Count());


    }

    /// <summary>
    ///A test for GapFill
    ///</summary>
    [TestMethod()]
    public void GapFillTest()
    {
      TimeStampSeries target = new TimeStampSeries(); // TODO: Initialize to an appropriate value
      target.TimeStepSize = TimeStepUnit.Month;


      for (int i =1;i<5;i++)
      {
        target.Items.Add(new TimeStampValue(new DateTime(2000, i, 1), i));
      }

      target.Items.Add(new TimeStampValue(new DateTime(2000, 9, 1), 9));

      Assert.AreEqual(5, target.Items.Count);

      target.GapFill(InterpolationMethods.DeleteValue);
      Assert.AreEqual(9, target.Items.Count);

      target.Items.Add(new TimeStampValue(new DateTime(2002, 9, 1), 9));
      target.GapFill(InterpolationMethods.DeleteValue);

      for (int i = 1; i < target.Items.Count; i++)
      {
        Assert.IsTrue(TSTools.GetTimeStep(target.Items[i - 1].Time, target.Items[i].Time) == TimeStepUnit.Month);

      }
      
      Assert.AreEqual(33, target.Items.Count);

    
    }

    [TestMethod()]
    public void SerializeTest()
    {
      TimeStampSeries target = new TimeStampSeries(); // TODO: Initialize to an appropriate value
      target.TimeStepSize = TimeStepUnit.Month;


      for (int i = 1; i < 5; i++)
      {
        target.Items.Add(new TimeStampValue(new DateTime(2000, i, 1), i));
      }

      var xml = target.Serialize();

      var newts = TimeStampSeries.DeSerialize(xml);


      Assert.AreEqual(target.Count, newts.Count);
    }


  }
}
