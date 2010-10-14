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
	/// Summary description for XYPolylineTest.
	/// </summary>
	[TestClass]
	public class XYPolylineTest
	{
		public XYPolylineTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[TestMethod]
		public void GetLength()
		{
			XYPolyline xyPolyline = new XYPolyline();
			xyPolyline.Points.Add(new XYPoint(6,2));
			xyPolyline.Points.Add(new XYPoint(2,2));
			xyPolyline.Points.Add(new XYPoint(8,2));
			xyPolyline.Points.Add(new XYPoint(8,4));
			xyPolyline.Points.Add(new XYPoint(5,4));
			xyPolyline.Points.Add(new XYPoint(9,7));
			
			Assert.AreEqual((double) 20, xyPolyline.GetLength()); 
		}

		[TestMethod]
		public void GetLine()
		{
			XYPolyline xyPolyline = new XYPolyline();
			xyPolyline.Points.Add(new XYPoint(6,2));
			xyPolyline.Points.Add(new XYPoint(2,2));
			xyPolyline.Points.Add(new XYPoint(8,2));
			xyPolyline.Points.Add(new XYPoint(8,4));
			xyPolyline.Points.Add(new XYPoint(5,4));
			xyPolyline.Points.Add(new XYPoint(9,7));
	
			Assert.AreEqual(new XYLine(6,2,2,2),xyPolyline.GetLine(0));
			Assert.AreEqual(new XYLine(2,2,8,2),xyPolyline.GetLine(1));
			Assert.AreEqual(new XYLine(8,2,8,4),xyPolyline.GetLine(2));
			Assert.AreEqual(new XYLine(8,4,5,4),xyPolyline.GetLine(3));
			Assert.AreEqual(new XYLine(5,4,9,7),xyPolyline.GetLine(4));
		}
    
    [TestMethod]
    public void Equals()
    {
      XYPolyline p1 = new XYPolyline();
      p1.Points.Add(new XYPoint(0, 3));
      p1.Points.Add(new XYPoint(3, 0));
      p1.Points.Add(new XYPoint(8, 0));
      p1.Points.Add(new XYPoint(8, 2));
      p1.Points.Add(new XYPoint(3, 1));
      p1.Points.Add(new XYPoint(3, 3));
      p1.Points.Add(new XYPoint(8, 3));
      p1.Points.Add(new XYPoint(4, 7));

      XYPolyline p2 = new XYPolyline();
      p2.Points.Add(new XYPoint(0, 3));
      p2.Points.Add(new XYPoint(3, 0));
      p2.Points.Add(new XYPoint(8, 0));
      p2.Points.Add(new XYPoint(8, 2));
      p2.Points.Add(new XYPoint(3, 1));
      p2.Points.Add(new XYPoint(3, 3));
      p2.Points.Add(new XYPoint(8, 3));
      p2.Points.Add(new XYPoint(4, 7));
      
      XYPolyline p3 = new XYPolyline();
      p3.Points.Add(new XYPoint(0, 3));
      p3.Points.Add(new XYPoint(3, 0));
      p3.Points.Add(new XYPoint(8, 0));
      p3.Points.Add(new XYPoint(8, 2));
      p3.Points.Add(new XYPoint(3, 1.1));
      p3.Points.Add(new XYPoint(3, 3));
      p3.Points.Add(new XYPoint(8, 3));
      p3.Points.Add(new XYPoint(4, 7));

      XYPolyline p4 = new XYPolyline();
      p4.Points.Add(new XYPoint(0, 3));
      p4.Points.Add(new XYPoint(3, 0));
      p4.Points.Add(new XYPoint(8, 0));
      p4.Points.Add(new XYPoint(8, 2));
      p4.Points.Add(new XYPoint(3, 1));
      p4.Points.Add(new XYPoint(3, 3));
      p4.Points.Add(new XYPoint(8, 3));
 
      XYPolygon p5 = new XYPolygon();
      p5.Points.Add(new XYPoint(0, 3));
      p5.Points.Add(new XYPoint(3, 0));
      p5.Points.Add(new XYPoint(8, 0));
      p5.Points.Add(new XYPoint(8, 2));
      p5.Points.Add(new XYPoint(3, 1.1));
      p5.Points.Add(new XYPoint(3, 3));
      p5.Points.Add(new XYPoint(8, 3));
      p5.Points.Add(new XYPoint(4, 7));
      
      Assert.AreEqual(true, p1.Equals(p1),"Test1");     
      Assert.AreEqual(true, p1.Equals(p2),"Test2");     
      Assert.AreEqual(false, p1.Equals(p3),"Test3");
      Assert.AreEqual(false, p1.Equals(p4),"Test4");
      Assert.AreEqual(false, p1.Equals(p5),"Test5");
    }
	}
}
