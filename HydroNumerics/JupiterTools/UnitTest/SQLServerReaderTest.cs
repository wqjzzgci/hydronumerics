using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.JupiterTools.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for SQLServerReaderTest and is intended
    ///to contain all SQLServerReaderTest Unit Tests
    ///</summary>
  [TestClass()]
  public class SQLServerReaderTest
  {
    static SQLServerReader ssr;
    static Dictionary<string, IWell> wells;
    static JupiterXLTablesDataContext JXL;

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
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
      ssr = new SQLServerReader();

      wells = ssr.Wells();
      JXL = new JupiterXLTablesDataContext();
    }
    //
    //Use ClassCleanup to run code after all tests in a class have run
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
      JXL.Dispose();
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
    public void FirstTest()
    {

      Assert.AreEqual(254742, wells.Count());

    }

    [TestMethod]
    public void PostnummerTest()
    {
      int?[] i = new int?[] { null };

      Assert.AreEqual(282, ssr.Wells(new int?[] { 2400 }).Count);

      Assert.IsTrue(i.Contains(null));
      Assert.AreEqual(41464, ssr.Wells(i).Count);

    }



    [TestMethod]
    public void ReadPlants()
    {
      Stopwatch sw = new Stopwatch();

      sw.Start();
      var p = ssr.ReadPlants();
      sw.Stop();
      TimeSpan ts = sw.Elapsed;

    }

    [TestMethod]
    public void TotalTest()
    {
      Stopwatch sw = new Stopwatch();

      sw.Start();
      var p = ssr.ReadPlants();
      ssr.AddChemistry(wells);
      ssr.AddLithology(wells);
      ssr.Waterlevels(wells);
      sw.Stop();
      TimeSpan ts = sw.Elapsed;

    }

    [TestMethod]
    public void ChemistryTest()
    {
      Stopwatch sw = new Stopwatch();

      sw.Start();
      ssr.AddChemistry(wells);
      sw.Stop();
      TimeSpan ts = sw.Elapsed;
    }

    [TestMethod]
    public void LithologyTest()
    {
      Stopwatch sw = new Stopwatch();

      sw.Start();
      ssr.AddLithology(wells);
      sw.Stop();
      TimeSpan ts = sw.Elapsed;

      Dictionary<string, IWell> singleWells = new Dictionary<string, IWell>();

      foreach (KeyValuePair<string, IWell> KVP in wells.Take(2000))
        singleWells.Add(KVP.Key, KVP.Value);

      sw.Reset();

      sw.Start();
      ssr.AddLithology(singleWells);
      sw.Stop();
      TimeSpan ts2 = sw.Elapsed;

    }

    [Ignore]
    [TestMethod]
    public void Read()
    {
      Stopwatch sw = new Stopwatch();

      //sw.Start();
      //ssr.Waterlevels(wells);
      //sw.Stop();
      TimeSpan ts = sw.Elapsed;

      Dictionary<string, IWell> singleWells = new Dictionary<string, IWell>();

      foreach (KeyValuePair<string, IWell> KVP in wells.Take(4000))
        singleWells.Add(KVP.Key, KVP.Value);

      sw.Reset();

      sw.Start();
      ssr.Waterlevels(singleWells);
      sw.Stop();
      TimeSpan ts2 = sw.Elapsed;
    }




    /// <summary>
    ///A test for ReadPlants
    ///</summary>
    [TestMethod()]
    public void ReadPlantsTest()
    {
      HydroNumerics.JupiterTools.SQLServerReader target = new HydroNumerics.JupiterTools.SQLServerReader(); // TODO: Initialize to an appropriate value
      System.Collections.Generic.Dictionary<int, HydroNumerics.JupiterTools.Plant> expected = null; // TODO: Initialize to an appropriate value
      System.Collections.Generic.Dictionary<int, HydroNumerics.JupiterTools.Plant> actual;
      actual = target.ReadPlants();
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }

    /// <summary>
    ///A test for Waterlevels
    ///</summary>
    [TestMethod()]
    public void WaterlevelsTest()
    {
      HydroNumerics.JupiterTools.SQLServerReader target = new HydroNumerics.JupiterTools.SQLServerReader(); // TODO: Initialize to an appropriate value
      HydroNumerics.Wells.IWell well = null; // TODO: Initialize to an appropriate value
      target.Waterlevels(well);
      Assert.Inconclusive("A method that does not return a value cannot be verified.");
    }

    /// <summary>
    ///A test for Wells
    ///</summary>
    [TestMethod()]
    public void WellsTest()
    {
      HydroNumerics.JupiterTools.SQLServerReader target = new HydroNumerics.JupiterTools.SQLServerReader(); // TODO: Initialize to an appropriate value
      System.Nullable<int>[] PostalNumbers = null; // TODO: Initialize to an appropriate value
      System.Collections.Generic.Dictionary<string, HydroNumerics.Wells.IWell> expected = null; // TODO: Initialize to an appropriate value
      System.Collections.Generic.Dictionary<string, HydroNumerics.Wells.IWell> actual;
      actual = target.Wells(PostalNumbers);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }
  }
}
