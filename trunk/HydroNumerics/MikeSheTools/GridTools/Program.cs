using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GridTools
{
  class Program
  {
    static void Main(string[] args)
    {

      bool stay = true;

      while (stay)
      {
        stay = false;
        foreach (var v in System.Diagnostics.Process.GetProcesses())
        {
          if (v.Id != System.Diagnostics.Process.GetCurrentProcess().Id)
          {
            if (v.ProcessName.ToLower().StartsWith("gridtools"))
            {
              if (v.UserProcessorTime > TimeSpan.FromSeconds(0.2))
              {
                stay = true;
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
                break;
              }
            }
          }
        }
      }



      try
      {
        XDocument xd = XDocument.Load(args[0]);
        IEnumerable<XElement> Operations = xd.Element("GridOperations").Elements();

        foreach (var Op in Operations)
        {
          string OperationType = Op.FirstAttribute.Value;

          switch (OperationType)
          {
            case "LayerSummation":
              GridFunctions.LayerSummation(Op);
              break;
            case "FactorMath":
              GridFunctions.FactorMath(Op);
              break;
            case "MonthlyMath":
              GridFunctions.MonthlyMath(Op);
              break;
            case "GridMath":
              GridFunctions.GridMath(Op);
              break;
            case "TimeSummation":
              GridFunctions.TimeSummation(Op);
              break;
            case "TimeAverage":
              GridFunctions.TimeAverage(Op);
              break;
            case "TimeMin":
              GridFunctions.TimeMin(Op);
              break;
            case "TimeMax":
              GridFunctions.TimeMax(Op);
              break;
            case "Percentile":
              GridFunctions.Percentile(Op);
              break;
            case "InsertPointValues":
              GridFunctions.InsertPointValues(Op);
              break;
            default:
              break;
          }
        }
      }
      catch (System.IO.FileNotFoundException FE)
      {
        Console.WriteLine("Could not find: " + FE.FileName);
      }
    }
  }
}

