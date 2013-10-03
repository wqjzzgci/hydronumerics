using HydroNumerics.MikeSheTools.Mike11;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.MikeSheTools.Mike11.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for BranchIDTest and is intended
    ///to contain all BranchIDTest Unit Tests
    ///</summary>
  [TestClass()]
  public class BranchIDTest
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
    ///A test for Equals
    ///</summary>
    [TestMethod()]
    public void EqualsTest()
    {

      int k1 = "Uhre-Krarup-kybæk".CompareTo("Uhre-Krarup-kybæk");
      int k2 = "Uhre-Krarup-kybæk".CompareTo("Uhre-Krarup-kybaek");
      int k3 = "hre-Krarup-kybæk".CompareTo("Uhre-Krarup-kybaek");

      int k4 = "ae".CompareTo("æ");


      BranchID target = new BranchID() { Branchname = "Uhre-Krarup-kybæk", StartChainage = 0 };
      BranchID other = new BranchID() { Branchname = "Uhre-Krarup-kybaek", StartChainage = 0 };
      Assert.IsFalse(target.Equals(other));
      Assert.AreNotEqual(target.GetHashCode(), other.GetHashCode());

    }
  }
}
