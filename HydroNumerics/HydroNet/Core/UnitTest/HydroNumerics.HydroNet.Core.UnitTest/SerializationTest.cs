using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;

using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;


namespace HydroNumerics.HydroNet.Core.UnitTest
{
  [TestClass()]
  public class SerializationTest
  {

    [TestMethod]
    public void ModelSaveTest()
    {
      Model M = new Model();
      M._waterBodies.Add(new Stream(100, 1, 1));

      Model M2 = (Model)ReadWrite(M);

      Assert.AreEqual(M._waterBodies.Count, M2._waterBodies.Count);

    }

    [TestMethod]
    public void WaterPacketTest()
    {
      WaterPacket wp1 = new WaterPacket(23);
      ReadWrite(wp1);

      WaterPacket wp = new WaterPacket(1, 250);
      wp.MoveInTime( TimeSpan.FromDays(1));

      WaterPacket wp2 = (WaterPacket)ReadWrite(wp);

      WaterEquals(wp, wp2);
    }


    [TestMethod]
    public void FlowboundaryTest()
    {
      FlowBoundary fb = new FlowBoundary(2.2);
      fb.Area = 23;
      fb.Name = "ID";
      fb.WaterSample = new WaterPacket(23);
      FlowBoundary fb2 = (FlowBoundary) ReadWrite(fb);

      Assert.AreEqual(fb.Name, fb2.Name);
      Assert.AreEqual(fb.Area, fb2.Area);
      WaterEquals(fb.WaterSample, fb2.WaterSample);


      FlowBoundary fb3 = new FlowBoundary(new TimespanSeries("nedbør", new DateTime(2000, 1, 1), 10, 1, TimestepUnit.Days, 10));

      fb2 = (FlowBoundary)ReadWrite(fb3);

      Assert.AreEqual(180000.0, fb2.GetSourceWater(new DateTime(2000, 1, 2), TimeSpan.FromHours(5)).Volume, 0.01);
    }

    [TestMethod]
    public void GroundwaterBoundaryTest()
    {
      Stream stream = new Stream(1,1,1);
      stream.WaterLevel =2;
      GroundWaterBoundary gwb = new GroundWaterBoundary(stream, 2, 3, 4, 5);
      gwb.Area = 23;
      gwb.WaterSample = new WaterPacket(1);

      GroundWaterBoundary gwb2 = (GroundWaterBoundary)ReadWrite(gwb);
      Assert.AreEqual(gwb.Source(DateTime.Now), gwb2.Source(DateTime.Now));
      WaterEquals(gwb.GetSourceWater(DateTime.Now, TimeSpan.FromDays(1)), gwb2.GetSourceWater(DateTime.Now, TimeSpan.FromDays(1)));
      
    }

    [TestMethod]
    public void EvaporationRateBoundaryTest()
    {
      EvaporationRateBoundary evp = new EvaporationRateBoundary(23);
      evp.Area = 2;
      evp.Name = "Test";
      EvaporationRateBoundary evp2 = (EvaporationRateBoundary)ReadWrite(evp);
      Assert.AreEqual(evp.Name, evp2.Name);
      Assert.AreEqual(evp.Area, evp2.Area);
      Assert.AreEqual(evp.GetEvaporationVolume(DateTime.Now, TimeSpan.FromDays(1.5)), evp2.GetEvaporationVolume(DateTime.Now, TimeSpan.FromDays(1.5)));
    }

    [TestMethod]
    public void StreamTest()
    {
      Stream s = new Stream(2, 3, 4);
      s.ID = 1;

      Stream s2 = (Stream)ReadWrite(s);

      Assert.AreEqual(s.Volume, s2.Volume);
      Assert.AreEqual(s.ID, s2.ID);
    }

    [TestMethod]
    public void LakeTest()
    {
      Lake l = new Lake(10000);
      l.ID = 3;

      GroundWaterBoundary gwb = new GroundWaterBoundary(l, 2, 3, 4, 5);
      gwb.Area = 23;
      gwb.WaterSample = new WaterPacket(1);

      Lake l2 = (Lake)ReadWrite(l);

      Assert.AreEqual(l.Volume, l2.Volume);
      Assert.AreEqual(l.ID, l2.ID);
      Assert.AreEqual(l.Area, l2.Area);

      Assert.AreEqual(l.SinkSources.Count, l2.SinkSources.Count);

    }

    private void WaterEquals(IWaterPacket wp, IWaterPacket wp2)
    {
      Assert.AreEqual(wp.Volume, wp2.Volume);
      Assert.AreEqual(wp.WaterAge, wp2.WaterAge);
      Assert.AreEqual(wp.RelativeTimeTag, wp2.RelativeTimeTag);
      if (wp.Composition.Count > 0)
      {
        Assert.AreEqual(wp.Composition.Values.First(), wp2.Composition.Values.First());
        Assert.AreEqual(wp.Composition.Keys.First(), wp2.Composition.Keys.First());
      }
    }


    private object ReadWrite(object ToSerialize)
    {

      DataContractSerializer dc = new DataContractSerializer(ToSerialize.GetType(), ModelFactory.KnownTypes, int.MaxValue, false, true, null);

      using (FileStream fs = new FileStream("out.xml", FileMode.Create))
      {
        dc.WriteObject(fs, ToSerialize);
      }
      object FromSerialize;

      using (FileStream fs = new FileStream("out.xml", FileMode.Open))
      {
        FromSerialize = dc.ReadObject(fs);
      }

      return FromSerialize;

    }

  }
}
