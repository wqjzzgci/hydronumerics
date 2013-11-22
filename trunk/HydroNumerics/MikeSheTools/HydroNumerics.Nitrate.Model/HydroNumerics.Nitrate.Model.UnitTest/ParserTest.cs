using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ParserTest and is intended
    ///to contain all ParserTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ParserTest
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
    ///A test for ParseThis
    ///</summary>
    [TestMethod()]
    public void ParseThisTest()
    {
      string FileName = @"..\..\..\HydroNumerics.Nitrate.Model\HydroNumerics.Nitrate.Model.UnitTest\TestData\Eksempel_på_format_udvask_25102013.txt";

      DistributedLeaching dl = new DistributedLeaching();
      dl.LoadFile(FileName);

      var gridddata = dl.Grids;

      Assert.AreEqual(20344, gridddata.Values.First().GridID);

      var dat = gridddata[20344].GetValue(new DateTime(1991, 1, 1));
      Assert.AreEqual(0.62F, gridddata[20344].GetValue(new DateTime(1991,1,1)));
      Assert.AreEqual(0.2164F, gridddata[20344].GetValue(new DateTime(1991, 12, 31)));
      Assert.AreEqual(0.1855F, gridddata[20344].GetValue(new DateTime(1992, 1, 1)));

      Assert.AreEqual(0.4252F, gridddata[20377].GetValue(new DateTime(1998, 1, 1)));
      gridddata[20377].ReduceToMonhlyTimeSteps();

      Assert.AreNotEqual(0.4252F, gridddata[20377].GetValue(new DateTime(1998, 1, 1)));


    }
  }
}
