using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Windows.Markup;


using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;
using HydroNumerics.Geometry;


namespace HydroNumerics.HydroNet.Core.UnitTest
{
  [TestClass()]
  public class SerializationTest
  {

    [TestMethod]
    public void ModelSaveTest()
    {
      Model M = new Model();
      M._waterBodies.Add(new Stream("S", 100, 1, 1));

      Model M2 = (Model)ReadWrite(M);

      Assert.AreEqual(M._waterBodies.Count, M2._waterBodies.Count);

      IWaterBody  L = new Lake("L", 1);
      
      

    }

    [TestMethod]
    public void WaterPacketTest()
    {
      WaterPacket wp1 = new WaterPacket(23);
      ReadWrite(wp1);

      WaterPacket wp = new WaterPacket(1, 250);
      wp.MoveInTime( TimeSpan.FromDays(1),1);

      WaterPacket wp2 = (WaterPacket)ReadWrite(wp);

      WaterEquals(wp, wp2);
    }


    [TestMethod]
    public void FlowboundaryTest()
    {
      SinkSourceBoundary fb = new SinkSourceBoundary(2.2);
      fb.Name = "ID";
      fb.WaterSample = new WaterPacket(23);
      SinkSourceBoundary fb2 = (SinkSourceBoundary) ReadWrite(fb);

      Assert.AreEqual(fb.Name, fb2.Name);
      WaterEquals(fb.WaterSample, fb2.WaterSample);


      SinkSourceBoundary fb3 = new SinkSourceBoundary(new TimespanSeries("nedbør", new DateTime(2000, 1, 1), 10, 1, TimestepUnit.Days, 10));

      fb2 = (SinkSourceBoundary)ReadWrite(fb3);

      Assert.AreEqual(180000.0, fb2.GetSourceWater(new DateTime(2000, 1, 2), TimeSpan.FromHours(5)).Volume, 0.01);
    }

    [TestMethod]
    public void GroundwaterBoundaryTest()
    {
      Stream stream = new Stream("S", 1,1,1);
      stream.WaterLevel =2;
      GroundWaterBoundary gwb = new GroundWaterBoundary(stream, 2, 4, 5, XYPolygon.GetSquare(23) );
      gwb.WaterSample = new WaterPacket(1);

      GroundWaterBoundary gwb2 = (GroundWaterBoundary)ReadWrite(gwb);
      Assert.AreEqual(gwb.IsSource(DateTime.Now), gwb2.IsSource(DateTime.Now));
      WaterEquals(gwb.GetSourceWater(DateTime.Now, TimeSpan.FromDays(1)), gwb2.GetSourceWater(DateTime.Now, TimeSpan.FromDays(1)));
      
    }

    [TestMethod]
    public void EvaporationRateBoundaryTest()
    {
      EvaporationRateBoundary evp = new EvaporationRateBoundary(23);
      evp.ContactGeometry = XYPolygon.GetSquare(2);
      evp.Name = "Test";
      EvaporationRateBoundary evp2 = (EvaporationRateBoundary)ReadWrite(evp);
      Assert.AreEqual(evp.Name, evp2.Name);
      Assert.AreEqual(((XYPolygon)evp.ContactGeometry).GetArea(), ((XYPolygon)evp.ContactGeometry).GetArea());
      Assert.AreEqual(evp.GetSinkVolume(DateTime.Now, TimeSpan.FromDays(1.5)), evp2.GetSinkVolume(DateTime.Now, TimeSpan.FromDays(1.5)));
    }

    [TestMethod]
    public void StreamTest()
    {
      Stream s = new Stream("S" ,2, 3, 4);
      s.ID = 1;

      Stream s2 = (Stream)ReadWrite(s);

      Assert.AreEqual(s.Volume, s2.Volume);
      Assert.AreEqual(s.ID, s2.ID);
    }

    [TestMethod]
    public void TimeSpanSeriesTest()
    {
      TimespanSeries ts = new TimespanSeries("TSName", new DateTime(2010, 1, 1), 10, 1, TimestepUnit.Days, 5.5);
      TimespanSeries ts2 = (TimespanSeries) this.ReadWrite(ts);

      ts2.AddValue(DateTime.Now, DateTime.Now.AddDays(1), 2);
      

    }

    [TestMethod]
    public void LakeTest()
    {
      Lake l = new Lake("L", 10000);
      l.ID = 3;

      GroundWaterBoundary gwb = new GroundWaterBoundary(l, 2, 4, 5, XYPolygon.GetSquare(23));
      gwb.WaterSample = new WaterPacket(1);
      l.GroundwaterBoundaries.Add(gwb);

      Lake l2 = (Lake)ReadWrite(l);

      Assert.AreEqual(l.Volume, l2.Volume);
      Assert.AreEqual(l.ID, l2.ID);
      Assert.AreEqual(l.Area, l2.Area);

      Assert.AreEqual(l.GroundwaterBoundaries.Count, l2.GroundwaterBoundaries.Count);

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

    private object ReadWriteXAML(object ToSerialize)
    {

      using (FileStream fs = new FileStream("out.xml", FileMode.Create))
      {
        System.Windows.Markup.XamlWriter.Save(ToSerialize, fs);
      }
      object FromSerialize;

      using (FileStream fs = new FileStream("out.xml", FileMode.Open))
      {
        FromSerialize=XamlReader.Load(fs);
      }

      return FromSerialize;

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
