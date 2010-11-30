using System;
using System.Linq;
using System.Collections.Generic;

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


    /// <summary>
    ///A test for AddDataForNovanaExtraction
    ///</summary>
    [TestMethod()]
    public void AddDataForNovanaExtractionTest()
    {
      var Wells = R.Wells();

      var Anlaeg = R.ReadPlants(Wells);
      R.FillInExtraction(Anlaeg);

      Assert.AreEqual(4, Anlaeg.Values.Count(x => x.PumpingIntakes.Count == 0));
      R.AddDataForNovanaExtraction(Anlaeg.Values, DateTime.MinValue, DateTime.MaxValue);
    }



    /// <summary>
    ///A test for WellsForNovana
    ///</summary>
    [TestMethod()]
    public void WellsForNovanaTest()
    {
      var Wells = R.WellsForNovana(true, true, true, false);
      List<JupiterIntake> Intakes = new List<JupiterIntake>();

      foreach (IWell w in Wells)
      {
        foreach (JupiterIntake JI in w.Intakes)
          Intakes.Add(JI);
      }
      R.AddDataForNovanaPejl(Intakes);
    }



    [TestMethod]
    public void TableMergeTest()
    {
      NovanaTables.IntakeCommonDataTable DT = new NovanaTables.IntakeCommonDataTable();
      NovanaTables.PejlingerDataTable DT2 = new NovanaTables.PejlingerDataTable();


      NovanaTables.IntakeCommonRow dr = DT.NewIntakeCommonRow();
      dr.NOVANAID = "boring 1";
      dr.JUPKOTE = 10;

      DT.Rows.Add(dr);

      NovanaTables.PejlingerRow dr1 = DT2.NewPejlingerRow();
      dr1.NOVANAID = "boring2";
      DT2.Rows.Add(dr1);
      NovanaTables.PejlingerRow dr2 = DT2.NewPejlingerRow();
      dr2.NOVANAID = "boring 1";
      DT2.Rows.Add(dr2);

      int n = dr.Table.Columns.Count;
      DT.Merge(DT2);
      n = dr.Table.Columns.Count;

      dr["AKTDAGE"] = 2;



    }

  }
}
