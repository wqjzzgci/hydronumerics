using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using HydroNumerics.Time.Core;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;


using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.JupiterTools.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ReaderTest and is intended
    ///to contain all ReaderTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ReaderTest
  {


    private TestContext testContextInstance;

    private static Reader R;

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
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
      R = new Reader(@"..\..\..\TestData\AlbertslundPcJupiter.mdb");

    }
    //
    //Use ClassCleanup to run code after all tests in a class have run
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
      R.Dispose();
    }
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


    [TestMethod]
    public void SpeedTest()
    {
      Stopwatch sw = new Stopwatch();
      //sw.Start();
      //var well1 = R.WellsForNovana(true, true, false, false);
      //sw.Stop();
      //var t1 = sw.Elapsed;

      sw.Reset();
      sw.Start();
      var well2 = R.ReadWellsInSteps();
      R.ReadLithology(well2);


      //R.ReadWaterLevels(well2);
      
      JupiterXLFastReader rw = new JupiterXLFastReader(@"..\..\..\TestData\AlbertslundPcJupiter.mdb");

      rw.ReadWaterLevels(well2);
      
      var plants = R.ReadPlants(well2);
      R.FillInExtractionWithCount(plants);
      sw.Stop();

      var t2 = sw.Elapsed;
      var noobs = well2.Where(var=>var.Intakes.Sum(var2=>var2.HeadObservations.Items.Count)>0);
      Assert.AreEqual(629, noobs.Count());

      int i=0;

      //foreach(var well in noobs)
      //{
      //  Assert.AreEqual(well1[i].ID,well.ID);
      //  i++;
      //}

      int k = 1;
    }

    [TestMethod]
    public void GetPrimaryIDTest()
    {

      var wells = R.ReadWellsInSteps();
      var plants = R.ReadPlants(wells);

      JupiterXLFastReader rw = new JupiterXLFastReader(@"..\..\..\TestData\AlbertslundPcJupiter.mdb");

      Plant p = plants.First(var => var.PumpingIntakes.Count != 0);

      int id;
      Assert.IsTrue(rw.TryGetPrimaryID(p.PumpingIntakes.First(), p, out id));
      Assert.AreEqual(707, id);
    }

    [TestMethod]
    public void ReadTest()
    {
      var wells = R.ReadWellsInSteps();

      Assert.AreEqual(57, wells["193.   72"].Depth);
      Assert.AreEqual(28.3, wells["193.  125A"].Depth);

      Assert.AreEqual(17.9, wells["217.  260"].Intakes.First().Depth);

    }
    /// <summary>
    ///A test for AddDataForNovanaExtraction
    ///</summary>
    [TestMethod()]
    public void AddDataForNovanaExtractionTest()
    {
      var Wells = R.ReadWellsInSteps();

      var Anlaeg = R.ReadPlants(Wells);
      R.FillInExtractionWithCount(Anlaeg);

      Assert.AreEqual(4, Anlaeg.Count(x => x.PumpingIntakes.Count == 0));
    }



    /// <summary>
    ///A test for WellsForNovana
    ///</summary>
    [TestMethod()]
    public void WellsForNovanaTest()
    {
      var Wells = R.ReadWellsInSteps();
      List<JupiterIntake> Intakes = new List<JupiterIntake>();

      var well = Wells.First(var => var.Intakes.FirstOrDefault() != null & var.Intakes.FirstOrDefault().HeadObservations.Items.Where(var1 => var1.Description == "Ro").Count() > 100);

      Assert.AreEqual(214, well.Intakes.FirstOrDefault().HeadObservations.Items.Where(var=>var.Description=="Ro").Count());


      foreach (IWell w in Wells)
      {
        foreach (JupiterIntake JI in w.Intakes)
          Intakes.Add(JI);
      }
    }




  }
}
