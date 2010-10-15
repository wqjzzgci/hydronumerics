using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

      TimeSeriesGroup climate = TimeSeriesGroupFactory.Create("climate.xts");
      EvaporationRateBoundary evap = new EvaporationRateBoundary((TimespanSeries)climate.Items[1]);
      evap.ContactGeometry = Gjeller.Geometry;
      Gjeller.EvaporationBoundaries.Add(evap);

      SinkSourceBoundary precip = new SinkSourceBoundary(climate.Items[0]);
      precip.ContactGeometry = Gjeller.Geometry;
      Gjeller.Precipitation.Add(precip);

      Model M = new Model();
      M._waterBodies.Add(Gjeller);

      M.SetState("Initial", new DateTime(1995, 1, 1), new WaterPacket(1));


      M.MoveInTime(new DateTime(2005, 1, 1), TimeSpan.FromDays(1));
      M.Save(@"..\..\..\TestData\Gjeller.xml");

    }
  }
}
