﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Time.Core;


namespace HydroNumerics.HydroNet.Core.UnitTest
{
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
        L.Volume = length / count;
        L.SetState("Initial",Start, new WaterWithChemicals(L.Volume));
      }
      Chemical c = ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl);

      FlowBoundary fb = new FlowBoundary(10870.0 / (8.49 * 3600));
      fb.WaterSample = new WaterWithChemicals(1);
      lakes.First().SinkSources.Add(fb);

      WaterWithChemicals plug = new WaterWithChemicals(1);
      plug.AddChemical(c, 10000);
      lakes.First().ReceiveWater(Start, Start.AddHours(1), plug.DeepClone());

      lakes.First().Output.LogChemicalConcentration(c);

      lakes.Last().Output.LogChemicalConcentration(c);


      Stream us = new Stream(1, 1, 1);
      us.SinkSources.Add(fb);
      Stream s = new Stream(1000, 1, 1);

      Lake L2 = new Lake(870);

      Stream s1 = new Stream(9000, 1, 1);

      s.ReceiveWater(Start, Start.AddSeconds(1), plug.DeepClone());
      s1.Output.LogChemicalConcentration(c);

      us.DownStreamConnections.Add(s);
      s.DownStreamConnections.Add(L2);
      L2.DownStreamConnections.Add(s1);


      Model m = new Model();
      m._waterBodies.AddRange(lakes.Cast<IWaterBody>());

      m._waterBodies.Add((IWaterBody)us);
      m._waterBodies.Add((IWaterBody)s);
      m._waterBodies.Add((IWaterBody)L2);
      m._waterBodies.Add((IWaterBody)s1);

      m.SetState("Initial", Start, new WaterWithChemicals(1));



      m.MoveInTime(Start.AddHours(15), TimeSpan.FromMinutes(1));

      lakes.Last().Output.Save(@"C:\temp\LastLake.xts");
      s1.Output.Save(@"C:\temp\Stream.xts");


      int n = 15;
      List<IWaterBody> wbs = NetworkBuilder.CreateCombo(n, 10870 / n/2.0);
      foreach (IWaterBody wb in wbs)
      {
        wb.SetState("Initial", Start, new WaterWithChemicals(wb.Volume));
      }

      wbs.First().ReceiveWater(Start, Start.AddHours(1), plug.DeepClone());
      us.DownStreamConnections.Add(wbs.First());
      us.RestoreState("Initial");

      m._waterBodies.Clear();
      m._waterBodies.Add(us);
      m._waterBodies.AddRange(wbs);

      m.SetState("Initial", Start, new WaterWithChemicals(1));

      ((Stream)wbs.Last()).Output.LogChemicalConcentration(c);

      m.MoveInTime(Start.AddHours(15), TimeSpan.FromMinutes(1));

      ((Stream)wbs.Last()).Output.Save(@"C:\temp\Stream.xts");
    }

  }
}
