using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Geometry;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
  /// <summary>
  /// Summary description for CookEtAl
  /// </summary>
  [TestClass]
  public class CookEtAl
  {
    public CookEtAl()
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
      Model M = new Model();
      
        WaterPacket HyporhericWater = new WaterPacket(1);
        HyporhericWater.AddChemical(ChemicalFactory.Instance.GetChemical(ChemicalNames.Radon), 0.6 / HyporhericWater.Volume);       

      for (int i = 0; i < 10; i++)
      {
        Lake s1 = new Lake("s" + i,XYPolygon.GetSquare(50*2));
        s1.Depth = 0.3;
        StagnantExchangeBoundary seb = new StagnantExchangeBoundary(s1.Volume/20000);
        seb.WaterSample = HyporhericWater.DeepClone(s1.Area*0.2*0.4);

        s1.Output.LogAllChemicals = true;
        s1.Sinks.Add(seb);
        s1.Sources.Add(seb);

        if (i > 0)
          M._waterBodies[i - 1].AddDownStreamWaterBody(s1);

        M._waterBodies.Add(s1);

      }

      TimespanSeries ts = new TimespanSeries();
      ts.AddSiValue(DateTime.MinValue, new DateTime(2005,10,18,12,0,0),0);
      ts.AddSiValue(new DateTime(2005,10,18,12,0,0),new DateTime(2005,10,18,12,40,0),0.001*60);
      ts.AddSiValue(new DateTime(2005,10,18,12,40,0), DateTime.MaxValue, 0);

      SinkSourceBoundary Bromide = new SinkSourceBoundary(ts);
      WaterPacket P = new WaterPacket(1);
      P.AddChemical(new Chemical("Bromide", 1), 1.13);
      Bromide.WaterSample = P;

      M._waterBodies.First().Sources.Add(Bromide);
      M._waterBodies.First().Sources.Add(new SinkSourceBoundary(0.2));
      
      DateTime Start = new DateTime(2005,10,13);
      DateTime End = new DateTime(2005,10,19);

      M.SetState("Initial", Start, new WaterPacket(1));

      M.MoveInTime(new DateTime(2005, 10, 18, 12, 0, 0), TimeSpan.FromHours(2));
      M.MoveInTime(new DateTime(2005, 10, 18, 13, 0, 0), TimeSpan.FromHours(0.02));
      M.MoveInTime(End,TimeSpan.FromHours(2));
      M.Save(@"..\..\..\TestData\CookEtAl.xml");

    }
  }
}
