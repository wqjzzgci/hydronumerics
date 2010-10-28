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
      IEnumerable<XElement> WellChanges = changes.Element("ChangeItems").Elements("ChangeItem").Elements("Changes").Elements("Change").Where(var => var.Element("Table").Value == "BOREHOLE");

      foreach (var c in WellChanges)
      {
        string id = c.Element("PrimaryKeys").Element("Key").Value;
        IWell W = Wells[id];
        foreach (var cc in c.Element("Columns").Elements())
        switch (cc.Attribute("Name").Value)
        {
          case "XUTM":
            W.X = Double.Parse(cc.Value);
            break;
          case "YUTM":
            W.Y = Double.Parse(cc.Value);
            break;
          case "ELEVATION":
            W.Terrain = Double.Parse(cc.Value);
            break;
          default:
            break;
        }
        break;
      }

    }

    public override string ToString()
    {
      return changes.ToString();
    }
  }
}
