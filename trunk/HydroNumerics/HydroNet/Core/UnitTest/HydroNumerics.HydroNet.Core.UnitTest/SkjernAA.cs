using System;
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
      Stream s = new Stream(10870, 1, 1);
      s.SetState("Initial", Start, new WaterWithChemicals(s.Volume));
      us.SetState("Initial", Start, new WaterWithChemicals(us.Volume));
    
      s.ReceiveWater(Start, Start.AddSeconds(1), plug.DeepClone());
      s.Output.LogChemicalConcentration(c);

      us.DownStreamConnections.Add(s);


      Model m = new Model();
      m._waterBodies.AddRange(lakes.Cast<IWaterBody>());

      m._waterBodies.Add((IWaterBody)us);
      m._waterBodies.Add((IWaterBody)s);

      m.MoveInTime(Start, Start.AddHours(15), TimeSpan.FromMinutes(30),false);

      lakes.Last().Output.Save(@"C:\temp\LastLake.xts");
      s.Output.Save(@"C:\temp\Stream.xts");


    }

  }
}
