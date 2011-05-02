using HydroNumerics.MikeSheTools.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

using System.Diagnostics;

namespace HydroNumerics.MikeSheTools.ViewModel.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for JupiterViewModelTest and is intended
    ///to contain all JupiterViewModelTest Unit Tests
    ///</summary>
  [TestClass()]
  public class JupiterViewModelTest
  {
    private static JupiterViewModel target;

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
      target = new JupiterViewModel();
      target.ReadJupiter(@"..\..\..\..\JupiterTools\TestData\AlbertslundPcJupiter.mdb");

    }
    
    //Use ClassCleanup to run code after all tests in a class have run
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
      
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
    public void Dfs0WriteSpeedTest()
    {
      target.SelectionStartTime = new DateTime(2000, 1, 1);
      target.SelectionEndTime = new DateTime(2005, 1, 1);
      target.NumberOfObs = 10;

      var intakes = target.SortedAndFilteredWells.SelectMany(var => var.Intakes);

      Stopwatch sw = new Stopwatch();

      sw.Start();
      FileWriters.WriteToDfs0(@"c:\temp", intakes, target.SelectionStartTime, target.SelectionEndTime);
      sw.Stop();
      TimeSpan ts = sw.Elapsed;
      sw.Reset();
      sw.Start();
      FileWriters.WriteDetailedTimeSeriesDfs0(@"c:\temp", intakes, target.SelectionStartTime, target.SelectionEndTime);
      sw.Stop();
      TimeSpan ts2 = sw.Elapsed;


    }
    /// <summary>
    ///A test for ReadJupiter
    ///</summary>
    [TestMethod()]
    public void ReadJupiterTest()
    {
      target.OnlyRo = false;
      
      Assert.AreEqual(56, target.Plants.Count);
      Assert.AreEqual(1081, target.AllWells.Count());
    }
  }
}
