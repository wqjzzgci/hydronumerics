using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.HydroNet.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;


namespace HydroNumerics.HydroNet.Core.UnitTest
{
  /// <summary>
  /// Summary description for VedstedPresentation
  /// </summary>
  [TestClass]
  public class VedstedPresentation
  {
    public VedstedPresentation()
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
      Lake Vedsted = LakeFactory.GetLake("Vedsted Sø");
      Vedsted.Depth = 5;
      Vedsted.WaterLevel = 45.7;

      //Create and add precipitation boundary
      TimespanSeries Precipitation = new TimespanSeries();
      double[] values = new double[] { 108, 83, 73, 52, 61, 86, 99, 101, 75, 108, 85, 101 };
      LakeVedsted.AddMonthlyValues(Precipitation, 2007, values);
      FlowBoundary Precip = new FlowBoundary(Precipitation);
      Precip.ContactArea = Vedsted.SurfaceArea;
      Vedsted.SinkSources.Add(Precip);

      //Create and add evaporation boundary
      TimespanSeries Evaporation = new TimespanSeries();
      double[] values2 = new double[] { 4, 11, 34, 66, 110, 118, 122, 103, 61, 26, 7, 1 };
      LakeVedsted.AddMonthlyValues(Evaporation, 2007, values2);
      EvaporationRateBoundary eva = new EvaporationRateBoundary(Evaporation);
      eva.ContactArea = Vedsted.SurfaceArea;
      Vedsted.EvaporationBoundaries.Add(eva);

      //Create and add a discharge boundary
      TimestampSeries Discharge = new TimestampSeries();
      Discharge.AddSiValue(new DateTime(2007, 3, 12), 6986 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.AddSiValue(new DateTime(2007, 4, 3), 5894 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.AddSiValue(new DateTime(2007, 4, 25), 1205 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.RelaxationFactor = 1;
      Discharge.AllowExtrapolation = true;
      Assert.AreEqual(Discharge.GetValue(new DateTime(2007, 4, 25)), Discharge.GetValue(new DateTime(2007, 6, 25)), 0.0000001);
      FlowBoundary Kilde = new FlowBoundary(Discharge);
      Vedsted.SinkSources.Add(Kilde);

      //Add a groundwater boundary
      GroundWaterBoundary gwb = new GroundWaterBoundary(Vedsted, 1e-5, ((XYPolygon)Vedsted.Geometry).GetArea(), 1, 46);

      DateTime Start = new DateTime(2007, 1, 1);

      //Add the chemicals
      Chemical cl = ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl);
      
      //Tell the lake to log the chemicals
      Vedsted.Output.LogChemicalConcentration(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction));
      Vedsted.Output.LogChemicalConcentration(cl);

      IsotopeWater Iw = new IsotopeWater(1);
      Iw.SetIsotopeRatio(10);
      Iw.AddChemical(cl, 0.1);
      Precip.WaterSample = Iw.DeepClone();

      //Evaporate some of the water to get realistic initial conditions
      Iw.Evaporate(Iw.Volume / 2);
      Vedsted.SetState("Initial", Start, Iw.DeepClone());
      Kilde.WaterSample = Iw.DeepClone();

      Iw.Evaporate(Iw.Volume / 2);
      gwb.WaterSample = Iw.DeepClone();

      //Add to an engine
      Model Engine = new Model();
      Engine.Name = "Vedsted-opsætning";
      Engine._waterBodies.Add(Vedsted);

      //Set initial state
      Engine.SetState("Initial", Start, new WaterPacket(1));

      Engine.Save(@"c:\temp\setup.xml");


    }
  }
}
