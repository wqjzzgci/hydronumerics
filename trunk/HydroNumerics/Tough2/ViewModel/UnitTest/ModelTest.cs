using HydroNumerics.Tough2.ViewModel;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.Tough2.ViewModel.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ModelTest and is intended
    ///to contain all ModelTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ModelTest
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
    ///A test for ReadOutputFile
    ///</summary>
    [TestMethod()]
    public void ReadOutputFileTest()
    {

      Model target = new Model(@"C:\Jacob\Udvikling\NewT2voc\Models\Centrifuge\co2buble.txt"); // TODO: Initialize to an appropriate value
      target.Results.ReadOutputFile(@"C:\Jacob\Udvikling\NewT2voc\Models\Centrifuge\ud402.txt");
    }

    /// <summary>
    ///A test for GetINCON
    ///</summary>
    [TestMethod()]
    public void GetINCONTest()
    {
      Model target = new Model(@"C:\Jacob\Udvikling\NewT2voc\DotNetT2VOC\RelPermTemp\co2buble.txt");

      int k = 0;
      for (int i = 0; i < 50; i++)
      {
        double sa = i/50.0 + 0.01;

        for (int j = 0; j < 50; j++)
        {
          target.Elements[k].PrimaryVariablesIndex = 7;
          target.Elements[k].PrimaryVaribles[0] = 3.813E+06;
          target.Elements[k].PrimaryVaribles[2] = sa;
          target.Elements[k].PrimaryVaribles[3] = (1 - sa-0.005) * j / 50 + 0.0005;
          k++;
        }
      }
      for (int i = 0; i < 100; i++)
      {
        target.Elements[k].PrimaryVariablesIndex = 4;
        target.Elements[k].PrimaryVaribles[2] = i/100.0+0.0005;
        target.Elements[k].PrimaryVaribles[0] = 6e6;
        target.Elements[k].PrimaryVaribles[3] = 20;
        k++;
      }
      for (int i = 0; i < 100; i++)
      {
        target.Elements[k].PrimaryVariablesIndex = 5;
        target.Elements[k].PrimaryVaribles[2] = i / 100.0 + 0.0005;
        target.Elements[k].PrimaryVaribles[0] = 101300;
        target.Elements[k].PrimaryVaribles[3] = 20;
        k++;
      }


      using (StreamWriter sw = new StreamWriter(Path.Combine(target.ModelDirectory,"temp.txt")))
      {
        sw.Write(target.GetINCON());
      }
    }

    /// <summary>
    ///A test for OpenCoftFile
    ///</summary>
    [TestMethod()]
    public void OpenCoftFileTest()
    {
      Model target = new Model(@"C:\Jacob\Udvikling\NewT2voc\Models\Centrifuge\centrifuge - Kopi.txt");
      target.OpenCoftFile();
    }
  }
}
