using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.Factory;
using Microsoft.Research.Science.Data.Memory;
using Microsoft.Research.Science.Data.Imperative;


namespace Dfs2NetCDF.UnitTest
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class UnitTest1
  {
    public UnitTest1()
    {
      //
      // TODO: Add constructor logic here
      //
    }

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
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    // [ClassInitialize()]
    // public static void MyClassInitialize(TestContext testContext) { }
    //
    // Use ClassCleanup to run code after all tests in a class have run
    // [ClassCleanup()]
    // public static void MyClassCleanup() { }
    //
    // Use TestInitialize to run code before running each test 
    // [TestInitialize()]
    // public void MyTestInitialize() { }
    //
    // Use TestCleanup to run code after each test has run
    // [TestCleanup()]
    // public void MyTestCleanup() { }
    //
    #endregion

    string NetCDFFileName = @"c:\temp\file.nc";


    [TestMethod]
    public void TestMethod1()
    {
      int r =360;
      int z = 126;
      int t = 24*60;
      
      Int16[,,] grid = new Int16[r,z,t];
      int[] R = new int[r];
      int[] Z = new int[z];
      DateTime[] T = new DateTime[t];

      Random rnd = new Random();

      for (int k = 0; k < r; k++)
      {
        R[k]=k;
        for (int i = 0; i < z; i++)
        {
          for (int j = 0; j < t; j++)
          {
            grid[k, i, j] =  (Int16)rnd.Next(1024);
          }
        }
      }

      for (int j = 0; j < t; j++)
      {
        T[j] = new DateTime(2012,1,1).AddMinutes(j);
      }

      for (int i = 0; i < z; i++)
      {
        Z[i] = i;
      }
      // ... compute grid, x and y values
      DataSet ds = DataSet.Open(NetCDFFileName + "?openMode=create");
      
      ds.Add("R", "degrees", R, "R");
      ds.Add("Z", "distance", Z, "Z");
      ds.Add("T", T, "T");
      var val = ds.Add("values","count", grid, "R", "Z", "T");

      //ds.PutAttr("values", "units", "m/sec2");

      ds.Commit();

      //ds.Clone(NetCDFFileName + "?openMode=create").Dispose();

      ds.Dispose();    
    }


    [TestMethod]
    public void ReadTest()
    {

      Stopwatch sw = new Stopwatch();

      var uri = DataSetUri.Create(NetCDFFileName);
      uri.OpenMode = ResourceOpenMode.Open;

      //sw.Start();


      //DataSet ds = DataSet.Open(uri);

      //var val = ds.Variables.First(v => v.Name == "values");
      
      //var vals = ds.GetData<Int16[, ,]>(val.ID);

      //ds.Dispose();

      //sw.Stop();

      //var ts = sw.Elapsed;
      //sw.Reset();

      sw.Start();

      var ds = DataSet.Open(uri);

      sw.Stop();

      var ts2 = sw.Elapsed;

      var val = ds.Variables.First(v => v.Name == "values");


      var time = ds.Variables.First(t => t.Name == "T").GetData();

      var vv = ds.GetData<Int16[, ,]>(val.ID, DataSet.Range(0), DataSet.Range(0), DataSet.Range(0, 1439));

      for (int i=0;i<1400;i++)
        vv[0,0,i]=299;


      ds.PutData<Int16[, ,]>(val.ID, vv, DataSet.Range(1), DataSet.Range(1), DataSet.Range(0, 1439));

            int r =110;
      int z = 100;

      Int16[, ,] grid = new Int16[r, z, 1];

      Random rnd = new Random();

      for (int k = 0; k < r; k++)
      {
        for (int i = 0; i < z; i++)
        {
            grid[k, i, 0] = (Int16)rnd.Next(1024);
          }
        }

      sw.Reset();
      sw.Start();
      ds.PutData<Int16[, ,]>(val.ID, grid, DataSet.Range(0,r-1), DataSet.Range(0,z-1), DataSet.Range(5000));
      sw.Stop();
      ts2 = sw.Elapsed;



    }

  }
}
