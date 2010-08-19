using System;
using System.IO;
using HydroNumerics.MikeSheTools.PFS.SheFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.MikeSheTools.PFS.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for InputFileTest and is intended
    ///to contain all InputFileTest Unit Tests
    ///</summary>
  [TestClass()]
  public class SheFileTest
  {


    private static InputFile _she;

    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
      _she = new InputFile(@"..\..\..\PFS\unittest\TestData\TestModel.she");
    }


    [TestMethod]
    public void ReadDetailedTSTest()
    {
      Assert.AreEqual(2, _she.MIKESHE_FLOWMODEL.StoringOfResults.DetailedTimeseriesOutput.Item_1s.Count);
    }


    [TestMethod]
    public void ReadTest()
    {
      Assert.AreEqual(1, _she.MIKESHE_FLOWMODEL.SimSpec.ModelComp.WM);

      Assert.AreEqual(Path.GetFullPath(@"..\..\..\PFS\unittest\TestData\Model Domain and Grid.dfs2"), _she.MIKESHE_FLOWMODEL.Catchment.DFS_2D_DATA_FILE.FILE_NAME);

      Assert.AreEqual(4, _she.MIKESHE_FLOWMODEL.LandUse.CommandAreas.CommandAreas1.Count);
      Assert.AreEqual("140", _she.MIKESHE_FLOWMODEL.LandUse.CommandAreas.CommandAreas1[0].AreaName);

      Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0), _she.MIKESHE_FLOWMODEL.SimSpec.SimulationPeriod.StartTime);
      Assert.AreEqual(new DateTime(2000, 2, 1, 0, 0, 0), _she.MIKESHE_FLOWMODEL.SimSpec.SimulationPeriod.EndTime);

    }

    [TestMethod]
    public void ModifyTest()
    {

      _she.MIKESHE_FLOWMODEL.LandUse.CommandAreas.AddNewCommandArea();

      _she.MIKESHE_FLOWMODEL.SimSpec.SimulationPeriod.EndTime = DateTime.Now;
      _she.MIKESHE_FLOWMODEL.SimSpec.SimulationPeriod.StartTime = DateTime.Now;
      _she.SaveAs(@"..\..\..\PFS\unittest\TestData\TestModel_changed.she");

    }

    [TestMethod]
    public void WriteTest()
    {
      _she.MIKESHE_FLOWMODEL.SimSpec.ModelComp.WM = 0;
      _she.SaveAs(@"..\..\..\PFS\unittest\TestData\TestModel_changed.she");
    }

  }
}
