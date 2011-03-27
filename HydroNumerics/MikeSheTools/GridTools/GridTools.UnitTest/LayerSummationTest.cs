﻿using GridTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

      XElement Op = new XElement("GridOperation", new XAttribute("Type", GridOperation.GridMath),
        new XElement("DFS2FileName1", @"..\..\..\Testdata\Novomr1_inv_PreProcessed.DFS2"),
        new XElement("Item1", "1"),
        new XElement("MathOperation", "/"),
        new XElement("DFS2FileName2", @"..\..\..\Testdata\Novomr1_inv_PreProcessed.DFS2"),
        new XElement("Item2", "12"),
        new XElement("DFS2OutputFileName", @"..\..\..\Testdata\GridMathSum.DFS2")
        );


      LayerSummation.GridMath(Op);

    }
  }
}