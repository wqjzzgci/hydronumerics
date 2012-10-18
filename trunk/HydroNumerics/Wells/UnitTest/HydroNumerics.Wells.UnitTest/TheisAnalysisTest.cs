using HydroNumerics.Wells;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HydroNumerics.Geometry;

namespace HydroNumerics.Wells.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TheisAnalysisTest and is intended
    ///to contain all TheisAnalysisTest Unit Tests
    ///</summary>
  [TestClass()]
  public class TheisAnalysisTest
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
    ///A test for Drawdown
    ///</summary>
    [TestMethod()]
    public void DrawdownTest()
    {
      TheisAnalysis target = new TheisAnalysis(); // TODO: Initialize to an appropriate value
      IXYPoint PumpingWell = new XYPoint(0, 0);
      double PumpingRate = 5000.0/24.0 / 3600.0; //5000 m3/day
      double Storativity = 1.83e-3;
      double Transmissivity = 164.49 / 24.0 / 3600.0; //90 m2/day
      TimeSpan Time = TimeSpan.FromMinutes(100);
      IXYPoint ObservationPoint = new XYPoint(50, 50);
      double actual;
      actual = target.Drawdown(PumpingWell, PumpingRate, Storativity, Transmissivity, Time, ObservationPoint);

      Assert.AreEqual(2.959, actual, 0.01);
    }


    [TestMethod]
    public void NovoTest()
    {
      TheisAnalysis target = new TheisAnalysis(); // TODO: Initialize to an appropriate value
      IXYPoint PumpingWell = new XYPoint(0, 0);
      double PumpingRate = 1600.0 / 24.0 / 3600.0/3; //5000 m3/day
      double Storativity = 1e-3;
      double Transmissivity = 5e-3; //90 m2/day


      List<XYPoint> InjectionWells = new List<XYPoint>();

      InjectionWells.Add(new XYPoint(704288.01, 6201186.04));
      InjectionWells.Add(new XYPoint(704455.3,6201201.6 ));
      InjectionWells.Add(new XYPoint(704621.99, 6201219));
//      InjectionWells.Add(new XYPoint(704951.98, 6201249.18));

      List<XYPoint> ExtractionWells = new List<XYPoint>();

      ExtractionWells.Add(new XYPoint(704934.54, 6200994.85));
      ExtractionWells.Add(new XYPoint(704896.26, 6200720.37));
      ExtractionWells.Add(new XYPoint(704940, 6200550));



      var d = target.Drawdown(InjectionWells.First(), 56.0/3600.0, Storativity, Transmissivity, TimeSpan.FromHours(4), new XYPoint(InjectionWells.First().X + 2, InjectionWells.First().Y +2));

      List<double> drawdowns = new List<double>();

      IXYPoint ObservationPoint = new XYPoint(704932, 6200870);

      for (int i = 0; i < 100; i++)
      {
        double s = 0;
        TimeSpan Time = TimeSpan.FromDays(i);

        //foreach (var w in InjectionWells)
        //  s += target.Drawdown(w, -PumpingRate, Storativity, Transmissivity, Time, ObservationPoint);

        foreach (var w in ExtractionWells)
          s += target.Drawdown(w, PumpingRate, Storativity, Transmissivity, Time, ObservationPoint);

        drawdowns.Add(s);
      }

      Assert.AreEqual(2.959, d, 0.01);




    }


  }
}
