using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
  [TestClass]
  public class WaterPacketTest
  {

    [TestMethod]
    public void ConstructTest()
    {

      //Test that an argumentoutofrangeException is thrown if water volume is less than zero
      try
      {
        WaterPacket w = new WaterPacket(-1);
        throw new Exception();
      }
      catch (ArgumentOutOfRangeException AE)
      {

      }

      WaterPacket w2 = new WaterPacket(1, 100);
      Assert.AreEqual(1, w2.Composition[1]);

    }

    [TestMethod]
    public void RadonTest()
    {
      WaterPacket w = new WaterPacket(12);
      Chemical C =ChemicalFactory.Instance.GetChemical(ChemicalNames.Radon);

      //Checks old evaporation
      C.IsFirstOrderDegradable = false;

      w.AddChemical(C, 5);

      Assert.AreEqual(0.4167, w.GetConcentration(C),0.001);

      w.MoveInTime(TimeSpan.FromDays(1), 6);
      Assert.AreEqual(0.3846, w.GetConcentration(C),0.001);

      w.MoveInTime(TimeSpan.FromDays(1), 6);
      Assert.AreEqual(0.355, w.GetConcentration(C),0.001);

      //Checks new Reactions

      double d = w.GetConcentration(C);

      w.MoveInTime(TimeSpan.FromDays(3.8), ChemicalFactory.Instance.LakeReactions);
      Assert.AreEqual(d/2, w.GetConcentration(C), 0.01);

    }


    [TestMethod]
    public void TemperatureTest()
    {
      WaterPacket w = new WaterPacket(1);
      w.AddEnergy(2000);

      Assert.AreEqual(4.779e-4, w.Temperature, 1e-6);
      w.AddEnergy(-2000);
      Assert.AreEqual(0, w.Temperature, 1e-6);
      w.AddEnergy(50000);
      Assert.AreEqual(0.01194, w.Temperature, 1e-4);
      WaterPacket w2 = new WaterPacket(1);

      w.Add(w2);
      Assert.AreEqual(0.0059737, w.Temperature, 1e-4);

    }

    [TestMethod]
    public void AddTest()
    {
      int ID1 = 1;
      int ID2 = 2;

      WaterPacket w = new WaterPacket(1);
      w.IDForComposition = ID1;
      
      
      WaterPacket w2 = new WaterPacket(9);
      w2.IDForComposition = ID2;

      w.Add(w2);
      Assert.AreEqual(10, w.Volume);
      Assert.AreEqual(0.1, w.Composition[ID1]);
      Assert.AreEqual(0.9, w.Composition[ID2]);
      Assert.AreEqual(1, w2.Composition[ID2]);
    }

    [TestMethod]
    public void SubtractTest()
    {
      int ID1 = 1;
      WaterPacket w = new WaterPacket(500);
      w.IDForComposition = ID1;

      IWaterPacket w2 = w.Substract(230);

      Assert.AreEqual(270, w.Volume);
      Assert.AreEqual(230, w2.Volume);

      Assert.AreEqual(1, w.Composition[ID1]);
      Assert.AreEqual(1, w.Composition[ID1]);

      IWaterPacket w3 = w.Substract(1000);
      Assert.AreEqual(0, w.Volume);
      Assert.AreEqual(270, w3.Volume);

      Assert.AreEqual(1, w3.Composition[ID1]);

    }

    public void Tagtest()
    {
      WaterPacket w = new WaterPacket(500);

      w.Tag(1234);
      w.Tag(4567);

      Assert.IsTrue(w.LogString.ToString().Contains("1234"));
      Assert.IsTrue(w.LogString.ToString().Contains("4567"));


    }



    [TestMethod]
    public void EvaporateTest()
    {
      WaterPacket w = new WaterPacket(500);

      w.IDForComposition = 1;

      w.Evaporate(200);

      Assert.AreEqual(300, w.Volume);

      Assert.AreEqual(1, w.Composition[1]);

      w.Evaporate(1000);
      Assert.AreEqual(0, w.Volume);

    }
  }
}
