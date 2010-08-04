using HydroNumerics.MikeSheTools.LayerStatistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Diagnostics;

namespace HydroNumerics.MikeSheTools.LayerStatistics.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ProgramTest and is intended
    ///to contain all ProgramTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ProgramTest
  {

    string path = @"..\..\..\LayerStatistics\UnitTest\TestData\";

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
    ///A test for Main
    ///</summary>
    [TestMethod()]
    public void TotalTest()
    {

      Program.Main(new string[] { @path +"conf_mean.xml" });

      //Compare wells
      CompareFiles(@path + "Kopi af novomr1_pejl90-05_mean_sort1_wells.txt", @path + "novomr1_pejl90-05_mean_sort1_wells.txt");
      CompareFiles(@path + "Kopi af novomr1_pejl90-05_mean_sort1_layers.txt", @path + "novomr1_pejl90-05_mean_sort1_layers.txt");
      CompareFiles(@path + "Kopi af novomr1_pejl90-05_mean_sort1_me.txt", @path + "novomr1_pejl90-05_mean_sort1_me.txt");
      CompareFiles(@path + "Kopi af novomr1_pejl90-05_mean_sort1_rmse.txt", @path + "novomr1_pejl90-05_mean_sort1_rmse.txt");
      CompareFiles(@path + "Kopi af novomr1_pejl90-05_mean_sort1_observations.txt", @path + "novomr1_pejl90-05_mean_sort1_observations.txt");

    }


    private void CompareFiles(string filename1, string filename2)
    {
      string org;
      string newfile;
      using (StreamReader Sr = new StreamReader(filename1))
      {
        org = Sr.ReadToEnd();
      }
      using (StreamReader Sr = new StreamReader(filename2))
      {
        newfile = Sr.ReadToEnd();
      }
      Assert.AreEqual(org, newfile);

    }

    [TestMethod()]
    public void MultipleRunsTest()
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();


      for (int i = 0; i < 50; i++)
      {
        Program.Main(new string[] { @path + "conf_mean.xml" });
      }
      sw.Stop();
      int k = 0;
    }
  }
}
