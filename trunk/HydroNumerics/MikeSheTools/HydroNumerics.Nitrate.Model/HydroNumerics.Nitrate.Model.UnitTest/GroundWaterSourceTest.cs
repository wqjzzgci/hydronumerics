using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

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
      var particles = target.LoadParticles(@"f:\results\PTReg_DK1_R201401_minB_1Half_meters.shp");
      stw.Stop();

      Assert.AreEqual(2432948, particles.Count());

      stw.Reset();

      MainViewModel mv = new MainViewModel();
      mv.LoadCatchments(@"D:\NitrateModel\Overfladevand\oplande\id15_NSTmodel_02062014.shp");
      var ca = mv.AllCatchments[71288759];

      int pcount=0;
      foreach (var p in particles)
        if (ca.Geometry.Contains(p))
          pcount++;
      //int k = 0;
      Assert.AreEqual(3309, pcount);

      var x_avg = particles.Average(p => p.X);
      var y_avg = particles.Average(p => p.Y);

      var multies = mv.AllCatchments.Values.Where(can => can.Geometry is Geometry.MultiPartPolygon).ToList();

      target.CombineParticlesAndCatchments(multies, particles);

      int totalfound = mv.AllCatchments.Values.Sum(c => c.Particles.Count);
      Assert.AreEqual(172694, totalfound);


      target.CombineParticlesAndCatchments(mv.AllCatchments.Values.Where(can=>can.Geometry is Geometry.XYPolygon), particles);

      Assert.AreEqual(x_avg, particles.Average(p => p.X));
      Assert.AreEqual(y_avg, particles.Average(p => p.Y));

      int k3 = mv.AllCatchments.Values.Count(c => c.Particles.Count >= 20);

      totalfound = mv.AllCatchments.Values.Sum(c => c.Particles.Count);

      Assert.AreEqual(679135.13736685168, x_avg);
      Assert.AreEqual(6161058.8180519734, y_avg);
      //Assert.AreEqual(557, k1);
      //Assert.AreEqual(k2, k3);
      //Assert.AreEqual(2185894, totalfound);


      //k1 = mv.AllCatchments.Values.Count(c => c.addcalls > 0);
      //totalfound = mv.AllCatchments.Values.Sum(c => c.Particles.Count);
      Assert.AreEqual(2358588, totalfound);

      //using (HydroNumerics.Geometry.Shapes.ShapeWriter sw = new Geometry.Shapes.ShapeWriter(@"d:\temp\unsatendpoints.shp"))
      //{
      //  foreach (var m11 in particles)
      //  {
      //    sw.WritePointShape(m11.X, m11.Y);
      //  }
      //}


    }

    [TestMethod]
    [Ignore]
    public void LoadDaisyTest()
    {
      GroundWaterSource target = new GroundWaterSource();
      Stopwatch sw = new Stopwatch();
      sw.Start();

      target.LoadDaisyData(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2007.txt");
      target.LoadDaisyData(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2008.txt");
      target.LoadDaisyData(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2009.txt");
      sw.Stop();



//      Assert.AreEqual(0.3305, target.leachdata.Grids[16510].TimeData.GetValues(new DateTime(2008, 4, 1), new DateTime(2009, 4, 1)).First(),0.0001);


      int k = 0;


    }
    private object Lock = new object();

    [Ignore]
    [TestMethod]
    public void CreateLeachFile()
    {
      var gwsource = new GroundWaterSource();
      gwsource.DaisyFiles.Add(new SafeFile(){FileName =@"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1990.txt"});
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1991.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1992.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1993.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1994.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1995.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1996.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1997.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1998.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily1999.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2000.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2001.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2002.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2003.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2004.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2005.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2006.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2007.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2008.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2009.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2010.txt" });
      gwsource.DaisyFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\SoilFarms_dmi10kmgrid_daily2011.txt" });

      gwsource.SoilCodes = new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\DaisyLeaching\DKDomainNodes_LU_Soil_codes.shp" };

      gwsource.ParticleFiles.Add(new SafeFile() { FileName = @"D:\DK_information\TestData\FileStructure\Particles\PTReg_Extraction_1_20131007_dk2.shp" });
      gwsource.ParticleFiles.Last().Parameters.Add(100);

      MainViewModel mv = new MainViewModel();
      mv.LoadCatchments(@"D:\DK_information\TestData\FileStructure\id15_NSTmodel.shp");

      Stopwatch sw = new Stopwatch();
      sw.Start();
      gwsource.Initialize(new DateTime(1991, 1, 1), new DateTime(2010, 1, 1), mv.AllCatchments.Values);
      sw.Stop();

      
      var ts = sw.Elapsed;

      //using (HydroNumerics.Geometry.Shapes.ShapeWriter sw = new Geometry.Shapes.ShapeWriter(@"D:\DK_information\TestData\leach1990MontlyPar"))
      //{
      //  System.Data.DataTable dt = new System.Data.DataTable();

      //  dt.Columns.Add("ID15", typeof(int));
      //  dt.Columns.Add("Januar", typeof(double));
      //  dt.Columns.Add("Februar", typeof(double));
      //  dt.Columns.Add("Marts", typeof(double));
      //  dt.Columns.Add("April", typeof(double));
      //  dt.Columns.Add("Maj", typeof(double));
      //  dt.Columns.Add("Juni", typeof(double));
      //  dt.Columns.Add("Juli", typeof(double));
      //  dt.Columns.Add("August", typeof(double));
      //  dt.Columns.Add("September", typeof(double));
      //  dt.Columns.Add("Oktober", typeof(double));
      //  dt.Columns.Add("November", typeof(double));
      //  dt.Columns.Add("December", typeof(double));

      //  foreach (var c in mv.AllCatchments.Values)
      //  {
      //    var dr = dt.NewRow();
      //    dr[0] = c.ID;
      //    var data = CatchLeach[c.ID];

      //    for (int i = 0; i < 12; i++)
      //      dr[i + 1] = data[i];
      //    sw.Write(new HydroNumerics.Geometry.GeoRefData() { Geometry = c.Geometry, Data = dr });
      //  }
      //}

      //using (HydroNumerics.Geometry.Shapes.ShapeWriter sw = new Geometry.Shapes.ShapeWriter(@"D:\DK_information\TestData\leachYearlyscaledPar"))
      //{
      //  System.Data.DataTable dt = new System.Data.DataTable();

      //  dt.Columns.Add("ID15", typeof(int));

      //  for (int i= Start.Year; i<= End.Year;i++)
      //  {
      //    dt.Columns.Add(i.ToString(), typeof(double));
      //  }

      //  foreach (var c in mv.AllCatchments.Values)
      //  {
      //    var dr = dt.NewRow();
      //    dr[0] = c.ID;
      //    var data = CatchLeach[c.ID];

      //    for (int i = 0; i < End.Year - Start.Year; i++)
      //      dr[i + 1] = data.Skip(i * 12).Take(12).Sum()/((HydroNumerics.Geometry.IXYPolygon)c.Geometry).GetArea(); 
      //    sw.Write(new HydroNumerics.Geometry.GeoRefData() { Geometry = c.Geometry, Data = dr });
      //  }
      //}



      //double sum = CatchLeach.Values.Sum(c=>c.Sum(v=>v));

    }








    

    [Ignore]
    [TestMethod]
    public void LoadAndCombineTest()
    {
      MainViewModel mv = new MainViewModel();
      mv.LoadCatchments(@"D:\DK_information\TestData\FileStructure\id15_NSTmodel.shp");

      GroundWaterSource target = new GroundWaterSource();
      var particles =target.LoadParticles(@"D:\DK_information\TestData\FileStructure\Particles\PTReg_Extraction_1_20131007_dk2.shp");

      target.LoadSoilCodesGrid(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\DKDomainNodes_LU_Soil_codes.shp");
      target.LoadDaisyData(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\Leaching_area_2.txt");

      target.CombineParticlesAndCatchments(mv.AllCatchments.Values, particles);
      Assert.AreEqual(0, particles.Count(P => P == null));
      Assert.AreEqual(0, mv.AllCatchments.Values.SelectMany(c => c.Particles.Where(P => P == null)).Count());
      Stopwatch sw = new Stopwatch();
      sw.Start();
      target.BuildInputConcentration(new DateTime(1994, 1, 1), new DateTime(2008, 5, 1),mv.AllCatchments.Values, 100);
      sw.Stop();

    }

    
  }
}
