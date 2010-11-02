using System;

using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Geometry;
using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
  /// <summary>
  /// Summary description for LakeGjeller
  /// </summary>
  [TestClass]
  public class LakeGjeller
  {
    public LakeGjeller()
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
      Lake Gjeller = LakeFactory.GetLake("Gjeller Sø");
      Gjeller.Depth = 1.2;
      Gjeller.WaterLevel = 0.4;

      WaterWithChemicals GjellerWater = new WaterWithChemicals(1, 1);
      GjellerWater.AddChemical(ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl), 1);

      TimeSeriesGroup climate = TimeSeriesGroupFactory.Create("climate.xts");

      foreach (var I in climate.Items)
      {
        I.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
        I.AllowExtrapolation = true;
      }

      EvaporationRateBoundary evap = new EvaporationRateBoundary((TimespanSeries)climate.Items[1]);
      evap.ContactGeometry = Gjeller.Geometry;
      Gjeller.EvaporationBoundaries.Add(evap);

      SinkSourceBoundary precip = new SinkSourceBoundary(climate.Items[0]);
      precip.ContactGeometry = Gjeller.Geometry;
      Gjeller.Precipitation.Add(precip);
      precip.ID = 2;
      precip.WaterSample = GjellerWater.DeepClone();
      precip.WaterSample.IDForComposition = precip.ID; ;

      GroundWaterBoundary GWIN = new GroundWaterBoundary(Gjeller, 1e-5, 2, 0.45, XYPolygon.GetSquare(Gjeller.Area/2));
      GWIN.WaterSample = GjellerWater.DeepClone();
      GWIN.ID = 3;
      GWIN.WaterSample.IDForComposition = GWIN.ID;
      GWIN.Name = "Inflow";
      Gjeller.GroundwaterBoundaries.Add(GWIN);

      GroundWaterBoundary GWout = new GroundWaterBoundary(Gjeller, 1e-5, 2, 0.35, XYPolygon.GetSquare(Gjeller.Area / 2));
      GWout.Name = "Outflow";
      Gjeller.GroundwaterBoundaries.Add(GWout);


      TimespanSeries pumping = new TimespanSeries();
      pumping.AddSiValue(new DateTime(1990, 01, 01), new DateTime(2010, 01, 01), 0);     
      pumping.AddSiValue(new DateTime(2010,01,01),new DateTime(2010,05,01),0.05);
      pumping.AddSiValue(new DateTime(2010, 05, 01), DateTime.Now, 0);     


      SinkSourceBoundary DrainageWater = new SinkSourceBoundary(pumping);
      DrainageWater.ID = 4;
      DrainageWater.WaterSample = GjellerWater.DeepClone();
      DrainageWater.WaterSample.IDForComposition = DrainageWater.ID;
      DrainageWater.Name = "Indpumpet Drænvand";
      Gjeller.Sources.Add(DrainageWater);

      Model M = new Model();
      M._waterBodies.Add(Gjeller);
      Gjeller.Output.LogAllChemicals = true;
      Gjeller.Output.LogComposition = true;

      M.SetState("Initial", new DateTime(1995, 1, 1), GjellerWater);


      M.MoveInTime(DateTime.Now, TimeSpan.FromDays(10));
      M.Save(@"..\..\..\TestData\Gjeller.xml");

    }
  }
}
