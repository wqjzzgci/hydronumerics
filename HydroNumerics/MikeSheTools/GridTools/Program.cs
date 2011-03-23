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
        GridOperation Gop = (GridOperation)Enum.Parse(typeof(GridOperation), Op.FirstAttribute.Value.ToString());

        switch (Gop)
        {
          case GridOperation.LayerSummation:
            break;
          case GridOperation.FactorMath:
            break;
          case GridOperation.MonthlyMath:
            break;
          case GridOperation.GridMath:
            break;
          case GridOperation.TimeSummation:
            break;
          default:
            break;
        }



      }


    }
  }
}
