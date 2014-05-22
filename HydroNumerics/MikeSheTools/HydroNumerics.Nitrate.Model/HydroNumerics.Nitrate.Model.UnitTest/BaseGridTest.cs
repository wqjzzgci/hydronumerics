using HydroNumerics.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for BaseGridTest and is intended
    ///to contain all BaseGridTest Unit Tests
    ///</summary>
  [TestClass()]
  public class BaseGridTest
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
    ///A test for GetRowIndex
    ///</summary>
    [TestMethod()]
    public void GetRowIndexTest()
    {
      BaseGrid target = new BaseGrid();
      target.XOrigin = 0;
      target.YOrigin = 0;
      target.NumberOfColumns = 10;
      target.NumberOfRows = 10;
      target.GridSize = 10;

      Assert.AreEqual(0, target.GetColumnIndex(1));
      Assert.AreEqual(0, target.GetColumnIndex(10));
      Assert.AreEqual(1, target.GetColumnIndex(11));

      Assert.AreEqual(0, target.GetRowIndex(4));
      Assert.AreEqual(0, target.GetRowIndex(10));
      Assert.AreEqual(5, target.GetRowIndex(55));
      Assert.AreEqual(9, target.GetRowIndex(100));
      
    }
  }
}
