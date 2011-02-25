using HydroNumerics.JupiterTools.JupiterPlus;
using HydroNumerics.JupiterTools;
using HydroNumerics.Wells;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.JupiterTools.JupiterPlus.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ChangeReaderTest and is intended
    ///to contain all ChangeReaderTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ChangeReaderTest
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
    public void ApplyChangesToPlants()
    {
      ChangeReader target = new ChangeReader();
      target.ReadFile(@"..\..\..\TestData\SønderSøChanges.xml");
      Reader R = new Reader(@"..\..\..\TestData\AlbertslundPcJupiter.mdb");
      var wells = R.WellsForNovana(false,true,false,false);
      var plants = R.ReadPlants(wells);

      target.ApplyChangesToPlant(plants);

    }


    /// <summary>
    ///A test for ApplyChangeToWells
    ///</summary>
    [TestMethod()]
    public void ApplyChangeToWellsTest()
    {
      ChangeReader target = new ChangeReader();
      target.ReadFile(@"..\..\..\TestData\Xchanges.xml");
      Reader R = new Reader(@"..\..\..\TestData\AlbertslundPcJupiter.mdb");
      var Wells = R.Wells();

      var e =  Wells.GetEnumerator();
      e.MoveNext();
      double d = e.Current.X;
      string id = e.Current.ID;

      target.ApplyChangeToWells(Wells);
      Assert.AreEqual(2 * d, Wells[id].X);

    }
  }
}
