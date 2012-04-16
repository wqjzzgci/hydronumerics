using HydroNumerics.JupiterTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

using HydroNumerics.Wells;

namespace HydroNumerics.JupiterTools.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for SqlTest and is intended
    ///to contain all SqlTest Unit Tests
    ///</summary>
  [TestClass()]
  public class SqlTest
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
    ///A test for GetPlantChemistry
    ///</summary>
    [TestMethod()]
    public void GetPlantChemistryTest()
    {
      Sql target = new Sql(); // TODO: Initialize to an appropriate value
      Plant P = new Plant(83339);
      
      target.GetPlantChemistry(new int[]{1591}, new Plant[]{P});

      Assert.AreEqual(22, P.Chemistry.Count());
    }

    /// <summary>
    ///A test for GetWellChemistry
    ///</summary>
    [TestMethod()]
    public void GetWellChemistryTest()
    {
      Sql target = new Sql(); // TODO: Initialize to an appropriate value
      JupiterWell P = new JupiterWell("182.   11");

      target.GetWellChemistry(new int[] { 1591 }, new JupiterWell[] { P });

      Assert.AreEqual(3, P.ChemSamples.Count());
    }


    [TestMethod]
    public void IWellCollectionTest()
    {
      IWellCollection iw = new IWellCollection();

      iw.Add(new Well("182. 335"));
      iw.Add(new Well("182. 27"));
      iw.Add(new Well("182. 180"));
      iw.Add(new Well("182. 181"));
      iw.Add(new Well("182. 398"));
      iw.Add(new Well("182. 403"));
      iw.Add(new Well("182. 410"));
      iw.Add(new Well("182. 12B"));

      Assert.IsFalse(iw.Contains(new Well("182. 157")));
      
    }




    /// <summary>
    ///A test for GetWellsWithChemical
    ///</summary>
    [TestMethod()]
    public void GetWellsWithChemicalTest()
    {
      Sql target = new Sql("Server=tcp:te5cczmjwk.database.windows.net;Database=Sjaelland;User ID=JacobGudbjerg;Password=Stinj99!;Trusted_Connection=False;Encrypt=True"); // TODO: Initialize to an appropriate value
      int CompoundNo = 1591; // TODO: Initialize to an appropriate value
      JupiterWell[] expected = null; // TODO: Initialize to an appropriate value
      JupiterWell[] actual;

      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

      sw.Start();
      actual = target.GetWellsWithChemical(CompoundNo, 0, 60,50,0,12);
      sw.Stop();

      sw.Reset();

      sw.Start();
      target.GetWellChemistry(new int[] { CompoundNo }, new JupiterWell[] { actual[120] });
      sw.Stop();


      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }
  }
}
