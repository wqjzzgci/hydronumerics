using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.Tough2.ViewModel.UnitTest
{
  [TestClass]
  public class Rune
  {
    [TestMethod]
    public void AdjustMesh()
    {
      Mesh target = new Mesh(@"D:\T2VOC\FineGridRadial\mesh"); // TODO: Initialize to an appropriate value

      foreach (var v in target.Elements)
      {
        if (v.Z >= -3)
          v.Material = 3;
        else if (v.Z >= -3.2)
          v.Material = 5;
        else if (v.Z >= -4)
          v.Material = 3;
        else if (v.Z >= -4.6)
          v.Material = 1;
        else if (v.Z >= -5)
          v.Material = 3;
        else if (v.Z >= -6.6)
          v.Material = 1;
        else if (v.Z >= -6.8)
          v.Material = 4;
        else
          v.Material = 1;
      }

      target.Save();
    }

    [TestMethod]
    public void AdjustMesh2()
    {
      Mesh target = new Mesh(@"D:\T2VOC\inclined\mesh"); // TODO: Initialize to an appropriate value

      foreach (var v in target.Elements)
      {
        if (v.Z <= -2)
          v.Material = 2;
        else
          v.Material = 1;
      }
      target.Save();
    }

    [TestMethod]
      public void MyTestMethod()
      {
        Mesh target = new Mesh(@"D:\T2VOC\Small2DGrid\mesh"); // TODO: Initialize to an appropriate value

        double[] x = new double[] { 0, 1, 1.2, 1.4, 2 };
        double[] y = new double[] { -0.3, -0.3, -0.15, -0.3, -0.3 };
        MathNet.Numerics.Interpolation.Algorithms.CubicSplineInterpolation cl = new MathNet.Numerics.Interpolation.Algorithms.CubicSplineInterpolation(x.ToList(), y.ToList());


        foreach (var e in target.Elements)
        {

          if (e.X <= 1 || e.X >= 1.4)
          {
            if (e.Z < -0.3)
              e.Material = 1;
            else
              e.Material = 2;
          }
          else
          {
            var v = cl.Interpolate(e.X.Value);
            if (e.Z.Value < v)
              e.Material = 1;
            else
              e.Material = 2;

          }
        }
        target.Save();
      }

    

    [TestMethod]
    public void SetInDelvWells()
    {
      Mesh target = new Mesh(@"D:\T2VOC\FineGridRadial\mesh"); // TODO: Initialize to an appropriate value

      Gener g = new Gener();

      foreach (var v in target.Elements)
      {



      }


    }
  }
}
