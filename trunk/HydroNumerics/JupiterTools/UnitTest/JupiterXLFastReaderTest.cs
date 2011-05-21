using HydroNumerics.JupiterTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;

namespace HydroNumerics.JupiterTools.UnitTest
{
    /// <summary>
    ///This is a test class for JupiterXLFastReaderTest and is intended
    ///to contain all JupiterXLFastReaderTest Unit Tests
    ///</summary>
  [TestClass()]
  public class JupiterXLFastReaderTest
  {

    private static JupiterXLFastReader Reader;

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
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
      Reader = new JupiterXLFastReader(@"..\..\..\TestData\AlbertslundPcJupiter.mdb");
    }
    
//    Use ClassCleanup to run code after all tests in a class have run
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
    }
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
    ///A test for GetPlant
    ///</summary>
    [TestMethod()]
    public void GetPlantTest()
    {
      int plantid;
      string BoreholeNO;
      int IntakeNo;
      Assert.IsTrue( Reader.TryGetPlant(291624, out plantid, out BoreholeNO, out IntakeNo));
      Assert.AreEqual(106349, plantid);
      Assert.AreEqual("200. 2058", BoreholeNO);
      Assert.AreEqual(1, IntakeNo);
    }

    /// <summary>
    ///A test for GetPrimaryID
    ///</summary>
    [TestMethod()]
    public void GetPrimaryIDTest()
    {
      PumpingIntake Intake = null; // TODO: Initialize to an appropriate value
      Plant plant = null; // TODO: Initialize to an appropriate value
      int expected = 0; // TODO: Initialize to an appropriate value
      int actual;
      Assert.IsTrue(Reader.TryGetPrimaryID(Intake, plant, out actual));
      Assert.AreEqual(expected, actual);
    }

    [TestMethod()]
    public void GetPrimaryIDTest2()
    {
      Wells.Well w = new HydroNumerics.Wells.Well("193.   72");
      w.AddNewIntake(1);

      int watlevelno;
      Assert.IsTrue(Reader.TryGetPrimaryID(w.Intakes.First(),new DateTime(1994,8,1), out watlevelno));
      Assert.AreEqual(14, watlevelno);
    }

    /// <summary>
    ///A test for GetLatestDate
    ///</summary>
    [TestMethod()]
    public void GetLatestDateTest()
    {
      Dictionary<string, string> PrimaryKeys = new Dictionary<string, string>();
      PrimaryKeys.Add("BOREHOLENO", "193.   72");

      Nullable<DateTime> actual;
      Reader.TryGetLatestDate(JupiterTables.BOREHOLE, PrimaryKeys, out actual);
      Assert.AreEqual(new DateTime(2007,7,19,15,17,01), actual);

      PrimaryKeys["BOREHOLENO"] = "200.  275";
      Reader.TryGetLatestDate(JupiterTables.BOREHOLE, PrimaryKeys, out actual);
      Assert.AreEqual(new DateTime(1980, 1, 1), actual);
    }
  }
}
