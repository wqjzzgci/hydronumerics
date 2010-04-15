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
    private Chemical Na;
    private Chemical Cl;
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
      ChemicalFactory cn = ChemicalFactory.Instance;
      Na = cn.GetChemical(ChemicalNames.Na);
      Cl = cn.GetChemical(ChemicalNames.Cl);

      WWC = new WaterWithChemicals(100);
      WWC.AddChemical(Na, 3);
      WWC.AddChemical(Cl, 2);

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
      Assert.IsTrue(WWC.Chemicals.ContainsKey(Cl));
      Assert.IsTrue(WWC.Chemicals.ContainsKey(Na));
      Assert.AreEqual(2, WWC.Chemicals[Cl]);
      Assert.AreEqual(3, WWC.Chemicals[Na]);

    }

    [TestMethod]
    public void SubstractTest()
    {
      WaterWithChemicals wwc2 = (WaterWithChemicals) WWC.Substract(70);

      Assert.AreEqual(70, wwc2.Volume);

      Assert.IsTrue(wwc2.Chemicals.ContainsKey(Cl));
      Assert.IsTrue(wwc2.Chemicals.ContainsKey(Na));
      Assert.AreEqual(0.7 * 3.0, wwc2.Chemicals[Na],0.0000001);
      Assert.AreEqual(0.3 * 3.0, WWC.Chemicals[Na], 0.0000001);

      Assert.AreEqual(WWC.GetConcentration(Cl), wwc2.GetConcentration(Cl));
      
    }

    /// <summary>
    ///A test for Add
    ///</summary>
    [TestMethod()]
    public void AddTest()
    {
      WaterWithChemicals WWC2 = new WaterWithChemicals(50);
      WWC2.Add(WWC);

      Assert.IsTrue(WWC2.Chemicals.ContainsKey(Cl));
      Assert.IsTrue(WWC2.Chemicals.ContainsKey(Na));
      Assert.AreEqual(2, WWC2.Chemicals[Cl]);
      Assert.AreEqual(150, WWC2.Volume, 0.0001);

    }

    [TestMethod]
    public void DeepCloneTest()
    {

      WaterWithChemicals actual = (WaterWithChemicals)WWC.DeepClone();
      Assert.AreEqual(WWC.GetConcentration(Na), actual.GetConcentration(Na));
      Assert.AreEqual(WWC.GetConcentration(Cl), actual.GetConcentration(Cl));

      actual = (WaterWithChemicals)WWC.DeepClone(250);
      Assert.AreEqual(WWC.GetConcentration(Na), actual.GetConcentration(Na),0.00001);
      Assert.AreEqual(2.5*3, actual.Chemicals[Na], 0.00001);


    }
  }
}
