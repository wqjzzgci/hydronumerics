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
    public XElement WellX(string WellID, Change<double> change)
    {
      XElement c = BoreHoleChange(WellID);
      c.Add(new XElement("Property", "XUTM"),
        change.ToXML());
      return c;
    }

    public XElement WellY(string WellID, Change<double> change)
    {
      XElement c = BoreHoleChange(WellID);
      c.Add(new XElement("Property", "YUTM"),
        change.ToXML());
      return c;
    }

    public XElement WellTerrain(string WellID, Change<double> change)
    {
      XElement c = BoreHoleChange(WellID);
      c.Add(new XElement("Property", "ELEVATION"),
        change.ToXML());
      return c;
    }

    private XElement BoreHoleChange(string WellID)
    {
      XElement c = Change();
      c.Add(new XElement("Table", "BOREHOLE"),
       new XElement("IdentifierKeys", new XElement("Key", WellID)));
       return c;
    }

    private XElement Change()
    {
      return new XElement("Change");
    }


  }
}
