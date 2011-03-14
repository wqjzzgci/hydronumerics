using HydroNumerics.JupiterTools.JupiterPlus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System;

namespace HydroNumerics.JupiterTools.JupiterPlus.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ChangeTest and is intended
    ///to contain all ChangeTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ChangeTest
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
    ///A test for ToXML
    ///</summary>
    [TestMethod()]
    public void ToXMLTest()
    {
      Change target = new Change(); // TODO: Initialize to an appropriate value
      target.ColumnName = "BOREHOLE";
      target.Action = Action.EditValue;
      target.Comments.Add("No comments");
      target.Date = DateTime.Now;
      target.NewValue = "654000";
      target.OldValue = "0";
      target.PrimaryKeys.Add(new HydroNumerics.Core.Tuple<string, string>("BOREHOLENO", "193. 127"));
      target.Project = "NOVANA";
      target.User = "JAG";

      var k = target.ToXML();
     


    }
  }
}
