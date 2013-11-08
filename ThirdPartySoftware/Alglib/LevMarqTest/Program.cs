using System;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JacobGudbjerg.Daisy.ViewModel;


namespace LevMarqTest
{
  class Program
  {
    private static DaisyLolland model;

    private static List<List<short>> Data;

    private static string workingdir = @"d:\Dropbox\Daisy2";

    public static bool RunNext = true;

    public static void function1_fvec(double[] x, double[] fi, object obj)
    {
      if (model == null)
      {
        model = new DaisyLolland( workingdir, "DaisyModel.dai");

        var ClayColumns = model.DaisyColumns.Columns.Where(c => c.Name.StartsWith("Clay")).ToList();
        foreach (var c in ClayColumns.Skip(2))
          model.DaisyColumns.Columns.Remove(c);

        var SandColumns = model.DaisyColumns.Columns.Where(c => c.Name.StartsWith("Sand")).ToList();
        foreach (var c in SandColumns.Skip(1))
          model.DaisyColumns.Columns.Remove(c);

        model.DaisyColumns.Columns.Remove(model.DaisyColumns.Columns.Single(d => d.Name.ToLower() == "hummingen"));

        model.DaisyColumns.Columns.Remove(model.DaisyColumns.Columns.Single(d => d.Name.ToLower() == "kramnitse"));

      }

      model.SetParams(x);
      if (RunNext)
      {
        model.RunDaisy();
      }
      RunNext = true;
      fi[0] = Math.Pow(1.0- model.Get2Error(720),2) + Math.Pow(1.0-model.GetFBalError(720),2);

      string outs = "";
      foreach (var d in x)
        outs += "P = " + d;

      using (StreamWriter sw = new StreamWriter(Path.Combine(workingdir, "Kalib.log"), true))
      {
        sw.WriteLine("current error: " + fi[0].ToString() + outs);
        sw.WriteLine("FBal: " + model.GetFBalError(720) + "; R2 = " + model.Get2Error(720) + "; RMS = " + model.GetRMSError(720));
      }

      Console.WriteLine("current error: " + fi[0].ToString() + outs);
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
      double[] x = new double[] { 1.1, -1.4, 0.001, 0.009};
      double[] bndl = new double[] { 0.01, -2.5, 0.0001, 0.0001 };
      double[] bndu = new double[] { 10, -0.8, 0.003, 0.1 };
      double epsg = 0.00001;
      double epsf = 0;
      double epsx = 0.01;
      int maxits = 5009;
      alglib.minlmstate state;
      alglib.minlmreport rep;

      alglib.minlmcreatev(1, x, 0.2, out state);
      alglib.minlmsetbc(state, bndl, bndu);
      alglib.minlmsetscale(state, x);
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
