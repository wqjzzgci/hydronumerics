using System;
using System.IO;

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
      cw.AddChangeItem("JAG", "GEUSProj", DateTime.Now);
      int i=0;
      foreach(var W in wells.Values)
      {

        cw.AddWellX(W.ID, W.X*2);
        cw.AddWellY(W.ID, W.Y / 2); 

        i++;
        if (i > 50)
          break;
      }

      cw.Save(@"..\..\..\TestData\Xchanges.xml");
    }

    [TestMethod]
    public void DeletePlantIntakeTest()
    {
      ChangeWriter cw = new ChangeWriter();
      cw.AddChangeItem("JAG", "NOVANA", new DateTime(2010,2,15));

      using (StreamReader sr = new StreamReader(@"..\..\..\TestData\SønderSø.txt"))
      {
        while (!sr.EndOfStream)
        {
          var arr = sr.ReadLine().Split('\t');
          cw.AddDeleteIntakeFromPlant(2065,arr[0],int.Parse(arr[1]));
        }
      }
      cw.Save(@"..\..\..\TestData\SønderSøChanges.xml");

    }

        /// <summary>
    ///A test for WellX
    ///</summary>
    [TestMethod()]
    public void WellXTest2()
    {
      ChangeWriter cw = new ChangeWriter();
      cw.AddChangeItem("JAG", "GEUS", DateTime.Now);

      cw.AddWellX("129. 98", 25);
      cw.AddWellY("129. 98", 22);
      cw.AddWellTerrain("129. 98", 224);

      cw.AddWellX("129. 99", 10000);
      cw.AddWellY("129. 99", 10);

    
      string txt = cw.ToString();

      cw.Save(@"..\..\..\TestData\changes.xml");
    }
      

  }
}
