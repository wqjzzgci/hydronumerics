using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
  /// <summary>
  /// Summary description for RadonExample
  /// </summary>
  [TestClass]
  public class RadonExample
  {
    public RadonExample()
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
      Lake L = new Lake("Deep lake", XYPolygon.GetSquare(10000));
      L.Depth = 4;
      L.Output.LogAllChemicals = true;

      Lake L2 = new Lake("Shallow lake", XYPolygon.GetSquare(40000));
      L2.Depth = 1;
      L2.Output.LogAllChemicals = true;

      SinkSourceBoundary flow = new SinkSourceBoundary(L.Volume / (15.0 * 86400.0));

      L.Sources.Add(flow);
      L2.Sources.Add(flow);

      Chemical rn = ChemicalFactory.Instance.GetChemical(ChemicalNames.Radon);
      Chemical cl = ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl);
      WaterPacket groundwater = new WaterPacket(1);
      groundwater.SetConcentration(rn, 200);
      groundwater.SetConcentration(cl, 200);
      SinkSourceBoundary gwflow = new SinkSourceBoundary(L.Volume / (15.0 * 86400.0));
      gwflow.WaterSample = groundwater;

      L.Sources.Add(gwflow);
      L2.Sources.Add(gwflow);

      Model M = new Model();
      M.WaterBodies.Add(L);
      M.WaterBodies.Add(L2);

      DateTime start = new DateTime(2010,1,1);

      M.SetState("Initial", start, new WaterPacket(1));
      M.MoveInTime(new DateTime(2010, 12, 31), TimeSpan.FromDays(5));
      M.Save(@"..\..\..\TestData\Radon.xml");

    }
  }
}
