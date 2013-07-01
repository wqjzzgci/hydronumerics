using System.Linq;
using HydroNumerics.MikeSheTools.PFS.NWK11;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.MikeSheTools.PFS.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for NWK11FileTest and is intended
    ///to contain all NWK11FileTest Unit Tests
    ///</summary>
  [TestClass()]
  public class NWK11FileTest
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
    ///A test for Save
    ///</summary>
    [TestMethod()]
    public void SaveTest()
    {
      string SheFileName = @"..\..\..\PFS\unittest\TestData\novomr6.nwk11"; // TODO: Initialize to an appropriate value
      NWK11File target = new NWK11File(SheFileName); // TODO: Initialize to an appropriate value

      Assert.AreEqual(581119.81, target.MIKE_11_Network_editor.POINTS.points.First(p=>p.Par1 ==1700).Par2);

      Assert.IsTrue( target.MIKE_11_Network_editor.COMPUTATIONAL_SETUP.SaveAllGridPoints1);
      Assert.AreEqual("BIRKMOSE_BAEK", target.MIKE_11_Network_editor.BRANCHES.branchs[9].definitions.Par1);

//      Assert.AreEqual(7024, target.MIKE_11_Network_editor.BRANCHES.branchs[4].ComputationalPoints[5].Chainage);

      Assert.AreEqual(0.000101257, target.MIKE_11_Network_editor.MIKESHECOUPLING.MikeSheCouplings[0].Par5);

      Assert.AreEqual(9313, target.MIKE_11_Network_editor.MIKESHECOUPLING.MikeSheCouplings[3].Par3);
      Assert.AreEqual(-7462, target.MIKE_11_Network_editor.MIKESHECOUPLING.MikeSheCouplings[4].Par2);
      Assert.AreEqual("BAEKKEN", target.MIKE_11_Network_editor.MIKESHECOUPLING.MikeSheCouplings[5].Par1);

      Assert.AreEqual(143, target.MIKE_11_Network_editor.MIKESHECOUPLING.MikeSheCouplings.Count);

      target.SaveAs(@"..\..\..\PFS\unittest\TestData\novomr6_new.nwk11");
    }
  }
}
