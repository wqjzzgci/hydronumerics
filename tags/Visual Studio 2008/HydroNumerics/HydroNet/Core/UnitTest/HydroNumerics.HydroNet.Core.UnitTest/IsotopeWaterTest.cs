using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.HydroNet.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;

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
      TimestampSeries ts = new TimestampSeries();

      ts.AddSiValue(new DateTime(2000, 1, 1),5);
      ts.AllowExtrapolation = true;
      ts.ExtrapolationMethod = ExtrapolationMethods.Linear;
      ts.RelaxationFactor = 1;
      Iw.EvaporationConcentration = ts;

      Iw.Evaporate(1);
      double v1 =Iw.GetIsotopeRatio();
      Iw.Evaporate(2);
      double v2 = Iw.GetIsotopeRatio();
      Iw.Evaporate(5);
      double v5 = Iw.GetIsotopeRatio();
      Iw.Evaporate(90);
      double v90 = Iw.GetIsotopeRatio();
      Assert.AreEqual(10.101, v1, 0.01);
      Assert.AreEqual(10.309, v2, 0.01);
      Assert.AreEqual(10.870, v5, 0.01);
      Assert.AreEqual(500, Iw.GetIsotopeRatio(), 0.01);
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

    [TestMethod]
    public void KrabbenhoftExample()
    {
      Lake L = new Lake("Sparkling Lake", XYPolygon.GetSquare(0.81e6));
      L.Depth = 8.84e6 / L.Area;
      L.Output.LogAllChemicals = true;

      IsotopeWater LakeWater = new IsotopeWater(1);
      LakeWater.SetIsotopeRatio(5.75);
      TimestampSeries EvapoConcentrations = new TimestampSeries();
      EvapoConcentrations.AddSiValue(new DateTime(1985, 4, 1), 3.95);
      EvapoConcentrations.AddSiValue(new DateTime(1985, 5, 1), 13.9);
      EvapoConcentrations.AddSiValue(new DateTime(1985, 6, 1), 25.24);
      EvapoConcentrations.AddSiValue(new DateTime(1985, 7, 1), 23.97);
      EvapoConcentrations.AddSiValue(new DateTime(1985, 8, 1), 17.13);
      EvapoConcentrations.AddSiValue(new DateTime(1985, 9, 1), 10.40);
      EvapoConcentrations.AddSiValue(new DateTime(1985, 10, 1), 6.12);
      EvapoConcentrations.AddSiValue(new DateTime(1985, 10, 1), 33.24);
      EvapoConcentrations.AllowExtrapolation = true;
      EvapoConcentrations.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
      LakeWater.EvaporationConcentration = EvapoConcentrations;

      TimestampSeries PrecipConcentrations = new TimestampSeries();
      PrecipConcentrations.AddSiValue(new DateTime(1985, 1, 1), 22.8);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 2, 1), 22.8);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 3, 1), 22.8);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 4, 1), 14.8);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 5, 1), 10.7);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 6, 1), 6.3);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 7, 1), 5.1);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 8, 1), 8.4);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 9, 1), 11.1);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 10, 1), 13.8);
      PrecipConcentrations.AddSiValue(new DateTime(1985, 10, 1), 21.9);
      PrecipConcentrations.AllowExtrapolation = true;
      PrecipConcentrations.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;

      TimespanSeries Precipitation = new TimespanSeries();
      Precipitation.Unit = new HydroNumerics.Core.Unit("cm/month", 1.0 / 100.0 / (86400.0 * 30.0), 0);
      Precipitation.AddValue(new DateTime(1985, 1, 1), new DateTime(1985, 3, 1), 0);
      Precipitation.AddValue(new DateTime(1985, 3, 1), new DateTime(1985, 3, 31), 12.5);
      Precipitation.AddValue(new DateTime(1985, 4, 1), new DateTime(1985, 4, 30), 7.1);
      Precipitation.AddValue(new DateTime(1985, 5, 1), new DateTime(1985, 5, 31), 7.6);
      Precipitation.AddValue(new DateTime(1985, 6, 1), new DateTime(1985, 6, 30), 8.8);
      Precipitation.AddValue(new DateTime(1985, 7, 1), new DateTime(1985, 7, 31), 8.6);
      Precipitation.AddValue(new DateTime(1985, 8, 1), new DateTime(1985, 8, 31), 12.7);
      Precipitation.AddValue(new DateTime(1985, 9, 1), new DateTime(1985, 9, 30), 11);
      Precipitation.AddValue(new DateTime(1985, 10, 1), new DateTime(1985, 10, 31), 6.2);
      Precipitation.AddValue(new DateTime(1985, 11, 1), new DateTime(1985, 11, 30), 4.8);
      Precipitation.AddValue(new DateTime(1985, 11, 30), new DateTime(1985, 12, 31), 0);
      Precipitation.AllowExtrapolation = true;
      Precipitation.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;

      Assert.AreEqual(79, 12*Precipitation.GetValue(new DateTime(1985,1,1), new DateTime(1985,12,31)),3);

      SourceBoundary Precip = new SourceBoundary(Precipitation);
      Precip.WaterSample = new IsotopeWater(1);
      Precip.AddChemicalConcentrationSeries(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction), PrecipConcentrations);

      TimespanSeries Evaporation = new TimespanSeries();
      Evaporation.Unit = new HydroNumerics.Core.Unit("cm/month", 1.0 / 100.0 / (86400.0 * 30.0), 0);
      Evaporation.AddValue(new DateTime(1985, 1, 1), new DateTime(1985, 4, 1), 0);
      Evaporation.AddValue(new DateTime(1985, 4, 1), new DateTime(1985, 4, 30), 2.8);
      Evaporation.AddValue(new DateTime(1985, 5, 1), new DateTime(1985, 5, 31), 7.0);
      Evaporation.AddValue(new DateTime(1985, 6, 1), new DateTime(1985, 6, 30), 10.5);
      Evaporation.AddValue(new DateTime(1985, 7, 1), new DateTime(1985, 7, 31), 11.1);
      Evaporation.AddValue(new DateTime(1985, 8, 1), new DateTime(1985, 8, 31), 10.0);
      Evaporation.AddValue(new DateTime(1985, 9, 1), new DateTime(1985, 9, 30), 7.0);
      Evaporation.AddValue(new DateTime(1985, 10, 1), new DateTime(1985, 10, 31), 4.7);
      Evaporation.AddValue(new DateTime(1985, 11, 1), new DateTime(1985, 11, 30), 0.6);
      Evaporation.AddValue(new DateTime(1985, 11, 30), new DateTime(1985, 12, 31), 0);
      Evaporation.AllowExtrapolation = true;
      Evaporation.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
      EvaporationRateBoundary erb = new EvaporationRateBoundary(Evaporation);

      Assert.AreEqual(54, 12*Evaporation.GetValue(new DateTime(1985,1,1), new DateTime(1985,12,31)),3);

      
      GroundWaterBoundary grb = new GroundWaterBoundary(L, 1e-7, 1, 1, (XYPolygon) L.Geometry);
      grb.FlowType = GWType.Flow;
      grb.WaterFlow = new TimespanSeries();
      grb.WaterFlow.AddSiValue(DateTime.MinValue,DateTime.MaxValue, Evaporation.Unit.ToSiUnit(29/12) * L.Area);
      IsotopeWater gwsp25 = new IsotopeWater(1);
      gwsp25.SetIsotopeRatio(11.5);
      grb.WaterSample = gwsp25;

      GroundWaterBoundary gout = new GroundWaterBoundary(L, 1e-7, 1, -1, (XYPolygon)L.Geometry);
      gout.FlowType = GWType.Flow;
      gout.WaterFlow = new TimespanSeries();
      gout.WaterFlow.AddSiValue(DateTime.MinValue, DateTime.MaxValue, - Evaporation.Unit.ToSiUnit(54/12) * L.Area);
      
      DateTime Start = new DateTime(1985,1,1);
      L.Precipitation.Add(Precip);
      Precip.ContactGeometry = L.Geometry;
      L.EvaporationBoundaries.Add(erb);
      erb.ContactGeometry = L.Geometry;
      L.GroundwaterBoundaries.Add(grb);
      L.GroundwaterBoundaries.Add(gout);

      Model M = new Model();
      M.WaterBodies.Add(L);
      M.SetState("Initial", Start, LakeWater);

      L.Depth *= 1.5;
      ((IsotopeWater)L.CurrentStoredWater).CurrentTime = Start;
      M.MoveInTime(new DateTime(1985, 12, 31), TimeSpan.FromDays(10));

      M.Save(@"..\..\..\TestData\Krabbenhoft.xml");
    }
  }
}
