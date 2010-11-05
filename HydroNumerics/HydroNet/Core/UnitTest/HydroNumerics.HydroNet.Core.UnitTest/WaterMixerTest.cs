using HydroNumerics.HydroNet.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for WaterMixerTest and is intended
    ///to contain all WaterMixerTest Unit Tests
    ///</summary>
  [TestClass()]
  public class WaterMixerTest
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
    ///A test for Mix
    ///</summary>
    [TestMethod()]
    public void MixTest()
    {
      List<IWaterPacket> Waters = new List<IWaterPacket>();

      Waters.Add(new WaterPacket(1, 100));
      Waters.Add(new IsotopeWater(30));
      Waters[1].IDForComposition = 2;

      IWaterPacket actual;
      actual = WaterMixer.Mix(Waters);
      Assert.IsTrue(actual.GetType().Equals(Waters[1].GetType() ));
      Assert.IsFalse(actual.GetType().Equals(Waters[0].GetType()));
      Assert.AreEqual(130, actual.Volume);
      Assert.AreEqual(100.0 / 130.0, actual.Composition[1],0.00001);
      Assert.AreEqual(30.0 / 130.0, actual.Composition[2],0.00001);
    }
  }
}
