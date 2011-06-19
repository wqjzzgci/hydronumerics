using HydroNumerics.MikeSheTools.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

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

    private static Stopwatch sw = new Stopwatch();
    #region Additional test attributes
    // 
    //You can use the following additional attributes as you write your tests:
    //
    //Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
      target = new JupiterViewModel();
      target.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(target_PropertyChanged);
      sw.Start();
      target.ReadJupiter(@"..\..\..\..\JupiterTools\TestData\AlbertslundPcJupiter.mdb");

    }

    static void target_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "SortedAndFilteredWells")
      {
        sw.Stop();
        TimeSpan el = sw.Elapsed;
      }
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

   
    /// <summary>
    ///A test for ReadJupiter
    ///</summary>
    [TestMethod()]
    public void ReadJupiterTest()
    {
      System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
      target.OnlyRo = false;
      
      Assert.AreEqual(56, target.AllPlants.Count);
      Assert.AreEqual(1081, target.AllWells.Count());
    }


    [TestMethod]
    public void FixErrorsTest()
    {
      var wellswitherrors = target.AllWells.Values.Where(var => var.MissingData);

      var fixables = wellswitherrors.Where(var => var.HasFixableErrors);

      List<string> Messages = new List<string>();

      foreach (var v in fixables)
      {
        v.Fix();
        Messages.Add(v.StatusString);
      }
      wellswitherrors = target.AllWells.Values.Where(var => var.MissingData);



    }
   
  }
}
