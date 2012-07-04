using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ProjNet.Converters;
using ProjNet;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;



namespace HydroNumerics.Geometry.UnitTest
{
  [TestClass]
  public class ConversionTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      var UTMSystem = ProjNet.CoordinateSystems.ProjectedCoordinateSystem.WGS84_UTM(32, true);
      ICoordinateTransformation trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84, UTMSystem);
      double[] p1 = trans.MathTransform.Transform(new double[] { 12.45693, 55.65322 });
      double utmy = p1[1];
      double utmx = p1[0];

      var t = DateTime.Now.Subtract(new DateTime(2012, 7, 1));


    }
  }
}
