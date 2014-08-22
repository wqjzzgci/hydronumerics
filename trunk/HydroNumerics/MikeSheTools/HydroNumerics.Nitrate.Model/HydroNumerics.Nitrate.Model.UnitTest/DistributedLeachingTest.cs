using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for DistributedLeachingTest and is intended
    ///to contain all DistributedLeachingTest Unit Tests
    ///</summary>
  [TestClass()]
  public class DistributedLeachingTest
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
    ///A test for LoadAndSum
    ///</summary>
    [TestMethod()]
    public void LoadAndSumTest()
    {
      DistributedLeaching target = new DistributedLeaching(); // TODO: Initialize to an appropriate value
      double actual;
      actual = target.LoadAndSum(@"D:\NitrateModel\NLES\SoilFarms_dmi10kmgrid_daily1990.txt");
      Assert.AreEqual(287988804, actual, 1);
    }


    /// <summary>
    ///A test for GetSum
    ///</summary>
    [TestMethod()]
    public void GetSumTest()
    {
      DistributedLeaching target = new DistributedLeaching(); // TODO: Initialize to an appropriate value
      DateTime Start = new DateTime(1990, 4, 1);
      DateTime End = new DateTime(1991, 3, 31);
      target.LoadSoilCodesGrid(@"D:\NitrateModel\model\DKDomainNodes_LU_Soil_codes.shp");
      target.LoadFileParallel(@"D:\NitrateModel\SoilFarms_dmi10kmgrid_daily1990.txt");
//      target.LoadFileParallel(@"D:\NitrateModel\NLES\SoilFarms_dmi10kmgrid_daily2001.txt");
      double actual;
      actual = target.GetSum(Start, End);
      Assert.AreEqual(239770897, actual,1);
    }

    /// <summary>
    ///A test for DebugPrint
    ///</summary>
    [TestMethod()]
    public void DebugPrintTest()
    {
      DistributedLeaching target = new DistributedLeaching();
      string outputpath = @"D:\NitrateModel\model\output";
      string FileName = @"D:\NitrateModel\SoilFarms_dmi10kmgrid_daily1990.txt";
      string SoilGridCodesFileName = @"D:\NitrateModel\model\DKDomainNodes_LU_Soil_codes.shp";
      target.DebugPrint(outputpath, FileName, SoilGridCodesFileName);
    }
  }
}
