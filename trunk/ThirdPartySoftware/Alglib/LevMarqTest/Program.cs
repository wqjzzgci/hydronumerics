using System;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conwx.OV.Radar;

namespace LevMarqTest
{
  class Program
  {
    private static DistanceCorrector dc;

    private static List<List<short>> Data;

    public static void function1_fvec(double[] x, double[] fi, object obj)
    {
      if (dc == null)
      {
        dc = new DistanceCorrector();
        using (FileStream Fs = new FileStream(@"C:\radardata\distanceCorrection.xml", FileMode.Open))
        {
          DataContractSerializer ds = new DataContractSerializer(dc.GetType());
          dc = (DistanceCorrector)ds.ReadObject(Fs);
        }
        Data = new List<List<short>>();
//        Data.Add(dc.GetPercentiles(50)); 
//        Data.Add(dc.GetPercentiles(65)); 
        //Data.Add(dc.GetPercentiles(75)); 
        //Data.Add(dc.GetPercentiles(85));
        Data.Add(dc.GetPercentiles(95));
      }
      int startr=24;

      int[] remove = new []{53,77,109};
      double fvalue = 0;
      for (int j = 0; j < Data.Count; j++)
      {
        for (int i = startr; i < 126; i++)
        {
          if (!remove.Contains(i))
          {
            double a = x[0];// +x[1] * Data[j][i];
            double b = x[1];// +x[3] * Data[j][i];
            double c = x[2];// +x[5] * Data[j][i];
            fvalue += Math.Pow(Data[j][startr] - Data[j][i] * (a + b * i +c*Math.Pow(i,2)), 2);
          }
        }
      }
      fi[0] = fvalue;
      //
      // this callback calculates
      // f0(x0,x1) = 100*(x0+3)^4,
      // f1(x0,x1) = (x1-3)^4
      //
//      fi[0] = 10 * System.Math.Pow(x[0] + 3, 2) +System.Math.Pow(x[1] - 3, 2);
     // fi[1] = System.Math.Pow(x[1] - 3, 2);
    }
    public static int Main(string[] args)
    {
      //
      // This example demonstrates minimization of F(x0,x1) = f0^2+f1^2, where 
      //
      //     f0(x0,x1) = 10*(x0+3)^2
      //     f1(x0,x1) = (x1-3)^2
      //
      // using "V" mode of the Levenberg-Marquardt optimizer.
      //
      // Optimization algorithm uses:
      // * function vector f[] = {f1,f2}
      //
      // No other information (Jacobian, gradient, etc.) is needed.
      //
      double[] x = new double[] { 0.68143553477961927, 0.011414501096002361, 0 };
      double epsg = 0.00000001;
      double epsf = 0;
      double epsx = 0;
      int maxits = 500000;
      alglib.minlmstate state;
      alglib.minlmreport rep;

      alglib.minlmcreatev(1, x, 0.000001, out state);
      alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);
      alglib.minlmoptimize(state, function1_fvec, null, null);
      alglib.minlmresults(state, out x, out rep);
      
      System.Console.WriteLine("{0}", rep.iterationscount); // EXPECTED: 4
      System.Console.WriteLine("{0}", alglib.ap.format(x, 2)); // EXPECTED: [-3,+3]

      double[] fi = new double[1];

      function1_fvec(x, fi, null);
      System.Console.WriteLine(fi[0]);

      System.Console.ReadLine();
      return 0;
    }
  }
}
