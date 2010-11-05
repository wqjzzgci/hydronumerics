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
      double v1 =Iw.GetIsotopeRatio();
      Iw.Evaporate(2);
      double v2 = Iw.GetIsotopeRatio();
      Iw.Evaporate(5);
      double v5 = Iw.GetIsotopeRatio();
      Iw.Evaporate(90);
      double v90 = Iw.GetIsotopeRatio();
      Assert.AreEqual(10.011, v1, 0.01);
      Assert.AreEqual(10.0307, v2, 0.01);
      Assert.AreEqual(10.08, v5, 0.01);

      Assert.AreEqual(11.17, Iw.GetIsotopeRatio(), 0.01);

    }

    [TestMethod]
    public void CastingTest()
    {
      IsotopeWater Iw = new IsotopeWater(100);
      Iw.SetIsotopeRatio(0.5);

      Assert.IsFalse(Iw.GetType().Equals(typeof(WaterPacket)));

      WaterPacket wc = Iw as WaterPacket;
      Assert.IsNotNull(wc);

      Assert.IsTrue(wc.Chemicals.ContainsKey(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction)));

      WaterPacket w = new WaterPacket(1);
      wc = w as IsotopeWater;
      Assert.IsNull(wc);

    }

  }
}
