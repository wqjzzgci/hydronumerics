using System;
using System.Linq;

using HydroNumerics.HydroNet.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.HydroNet.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for StreamTest and is intended
    ///to contain all StreamTest Unit Tests
    ///</summary>
  [TestClass()]
  public class LakeTest
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
    ///A test for CurrentRoutedWater
    ///</summary>
    [TestMethod()]
    public void RoutingOfRecievedWaterTest()
    {
      Lake S = new Lake(new WaterPacket(100));
      Lake storage = new Lake(10000);
      S.AddDownstreamConnection(storage);

      TimeSpan ts = new TimeSpan(1,0,0);

      InfiniteSource WaterProvider = new InfiniteSource(new WaterPacket(2, 200));
      
      IWaterPacket actual;
      S.ReceiveWater(DateTime.Now, DateTime.Now, WaterProvider.GetWater(200));

      S.MoveInTime(ts);

      actual = S.CurrentStoredWater;
      Assert.AreEqual(100, actual.Volume);

      Assert.AreEqual(200, storage.CurrentStoredWater.Volume);
      Assert.AreEqual(200.0/300.0, storage.CurrentStoredWater.Composition[2],0.000001);

      //In the next timestep there will be no water to route
      S.MoveInTime(ts);
      Assert.AreEqual(200, storage.CurrentStoredWater.Volume);
      Assert.AreEqual(200.0 / 300.0, storage.CurrentStoredWater.Composition[2], 0.000001);
    }

    [TestMethod()]
    public void RoutingOfGroundwaterTest()
    {

      Lake S = new Lake(new WaterPacket(100));
      Lake storage = new Lake(10000);
      S.AddDownstreamConnection(storage);

      TimeSpan ts = new TimeSpan(1,0,0);

      int Id = 2;
      IWaterPacket expected = new WaterPacket(Id, 200);
      IWaterPacket actual;

      S.WaterLevel = 8;
      GroundWaterBoundary b = new GroundWaterBoundary(S, 0.001, 2.5, 100, 10);
      b.WaterSample = expected;
      S.AddWaterSinkSource(b);
      S.MoveInTime(ts);

      actual = storage.CurrentStoredWater;
      double ExpectedVolume = b.GetSourceWater(DateTime.Now, ts).Volume;

      Assert.AreEqual(expected.Composition.Keys.First(), actual.Composition.Keys.First());
      Assert.AreEqual(ExpectedVolume, actual.Volume, 0.000001);

      S.MoveInTime(new TimeSpan(2,0,0));

      actual = storage.CurrentStoredWater;
      Assert.AreEqual(expected.Composition.Keys.First(), actual.Composition.Keys.First());
      Assert.AreEqual(0.54, actual.Volume, 0.000001);

      S.ReceiveWater(DateTime.Now, DateTime.Now, expected);
      S.MoveInTime(new TimeSpan(2, 0, 0));
      actual = storage.CurrentStoredWater;

      Assert.AreEqual(200.9, actual.Volume, 0.000001);


    }
  }
}
