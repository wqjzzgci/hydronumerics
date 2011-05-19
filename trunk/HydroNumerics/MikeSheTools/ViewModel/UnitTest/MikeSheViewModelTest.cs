using HydroNumerics.MikeSheTools.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using HydroNumerics.MikeSheTools.Core;
using System.Collections.Generic;

namespace HydroNumerics.MikeSheTools.ViewModel.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MikeSheViewModelTest and is intended
    ///to contain all MikeSheViewModelTest Unit Tests
    ///</summary>
  [TestClass()]
  public class MikeSheViewModelTest
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
    ///A test for MoveIntakesToChalk
    ///</summary>
    [Ignore]
    [TestMethod()]
    public void MoveIntakesToChalkTest()
    {

      JupiterViewModel jvm = new JupiterViewModel();
      jvm.ReadJupiter(@"C:\Users\Jacob\Projekter\MikeSheWrapperForGEUS\MCNordjylland.mdb");
      jvm.LoadMikeSheMethod(@"C:\Users\Jacob\Projekter\MikeSheWrapperForGEUS\novomr6\result\Novomr6_inv10.she");

      MikeSheViewModel target = jvm.Mshe;

      target.Layers.Single(var => var.DfsLayerNumber == 0).IsChalkLayer = true;
      target.wells = jvm.SortedAndFilteredPlants.SelectMany(var => var.Wells);
      target.RefreshChalk();
    }
  }
}
