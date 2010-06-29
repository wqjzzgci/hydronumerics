using HydroNumerics.Geometry.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.Geometry.Shapes.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for PointShapeReaderTest and is intended
    ///to contain all PointShapeReaderTest Unit Tests
    ///</summary>
  [TestClass()]
  public class PointShapeReaderTest
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
    ///A test for ReadNext
    ///</summary>
    [TestMethod()]
    public void ReadNextTest()
    {
      string File = @"..\..\TestData\WriteTest.Shp";
      PointShapeReader target = new PointShapeReader(File); 
      var geo = target.ReadNext();
    }

    /// <summary>
    ///A test for ReadNext
    ///</summary>
    [TestMethod()]
    public void ReadNextTest2()
    {
      string File = @"..\..\TestData\CommandAreas.Shp";
      PointShapeReader target = new PointShapeReader(File);
      var geo = target.ReadNext();
      double d =((XYPolygon)geo).GetArea();
      Assert.IsTrue(0 < d);
    }

    [TestMethod]
    public void GeoData()
    {
      string File = @"..\..\TestData\CommandAreas.Shp";
     PointShapeReader target = new PointShapeReader(File);

     foreach (GeoRefData grd in target.GeoData)
     {
       var k = 2;
     }

    }

  }
}
