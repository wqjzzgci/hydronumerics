using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;

using HydroNumerics.HydroNet.Core;
using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Geometry;

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

      Stream_Accessor S = new Stream_Accessor("S", 25,1,1);

      DateTime Start = DateTime.Now;

      S.SetState("Initial", Start, new WaterPacket(1, 25));
      
      Stream s2 = new Stream("s2", 50,1,1);
      s2.SetState("Initial", Start, new WaterPacket(50));
      
      Stream s3 = new Stream("s3", 300,1,1);
      s3.SetState("Initial", Start, new WaterPacket(300));

      S.AddDownStreamWaterBody(s2);
      s2.AddDownStreamWaterBody(s3);

      TimeSpan ts = new TimeSpan(1,0,0);

      WaterPacket WaterProvider = new WaterPacket(2, 200);

      S.AddWaterPacket(Start, Start.AddDays(1), WaterProvider.DeepClone(200));



      S.Update(S.CurrentTime.Add(ts));

      Assert.AreEqual(0, S._incomingWater.Count);
      Assert.AreEqual(1, S.CurrentStoredWater.Composition[2]);

      s2.Update(S.CurrentTime.Add(ts));
      s3.Update(S.CurrentTime.Add(ts));

      Assert.AreEqual(1, s2.CurrentStoredWater.Composition[2]);
      
      //In the next timestep there will be no water to route
      S.Update(S.CurrentTime.Add(ts));

    }

    [TestMethod]
    public void EvapoAndSinks()
    {
      Stream_Accessor S = new Stream_Accessor("S", 100,1,1);

      DateTime Start = DateTime.Now;
      S.SetState("Initial", Start, new WaterPacket(100));


      S.EvaporationBoundaries.Add(new EvaporationRateBoundary(1000));
      S.Sinks.Add(new SinkSourceBoundary(-500));
      S.Sources.Add(new SinkSourceBoundary(500));

      S.Update(S.CurrentTime.Add(TimeSpan.FromSeconds(1)));

     // Assert.AreEqual(0, S.CurrentStoredWater.Volume, 0.00001);
      Assert.AreEqual(1, S.CurrentStoredWater.WaterAge.TotalSeconds, 0.00001);
      S.Update(S.CurrentTime.Add(TimeSpan.FromSeconds(10)));
      Assert.AreEqual(11, S.CurrentStoredWater.WaterAge.TotalSeconds, 0.00001);

      S.Sources.Add(new SinkSourceBoundary(5000));
      S.Update(S.CurrentTime.Add(TimeSpan.FromSeconds(1)));
      Assert.AreEqual(100, S.CurrentStoredWater.Volume, 0.00001);
      Assert.AreEqual(0.013, S.CurrentStoredWater.WaterAge.TotalSeconds, 0.00001);
    }

    [TestMethod]
    public void OnlyInflow()
    {
      Stream_Accessor S = new Stream_Accessor("S", 100,1,1);
      S.Sources.Add(new SinkSourceBoundary(200));

      S.Update(S.CurrentTime.Add(TimeSpan.FromSeconds(1)));

      Assert.AreEqual(100, ((TimespanSeries)S.Output.Items.First()).Items[0].Value);

    }
  

    [TestMethod()]
    public void RoutingOfInflow()
    {
      Stream S = new Stream("S", 10,1,1);
      DateTime Start = DateTime.Now;
      S.SetState("Initial", Start, new WaterPacket(1, 10));

      Stream s2 = new Stream("s2", 10,1,1);


      Lake Storage = new Lake("storage", 100000);

      S.AddDownStreamWaterBody(s2);
      s2.AddDownStreamWaterBody(Storage);

      SinkSourceBoundary FB = new SinkSourceBoundary(5.0 / 60);
      FB.WaterSample = new WaterPacket(5, 5);

      S.Sources.Add(FB);
      
      TimeSpan ts = new TimeSpan(0, 1, 0);

      WaterPacket WaterProvider = new WaterPacket(2, 5);

      S.AddWaterPacket(Start, Start.AddDays(1), WaterProvider.DeepClone(15));
      S.Update(S.CurrentTime.Add(ts));
      s2.Update(S.CurrentTime.Add(ts));

      Assert.AreEqual(10, S.CurrentStoredWater.Volume, 0.00001);
      Assert.AreEqual(0.137, S.CurrentStoredWater.Composition[5], 0.001);
      Assert.AreEqual(0.863, S.CurrentStoredWater.Composition[2], 0.001);   
      Assert.AreEqual(0.1309, s2.CurrentStoredWater.Composition[1], 0.001);

    }


    [TestMethod()]
    public void RoutingOfInflow2()
    {
      Stream S = new Stream("S", 25,1,1);

      DateTime Start = DateTime.Now;
      S.SetState("Initial", Start, new WaterPacket(1, 25));

      Stream s2 = new Stream("s2", 50,1,1);
      s2.SetState("Initial", Start, new WaterPacket(50));

      Stream s3 = new Stream("s3", 300,1,1);
      s3.SetState("Initial", Start, new WaterPacket(300));

      S.AddDownStreamWaterBody(s2);
      s2.AddDownStreamWaterBody(s3);

      SinkSourceBoundary FB = new SinkSourceBoundary(0.0005);
      FB.WaterSample = new WaterPacket(5, 5);

      S.Sources.Add(FB);

      TimeSpan ts = new TimeSpan(1, 0, 0);

      WaterPacket WaterProvider = new WaterPacket(2, 200);

      S.AddWaterPacket(DateTime.Now, DateTime.Now, WaterProvider.DeepClone(200));
      S.Update(S.CurrentTime.Add(ts));
    }

    [TestMethod]
    public void RoutingOfInflow3()
    {
      Stream s = new Stream("s", 10,1,1);
      s.SetState("Initial", DateTime.Now, new WaterPacket(10));
      SinkSourceBoundary fb = new SinkSourceBoundary(1);
      s.Sources.Add(fb);
      
      s.Update(s.CurrentTime.Add(TimeSpan.FromHours(1)));

      Assert.AreEqual(1, s.Output.Outflow.Items.First().Value, 0.00001);

    }


    [TestMethod()]
    public void RoutingOfGroundwaterTest()
    {

      Stream S = new Stream("S", 100,1,1);

      S.SetState("Initial", DateTime.Now, new WaterPacket(100));

      TimeSpan ts = new TimeSpan(0,1,0);

      int Id = 2;
      IWaterPacket expected = new WaterPacket(Id, 200);
      IWaterPacket actual;

      S.WaterLevel = 8;
      GroundWaterBoundary b = new GroundWaterBoundary(S, 0.001, 10, 100, XYPolygon.GetSquare(250));
      b.WaterSample = expected;
      S.GroundwaterBoundaries.Add(b);
      S.Update(S.CurrentTime.Add(ts));

      actual = S.CurrentStoredWater;
      double ExpectedVolume = b.GetSourceWater(DateTime.Now, ts).Volume;

      Assert.AreEqual(expected.Composition.Keys.First(), actual.Composition.Keys.First());
      Assert.AreEqual(100, actual.Volume, 0.000001);

      S.Update(S.CurrentTime.Add(ts));

      actual = S.CurrentStoredWater;
      Assert.AreEqual(expected.Composition.Keys.First(), actual.Composition.Keys.First());
      Assert.AreEqual(100, actual.Volume, 0.000001);

      S.AddWaterPacket(DateTime.Now, DateTime.Now, expected);
      S.Update(S.CurrentTime.Add(ts));
      actual = S.CurrentStoredWater;

      Assert.AreEqual(100, actual.Volume, 0.000001);

    }

    [TestMethod]
    public void RoutingOfChemical()
    {
      Stream_Accessor s = new Stream_Accessor("s", 100, 1, 1);

      s.SetState("Initial", DateTime.Now, new WaterPacket(100));

      SinkSourceBoundary fb = new SinkSourceBoundary(50);
      s.Sources.Add(fb);
      WaterWithChemicals Wcc = new WaterWithChemicals(50);
      Chemical c = ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl);
      Wcc.AddChemical(c, 1);

      fb.WaterSample = Wcc.DeepClone();

      double conc = Wcc.GetConcentration(c);
      Assert.AreEqual(1.0 / 50.0, conc, 0.000001);

      TimeSpan ts = new TimeSpan(0, 0, 1);

      s.Output.LogChemicalConcentration(c);

      s.AddWaterPacket(DateTime.Now, DateTime.Now.AddDays(1), Wcc);
      s.Update(s.CurrentTime.Add(ts));

      WaterWithChemicals WccNew = (WaterWithChemicals)s._waterInStream.Last();
      Assert.AreEqual(64.8721, WccNew.Volume, 0.0001);
      Assert.AreEqual(TimeSpan.FromSeconds(0.5), WccNew.WaterAge);
      Assert.AreEqual(conc, WccNew.GetConcentration(c));

      s.AddWaterPacket(DateTime.Now, DateTime.Now.AddDays(1), Wcc);
      s.Update(s.CurrentTime.Add(TimeSpan.FromDays(1)));

      Assert.AreEqual(conc, s.Output.ChemicalsToLog[c].Items[0].Value);


    }

    [TestMethod]
    public void OutputTest()
    {

      TimeSpan ts = new TimeSpan(0,1,0);
      Stream S = new Stream("S", 10,1,1);

      S.SetState("Initial", DateTime.Now, new WaterPacket(1, 10));
      

      for (int i = 0; i < 10; i++)
      {
        S.AddWaterPacket(DateTime.Now, DateTime.Now.AddDays(1), new WaterPacket(25 * i));
        S.Update(S.CurrentTime.Add(ts));
      }

      for (int i = 0; i < 10; i++)
      {
        Assert.AreEqual(i * 25.0 / ts.TotalSeconds, S.Output.Outflow.Items[i].Value);
      }
    }

    [TestMethod]
    public void SortingOfIncomingWater()
    {
      Stream_Accessor s = new Stream_Accessor("s", 100,1,1);

      
      WaterPacket wp1 = new WaterPacket(1, 50);
      WaterPacket wp2 = new WaterPacket(2,100);

      s.AddWaterPacket(new DateTime(2000, 1, 1), new DateTime(2000, 1, 11), wp1);
      s.AddWaterPacket(new DateTime(2000, 1, 6), new DateTime(2000, 1, 11), wp2);

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

      s.AddWaterPacket(new DateTime(2000, 1, 1), new DateTime(2000, 1, 3), wp3);
      s.AddWaterPacket(new DateTime(2000, 1, 2), new DateTime(2000, 1, 3), wp4);
      s.AddWaterPacket(new DateTime(2001, 1, 1, 12, 0, 0), new DateTime(2001, 1, 5), wp5);

      s.PrePareIncomingWater();

     

    }


  }
}
