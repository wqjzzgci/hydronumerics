using HydroNumerics.MikeSheTools.DFS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Diagnostics;

namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for DFSBaseTest and is intended
    ///to contain all DFSBaseTest Unit Tests
    ///</summary>
  [TestClass()]
  public class DFSBaseTest
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


    internal virtual DFSBase CreateDFSBase()
    {
      // TODO: Instantiate an appropriate concrete class.
      DFSBase target = null;
      return target;
    }

    /// <summary>
    ///A test for Percentile
    ///</summary>
    [TestMethod()]
    public void PercentileTest()
    {
      DFSBase target = DfsFileFactory.OpenFile(@"..\..\..\TestData\novomr4_indv_dfs0_ud1.dfs0");
      double[] Percentiles = new double[] { 0.1, 0.5, 0.9 };
      DFSBase outf = DfsFileFactory.CreateFile(@"..\..\..\TestData\novomr4_indv_dfs0_ud1_percentiles.dfs0", Percentiles.Count());

      outf.CopyFromTemplate(target);
      int Item = 1; // TODO: Initialize to an appropriate value

      int k = 0;
      //Create the items
      foreach (double j in Percentiles)
      {
        outf.Items[k].EumItem = target.Items[Item-1].EumItem;
        outf.Items[k].EumUnit = target.Items[Item-1].EumUnit;
        outf.Items[k].Name = j.ToString() + " Percentile";
        k++;
      }

      int[] TSteps = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

      target.Percentile(Item, TSteps, outf, Percentiles);
      outf.Dispose();
      target.Dispose();

      DFS0 df = new DFS0(@"..\..\..\TestData\novomr4_indv_dfs0_ud1_percentiles.dfs0");

      Assert.AreEqual(25952,df.GetData(0,1),0.5);
      Assert.AreEqual(27294, df.GetData(0, 2),0.5);
      Assert.AreEqual(33422, df.GetData(0, 3), 0.5);

      df.Dispose();
    }

    [TestMethod()]
    public void PercentileTest2()
    {
      DFSBase target = DfsFileFactory.OpenFile(@"..\..\..\TestData\TestDataSet.dfs2");
      double[] Percentiles = new double[] { 0.1, 0.5, 0.9 };
      DFSBase outf = DfsFileFactory.CreateFile(@"..\..\..\TestData\TestDataSet_percentiles_limit.dfs2", Percentiles.Count());
      DFSBase outf2 = DfsFileFactory.CreateFile(@"..\..\..\TestData\TestDataSet_percentiles.dfs2", Percentiles.Count());

      outf.CopyFromTemplate(target);
      outf2.CopyFromTemplate(target);
      int Item = 1;

      int k = 0;
      //Create the items
      foreach (double j in Percentiles)
      {
        outf.Items[k].EumItem = target.Items[Item-1].EumItem;
        outf.Items[k].EumUnit = target.Items[Item-1].EumUnit;
        outf.Items[k].Name = j.ToString() + " Percentile";
        outf2.Items[k].EumItem = target.Items[Item - 1].EumItem;
        outf2.Items[k].EumUnit = target.Items[Item - 1].EumUnit;
        outf2.Items[k].Name = j.ToString() + " Percentile";
        k++;
      }


      int[] TSteps = new int[target.NumberOfTimeSteps];
      for (int i = 0; i < target.NumberOfTimeSteps; i++)
        TSteps[i] = i;

      Stopwatch sw = new Stopwatch();

      sw.Start();
      target.Percentile(Item, TSteps, outf, Percentiles,10);
      sw.Stop();
      TimeSpan el = sw.Elapsed;

      sw.Reset();
      sw.Start();
      target.Percentile(Item, TSteps, outf2, Percentiles);
      sw.Stop();
      TimeSpan el2 = sw.Elapsed;
      outf.Dispose();
      outf2.Dispose();
      target.Dispose();

      DFS2 fil1 = new DFS2(@"..\..\..\TestData\TestDataSet_percentiles_limit.dfs2");
      DFS2 fil2 = new DFS2(@"..\..\..\TestData\TestDataSet_percentiles.dfs2");

      for (int i=1;i<= Percentiles.Count();i++)
      {
      var m1 = fil1.GetData(0,i);
      var m2 = fil2.GetData(0,i);

        for (int j=0;j<m1.Data.Count();j++)
          Assert.AreEqual(m1.Data[j],m2.Data[j]);
      }

      fil1.Dispose();
      fil2.Dispose();
    }

    [Ignore]
    [TestMethod()]
    public void PercentileTest3()
    {
      DFSBase target = DfsFileFactory.OpenFile(@"c:\temp\KFT-SJ_inv_3DSZ.dfs3");
      double[] Percentiles = new double[] { 0.1, 0.5, 0.9 };
      DFSBase outf = DfsFileFactory.CreateFile(@"c:\temp\TestDataSet_percentiles_limit.dfs3", Percentiles.Count());
      DFSBase outf2 = DfsFileFactory.CreateFile(@"c:\temp\TestDataSet_percentiles.dfs3", Percentiles.Count());

      outf.CopyFromTemplate(target);
      outf2.CopyFromTemplate(target);
      int Item = 1;

      int k = 0;
      //Create the items
      foreach (double j in Percentiles)
      {
        outf.Items[k].EumItem = target.Items[Item - 1].EumItem;
        outf.Items[k].EumUnit = target.Items[Item - 1].EumUnit;
        outf.Items[k].Name = j.ToString() + " Percentile";
        outf2.Items[k].EumItem = target.Items[Item - 1].EumItem;
        outf2.Items[k].EumUnit = target.Items[Item - 1].EumUnit;
        outf2.Items[k].Name = j.ToString() + " Percentile";
        k++;
      }


      int[] TSteps = new int[target.NumberOfTimeSteps];
      for (int i = 0; i < target.NumberOfTimeSteps; i++)
        TSteps[i] = i;

      Stopwatch sw = new Stopwatch();

      sw.Start();
      target.Percentile(Item, TSteps, outf, Percentiles, 300);
      sw.Stop();
      TimeSpan el = sw.Elapsed;

      sw.Reset();
      sw.Start();
      target.Percentile(Item, TSteps, outf2, Percentiles,100);
      sw.Stop();
      TimeSpan el2 = sw.Elapsed;
      outf.Dispose();
      outf2.Dispose();
      target.Dispose();

     
    }


  }
}
