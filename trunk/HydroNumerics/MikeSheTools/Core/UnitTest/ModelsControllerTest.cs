using HydroNumerics.MikeSheTools.Core;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ModelsControllerTest and is intended
    ///to contain all ModelsControllerTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ModelsControllerTest
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
    ///A test for RunParameterSets
    ///</summary>
    [TestMethod()]
    public void RunParameterSetsTest()
    {
      ModelsController target = new ModelsController(); // TODO: Initialize to an appropriate value
      List<SortedList<int, double>> ParameterValues = null; // TODO: Initialize to an appropriate value

//      target.AddModel(@"..\..\..\TestData\Karup_Example_DemoMode.SHE");
      target.AddModel(@"..\..\..\TestData\Model2\Karup_Example_DemoMode.SHE");
      
      target.RunParameterSets(ParameterValues);
      int k = 0;
    }
  }
}
