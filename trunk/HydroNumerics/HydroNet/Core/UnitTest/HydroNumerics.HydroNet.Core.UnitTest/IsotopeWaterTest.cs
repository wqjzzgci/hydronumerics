using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.HydroNet.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HydroNumerics.HydroNet.Core.UnitTest
{
  [TestClass()]
  public class IsotopeWaterTest
  {

    [TestMethod]
    public void EvaporateTest()
    {
      IsotopeWater Iw = new IsotopeWater(100);
      Iw.SetIsotopeRatio(10);

      Iw.Evaporate(1);
      Assert.AreEqual(10.11, Iw.GetIsotopeRatio(),0.1);
      Iw.Evaporate(2);
      Assert.AreEqual(10.34, Iw.GetIsotopeRatio(), 0.1);
      Iw.Evaporate(5);
      Assert.AreEqual(10.96, Iw.GetIsotopeRatio(), 0.1);
    }

  }
}
