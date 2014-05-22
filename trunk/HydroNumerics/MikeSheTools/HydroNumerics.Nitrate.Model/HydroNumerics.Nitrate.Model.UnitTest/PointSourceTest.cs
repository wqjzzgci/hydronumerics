using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for PointSourceTest and is intended
    ///to contain all PointSourceTest Unit Tests
    ///</summary>
  [TestClass()]
  public class PointSourceTest
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
    [Ignore]
    public void InitializeTest()
    {

      MainViewModel target2 = new MainViewModel(); 
      target2.LoadCatchments(@"D:\DK_information\TestData\FileStructure\id15_NSTmodel.shp");

      PointSource target = new PointSource(); 
      DateTime Start = new DateTime(); 
      DateTime End = new DateTime();
      target.ShapeFile = new SafeFile() { FileName = @"D:\DK_information\Overfladevand\Punktkilder\spredt_pkt.shp" };
      target.DBFFile = new SafeFile() { FileName = @"D:\DK_information\Overfladevand\Punktkilder\spredt_data_final.dbf" };

      target.Initialize(Start, End, target2.AllCatchments.Values);
    }
  }
}
