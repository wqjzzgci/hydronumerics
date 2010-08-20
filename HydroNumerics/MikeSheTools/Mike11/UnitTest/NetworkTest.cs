using HydroNumerics.MikeSheTools.Mike11;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.MikeSheTools.Mike11.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for NetworkTest and is intended
    ///to contain all NetworkTest Unit Tests
    ///</summary>
  [TestClass()]
  public class NetworkTest
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
    ///A test for WriteToShape
    ///</summary>
    [TestMethod()]
    public void WriteToShapeTest()
    {
      string NWK11FileName = @"C:\Users\Jacob\Work\HydroNumerics\MikeSheTools\PFS\UnitTest\TestData\novomr6.nwk11"; // TODO: Initialize to an appropriate value
      HydroNumerics.MikeSheTools.Mike11.Network target = new HydroNumerics.MikeSheTools.Mike11.Network(NWK11FileName); // TODO: Initialize to an appropriate value
      string shapefilename = @"C:\Users\Jacob\Work\HydroNumerics\MikeSheTools\PFS\UnitTest\TestData\novomr6.shp"; // TODO: Initialize to an appropriate value
      target.WriteToShape(shapefilename);
    }
  }
}
