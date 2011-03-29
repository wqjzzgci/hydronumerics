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
            break;
          case "MonthlyMath":
            break;
          case "GridMath":
            break;
          case "TimeSummation":
            break;
          default:
            break;
        }



      }


    }
  }
}
