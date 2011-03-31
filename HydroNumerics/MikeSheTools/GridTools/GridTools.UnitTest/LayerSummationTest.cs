using GridTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml.Linq;

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
         new XElement("Items", "1"),
         new XElement("TimeSteps", ""),
         new XElement("MathOperation", "-"),
         new XElement("Factor", "2.5"));
      GridFunctions.FactorMath(ops);    
    }

    [TestMethod]
    public void TimeSummationTest()
    {
      new XElement("GridOperations");

      XElement Op = new XElement("GridOperation", new XAttribute("Type", "TimeSummation"),
        new XElement("DFSFileName", @"..\..\..\TestData\TestModel.she - Result Files\TestModel_3DSZflow.dfs3"),
        new XElement("Items", ""),
        new XElement("TimeInterval", "Week"),
        new XElement("DFSOutputFileName", @"..\..\..\TestData\TestModel.she - Result Files\MonthlySum.dfs3")
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
         new XElement("MathOperation", "*"),
        new XElement("MonthlyValues", "1.1,222,3,4,5,6,7,8,9,10,11,12"));
      GridFunctions.MonthlyMath(ops);    

    }

  }
}
