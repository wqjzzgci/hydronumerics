using HydroNumerics.Geometry.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using HydroNumerics.Geometry;
using System.Windows.Media.Imaging;
using System.IO;

namespace HydroNumerics.Geometry.Net.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MapTest and is intended
    ///to contain all MapTest Unit Tests
    ///</summary>
  [TestClass()]
  public class MapTest
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
    ///A test for GetImagery
    ///</summary>
    [TestMethod()]
    public void GetImageryTest()
    {
      IXYPoint point = new XYPoint(715281.56, 6189341.78);
      double dx = 5000;
      double dy = 5000; 
      int utmzone = 32; 
      BitmapImage actual;
      actual = Map.GetImagery(point, dx, dy, utmzone);
      System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));

      FileStream filestream = new FileStream(@"c:\temp\pict.jpg", FileMode.Create);
      JpegBitmapEncoder encoder = new JpegBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(actual));
      encoder.Save(filestream);
      filestream.Dispose();
      
    }
  }
}
