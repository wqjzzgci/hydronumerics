using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for AtmosphericDepositionTest and is intended
    ///to contain all AtmosphericDepositionTest Unit Tests
    ///</summary>
  [TestClass()]
  public class AtmosphericDepositionTest
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
    ///A test for Initialize
    ///</summary>
    [TestMethod()]
    public void InitializeTest()
    {
      MainViewModel catchme = new MainViewModel(); 
      catchme.LoadCatchments(@"D:\DK_information\TestData\FileStructure\id15_NSTmodel.shp");


      AtmosphericDeposition target = new AtmosphericDeposition(); 
      DateTime Start = new DateTime(); 
      DateTime End = new DateTime(); 
      target.ShapeFileName = @"D:\DK_information\TestData\FileStructure\Ndeposition\EMEP_centroid_DK.shp";
      target.ExcelFileName = @"D:\DK_information\TestData\FileStructure\Ndeposition\EMEP_Ndep_1990_2013.xlsx";
      target.Initialize(Start, End, catchme.AllCatchments.Values);
      Assert.AreEqual(0.0144132844, target.GetValue(catchme.AllCatchments.Values.First(), new DateTime(1990, 5, 1)),1e-6);

    
    
    }
  }
}
