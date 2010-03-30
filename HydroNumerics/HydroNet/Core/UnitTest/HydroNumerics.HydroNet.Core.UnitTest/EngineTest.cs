using HydroNumerics.HydroNet.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Linq;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for EngineTest and is intended
    ///to contain all EngineTest Unit Tests
    ///</summary>
  [TestClass()]
  public class EngineTest
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
    ///A test for MoveInTime
    ///</summary>
    [TestMethod()]
    public void MoveInTimeTest1()
    {
      var Network = NetworkBuilder.CreateBranch(10);


      Network.First().AddWaterSinkSource(new FlowBoundary(1));

      Engine target = new Engine(Network.Cast<IWaterBody>());


      DateTime Start = new DateTime(2010, 1, 1);
      DateTime End = new DateTime(2010, 1, 10);
      TimeSpan TimeStep = new TimeSpan(1, 0, 0, 0);
      target.MoveInTime(Start, End, TimeStep);

      Assert.AreEqual(Network.First().CurrentStoredWater.Volume, Network.Last().CurrentStoredWater.Volume,0.0001);
      Assert.AreEqual(End, Network.First().CurrentStartTime);

    }

    /// <summary>
    ///A test for MoveInTime
    ///100      300
    ///*         *
    /// *       *
    ///  *     *
    ///   *   *
    ///    * *
    ///     *
    ///     *
    ///     *
    ///     *
    ///     *
    ///    400
    ///</summary>
    [TestMethod()]
    public void MoveInTimeTest2()
    {
      FlowBoundary b1 = new FlowBoundary(100);
      b1.WaterSample = new WaterPacket(1, 1);

      FlowBoundary b2 = new FlowBoundary(300);
      b2.WaterSample = new WaterPacket(2, 1);

      var Network = NetworkBuilder.CreateSortedYBranch(5, b1, b2);

      Engine target = new Engine(Network.Cast<IWaterBody>());


      DateTime Start = new DateTime(2010, 1, 1);
      DateTime End = new DateTime(2010, 1, 2);
      TimeSpan TimeStep = new TimeSpan(1, 0, 0, 0);
      target.MoveInTime(Start, End, TimeStep);

      Assert.AreEqual(Network.First().Output.Outflow.TimeValues.Last().Value * 4, Network.Last().Output.Outflow.TimeValues.Last().Value);
      Assert.AreEqual(End, Network.First().CurrentStartTime);

      Assert.AreEqual(0.25, Network.Last().CurrentStoredWater.Composition[b1.WaterSample.Composition.Keys.First()], 0.0001);
      Assert.AreEqual(0.75, Network.Last().CurrentStoredWater.Composition[b2.WaterSample.Composition.Keys.First()], 0.0001);

    }

        /// <summary>
    ///A test for MoveInTime with ChemicalWater
    ///100      300
    ///*         *
    /// *       *
    ///  *     *
    ///   *   *
    ///    * *
    ///     *
    ///     *
    ///     *
    ///     *
    ///     *
    ///    400
    ///</summary>
    [TestMethod()]
    public void MoveInTimeTest3()
    {
      FlowBoundary b1 = new FlowBoundary(100);
      WaterWithChemicals W1 = new WaterWithChemicals(1, 1);
      W1.AddChemical(new Chemical(new ChemicalType("Cl", 32), 2.3));

      double CLConc = W1.GetConcentration("Cl");

      b1.WaterSample = W1;

      FlowBoundary b2 = new FlowBoundary(300);

      WaterWithChemicals W2 = new WaterWithChemicals(2, 1);
      W2.AddChemical(new Chemical(new ChemicalType("Na", 12), 2.3));

      b2.WaterSample = W2;

      var Network = NetworkBuilder.CreateSortedYBranch(5, b1, b2);

 //     foreach (Stream IW in Network)
//        IW.CurrentStoredWater = new WaterWithChemicals(100);

      Engine target = new Engine(Network.Cast<IWaterBody>());


      DateTime Start = new DateTime(2010, 1, 1);
      DateTime End = new DateTime(2010, 1, 2);
      TimeSpan TimeStep = new TimeSpan(1, 0, 0, 0);
      target.MoveInTime(Start, End, TimeStep);

      Assert.AreEqual(((WaterWithChemicals)Network.First().CurrentStoredWater).Chemicals["Cl"].Moles, ((WaterWithChemicals)Network.Last().CurrentStoredWater).Chemicals["Cl"].Moles*4,0.000001);
      //Assert.AreEqual(((WaterWithChemicals)Network[5].CurrentRoutedWater).Chemicals["Cl"].Moles, ((WaterWithChemicals)Network.Last().CurrentRoutedWater).Chemicals["Na"].Moles);
    }

    [TestMethod]
    public void CompareStreamAndLakes()
    {
      var StreamNetwork =NetworkBuilder.CreateBranch(10);
      var LakeNetwork =NetworkBuilder.CreateConnectedLakes(10);

      Engine Streams = new Engine(StreamNetwork.Cast<IWaterBody>());
      Engine Lakes = new Engine(LakeNetwork.Cast<IWaterBody>());

      FlowBoundary b1 = new FlowBoundary(100);
      StreamNetwork.First().AddWaterSinkSource(b1);
      LakeNetwork.First().AddWaterSinkSource(b1);

      Stopwatch SW = new Stopwatch();
      Stopwatch SW2 = new Stopwatch();


      SW.Start();
      Streams.MoveInTime(new DateTime(2000, 1, 1), new DateTime(2000, 1, 10), TimeSpan.FromHours(5));
      SW.Stop();
      SW2.Start();
      Lakes.MoveInTime(new DateTime(2000, 1, 1), new DateTime(2000, 1, 10), TimeSpan.FromHours(5));
      SW2.Stop();

     TimeSeries TS1 = StreamNetwork.Last().Output.TimeSeriesList.First();
     TimeSeries TS2 = LakeNetwork.Last().Output.TimeSeriesList.First();

     for (int i = 0; i < TS1.TimeValues.Count; i++)
     {
       Assert.AreEqual(TS1.TimeValues[i].Value, TS2.TimeValues[i].Value, 0.000001);
       Assert.AreEqual(TS1.TimeValues[i].Time, TS2.TimeValues[i].Time);

     }


    }
  }
}
