using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Diagnostics;

using HydroNumerics.Core.Time;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MainViewModelTest and is intended
    ///to contain all MainViewModelTest Unit Tests
    ///</summary>
  [TestClass()]
  public class MainViewModelTest
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
    ///A test for CurrentCatchment
    ///</summary>
    [Ignore]
    [TestMethod()]
    public void CurrentCatchmentTest()
    {
      MainModel target = new MainModel();
      target.LoadCatchments(@"D:\DK_information\TestData\FileStructure\id15_NSTmodel.shp");
      target.CurrentCatchment = target.AllCatchments.Values.First();
      Stopwatch sw = new Stopwatch();
      sw.Start();

      

      sw.Stop();

      sw.Reset();

    }


    

    [TestMethod]
    public void TestPolygonHole()
    {
      MainModel target = new MainModel();
      target.LoadCatchments(@"D:\NitrateModel\Overfladevand\oplande\id15_NSTmodel_maj2014.shp");
      Assert.IsFalse( target.AllCatchments[44601235].Geometry.Contains(563937, 6211641));
    }

    /// <summary>
    ///A test for LoadMikeSheData
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void LoadMikeSheDataTest()
    {
      MainModel target = new MainModel(); // TODO: Initialize to an appropriate value
      target.LoadCatchments(@"D:\DK_information\TestData\FileStructure\id15_NSTmodel.shp");
      target.LoadMikeSheData(@"E:\dhi\data\dkm\dk2\result\DK2_v3_gvf_PT_100p_24hr.she");

      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"d:\temp\precip.csv"))
      {
        foreach (var c in target.AllCatchments.Values)
        {
          if (c.Precipitation == null)
            sw.WriteLine(c.ID + ",,");
          else
          {
            foreach (var year in c.Precipitation.GetTs(TimeStepUnit.Year).Items)
              sw.WriteLine(c.ID + "," + year);
          }
        }
      }
    }

    class MainViewModelIn : MainModel
    {

      public SmallLakesSink lake
      {
        get
        {
          return this.InternalReductionModels.FirstOrDefault(i => i.GetType() == typeof(SmallLakesSink)) as SmallLakesSink;
        }
      }
    }

    [TestMethod()]
    public void MainViewModelConstructorTest2()
    {


      MainViewModelIn target = new MainViewModelIn();
      target.ReadConfiguration(@"F:\Oplandsmodel\NitrateModel\config_clgw122_Rerun11.xml");
      target.Initialize();

      double m1 = target.lake.GetReduction(target.AllCatchments[31100002], double.MaxValue, new DateTime(2010, 1, 1));
      double m2 = target.lake.GetReduction(target.AllCatchments[31100002], double.MaxValue, new DateTime(2010, 5, 1));
      double m3 = target.lake.GetReduction(target.AllCatchments[31100002], double.MaxValue, new DateTime(2010, 12, 1));
      Assert.AreEqual(7.71675040416482E-06, m1, 1e-12);
      Assert.AreEqual(4.01512112598263E-06, m2, 1e-12);
      Assert.AreEqual(9.49467691067628E-06, m3, 1e-12);

    }


    /// <summary>
    ///A test for MainViewModel Constructor
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void MainViewModelConstructorTest()
    {
      MainModel target = new MainModel();
      target.ReadConfiguration(@"D:\Work\HydroNumerics\MikeSheTools\HydroNumerics.Nitrate.Model\config.xml");
      target.Initialize();

      target.Run();

      target.Print();
    }
  }
}
