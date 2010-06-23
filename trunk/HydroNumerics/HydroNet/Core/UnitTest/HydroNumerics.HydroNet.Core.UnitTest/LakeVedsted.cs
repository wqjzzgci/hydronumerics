using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core.UnitTest
{
  public class LakeVedsted
  {

    [TestMethod]
    public void GroundWaterTest()
    {
      double area = 7.7 * 10000;
      Lake Vedsted = new Lake(area * 5);
      Vedsted.Area = area;
      Vedsted.WaterLevel = 45.7;
      Vedsted.Name = "Vedsted";


      //Create and add precipitation boundary
      TimespanSeries Precipitation = new TimespanSeries();

      double[] values = new double[] { 108, 83, 73, 52, 61, 86, 99, 101, 75, 108, 85, 101 };
      AddMonthlyValues(Precipitation, 2007, values);
      FlowBoundary Precip = new FlowBoundary(Precipitation);
      Precip.Area = Vedsted.Area;
      Vedsted.SinkSources.Add(Precip);

      //Create and add evaporation boundary
      TimespanSeries Evaporation = new TimespanSeries();
      double[] values2 = new double[] {4,11,34,66,110,118,122,103,61,26,7,1 };
      AddMonthlyValues(Evaporation, 2007, values2);
      EvaporationRateBoundary eva = new EvaporationRateBoundary(Evaporation);
      eva.Area = Vedsted.Area;
      Vedsted.EvaporationBoundaries.Add(eva);

      //Add a virtual lake to collect outflow
      Lake CollectLake = new Lake(100000000);
      CollectLake.Name = "Dummy";
      Vedsted.DownStreamConnections.Add(CollectLake);

      //Add to an engine
      Model E = new Model();
      E._waterBodies.Add(Vedsted);
      E._waterBodies.Add(CollectLake);

      TimestampSeries Discharge = new TimestampSeries();
      Discharge.AddSiValue(new DateTime(2007, 3, 12), 6986 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.AddSiValue(new DateTime(2007, 4, 3), 5894 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.AddSiValue(new DateTime(2007, 4, 25), 1205 / TimeSpan.FromDays(365).TotalSeconds);
      Discharge.RelaxationFactor = 1;

      double d = Discharge.GetValue(new DateTime(2007, 4, 12)) * TimeSpan.FromDays(365).TotalSeconds;

      Assert.AreEqual(Discharge.GetValue(new DateTime(2007, 4, 25)), Discharge.GetValue(new DateTime(2007, 6, 25)),0.0000001);

      FlowBoundary Kilde = new FlowBoundary(Discharge);
      Vedsted.SinkSources.Add(Kilde);

      GroundWaterBoundary B1 = new GroundWaterBoundary(Vedsted, 1.3e-4, Vedsted.Area / 10, 1, 45.47);
      B1.Name = "B1";
      Vedsted.SinkSources.Add(B1);

      GroundWaterBoundary B2 = new GroundWaterBoundary(Vedsted, 1e-6, Vedsted.Area / 10, 1, 44.96);
      B2.Name = "B2";
      Vedsted.SinkSources.Add(B2);

      GroundWaterBoundary B3 = new GroundWaterBoundary(Vedsted, 2e-6, Vedsted.Area / 10, 1, 44.63);
      B3.Name = "B3";
      Vedsted.SinkSources.Add(B3);

      GroundWaterBoundary B4 = new GroundWaterBoundary(Vedsted, 4.9e-7, Vedsted.Area / 10, 1, 44.75);
      B4.Name = "B4";
      Vedsted.SinkSources.Add(B4);

      GroundWaterBoundary B5 = new GroundWaterBoundary(Vedsted, 1.5e-8, Vedsted.Area / 10, 1, 44.27);
      B5.Name = "B5";
      Vedsted.SinkSources.Add(B5);

      GroundWaterBoundary B6 = new GroundWaterBoundary(Vedsted, 1.5e-8, Vedsted.Area / 10, 1, 44.16);
      B6.Name = "B6";
      Vedsted.SinkSources.Add(B6);

      GroundWaterBoundary B7 = new GroundWaterBoundary(Vedsted, 1.1e-6, Vedsted.Area / 10, 1, 45.15);
      B7.Name = "B7";
      Vedsted.SinkSources.Add(B7);

      GroundWaterBoundary B8 = new GroundWaterBoundary(Vedsted, 1.1e-6, Vedsted.Area / 10, 1, 44.54);
      B8.Name = "B8";
      Vedsted.SinkSources.Add(B8);

      GroundWaterBoundary B9 = new GroundWaterBoundary(Vedsted, 2.1e-8, Vedsted.Area / 10, 1, 45.4);
      B9.Name = "B9";
      Vedsted.SinkSources.Add(B9);

      GroundWaterBoundary B10 = new GroundWaterBoundary(Vedsted, 3.5e-6, Vedsted.Area / 10, 1, 45.16);
      B10.Name = "B10";
      Vedsted.SinkSources.Add(B10);

      ////Add seepage meter boundaries
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
      //Now move a year

      DateTime Start = new DateTime(2007, 1, 1);
      DateTime End = new DateTime(2007, 12, 31);

      E.SetState("Initial", Start, new WaterPacket(1));
      //Increase the volume to prevent outflow
      Vedsted.Volume *= 1.5;



     // E.MoveInTime(Start, End, TimeSpan.FromDays(1));

     // Vedsted.Output.Save(@"c:\temp\step1.xts");

     // double outflow = Vedsted.Output.Outflow.GetValue(Start, End.Subtract(TimeSpan.FromDays(5)));
      //double evapo = Vedsted.Output.Evaporation.GetValue(Start, End.Subtract(TimeSpan.FromDays(5)));

      E.MoveInTime(End, TimeSpan.FromDays(30));

      double outflow2 = Vedsted.Output.Outflow.GetValue(Start, End.Subtract(TimeSpan.FromDays(5)));
      double evapo2 = Vedsted.Output.Evaporation.GetValue(Start, End.Subtract(TimeSpan.FromDays(5)));

//      Vedsted.Output.Save(@"c:\temp\step2.xts");
      //Assert.AreEqual(outflow- evapo, outflow2 - evapo2, 0.000001);

      E.Save("Vedsted.xml");



      IsotopeWater iwlake = new IsotopeWater(area * 5);
      iwlake.SetIsotopeRatio(2.5);

      //Increase the volume to prevent outflow
      Vedsted.Volume /= 1.5;
      E.SetState("Isotop", Start, iwlake);
      Vedsted.Volume *= 1.5;

      Vedsted.SinkSources.Clear();
      IsotopeWater precipwater = new IsotopeWater(1);
      precipwater.SetIsotopeRatio(10);
      Precip.WaterSample = precipwater;
      Vedsted.SinkSources.Add(Precip);

      FlowBoundary fb = new FlowBoundary(50000 / 365 / 86400);
      IsotopeWater gw = new IsotopeWater(1);
      gw.SetIsotopeRatio(8.5);
      fb.WaterSample = gw;
      Vedsted.SinkSources.Add(fb);

      FlowBoundary fbout = new FlowBoundary(-50000 / 365 / 86400);
      Vedsted.SinkSources.Add(fbout);
      Vedsted.Output.LogChemicalConcentration(ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction));

      E.MoveInTime(End, TimeSpan.FromDays(30));
//      Vedsted.Output.Save(@"c:\temp\isotope.xts");

      E.Save(@"c:\temp\setup.xml");


      Model m = new Model(@"c:\temp\setup.xml");

    }

    private void AddMonthlyValues(TimespanSeries TS, int year, double[] values)
    {
      double conversion1 = 1.0 / 1000 / 86400 / 31;
      double conversion2 = 1.0 / 1000 / 86400 / 28;
      double conversion3 = 1.0 / 1000 / 86400 / 30;
      TS.AddSiValue(new DateTime(year, 1, 1),new DateTime(year, 2, 1), values[0] * conversion1);
      TS.AddSiValue(new DateTime(year, 2, 1), new DateTime(year, 3, 1), values[0] * conversion2);
      TS.AddSiValue(new DateTime(year, 3, 1), new DateTime(year, 4, 1), values[0] * conversion1);
      TS.AddSiValue(new DateTime(year, 4, 1), new DateTime(year, 5, 1), values[0] * conversion3);
      TS.AddSiValue(new DateTime(year, 5, 1), new DateTime(year, 6, 1), values[0] * conversion1);
      TS.AddSiValue(new DateTime(year, 6, 1), new DateTime(year, 7, 1), values[0] * conversion3);
      TS.AddSiValue(new DateTime(year, 7, 1), new DateTime(year, 8, 1), values[0] * conversion1);
      TS.AddSiValue(new DateTime(year, 8, 1), new DateTime(year, 9, 1), values[0] * conversion3);
      TS.AddSiValue(new DateTime(year, 9, 1), new DateTime(year, 10, 1), values[0] * conversion1);
      TS.AddSiValue(new DateTime(year, 10, 1), new DateTime(year,11, 1), values[0] * conversion3);
      TS.AddSiValue(new DateTime(year, 11, 1), new DateTime(year, 12, 1), values[0] * conversion1);
      TS.AddSiValue(new DateTime(year, 12, 1), new DateTime(year + 1, 1, 1), values[0] * conversion3);

    }

  }
}
