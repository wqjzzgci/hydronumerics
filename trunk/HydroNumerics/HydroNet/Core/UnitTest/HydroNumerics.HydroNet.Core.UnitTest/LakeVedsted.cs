using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
  [TestClass()]
  public class LakeVedsted
  {
      string testDataPath = @"..\..\..\TestData\";
    [TestMethod]
    public void GroundWaterTest()
    {
      WaterPacket GroundWater = new WaterPacket(1);
//      GroundWater.AddChemical(ChemicalFactory.Instance.GetChemical(ChemicalNames.Radon), 0.01);
      GroundWater.IDForComposition = 4;

      Lake Vedsted= LakeFactory.GetLake("Vedsted Sø");
      Vedsted.Depth = 5;
      Vedsted.WaterLevel = 45.7;

      //Create and add a discharge boundary
      TimestampSeries Discharge = new TimestampSeries();
      Discharge.AddSiValue(new DateTime(2007, 3, 12), 6986 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.AddSiValue(new DateTime(2007, 4, 3), 5894 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.AddSiValue(new DateTime(2007, 4, 25), 1205 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.RelaxationFactor = 1;
      Discharge.AllowExtrapolation = true;
      Assert.AreEqual(Discharge.GetValue(new DateTime(2007, 4, 25)), Discharge.GetValue(new DateTime(2007, 6, 25)), 0.0000001);
      SinkSourceBoundary Kilde = new SinkSourceBoundary(Discharge);
      Kilde.Name = "Small spring";
      Kilde.ID = 3;
      Kilde.WaterSample.IDForComposition = 3;
      Vedsted.Sources.Add(Kilde);


      Vedsted.Output.LogAllChemicals = true;
      Vedsted.Output.LogComposition = true;

      //Add to an engine
      Model Engine = new Model();
      Engine.Name = "Vedsted-opsætning";
      Engine._waterBodies.Add(Vedsted);

      //Set initial state
      WaterPacket InitialStateWater = new WaterPacket(1);
      InitialStateWater.IDForComposition = 1;
      DateTime Start = new DateTime(2007, 1, 1);
      DateTime End = new DateTime(2007, 12, 31);
      Engine.SetState("Initial", Start, InitialStateWater);
      Engine.SimulationEndTime = End;
      Engine.TimeStep = TimeSpan.FromDays(30);

      Engine.MoveInTime(End, TimeSpan.FromDays(30));
      Vedsted.Name = "Vedsted step 1";
      Engine.Save(testDataPath + Vedsted.Name + ".xml");
      Engine.RestoreState("Initial");

      //Create and add precipitation boundary
      TimespanSeries Precipitation = new TimespanSeries();
      Precipitation.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
      Precipitation.AllowExtrapolation = true;
      double[] values = new double[] { 108, 83, 73, 52, 61, 86, 99, 101, 75, 108, 85, 101 };
      AddMonthlyValues(Precipitation, 2007, values);
      SinkSourceBoundary Precip = new SinkSourceBoundary(Precipitation);
      Precip.ContactGeometry = Vedsted.SurfaceArea;
      Precip.Name = "Precipitation";
      Precip.ID = 2;
      Precip.WaterSample.IDForComposition = 2;
      Vedsted.Precipitation.Add(Precip);

      //Create and add evaporation boundary
      TimespanSeries Evaporation = new TimespanSeries();
      Evaporation.AllowExtrapolation = true;
      Evaporation.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
      double[] values2 = new double[] {4,11,34,66,110,118,122,103,61,26,7,1 };
      AddMonthlyValues(Evaporation, 2007, values2);
      EvaporationRateBoundary eva = new EvaporationRateBoundary(Evaporation);
      eva.ContactGeometry = Vedsted.SurfaceArea;
      eva.Name = "Evapo";
      
      Vedsted.EvaporationBoundaries.Add(eva);

      Engine.MoveInTime(End, TimeSpan.FromDays(30));
      Vedsted.Name = "Vedsted step 2";
      Engine.Save(testDataPath + Vedsted.Name + ".xml");
      Engine.RestoreState("Initial");


      //To be used by other tests
      Engine.Save(testDataPath + "VedstedNoGroundwater.xml");

      XYPolygon ContactArea = XYPolygon.GetSquare(Vedsted.Area/10);

      #region Groundwater boundaries
      //Add groundwater boundaries
      GroundWaterBoundary B1 = new GroundWaterBoundary(Vedsted, 1.3e-4, 1, 45.47, ContactArea);
      B1.Name = "B1";
      B1.ID = 4;
      B1.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B1);

      GroundWaterBoundary B2 = new GroundWaterBoundary(Vedsted, 1e-6, 1, 44.96, ContactArea);
      B2.Name = "B2";
      B2.ID = 5;
      B2.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B2);

      GroundWaterBoundary B3 = new GroundWaterBoundary(Vedsted, 2e-6, 1, 44.63, ContactArea);
      B3.Name = "B3";
      B3.ID = 6;
      B3.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B3);

      GroundWaterBoundary B4 = new GroundWaterBoundary(Vedsted, 4.9e-7, 1, 44.75, ContactArea);
      B4.Name = "B4";
      B4.ID = 7;
      B4.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B4);

      GroundWaterBoundary B5 = new GroundWaterBoundary(Vedsted, 1.5e-8, 1, 44.27, ContactArea);
      B5.Name = "B5";
      B5.ID = 8;
      B5.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B5);

      GroundWaterBoundary B6 = new GroundWaterBoundary(Vedsted, 1.5e-8, 1, 44.16, ContactArea);
      B6.Name = "B6";
      B6.ID = 9;
      B6.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B6);

      GroundWaterBoundary B7 = new GroundWaterBoundary(Vedsted, 1.1e-6, 1, 45.15, ContactArea);
      B7.Name = "B7";
      B7.ID = 10;
      B7.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B7);

      GroundWaterBoundary B8 = new GroundWaterBoundary(Vedsted, 1.1e-6, 1, 44.54, ContactArea);
      B8.Name = "B8";
      B8.ID = 11;
      B8.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B8);

      GroundWaterBoundary B9 = new GroundWaterBoundary(Vedsted, 2.1e-8, 1, 45.4, ContactArea);
      B9.Name = "B9";
      B9.ID = 12;
      B9.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B9);

      GroundWaterBoundary B10 = new GroundWaterBoundary(Vedsted, 3.5e-6, 1, 45.16, ContactArea);
      B10.Name = "B10";
      B10.ID = 13;
      B10.WaterSample = GroundWater;
      Vedsted.GroundwaterBoundaries.Add(B10);

      #endregion

      Engine.MoveInTime(End, TimeSpan.FromDays(30));
      Vedsted.Name = "Vedsted step 3";
      Engine.Save(testDataPath + Vedsted.Name + ".xml");
      Engine.RestoreState("Initial");

      Vedsted.GroundwaterBoundaries.Clear();

      var cl =ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction);

      GroundWaterBoundary Inflow = new GroundWaterBoundary(Vedsted, 1e-7,1,46.7,XYPolygon.GetSquare(Vedsted.Area/2));
      Inflow.Name = "Inflow";
      GroundWater.AddChemical(cl, 3);
      Inflow.WaterSample = GroundWater;

      Vedsted.RealData.AddChemicalTimeSeries(cl);
      Vedsted.RealData.ChemicalConcentrations[cl].AddSiValue(new DateTime(2007, 8, 7), 2.5);

      ((WaterPacket)InitialStateWater).AddChemical(cl, 2.5 * InitialStateWater.Volume);
      Engine.SetState("Initial", Start, InitialStateWater);

      GroundWaterBoundary Outflow = new GroundWaterBoundary(Vedsted, 1e-7,1,44.7,XYPolygon.GetSquare(Vedsted.Area/2));
      Outflow.Name = "Outflow";

      Vedsted.GroundwaterBoundaries.Add(Inflow);
      Vedsted.GroundwaterBoundaries.Add(Outflow);

      Engine.MoveInTime(End, TimeSpan.FromDays(30));
      Vedsted.Name = "Vedsted step 4";
      Engine.Save(testDataPath + Vedsted.Name + ".xml");
      Engine.RestoreState("Initial");



      #region ////Add seepage meter boundaries
      //GroundWaterBoundary S1 = new GroundWaterBoundary(Vedsted, 4e-5, 1, 2, 46);
      //Vedsted.SinkSources.Add(S1);
      //GroundWaterBoundary S2 = new GroundWaterBoundary(Vedsted, 4e-5, 1, 2, 46);
      //Vedsted.SinkSources.Add(S2);
      //GroundWaterBoundary S3 = new GroundWaterBoundary(Vedsted, 4e-5, 1, 2, 46);
      //Vedsted.SinkSources.Add(S3);
      //GroundWaterBoundary I1 = new GroundWaterBoundary(Vedsted, 4e-5, 1, 2, 46);
      //Vedsted.SinkSources.Add(I1);
      //GroundWaterBoundary I2 = new GroundWaterBoundary(Vedsted, 4e-5, 1, 2, 46);
      //Vedsted.SinkSources.Add(I2);
      //GroundWaterBoundary I3 = new GroundWaterBoundary(Vedsted, 4e-5, 1, 2, 46);
      //Vedsted.SinkSources.Add(I3);

