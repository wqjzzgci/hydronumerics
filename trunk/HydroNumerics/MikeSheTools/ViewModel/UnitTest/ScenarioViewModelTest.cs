using HydroNumerics.MikeSheTools.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace HydroNumerics.MikeSheTools.ViewModel.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ScenarioViewModelTest and is intended
    ///to contain all ScenarioViewModelTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ScenarioViewModelTest
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
    ///A test for ToXml
    ///</summary>
    [TestMethod()]
    public void ToXmlTest()
    {
      ScenarioViewModel target = new ScenarioViewModel(); // TODO: Initialize to an appropriate value

     
      XElement expected = null; // TODO: Initialize to an appropriate value
      XElement actual;
    
    
    }
  }
}
