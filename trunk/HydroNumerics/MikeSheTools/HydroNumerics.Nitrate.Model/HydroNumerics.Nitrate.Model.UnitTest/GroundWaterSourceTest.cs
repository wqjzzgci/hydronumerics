using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Diagnostics;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for GroundWaterSourceTest and is intended
    ///to contain all GroundWaterSourceTest Unit Tests
    ///</summary>
  [TestClass()]
  public class GroundWaterSourceTest
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
    ///A test for GroundWaterSource Constructor
    ///</summary>
    [TestMethod()]
    public void LoadParticlesTest()
    {
      GroundWaterSource target = new GroundWaterSource();
      System.Diagnostics.Stopwatch stw = new Stopwatch();


      stw.Start();
      target.LoadParticles(@"E:\dhi\data\dkm\dk1\result\DK1_2014_pt_produktion.she - Result Files\PTReg_Extraction_1_Sink_Unsaturated_zone.shp");
      stw.Stop();
      stw.Reset();
      int k = 0;

      using (HydroNumerics.Geometry.Shapes.ShapeWriter sw = new Geometry.Shapes.ShapeWriter(@"d:\temp\unsatendpoints.shp"))
      {
        foreach (var m11 in target.Particles)
        {
          sw.WritePointShape(m11.X, m11.Y);
        }
      }


    }

    [TestMethod]
    public void LoadDaisyTest()
    {
      GroundWaterSource target = new GroundWaterSource();
      Stopwatch sw = new Stopwatch();
      sw.Start();

      target.LoadDaisyData(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2007.txt");
      target.LoadDaisyData(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2008.txt");
      target.LoadDaisyData(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2009.txt");
      sw.Stop();



      Assert.AreEqual(0.3305, target.leachdata.Grids[16510].TimeData.GetValues(new DateTime(2008, 4, 1), new DateTime(2009, 4, 1)).First(),0.0001);


      int k = 0;


    }


    [TestMethod]
    public void LoadAndCombineTest()
    {
      MainViewModel mv = new MainViewModel();
      mv.LoadCatchments(@"D:\DK_information\TestData\FileStructure\id15_NSTmodel.shp");

      GroundWaterSource target = new GroundWaterSource();
      target.LoadParticles(@"D:\DK_information\TestData\FileStructure\Particles\PTReg_Extraction_1_20131007_dk2.shp");

      target.LoadSoilCodesGrid(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\DKDomainNodes_LU_Soil_codes.shp");
      target.LoadDaisyData(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\Leaching_area_2.txt");

      target.CombineParticlesAndCatchments(mv.AllCatchments.Values);
      Assert.AreEqual(0, target.Particles.Count(P => P == null));
      Assert.AreEqual(0, mv.AllCatchments.Values.SelectMany(c => c.Particles.Where(P => P == null)).Count());
      Stopwatch sw = new Stopwatch();
      sw.Start();
      target.BuildInputConcentration(new DateTime(1994, 1, 1), new DateTime(2008, 5, 1),mv.AllCatchments.Values, 100);
      sw.Stop();

    }

    
  }
}
