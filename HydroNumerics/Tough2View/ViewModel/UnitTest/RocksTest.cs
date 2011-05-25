using HydroNumerics.Tough2.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace HydroNumerics.Tough2.ViewModel.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for RocksTest and is intended
    ///to contain all RocksTest Unit Tests
    ///</summary>
  [TestClass()]
  public class RocksTest
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
    ///A test for WriteToStream
    ///</summary>
    [TestMethod()]
    public void WriteToStreamTest()
    {
      Rocks target = new Rocks();
      using (StreamReader sr = new StreamReader(@"C:\Jacob\Udvikling\NewT2voc\Models\Centrifuge\co2buble.txt"))
      {
        sr.ReadLine();
        sr.ReadLine();
        target.ReadFromStream(sr);
      }

      using (StreamWriter sr = new StreamWriter(@"C:\Jacob\Udvikling\NewT2voc\Models\Centrifuge\temp.txt"))
      {
        sr.Write(target.ToString());
        
      }


    }
  }
}
