using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
      Lake Vedsted = new Lake(new WaterPacket( area * 5));
      Vedsted.Area = area;
      Vedsted.WaterLevel = 45.7;

      //Create and add precipitation boundary
      TimeSeries Precipitation = new TimeSeries();
      double[] values = new double[] { 108, 83, 73, 52, 61, 86, 99, 101, 75, 108, 85, 101 };
      AddMonthlyValues(Precipitation, 2007, values);
      FlowBoundary Precip = new FlowBoundary(Precipitation);
      Precip.Area = Vedsted.Area;
      Vedsted.AddWaterSinkSource(Precip);

      //Create and add evaporation boundary
      TimeSeries Evaporation = new TimeSeries();
      double[] values2 = new double[] {4,11,34,66,110,118,122,103,61,26,7,1 };
      AddMonthlyValues(Evaporation, 2007, values2);
      TestEvaporation eva = new TestEvaporation(Evaporation);
      eva.Area = Vedsted.Area;
      Vedsted.AddEvaporationBoundary(eva);

      //Add a virtual lake to collect outflow
      Lake CollectLake = new Lake(100000000);
      Vedsted.AddDownstreamConnection(CollectLake);

      //Add to an engine
      List<IWaterBody> Lakes = new List<IWaterBody>();
      Lakes.Add(Vedsted);
      Lakes.Add(CollectLake);
      Engine E = new Engine(Lakes);

      //Add seepage meter boundary
      GroundWaterBoundary S1 = new GroundWaterBoundary(Vedsted, 4e-5, 1, 2, 46);
      Vedsted.AddWaterSinkSource(S1);
      //Now move a year
      E.MoveInTime(new DateTime(2007, 1, 1), new DateTime(2008, 1, 1), TimeSpan.FromDays(30));

    }

    private void AddMonthlyValues(TimeSeries TS, int year, double[] values)
    {
      double conversion1 = 1.0 / 1000 / 86400 / 31;
      double conversion2 = 1.0 / 1000 / 86400 / 28;
      double conversion3 = 1.0 / 1000 / 86400 / 30;
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 1, 31), values[0] * conversion1));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 2, 28), values[1] * conversion2));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 3, 31), values[2] * conversion1));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 4, 30), values[3] * conversion3));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 5, 31), values[4] * conversion1));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 6, 30), values[5] * conversion3));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 7, 31), values[6] * conversion1));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 8, 31), values[7] * conversion3));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 9, 30), values[8] * conversion1));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 10, 31), values[9] * conversion3));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 11, 30), values[10] * conversion1));
      TS.AddTimeValueRecord(new TimeValue(new DateTime(year, 12, 31), values[11] * conversion3)); 


    }

  }
}
