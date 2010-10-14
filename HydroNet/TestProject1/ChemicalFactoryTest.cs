using HydroNumerics.HydroNet.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for ChemicalFactoryTest and is intended
    ///to contain all ChemicalFactoryTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ChemicalFactoryTest
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
    ///A test for GetChemical
    ///</summary>
    [TestMethod()]
    public void GetChemicalTest()
    {
      HydroNumerics.HydroNet.Core.Chemicals ChemicalName = new HydroNumerics.HydroNet.Core.Chemicals(); // TODO: Initialize to an appropriate value
      HydroNumerics.HydroNet.Core.Chemical expected = null; // TODO: Initialize to an appropriate value
      HydroNumerics.HydroNet.Core.Chemical actual;
      actual = HydroNumerics.HydroNet.Core.ChemicalFactory.GetChemical(ChemicalName);
      Assert.AreEqual(expected, actual);
      Assert.Inconclusive("Verify the correctness of this test method.");
    }
  }
}
