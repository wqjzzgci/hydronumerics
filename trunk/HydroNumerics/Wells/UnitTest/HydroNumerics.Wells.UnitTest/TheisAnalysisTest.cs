using HydroNumerics.Wells;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

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
      double PumpingRate = 15 / 3600.0; //
      double Storativity = 5e-3;
      double Transmissivity = 1e-3; //


      List<IXYPoint> InjectionWells = new List<IXYPoint>();

//      InjectionWells.Add(new XYPoint(704288.01, 6201186.04));
//      InjectionWells.Add(new XYPoint(704455.3,6201201.6 ));
//      InjectionWells.Add(new XYPoint(704621.99, 6201219));
////      InjectionWells.Add(new XYPoint(704951.98, 6201249.18));

      List<IXYPoint> ExtractionWells = new List<IXYPoint>();

      //ExtractionWells.Add(new XYPoint(704934.54, 6200994.85));
      //ExtractionWells.Add(new XYPoint(704896.26, 6200720.37));
      //ExtractionWells.Add(new XYPoint(704940, 6200550));


      //InjectionWells.Add(new XYPoint(704698, 6202442));
      //ExtractionWells.Add(new XYPoint(704850, 6202365));


      using (ShapeReader sr = new ShapeReader(@"C:\Users\Jacob\Dropbox\Enopsol\BiogenLocation.shp"))
      {

        foreach (var g in sr.GeoData)
        {
          if ((int)g.Data[0] == 1)
            InjectionWells.Add((IXYPoint)g.Geometry);
          else if ((int)g.Data[0] == 2)
            ExtractionWells.Add((IXYPoint)g.Geometry);
        }
      }



      var d = target.Drawdown(InjectionWells.First(), 56.0/3600.0, Storativity, Transmissivity, TimeSpan.FromHours(4), new XYPoint(InjectionWells.First().X + 2, InjectionWells.First().Y +2));

      List<List<double>> drawdowns = new List<List<double>>();

      drawdowns.Add(new List<double>());
      drawdowns.Add(new List<double>());
      drawdowns.Add(new List<double>());

      //Frederiksgade
      IXYPoint ObservationPoint = new XYPoint(705669, 6202256);

      int j = 0;
      for (int i = 0; i < 100; i++)
      {
        double s = 0;
        TimeSpan Time = TimeSpan.FromDays(i);

        foreach (var w in ExtractionWells)
          s += target.Drawdown(w, +PumpingRate, Storativity, Transmissivity, Time, ObservationPoint);

        drawdowns[j].Add(s);


        foreach (var w in InjectionWells)
          s += target.Drawdown(w, -PumpingRate, Storativity, Transmissivity, Time, ObservationPoint);

        drawdowns[j+1].Add(s);

        double dw = target.Drawdown(ExtractionWells[0], PumpingRate, Storativity, Transmissivity, Time, new XYPoint(ExtractionWells[0].X+0.5,ExtractionWells[0].Y+0.5));
        drawdowns[j + 2].Add(dw);


      }

      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"c:\temp\enopsol.txt"))
      {
        for (int i = 0; i < 100; i++)
        {
          sw.WriteLine(drawdowns[0][i].ToString() + "\t" + drawdowns[1][i].ToString() + "\t" + drawdowns[2][i].ToString());

        }
      }
    }
  }
}
