using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using HydroNumerics.Wells;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ChangeReader
  {
    XDocument changes;

    public void ReadFile(string FileName)
    {
      changes = XDocument.Load(FileName);
      XmlSchemaInference infer = new XmlSchemaInference();
      XmlSchemaSet schema = infer.InferSchema(new XmlTextReader("Xchanges.xsd"));
      changes.Validate(schema, null);
    }

    public void ApplyChangesToPlant(Dictionary<int, Plant> Plants)
    {
      IEnumerable<XElement> PlantChanges = changes.Element("ChangeItems").Elements("ChangeItem").Elements("Changes").Elements("Change").Where(var => var.Element("Table").Value == "DRWPLANTINTAKE");

      foreach (var v in PlantChanges)
      {
        Plant P;
        int plantid = int.Parse(v.Element("PrimaryKeys").Elements("Key").Single(var=>var.Attribute("Name").Value =="PLANTID").Value);
        string BoreholeId =v.Element("PrimaryKeys").Elements("Key").Single(var=>var.Attribute("Name").Value =="BOREHOLENO").Value;
        int intakeno = int.Parse(v.Element("PrimaryKeys").Elements("Key").Single(var => var.Attribute("Name").Value == "INTAKENO").Value);


        if (Plants.TryGetValue(plantid, out P))
        {
          var I = P.PumpingIntakes.Single(var => var.Intake.IDNumber == intakeno && var.Intake.well.ID == BoreholeId);
          P.PumpingIntakes.Remove(I);
        }
      }
    }


    public void ApplyChangeToWells(IWellCollection Wells)
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
