using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
  /// <summary>
  /// Summary description for LakeHampen
  /// </summary>
  [TestClass]
  public class LakeHampen
  {
    public LakeHampen()
    {
      //
      // TODO: Add constructor logic here
      //
    }

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
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    [TestMethod]
    public void TestMethod1()
    {
      Lake Hampen = LakeFactory.GetLake("Hampen Sø");
      Hampen.Depth = 3.2e6 / 760000/1000;

      DateTime start = new DateTime(2008, 1, 1);
      DateTime end = new DateTime(2008, 12, 31);

      Assert.AreEqual(Hampen.Area, 722200,1);

      EvaporationRateBoundary er = new EvaporationRateBoundary(407.0 / 1000 / 365 / 86400);
      er.ContactGeometry = Hampen.Geometry;
      er.Name = "Fordampning";
      Hampen.EvaporationBoundaries.Add(er);

      SourceBoundary pr = new SourceBoundary(901.0 / 1000 / 365 / 86400);
      pr.ContactGeometry = Hampen.Geometry;
      pr.Name = "Nedbør";
      Hampen.Precipitation.Add(pr);

      SinkSourceBoundary outlet = new SinkSourceBoundary(-200.0 / 1000 / 365 / 86400);
      outlet.ContactGeometry = Hampen.Geometry;
      outlet.Name = "Udløb";
      Hampen.Sinks.Add(outlet);

      GroundWaterBoundary gwb = new GroundWaterBoundary();
      gwb.FlowType = GWType.Flow;
      gwb.Name = "Ud";
      gwb.WaterFlow = new HydroNumerics.Time.Core.TimespanSeries("inflow", new DateTime(2008, 1, 1), 2, 1, HydroNumerics.Time.Core.TimestepUnit.Years, -294.0 / 1000 / 365 / 86400*Hampen.Area);
      Hampen.GroundwaterBoundaries.Add(gwb);


      Model m = new Model();
      m._waterBodies.Add(Hampen);

      m.SetState("start", start , new WaterPacket(1));
      m.SimulationStartTime = start;
      m.SimulationEndTime = end;
      m.MoveInTime(end,TimeSpan.FromDays(30));
      m.Save(@"..\..\..\TestData\Hampen1.xml");

      WaterPacket ChlorideWater = new WaterPacket(1);
      ChlorideWater.SetConcentration(ChemicalNames.Cl, 20);
      ChlorideWater.SetConcentration(ChemicalNames.IsotopeFraction, 4);
      ChlorideWater.SetConcentration(ChemicalNames.Nitrate, 0.2);
      ChlorideWater.SetConcentration(ChemicalNames.Phosphate, 0.02);

      m.SetState("start", start, ChlorideWater);
      Hampen.Output.LogAllChemicals = true;

      double gwinflow = 1000.0;


      gwb.WaterFlow = new HydroNumerics.Time.Core.TimespanSeries("inflow", new DateTime(2008, 1, 1), 2, 1, HydroNumerics.Time.Core.TimestepUnit.Years, -(294.0 + gwinflow) / 1000 / 365 / 86400 * Hampen.Area);

      GroundWaterBoundary gwbin = new GroundWaterBoundary();
      gwbin.FlowType = GWType.Flow;
      gwbin.WaterFlow = new HydroNumerics.Time.Core.TimespanSeries("inflow", new DateTime(2008, 1, 1), 2, 1, HydroNumerics.Time.Core.TimestepUnit.Years, 0.955*gwinflow / 1000 / 365 / 86400 * Hampen.Area);
      ChlorideWater.SetConcentration(ChemicalNames.Cl, 30);
      ChlorideWater.SetConcentration(ChemicalNames.IsotopeFraction, 8);
      ChlorideWater.SetConcentration(ChemicalNames.Nitrate, 1.6);
      ChlorideWater.SetConcentration(ChemicalNames.Phosphate, 0.017);
      gwbin.Name = "Ind Skov";
      gwbin.WaterSample = ChlorideWater.DeepClone();
      Hampen.GroundwaterBoundaries.Add(gwbin);

      GroundWaterBoundary gwbin2 = new GroundWaterBoundary();
      gwbin2.FlowType = GWType.Flow;
      gwbin2.WaterFlow = new HydroNumerics.Time.Core.TimespanSeries("inflow", new DateTime(2008, 1, 1), 2, 1, HydroNumerics.Time.Core.TimestepUnit.Years, 0.045 * gwinflow / 1000 / 365 / 86400 * Hampen.Area);
      ChlorideWater.SetConcentration(ChemicalNames.Nitrate, 65.3);
      gwbin2.Name = "Ind Landbrug";
      gwbin2.WaterSample = ChlorideWater.DeepClone();

      Hampen.GroundwaterBoundaries.Add(gwbin2);
      ChlorideWater.SetConcentration(ChemicalNames.Cl, 10);
      ChlorideWater.SetConcentration(ChemicalNames.Phosphate, 0);
      ChlorideWater.SetConcentration(ChemicalNames.Nitrate, 1.7);
      pr.WaterSample = ChlorideWater.DeepClone();

      m.MoveInTime(end, TimeSpan.FromDays(30));
      m.Save(@"..\..\..\TestData\Hampen2.xml");
    
    }
  }
}
