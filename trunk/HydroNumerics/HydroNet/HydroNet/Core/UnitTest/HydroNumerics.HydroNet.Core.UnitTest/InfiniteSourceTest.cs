using HydroNumerics.HydroNet.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.HydroNet.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for InfiniteSourceTest and is intended
    ///to contain all InfiniteSourceTest Unit Tests
    ///</summary>
  [TestClass()]
  public class InfiniteSourceTest
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
    ///A test for GetWater
    ///</summary>
    [TestMethod()]
    public void GetWaterTest()
    {
      WaterWithChemicals WCC = new WaterWithChemicals(100);
      WCC.AddChemical(new Chemical(new ChemicalType("NA", 11), 3));

      InfiniteSource target = new InfiniteSource(WCC);
      WaterWithChemicals actual = (WaterWithChemicals)target.GetWater(2000);
      Assert.AreEqual(WCC.GetConcentration("NA"), actual.GetConcentration("NA"));
    }
  }
}
