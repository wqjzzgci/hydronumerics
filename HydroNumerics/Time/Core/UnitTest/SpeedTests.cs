using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.Time.Core.UnitTest
{
  /// <summary>
  /// Summary description for SpeedTests
  /// </summary>
  [TestClass]
  public class SpeedTests
  {
    public SpeedTests()
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

    [TestMethod]
    public void TestMethod1()
    {

      List<TimestampValue> _list = new List<TimestampValue>();

      DateTime Start = DateTime.Now;

      int n=100000;

      for (int i = 0; i < n; i++)
      {
        _list.Add(new TimestampValue(Start.AddSeconds(i), i));
      }

      Stopwatch sw = new Stopwatch();

      SortedList<DateTime, double> _sortedList = new SortedList<DateTime, double>();

      sw.Start();
      for (int i = n - 1; i > 0; i--)
      {
        _sortedList.Add(_list[i].Time, _list[i].Value);
      }
      sw.Stop();
      Console.WriteLine("SortedList:" + sw.Elapsed);

      SortedDictionary<DateTime, double> _sortedDictionary = new SortedDictionary<DateTime, double>();

      sw.Reset();
      sw.Start();
      for (int i = n - 1; i > 0; i--)
      {
        _sortedDictionary.Add(_list[i].Time, _list[i].Value);
      }

      sw.Stop();
      Console.WriteLine("SortedDictionary:" + sw.Elapsed);

      TimestampSeries ts = new TimestampSeries();
      sw.Reset();
      sw.Start();
      for (int i = n-1; i >0; i--)
      {
        ts.AddValue(_list[i].Time, _list[i].Value);
      }
      sw.Stop();
      Console.WriteLine("TimestampSeries:" + sw.Elapsed);


      List<TimestampValue> _listAndSort = new List<TimestampValue>();
      sw.Reset();
      sw.Start();
      for (int i = n - 1; i > 0; i--)
      {
        _listAndSort.Add(_list[i]);
      }
      _listAndSort.Sort((var,var2) => var.Time.CompareTo(var2.Time));

      sw.Stop();
      Console.WriteLine("_listAndSort:" + sw.Elapsed);

      sw.Reset();
      sw.Start();
      _listAndSort.Sort((var, var2) => var.Time.CompareTo(var2.Time));

      sw.Stop();

      Console.WriteLine("Sort sorted list:" + sw.Elapsed);


    }
  }
}
