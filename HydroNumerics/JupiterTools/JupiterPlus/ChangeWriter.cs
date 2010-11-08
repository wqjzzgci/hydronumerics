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
    XElement _currentChanges;

    public ChangeWriter()
    {
      _changes = new XDocument();
      _changes.Add(new XElement("ChangeItems"));
    }

    public void AddChangeItem(string User, string Project, DateTime TimeOfChange)
    {
      _currentChanges = new XElement("Changes");
      _changes.Element("ChangeItems").Add(new XElement("ChangeItem",
        new XElement("User", User),
        new XElement("Project", Project),
        new XElement("Time", TimeOfChange.ToShortDateString()),
        _currentChanges));
    }

    public void Save(string FileName)
    {
      _changes.Save(FileName);
    }


    public void AddWellX(string WellID, double NewValue)
    {
      GetBoreHoleColumn(WellID).Add( NewValue.ToString(), new XAttribute("Name","XUTM" ));  
    }

    public void AddWellY(string WellID, double NewValue)
    {
      GetBoreHoleColumn(WellID).Add(NewValue.ToString(), new XAttribute("Name", "YUTM"));
    }

    public void AddWellTerrain(string WellID, double NewValue)
    {
      GetBoreHoleColumn(WellID).Add(NewValue.ToString(), new XAttribute("Name", "ELEVATION"));
    }

    private XElement GetBoreHoleColumn(string WellID)
    {
      XElement X = _currentChanges.Elements().FirstOrDefault(var => var.Element("PrimaryKeys").Element("Key").Value == WellID);
      if (X == null)
      {
        X = new XElement("Change", new XElement("Table", "BOREHOLE"), new XAttribute("Action", "AlterRow"),
          new XElement("PrimaryKeys", new XElement("Key", WellID, new XAttribute("Name", "BOREHOLEID"))),
          new XElement("Columns"));
        _currentChanges.Add(X);
      }
      XElement newColumn = new XElement("Column");
      X.Element("Columns").Add(newColumn );
      return newColumn;
      
    }

    public override string ToString()
    {
      return _changes.ToString();
    }


  }
}
