using HydroNumerics.MikeSheTools.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Collections.Generic;

using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for LayersCollectionTest and is intended
    ///to contain all LayersCollectionTest Unit Tests
    ///</summary>
  [TestClass()]
  public class LayersCollectionTest
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
    ///A test for MikeSheFileName
    ///</summary>
    [TestMethod()]
    public void MikeSheFileNameTest()
    {

      LayersCollection LC= new LayersCollection();

      Wells.Well W = new HydroNumerics.Wells.Well("w1", 50, 50);
      W.AddNewIntake(1);
      Screen sc = new Screen(W.Intakes.First());
      sc.BottomAsKote = 2;
      sc.TopAsKote = 5;

      List<IWell> wells = new List<IWell>();
      wells.Add(W);
      LC.Wells = wells;

      LC.MikeSheFileName = @"C:\Users\Jacob\Work\HydroNumerics\MikeSheTools\Core\UnitTest\TestData\testmodel.she";


    }
  }
}
