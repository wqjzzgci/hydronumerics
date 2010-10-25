using System;

using HydroNumerics.JupiterTools.JupiterPlus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;

namespace HydroNumerics.JupiterTools.JupiterPlus.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ChangeWriterTest and is intended
    ///to contain all ChangeWriterTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ChangeWriterTest
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


    [TestMethod]
    public void CreateTestDoc()
    {
      Reader R = new Reader(@"..\..\..\TestData\AlbertslundPcJupiter.mdb");
      var wells = R.Wells();

      ChangeWriter cw = new ChangeWriter();

      int i=0;
      foreach(var W in wells.Values)
      {
        Change<double> c = new Change<double>();
        c.Date=DateTime.Now;
        c.User="Jag";
        c.Project = "GEUSProj";
        c.OldValue = W.X;
        c.NewValue = W.X*2;

        cw.AddWellX(W.ID, c); 

        i++;
        if (i > 50)
          break;
      }

      cw.Save("Xchanges.xml");
      


    }

    /// <summary>
    ///A test for WellX
    ///</summary>
    [TestMethod()]
    public void WellXTest()
    {
      ChangeWriter target = new ChangeWriter();

      Change<double> change = new Change<double>();
      change.Date = DateTime.Now;
      change.NewValue = 10;
      change.OldValue = -99;
      change.User = "JAG";
      change.Project = "Sømod";

      string txt = target.WellX("192.098", change).ToString();
    }
  }
}
