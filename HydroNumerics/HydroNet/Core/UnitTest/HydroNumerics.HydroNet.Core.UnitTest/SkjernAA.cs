using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;


namespace HydroNumerics.HydroNet.Core.UnitTest
{
  [TestClass]
  public class SkjernAA
  {
    [TestMethod]
    public void TracerTest()
    {
      int count = 3;
      double length = 10870;
      DateTime Start= new DateTime(2000,1,1);

      List<Lake> lakes = NetworkBuilder.CreateConnectedLakes(count);
      
      foreach (Lake L in lakes)
      {
        L.SurfaceArea = XYPolygon.GetSquare(length / count);
        L.Depth = 1;
        L.SetState("Initial",Start, new WaterPacket(L.Volume));
      }
      Chemical c = ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl);

      SinkSourceBoundary fb = new SinkSourceBoundary(10870.0 / (8.49 * 3600));
      fb.WaterSample = new WaterPacket(1);
      lakes.First().Sources.Add(fb);

      WaterPacket plug = new WaterPacket(1);
      plug.AddChemical(c, 10000);
      lakes.First().AddWaterPacket(Start, Start.AddHours(1), plug.DeepClone());

      lakes.First().Output.LogChemicalConcentration(c);

      lakes.Last().Output.LogChemicalConcentration(c);


      Stream us = new Stream("us",1, 1, 1);
      us.Sources.Add(fb);
      Stream s = new Stream("s",1000, 1, 1);

      Lake L2 = new Lake("L2",870);

      Stream s1 = new Stream("s1", 9000, 1, 1);

      s.AddWaterPacket(Start, Start.AddSeconds(1), plug.DeepClone());
      s1.Output.LogChemicalConcentration(c);

      us.AddDownStreamWaterBody(s);
      s.AddDownStreamWaterBody(L2);
      L2.AddDownStreamWaterBody(s1);


      Model m = new Model();
      m._waterBodies.AddRange(lakes.Cast<IWaterBody>());

      m._waterBodies.Add((IWaterBody)us);
      m._waterBodies.Add((IWaterBody)s);
      m._waterBodies.Add((IWaterBody)L2);
      m._waterBodies.Add((IWaterBody)s1);

      m.SetState("Initial", Start, new WaterPacket(1));



      m.MoveInTime(Start.AddHours(15), TimeSpan.FromMinutes(1));

      lakes.Last().Output.Save(@"C:\temp\LastLake.xts");
      s1.Output.Save(@"C:\temp\Stream.xts");


      int n = 15;
      List<IWaterBody> wbs = NetworkBuilder.CreateCombo(n, 10870 / n/2.0);
      foreach (IWaterBody wb in wbs)
      {
        wb.SetState("Initial", Start, new WaterPacket(wb.Volume));
      }

      wbs.First().AddWaterPacket(Start, Start.AddHours(1), plug.DeepClone());
      us.AddDownStreamWaterBody(wbs.First());
      us.RestoreState("Initial");

      m._waterBodies.Clear();
      m._waterBodies.Add(us);
      m._waterBodies.AddRange(wbs);

      m.SetState("Initial", Start, new WaterPacket(1));

      ((Stream)wbs.Last()).Output.LogChemicalConcentration(c);

      m.MoveInTime(Start.AddHours(15), TimeSpan.FromMinutes(1));

      ((Stream)wbs.Last()).Output.Save(@"C:\temp\Stream.xts");
    }

  }
}
