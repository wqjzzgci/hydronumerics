using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Core
{
  public class SimlabFile
  {

    public SortedList<CalibrationParameter, double[]> Samples = new SortedList<CalibrationParameter, double[]>();

    public string FileName { get; private set; }

    public void Load(string FileName)
    {
      this.FileName = FileName;
      using (StreamReader sr = new StreamReader(FileName))
      {
        sr.ReadLine();
        int noofsamples = int.Parse(sr.ReadLine().Trim());
        int noofpars = int.Parse(sr.ReadLine().Trim());
        sr.ReadLine();
        List<double[]> data = new List<double[]>();

        for (int i = 0; i < noofpars; i++)
        {
          data.Add(new double[noofsamples]);
        }

        for (int i = 0; i < noofsamples; i++)
        {
          var splits = sr.ReadLine().Split(new string[] { "\t" }, StringSplitOptions.None);
          for (int j = 0; j < noofpars; j++)
            data[j][i] = double.Parse(splits[j]);
        }

        int k = 0;
        while (!sr.EndOfStream && sr.ReadLine().ToLower() != "Default Truncations:".ToLower()) ;

        if (!sr.EndOfStream)
        {
          sr.ReadLine();
          sr.ReadLine();
          sr.ReadLine();

          for (int i = 0; i < noofpars; i++)
          {
            sr.ReadLine();
            CalibrationParameter par = new CalibrationParameter();
            par.ShortName = sr.ReadLine().Trim();
            Samples.Add(par, data[i]);
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
          }
        }
      }
    }
  }
}
