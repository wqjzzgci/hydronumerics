using HydroNumerics.JupiterTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;


namespace HydroNumerics.JupiterTools.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for SqlTest and is intended
    ///to contain all SqlTest Unit Tests
    ///</summary>
  [TestClass()]
  public class SqlTest
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
    ///A test for GetPlantChemistry
    ///</summary>
    [TestMethod()]
    public void GetPlantChemistryTest()
    {
      Sql target = new Sql(); // TODO: Initialize to an appropriate value
      Plant P = new Plant(83339);
      
      var actual = target.GetPlantChemistry(1591, P);

      Assert.AreEqual(22, actual.Count());
    }

    /// <summary>
    ///A test for GetWellChemistry
    ///</summary>
    [TestMethod()]
    public void GetWellChemistryTest()
    {
      Sql target = new Sql(); // TODO: Initialize to an appropriate value
      HydroNumerics.Wells.Well P = new Wells.Well("182.   11");

      var actual = target.GetWellChemistry(1591, P);

      Assert.AreEqual(3, actual.Count());
    }

  
  
  }
}
