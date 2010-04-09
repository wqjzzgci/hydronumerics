using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

using HydroNumerics.HydroNet.Core;
using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.HydroNet.Core.UnitTest
{

    
    /// <summary>
    ///This is a test class for StreamTest and is intended
    ///to contain all StreamTest Unit Tests
    ///</summary>
  [TestClass()]
  public class StreamTest
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

      Stream_Accessor S = new Stream_Accessor(25,1,1);

      DateTime Start = DateTime.Now;

      S.SetState("Initial", Start, new WaterPacket(1, 25));
      
      Stream s2 = new Stream(50,1,1);
      s2.SetState("Initial", Start, new WaterPacket(50));
      
      Stream s3 = new Stream(300,1,1);
      s3.SetState("Initial", Start, new WaterPacket(300));

      S.AddDownstreamConnection(s2);
      s2.AddDownstreamConnection(s3);

      TimeSpan ts = new TimeSpan(1,0,0);

      WaterPacket WaterProvider = new WaterPacket(2, 200);

      S.ReceiveWater(Start, Start.AddDays(1), WaterProvider.DeepClone(200));



      S.MoveInTime(ts);

      Assert.AreEqual(0, S._incomingWater.Count);
      Assert.AreEqual(1, S.CurrentStoredWater.Composition[2]);

      s2.MoveInTime(ts);
      s3.MoveInTime(ts);

      Assert.AreEqual(1, s2.CurrentStoredWater.Composition[2]);
      
      //In the next timestep there will be no water to route
      S.MoveInTime(ts);

    }

    [TestMethod]
    public void EvapoAndSinks()
    {
      Stream_Accessor S = new Stream_Accessor(100,1,1);

      DateTime Start = DateTime.Now;
      S.SetState("Initial", Start, new WaterPacket(100));


      S.AddEvaporationBoundary(new EvaporationRateBoundary(1000));
      S.AddWaterSinkSource(new FlowBoundary(-500));
      S.AddWaterSinkSource(new FlowBoundary(500));

      S.MoveInTime(TimeSpan.FromSeconds(1));

     // Assert.AreEqual(0, S.CurrentStoredWater.Volume, 0.00001);
      Assert.AreEqual(1, S.CurrentStoredWater.WaterAge.TotalSeconds, 0.00001);
      S.MoveInTime(TimeSpan.FromSeconds(10));
      Assert.AreEqual(11, S.CurrentStoredWater.WaterAge.TotalSeconds, 0.00001);

      S.AddWaterSinkSource(new FlowBoundary(5000));
      S.MoveInTime(TimeSpan.FromSeconds(1));
      Assert.AreEqual(100, S.CurrentStoredWater.Volume, 0.00001);
      Assert.AreEqual(0.013, S.CurrentStoredWater.WaterAge.TotalSeconds, 0.00001);
    }

    [TestMethod]
    public void OnlyInflow()
    {
      Stream_Accessor S = new Stream_Accessor(100,1,1);
      S.AddWaterSinkSource(new FlowBoundary(200));

      S.MoveInTime(TimeSpan.FromSeconds(1));

      Assert.AreEqual(100, S.Output.TimeSeriesList.First().TimeValues[0].Value);

    }
  

    [TestMethod()]
    public void RoutingOfInflow()
    {
      Stream S = new Stream(10,1,1);
      DateTime Start = DateTime.Now;
      S.SetState("Initial", Start, new WaterPacket(1, 10));

      Stream s2 = new Stream(10,1,1);


      Lake Storage = new Lake(100000);

      S.AddDownstreamConnection(s2);
      s2.AddDownstreamConnection(Storage);

      FlowBoundary FB = new FlowBoundary(5.0 / 60);
      FB.WaterSample = new WaterPacket(5, 5);

      S.AddWaterSinkSource(FB);
      
      TimeSpan ts = new TimeSpan(0, 1, 0);

      WaterPacket WaterProvider = new WaterPacket(2, 5);

      S.ReceiveWater(Start, Start.AddDays(1), WaterProvider.DeepClone(15));
      S.MoveInTime(ts);
      s2.MoveInTime(ts);

      Assert.AreEqual(10, S.CurrentStoredWater.Volume, 0.00001);
      Assert.AreEqual(0.137, S.CurrentStoredWater.Composition[5], 0.001);
      Assert.AreEqual(0.863, S.CurrentStoredWater.Composition[2], 0.001);   
      Assert.AreEqual(0.1309, s2.CurrentStoredWater.Composition[1], 0.001);

    }


    [TestMethod()]
    public void RoutingOfInflow2()
    {
      Stream S = new Stream(25,1,1);

      DateTime Start = DateTime.Now;
      S.SetState("Initial", Start, new WaterPacket(1, 25));

      Stream s2 = new Stream(50,1,1);
      s2.SetState("Initial", Start, new WaterPacket(50));

      Stream s3 = new Stream(300,1,1);
      s3.SetState("Initial", Start, new WaterPacket(300));

      S.AddDownstreamConnection(s2);
      s2.AddDownstreamConnection(s3);

      FlowBoundary FB = new FlowBoundary(0.0005);
      FB.WaterSample = new WaterPacket(5, 5);

      S.AddWaterSinkSource(FB);

      TimeSpan ts = new TimeSpan(1, 0, 0);

      WaterPacket WaterProvider = new WaterPacket(2, 200);

      S.ReceiveWater(DateTime.Now, DateTime.Now, WaterProvider.DeepClone(200));
      S.MoveInTime(ts);
    }

    [TestMethod]
    public void RoutingOfInflow3()
    {
      Stream s = new Stream(10,1,1);
      s.SetState("Initial", DateTime.Now, new WaterPacket(10));
      FlowBoundary fb = new FlowBoundary(1);
      s.AddWaterSinkSource(fb);
      
      s.MoveInTime(TimeSpan.FromHours(1));

      Assert.AreEqual(1, s.Output.Outflow.TimeValues.First().Value, 0.00001);

    }


    [TestMethod()]
    public void RoutingOfGroundwaterTest()
    {

      Stream S = new Stream(100,1,1);

      S.SetState("Initial", DateTime.Now, new WaterPacket(100));

      TimeSpan ts = new TimeSpan(0,1,0);

      int Id = 2;
      IWaterPacket expected = new WaterPacket(Id, 200);
      IWaterPacket actual;

      S.WaterLevel = 8;
      GroundWaterBoundary b = new GroundWaterBoundary(S, 0.001, 250, 10, 100);
      b.WaterSample = expected;
      S.AddWaterSinkSource(b);
      S.MoveInTime(ts);

      actual = S.CurrentStoredWater;
      double ExpectedVolume = b.GetSourceWater(DateTime.Now, ts).Volume;

      Assert.AreEqual(expected.Composition.Keys.First(), actual.Composition.Keys.First());
      Assert.AreEqual(100, actual.Volume, 0.000001);

      S.MoveInTime(ts);

      actual = S.CurrentStoredWater;
      Assert.AreEqual(expected.Composition.Keys.First(), actual.Composition.Keys.First());
      Assert.AreEqual(100, actual.Volume, 0.000001);

      S.ReceiveWater(DateTime.Now, DateTime.Now, expected);
      S.MoveInTime(ts);
      actual = S.CurrentStoredWater;

      Assert.AreEqual(100, actual.Volume, 0.000001);

    }

    [TestMethod]
    public void RoutingOfChemical()
    {
      Stream_Accessor s = new Stream_Accessor(100,1,1);

      s.SetState("Initial", DateTime.Now, new WaterPacket(100));

      FlowBoundary fb = new FlowBoundary(50);
      s.AddWaterSinkSource(fb);
      WaterWithChemicals Wcc = new WaterWithChemicals(50);
      Wcc.AddChemical(new Chemical(new ChemicalType("na", 31), 1));
      TimeSpan ts = new TimeSpan(0,0,1);
      s.ReceiveWater(DateTime.Now, DateTime.Now.AddDays(1), Wcc);
      s.MoveInTime(ts);
      
      WaterWithChemicals WccNew = (WaterWithChemicals) s._waterInStream.Last();
      Assert.AreEqual(64.8721, WccNew.Volume,0.0001);
      //Assert.AreEqual(TimeSpan.FromSeconds(0.5), WccNew.WaterAge);
      Assert.AreEqual(1, WccNew.Chemicals["na"].Moles);

    }

    [TestMethod]
    public void OutputTest()
    {

      TimeSpan ts = new TimeSpan(0,1,0);
      Stream S = new Stream(10,1,1);

      S.SetState("Initial", DateTime.Now, new WaterPacket(1, 10));
      

      for (int i = 0; i < 10; i++)
      {
        S.ReceiveWater(DateTime.Now, DateTime.Now.AddDays(1), new WaterPacket(25 * i));
        S.MoveInTime(ts);
      }

      for (int i = 0; i < 10; i++)
      {
        Assert.AreEqual(i * 25.0/ts.TotalSeconds , S.Output.Outflow.TimeValues[i].Value);
      }
    }

    [TestMethod]
    public void SortingOfIncomingWater()
    {
      Stream_Accessor s = new Stream_Accessor(100,1,1);

      
      WaterPacket wp1 = new WaterPacket(1, 50);
      WaterPacket wp2 = new WaterPacket(2,100);

      s.ReceiveWater(new DateTime(2000, 1, 1), new DateTime(2000, 1, 11), wp1);
      s.ReceiveWater(new DateTime(2000, 1, 6), new DateTime(2000, 1, 11), wp2);

      s.PrePareIncomingWater();

      Assert.AreEqual(2, s._incomingWater.Count);
      
      IWaterPacket iwp = s._incomingWater.Dequeue();
      Assert.AreEqual(25, iwp.Volume);
      Assert.AreEqual(1, iwp.Composition[1]);

      iwp = s._incomingWater.Dequeue();
      Assert.AreEqual(125, iwp.Volume);
      Assert.AreEqual(25.0 / 125.0, iwp.Composition[1]);
      Assert.AreEqual(100.0 / 125.0, iwp.Composition[2]);
      
      WaterPacket wp3 = new WaterPacket(3,100);
      WaterPacket wp4 = new WaterPacket(4, 200);
      WaterPacket wp5 = new WaterPacket(5, 300);

      s.ReceiveWater(new DateTime(2000, 1, 1), new DateTime(2000, 1, 3), wp3);
      s.ReceiveWater(new DateTime(2000, 1, 2), new DateTime(2000, 1, 3), wp4);
      s.ReceiveWater(new DateTime(2001, 1, 1, 12,0,0), new DateTime(2001, 1, 5), wp5);

      s.PrePareIncomingWater();

     

    }


  }
}
