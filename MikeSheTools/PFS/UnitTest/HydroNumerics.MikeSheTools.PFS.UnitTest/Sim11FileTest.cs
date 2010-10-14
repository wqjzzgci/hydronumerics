using System;
using System.IO;

using HydroNumerics.MikeSheTools.PFS.Sim11;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DHI.Generic.MikeZero;



namespace HydroNumerics.MikeSheTools.PFS.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for InputTest and is intended
    ///to contain all InputTest Unit Tests
    ///</summary>
  [TestClass()]
  public class Sim11FileTest
  {

    private static Sim11File _sm11;

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
      _sm11 = new Sim11File(@"..\..\..\testdata\mike11\novomr6_release2009.sim11");
    }
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
    ///A test for NWK11FileName
    ///</summary>
    [TestMethod()]
    public void NWK11FileNameTest()
    {
      Assert.AreEqual(Path.GetFullPath(@"..\..\..\testdata\mike11\novomr6.nwk11").ToLower(), _sm11.FileNames.NWK11FileName.ToLower());
    }

    /// <summary>
    ///A test for BND11FileName
    ///</summary>
    [TestMethod()]
    public void BND11FileNameTest()
    {
      Assert.AreEqual(Path.GetFullPath(@"..\..\..\testdata\mike11\novomr6.bnd11").ToLower(), _sm11.FileNames.BND11FileName.ToLower());
    }

    /// <summary>
    ///A test for XNS11FileName
    ///</summary>
    [TestMethod()]
    public void XNS11FileNameTest()
    {
      Assert.AreEqual(Path.GetFullPath(@"..\..\..\testdata\mike11\novomr6.xns11").ToLower(), _sm11.FileNames.XNS11FileName.ToLower());
    }
  }
}
