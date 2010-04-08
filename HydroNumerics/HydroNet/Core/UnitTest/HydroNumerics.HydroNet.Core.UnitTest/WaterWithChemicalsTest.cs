using HydroNumerics.HydroNet.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.HydroNet.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for WaterWithChemicalsTest and is intended
    ///to contain all WaterWithChemicalsTest Unit Tests
    ///</summary>
  [TestClass()]
  public class WaterWithChemicalsTest
  {

    private WaterWithChemicals WWC;
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
    [TestInitialize()]
    public void MyTestInitialize()
    {
      WWC = new WaterWithChemicals(100);

      WWC.AddChemical(new Chemical(new ChemicalType("Cl", 32), 2));
      WWC.AddChemical(new Chemical(new ChemicalType("Na", 12), 2));

    }
    //
    //Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{
    //}
    //
    #endregion


    /// <summary>
    ///A test for AddChemical
    ///</summary>
    [TestMethod()]
    public void AddChemicalTest()
    {
      Assert.IsTrue(WWC.Chemicals.ContainsKey("Cl"));
      Assert.IsTrue(WWC.Chemicals.ContainsKey("Na"));
      Assert.AreEqual(2, WWC.Chemicals["Cl"].Moles);
      Assert.AreEqual(64, WWC.Chemicals["Cl"].Mass);
      Assert.AreEqual(24, WWC.Chemicals["Na"].Mass);

    }

    [TestMethod]
    public void SubstractTest()
    {
      IWaterPacket wwc2 = WWC.Substract(70);
    }

    /// <summary>
    ///A test for Add
    ///</summary>
    [TestMethod()]
    public void AddTest()
    {
      WaterWithChemicals WWC2 = new WaterWithChemicals(50);
      WWC2.Add(WWC);

      Assert.IsTrue(WWC2.Chemicals.ContainsKey("Cl"));
      Assert.IsTrue(WWC2.Chemicals.ContainsKey("Na"));
      Assert.AreEqual(2, WWC2.Chemicals["Cl"].Moles);
      Assert.AreEqual(64, WWC2.Chemicals["Cl"].Mass);
      Assert.AreEqual(24, WWC2.Chemicals["Na"].Mass);
      Assert.AreEqual(150, WWC2.Volume, 0.0001);

    }

    [TestMethod]
    public void DeepCloneTest()
    {
      WaterWithChemicals WCC = new WaterWithChemicals(100);
      WCC.AddChemical(new Chemical(new ChemicalType("NA", 11), 3));

      WaterWithChemicals actual = (WaterWithChemicals)WCC.DeepClone(2000);
      Assert.AreEqual(WCC.GetConcentration("NA"), actual.GetConcentration("NA"));

    }
  }
}
