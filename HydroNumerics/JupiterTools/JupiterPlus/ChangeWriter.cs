using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.JupiterTools;


namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ChangeWriter
  {
    XDocument _changes;

    public ChangeWriter()
    {
      _changes = new XDocument();
      _changes.Add(new XElement("Changes"));
    }

    public void Save(string FileName)
    {
      _changes.Save(FileName);
      }

    public void AddWellX(string WellID, Change<double> change)
    {
      _changes.Element("Changes").Add(WellX(WellID, change));
    }


    public XElement WellX(string WellID, Change<double> change)
    {
      XElement c = BoreHoleChange(WellID, "XUTM", change);
      return c;
    }

    public XElement WellY(string WellID, Change<double> change)
    {
      XElement c = BoreHoleChange(WellID,"YUTM", change);
      return c;
    }

    public XElement WellTerrain(string WellID, Change<double> change)
    {
      XElement c = BoreHoleChange(WellID, "ELEVATION", change);
      return c;
    }

    private XElement BoreHoleChange(string WellID, string PropertyName, Change<double> change)
    {
      XElement c = ChangeElement();
      c.Add(new XElement("Table", "BOREHOLE"),
       new XElement("PrimaryKeys", new XElement("Key", WellID)),
       new XElement("Column", PropertyName),
        change.ToXML());
       
       return c;
    }

    private XElement ChangeElement()
    {
      return new XElement("Change");
    }


  }
}
