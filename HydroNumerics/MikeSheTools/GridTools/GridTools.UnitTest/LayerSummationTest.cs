using GridTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;

using HydroNumerics.MikeSheTools.DFS;


namespace GridTools.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for LayerSummationTest and is intended
    ///to contain all LayerSummationTest Unit Tests
    ///</summary>
  [TestClass()]
  public class LayerSummationTest
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


    [TestMethod]
    public void ParseStringTest()
    {

      var vals = GridFunctions_Accessor.ParseString("1-4,7", 0, 4);
      Assert.AreEqual(1, vals[0]);
      Assert.AreEqual(2, vals[1]);
      Assert.AreEqual(3, vals[2]);
      Assert.AreEqual(4, vals[3]);
      Assert.AreEqual(7, vals[4]);

      vals = GridFunctions_Accessor.ParseString("", 0, 4);
      Assert.AreEqual(0, vals[0]);
      Assert.AreEqual(1, vals[1]);
      Assert.AreEqual(2, vals[2]);
      Assert.AreEqual(3, vals[3]);
      
      vals = GridFunctions_Accessor.ParseString("", 1, 5);
      Assert.AreEqual(1, vals[0]);
      Assert.AreEqual(2, vals[1]);
      Assert.AreEqual(3, vals[2]);
      Assert.AreEqual(4, vals[3]);

      vals = GridFunctions_Accessor.ParseString("1, 2", 1, 5);
      Assert.AreEqual(1, vals[0]);
      Assert.AreEqual(2, vals[1]);
 
    }


    [Ignore]
    [TestMethod]
    public void MartinData()
    {

      Program_Accessor.Main(new string[] { @"..\..\..\Testdata\GWL_diff.xml" });

      DFS2 outp = new DFS2(@"..\..\..\Testdata\test_Diff_GWL.dfs2");

      Assert.AreEqual(13, outp.GetData(0, 1)[234, 160]);
      Assert.AreEqual(13, outp.GetData(1, 1)[234, 160]);
      Assert.AreEqual(14, outp.GetData(2, 1)[234, 160]);
      Assert.AreEqual(42.4304, outp.GetData(4, 1)[231, 160],0.001);
      outp.Dispose();

    }

    [TestMethod]
    public void LayerSummationTestKarup()
    {
      XElement Op = new XElement("GridOperation", new XAttribute("Type", "LayerSummation"),
        new XElement("DFS3FileName", @"..\..\..\Testdata\Karup_Example_DemoMode.SHE - Result Files\Karup_Example_DemoMode_3DSZ.dfs3"),
        new XElement("Items", "1"),
        new XElement("Layers", ""),
        new XElement("DFS2OutputFileName", @"..\..\..\Testdata\Karup_Example_DemoMode.SHE - Result Files\Karup_Example_DemoMode_3DSZ.dfs2")
        );
      GridFunctions.LayerSummation(Op);
    }


    /// <summary>
    ///A test for GridMath
    ///</summary>
    [TestMethod()]
    public void GridMathTest()
    {
       new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "GridMath"),
        new XElement("DFS2FileName1", @"..\..\..\Testdata\Novomr1_inv_PreProcessed.DFS2"),
        new XElement("Item1", "1"),
        new XElement("MathOperation", "/"),
        new XElement("DFS2FileName2", @"..\..\..\Testdata\Novomr1_inv_PreProcessed.DFS2"),
        new XElement("Item2", "12"),
        new XElement("DFS2OutputFileName", @"..\..\..\Testdata\GridMathSum.DFS2")
        );

      GridFunctions.GridMath(Op);
    }


    /// <summary>
    ///A speed test on 1.6 GB file
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void GridMathTest2()
    {
      new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "GridMath"),
        new XElement("DFS2FileName1", @"C:\Users\Jacob\Projekter\Projekt for Lars\Novomr3_dmu2010_2DSZflow.dfs2"),
        new XElement("Item1", "1"),
        new XElement("MathOperation", "+"),
        new XElement("DFS2FileName2", @"C:\Users\Jacob\Projekter\Projekt for Lars\Novomr3_dmu2010_2DSZflow.dfs2"),
        new XElement("Item2", "2"),
        new XElement("DFS2OutputFileName", @"C:\Users\Jacob\Projekter\Projekt for Lars\GridMathSum.DFS2")
        );

      GridFunctions.GridMath(Op);
    }

    
    [TestMethod]
    public void FactorMathTest()
    {
      File.Copy(@"..\..\..\Testdata\Model Domain and Grid.dfs2", @"..\..\..\Testdata\FactorCalcTest.dfs2", true);

      XElement ops = new XElement("GridOperation", new XAttribute("Type", "FactorMath"),
         new XElement("DFSFileName", @"..\..\..\Testdata\FactorCalcTest.dfs2"),
         new XElement("Items", ""),
         new XElement("TimeSteps", ""),
         new XElement("MathOperation", "-"),
         new XElement("Factor", "2.5"));
      GridFunctions.FactorMath(ops);


      ops = new XElement("GridOperation", new XAttribute("Type", "FactorMath"),
         new XElement("DFSFileName", @"..\..\..\Testdata\Model Domain and Grid.dfs2"),
         new XElement("Items", ""),
         new XElement("TimeSteps", ""),
         new XElement("MathOperation", "-"),
         new XElement("Factor", "2.5"),
              new XElement("DFSOutputFileName", @"..\..\..\TestData\FactorCalcTest2.dfs2"));
      GridFunctions.FactorMath(ops);    

      ops = new XElement("GridOperation", new XAttribute("Type", "FactorMath"),
         new XElement("DFSFileName", @"..\..\..\Testdata\TestModel.she - Result Files\TestModel_2DSZ.dfs2"),
         new XElement("Items", ""),
         new XElement("TimeSteps", ""),
         new XElement("MathOperation", "*"),
         new XElement("Factor", "10"),
         new XElement("DFSOutputFileName", @"..\..\..\TestData\FactorCalcTest3.dfs2"));
      GridFunctions.FactorMath(ops);    
    
    }

    [TestMethod]
    public void TimeSummationTest()
    {
      new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "TimeSummation"),
        new XElement("DFSFileName", @"..\..\..\TestData\TestModel.she - Result Files\TestModel_3DSZflow.dfs3"),
        new XElement("Items", ""),
        new XElement("TimeInterval", "Year"),
        new XElement("DFSOutputFileName", @"..\..\..\TestData\TestModel.she - Result Files\YearlySum.dfs3")
        );

      GridFunctions.TimeSummation(Op);

      DFS3 dfs = new DFS3(@"..\..\..\TestData\TestModel.she - Result Files\YearlySum.dfs3");
      Assert.AreEqual(1, dfs.NumberOfTimeSteps);
      Assert.AreEqual(5, dfs.NumberOfItems);
      Assert.AreEqual(-6.925168e-007, dfs.GetData(0, 1)[25, 6, 0], 1e-10);
      Assert.AreEqual(-3.86626e-005, dfs.GetData(0, 4)[21, 16, 0], 1e-8);
      dfs.Dispose();

    
    }


    [TestMethod]
    public void TimeMinTest()
    {
      new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "TimeMin"),
        new XElement("DFSFileName", @"..\..\..\TestData\TestModel.she - Result Files\TestModel_3DSZflow.dfs3"),
        new XElement("Items", ""),
        new XElement("TimeInterval", "Year"),
        new XElement("DFSOutputFileName", @"..\..\..\TestData\TestModel.she - Result Files\YearlyMin.dfs3")
        );

      GridFunctions.TimeMin(Op);

      DFS3 dfs = new DFS3(@"..\..\..\TestData\TestModel.she - Result Files\YearlyMin.dfs3");
      Assert.AreEqual(1, dfs.NumberOfTimeSteps);
      Assert.AreEqual(5, dfs.NumberOfItems);
      Assert.AreEqual(-1.601641e-007, dfs.GetData(0, 1)[24, 7, 0], 1e-10);
      Assert.AreEqual(-100.7973, dfs.GetData(0, 3)[18, 11, 0], 1e-2);
      dfs.Dispose();

    }

    [TestMethod]
    public void TimeMaxTest()
    {
      new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "TimeMax"),
        new XElement("DFSFileName", @"..\..\..\TestData\TestModel.she - Result Files\TestModel_3DSZflow.dfs3"),
        new XElement("Items", ""),
        new XElement("TimeInterval", "Year"),
        new XElement("DFSOutputFileName", @"..\..\..\TestData\TestModel.she - Result Files\YearlyMax.dfs3")
        );

      GridFunctions.TimeMax(Op);

      DFS3 dfs = new DFS3(@"..\..\..\TestData\TestModel.she - Result Files\YearlyMax.dfs3");
      Assert.AreEqual(1, dfs.NumberOfTimeSteps);
      Assert.AreEqual(2.000215e-005, dfs.GetData(0, 1)[19, 14, 0], 1e-7);
      Assert.AreEqual(2.527396e-007, dfs.GetData(0, 4)[21, 17, 0],1e-9);
      dfs.Dispose();
    }



    [TestMethod]
    [Ignore]
    public void TimeSummationTest2()
    {

      Stopwatch sw = new Stopwatch();
      sw.Start();
      new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "TimeSummation"),
        new XElement("DFSFileName", @"C:\Users\Jacob\Projekter\Projekt for Lars\Novomr3_dmu2010_3DSZflow.dfs3"),
        new XElement("Items", ""),
        new XElement("TimeInterval", "Year"),
        new XElement("TimeIntervalSteps","1"),
        new XElement("DFSOutputFileName", @"C:\Users\Jacob\Projekter\Projekt for Lars\YearlySum.dfs3")
        );

      GridFunctions.TimeSummation(Op);

      sw.Stop();
      var e = sw.Elapsed;
    }

    [TestMethod]
    [Ignore]
    public void TimeSummationTest3()
    {
      File.Copy(@"C:\Users\Jacob\Projekter\Projekt for Lars\Novomr3_dmu2010_2DSZflow.dfs2", @"C:\Users\Jacob\Projekter\Projekt for Lars\Novomr3_dmu2010_2DSZflow_copy.dfs2");

      new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "TimeSummation"),
        new XElement("DFSFileName", @"C:\Users\Jacob\Projekter\Projekt for Lars\Novomr3_dmu2010_2DSZflow_copy.dfs2"),
        new XElement("Items", ""),
        new XElement("TimeInterval", "Year"),
        new XElement("TimeIntervalSteps", "1")
        );

      GridFunctions.TimeSummation(Op);
    }



    [TestMethod]
    public void MonthlyMath()
    {
      File.Copy(@"..\..\..\Testdata\TestModel.she - Result Files\TestModel_3DSZflow.dfs3", @"..\..\..\Testdata\TestModel.she - Result Files\MonthlyMath.dfs3", true);

      XElement ops = new XElement("GridOperation", new XAttribute("Type", "MonthlyMath"),
         new XElement("DFSFileName", @"..\..\..\Testdata\TestModel.she - Result Files\MonthlyMath.dfs3"),
         new XElement("Items", "1"),
         new XElement("TimeSteps", ""),
         new XElement("MathOperation", "*"),
        new XElement("MonthlyValues", "1.1,222,3,4,5,6,7,8,9,10,11,12"));
      GridFunctions.MonthlyMath(ops);

      ops = new XElement("GridOperation", new XAttribute("Type", "MonthlyMath"),
      new XElement("DFSFileName", @"..\..\..\Testdata\TestModel.she - Result Files\TestModel_3DSZflow.dfs3"),
      new XElement("Items", "1"),
         new XElement("TimeSteps", "1-10"),
      new XElement("MathOperation", "*"),
     new XElement("MonthlyValues", "1.1,222,3,4,5,6,7,8,9,10,11,12"),
      new XElement("DFSOutputFileName", @"..\..\..\Testdata\TestModel.she - Result Files\MonthlyMath2.dfs3"));
      GridFunctions.MonthlyMath(ops);    


    }
    [TestMethod]
    [Ignore]
    public void PercentileTest2()
    {
      Stopwatch sw = new Stopwatch();

      sw.Start();
      new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "Percentile"),
        new XElement("DFSFileName", @"C:\Users\Jacob\Projekter\Projekt for Lars\Novomr3_dmu2010_2DSZflow.dfs2"),
        new XElement("Item", "1"),
        new XElement("TimeSteps", ""),
        new XElement("Percentiles", "0.1,0.5,0.9"),
        new XElement("DFSOutputFileName", @"C:\Users\Jacob\Projekter\Projekt for Lars\percentile.dfs2")
        );

      GridFunctions.Percentile(Op);

      sw.Stop();

      TimeSpan ts = sw.Elapsed;
    }


    [TestMethod]
    public void PercentileTest()
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();

      

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "Percentile"),
        new XElement("DFSFileName", @"..\..\..\TestData\TestDataSet.dfs2"),
        new XElement("Item", "1"),
        new XElement("TimeSteps", ""),
        new XElement("Percentiles", "0.1,0.5,0.9"),
        new XElement("DFSOutputFileName", @"..\..\..\TestData\TestDataSet_percentile.dfs2")
        );

      GridFunctions.Percentile(Op);

      sw.Stop();

      var t = sw.Elapsed;
    }

    [TestMethod]
    public void PercentileMonthTest()
    {

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "Percentile"),
        new XElement("DFSFileName", @"..\..\..\TestData\TestDataSet.dfs2"),
        new XElement("Item", "1"),
        new XElement("TimeSteps", ""),
        new XElement("Percentiles", "0.1,0.5,0.9"),
        new XElement("TimeInterval", "month"),
        new XElement("DFSOutputFileName", @"..\..\..\TestData\TestDataSet_percentile.dfs2")
        );

      GridFunctions.Percentile(Op);

      Op.Element("TimeInterval").Value = "year";

      GridFunctions.Percentile(Op);

    }


  }
}
