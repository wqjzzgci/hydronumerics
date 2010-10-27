using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Wells;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ChangeReader
  {
    XDocument changes;

    public void ReadFile(string FileName)
    {
      changes = XDocument.Load(FileName);

    }


    public void ApplyChangeToWells(Dictionary<string, IWell> Wells)
    {
      IEnumerable<XElement> WellChanges = changes.Elements("Changes").Elements("Change");

      foreach (var c in WellChanges)
      {
        switch (c.Element("Table").Value)
        {
          case "BOREHOLE":
            string id = c.Element("PrimaryKeys").Element("Key").Value;
            IWell W = Wells[id];
            switch (c.Element("Column").Value)
            {
              case "XUTM":
                W.X = Double.Parse(c.Element("NewValue").Value);
                break;
              case "YUTM":
                W.Y = Double.Parse(c.Element("NewValue").Value);
                break;
              case "TERRAIN":
                W.Terrain = Double.Parse(c.Element("NewValue").Value);
                break;
              default:
                break;
            }
            break;
          default:
            break;
        }
      }
    }

  }
}
