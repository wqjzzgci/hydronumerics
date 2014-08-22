using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using HydroNumerics.Core.Time;
using System.Collections.Generic;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ExtensionsTest and is intended
    ///to contain all ExtensionsTest Unit Tests
    ///</summary>
  [TestClass()]
  public class ExtensionsTest
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
    ///A test for FromCSV
    ///</summary>
    [TestMethod()]
    public void FromCSVTest()
    {
      DataTable data = new DataTable();
      string filename = string.Empty; // TODO: Initialize to an appropriate value
      data.FromCSV( @"D:\DK_information\output.csv","");
      data.ToCSV(@"D:\DK_information\output2.csv");
    }

    /// <summary>
    ///A test for ToCSV
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void ToCSVTest()
    {
      DataTable data = new DataTable();
      data.FromCSV(@"D:\DK_information\output_temp2.csv","");
      int ID15 = 82101044; 
      Extensions.ToCSV(data, ID15, @"D:\DK_information\ts.csv");
    }

    /// <summary>
    ///A test for ToExcelTemplate
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void ToExcelTemplateTest()
    {
      DataTable data = new DataTable();
      data.FromCSV(@"D:\DK_information\output_temp2.csv","");
      string TemplateFilename = @"D:\DK_information\Output\DetailedStationOutputTemplate.xls";
      Extensions.ToExcelTemplate(data, TemplateFilename, @"D:\DK_information\Output");
    }

    /// <summary>
    ///A test for ToCSV
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void ToCSVTest1()
    {
      DataTable data = new DataTable();
      data.FromCSV(@"D:\DK_information\output_temp2.csv", "");

      string parametername = "Groundwater";
      string filename = @"D:\DK_information\groundwater.csv";
      Extensions.ToCSV(data, parametername, filename);
    }

    /// <summary>
    ///A test for ExtractTimeSeries
    ///</summary>
    [TestMethod()]
    [Ignore]
    public void ExtractTimeSeriesTest()
    {
      DataTable data = new DataTable();
      data.FromCSV(@"D:\DK_information\output_temp2.csv", "");

      string parametername = "Groundwater";
      var actual = Extensions.ExtractTimeSeries(data, parametername);
    }
  }
}
