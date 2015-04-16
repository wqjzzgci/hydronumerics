using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using HydroNumerics.Geometry;

namespace HydroNumerics.Geometry.UnitTest
{
	/// <summary>
	/// Summary description for XYPointTest.
	/// </summary>
	[TestClass]
	public class XYPointTest
	{

    [TestMethod]
    public void System34Test()
    {
      //var info = ProjNet.Converters.WellKnownText.CoordinateSystemWktReader.Parse("PROJCS[\"Danish System 34 Sjaelland\", GEOGCS [ \"European 1950 (Denmark)\", DATUM [\"European 1950 (Denmark)\", SPHEROID [\"International 1924\", 6378388, 297],-81.0703, -89.3603, -115.7526, .48488, .02436, .41321, -.540645], PRIMEM [ \"Greenwich\", 0.000000 ], UNIT [\"Decimal Degree\", 0.01745329251994330]], PROJECTION [\"Transverse Mercator Sjaelland\"], PARAMETER [\"Scale_Factor\", 0.999600], PARAMETER [\"Central_Meridian\", 9.000000], PARAMETER [\"False_Easting\", 500000.000000], UNIT [\"Meter\", 1.000000000000]]");



    }

    [TestMethod]
    public void Equals()
    {
      XYPoint p1 = new XYPoint(2,3);
      XYPoint p2 = new XYPoint(2,3);
      XYPoint p3 = new XYPoint(2,-3);
      XYLine l1 = new XYLine(2,3,3,4);
      Assert.AreEqual(true, p1.Equals(p1),"Test1");
      Assert.AreEqual(true, p1.Equals(p2),"Test2");
      Assert.AreEqual(false, p1.Equals(p3),"Test3");
      Assert.AreEqual(false, p1.Equals(l1),"Test4");
    }

		[TestMethod]
		public void PropertyTest()
		{
			XYPoint xypoint = new XYPoint(2,3);
			Assert.AreEqual((double) 2, xypoint.X);
			Assert.AreEqual((double) 3, xypoint.Y);

			xypoint.X = 6;
			xypoint.Y = 7;
			Assert.AreEqual((double) 6, xypoint.X);
			Assert.AreEqual((double) 7, xypoint.Y);

		}
	}
}