#endregion


      Assert.AreEqual(Evaporation.EndTime, Engine.MaximumEndTime);
      Engine.Save(testDataPath + "Vedsted.xml");

      Engine.MoveInTime(End, TimeSpan.FromDays(30));

      double outflow2 = Vedsted.Output.Outflow.GetValue(Start, End.Subtract(TimeSpan.FromDays(5)));
      double evapo2 = Vedsted.Output.Evaporation.GetValue(Start, End.Subtract(TimeSpan.FromDays(5)));

      Engine.Save(testDataPath + "Vedsted2.xml");

    }

    public static void AddMonthlyValues(TimespanSeries TS, int year, double[] values)
    {
      double conversion1 = 1.0 / 1000 / 86400 / 31;
      double conversion2 = 1.0 / 1000 / 86400 / 28;
      double conversion3 = 1.0 / 1000 / 86400 / 30;
      TS.AddSiValue(new DateTime(year, 1, 1),new DateTime(year, 2, 1), values[0] * conversion1);
      TS.AddSiValue(new DateTime(year, 2, 1), new DateTime(year, 3, 1), values[1] * conversion2);
      TS.AddSiValue(new DateTime(year, 3, 1), new DateTime(year, 4, 1), values[2] * conversion1);
      TS.AddSiValue(new DateTime(year, 4, 1), new DateTime(year, 5, 1), values[3] * conversion3);
      TS.AddSiValue(new DateTime(year, 5, 1), new DateTime(year, 6, 1), values[4] * conversion1);
      TS.AddSiValue(new DateTime(year, 6, 1), new DateTime(year, 7, 1), values[5] * conversion3);
      TS.AddSiValue(new DateTime(year, 7, 1), new DateTime(year, 8, 1), values[6] * conversion1);
      TS.AddSiValue(new DateTime(year, 8, 1), new DateTime(year, 9, 1), values[7] * conversion3);
      TS.AddSiValue(new DateTime(year, 9, 1), new DateTime(year, 10, 1), values[8] * conversion1);
      TS.AddSiValue(new DateTime(year, 10, 1), new DateTime(year,11, 1), values[9] * conversion3);
      TS.AddSiValue(new DateTime(year, 11, 1), new DateTime(year, 12, 1), values[10] * conversion1);
      TS.AddSiValue(new DateTime(year, 12, 1), new DateTime(year + 1, 1, 1), values[11] * conversion3);

    }
    [TestMethod]
    public void IsotopeTest()
    {
      DateTime Start = new DateTime(2007, 1, 1);
      DateTime End = new DateTime(2007, 12, 31);

      Model m = ModelFactory.GetModel(testDataPath + "VedstedNoGroundwater.xml");
      Lake Vedsted = (Lake)m._waterBodies[0];
      Vedsted.Sources.RemoveAt(0);

      Chemical cl = ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl);
      Vedsted.Output.LogChemicalConcentration(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction));
      Vedsted.Output.LogChemicalConcentration(cl);

      IsotopeWater Iw = new IsotopeWater(1);
      Iw.SetIsotopeRatio(10);
      Iw.AddChemical(cl, 0.1);

      Assert.AreEqual(10,Iw.GetConcentration(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction)));
       m.SetState("Initial", Start, Iw);

       Assert.AreEqual(10, ((WaterPacket)Vedsted.CurrentStoredWater).GetConcentration(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction)));

       IsotopeWater precip = new IsotopeWater(1);
       precip.SetIsotopeRatio(5);
      m.MoveInTime(End, TimeSpan.FromDays(30));

      foreach (var v in Vedsted.Output.Items[6].Values)
        Console.WriteLine(v);
      foreach (var v in Vedsted.Output.Items[5].Values)
        Console.WriteLine(v);

      Console.WriteLine(Vedsted.GetStorageTime(Start.AddDays(40), End.AddDays(-40)));

      ModelFactory.SaveModel(testDataPath + "VedstedIso.xml",m);
    }

    [TestMethod]
    public void Vedsted3()
    {
      WaterPacket GroundWater = new WaterPacket(1);
      GroundWater.AddChemical(ChemicalFactory.Instance.GetChemical(ChemicalNames.Radon), 0.01);

      Model m = ModelFactory.GetModel(testDataPath + "VedstedNoGroundwater.xml");
      Lake Vedsted = (Lake)m._waterBodies[0];

      Vedsted.Precipitation.First().WaterSample.IDForComposition = 2;
      Vedsted.Sources.First().WaterSample.IDForComposition = 3;

      GroundWaterBoundary Inflow = new GroundWaterBoundary(Vedsted,1e-6,1,46.7,XYPolygon.GetSquare(Vedsted.Area/2));
      Inflow.Name ="Inflow";
      Inflow.ID =4;
      Inflow.WaterSample = GroundWater;
      Inflow.WaterSample.IDForComposition = 4;

      GroundWaterBoundary Outflow = new GroundWaterBoundary(Vedsted, 1e-6, 1, 44.7, XYPolygon.GetSquare(Vedsted.Area / 2));
      Outflow.Name = "Outflow";
      Outflow.ID = 5;
      Outflow.WaterSample = GroundWater;

      Vedsted.GroundwaterBoundaries.Add(Inflow);
      Vedsted.GroundwaterBoundaries.Add(Outflow);

      DateTime End = new DateTime(2009, 12, 31);

      m.MoveInTime(End, TimeSpan.FromDays(30));

      ModelFactory.SaveModel(testDataPath + "Vedsted3.xml", m);


    }
  }
}
