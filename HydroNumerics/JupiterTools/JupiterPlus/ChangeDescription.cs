using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Core;

using HydroNumerics.JupiterTools;

namespace HydroNumerics.JupiterTools.JupiterPlus
{

  public class ChangeDescription
  { 
    public string User { get; set; }
    public string Project { get; set; }
    public DateTime Date { get; set; }

    public List<string> Comments { get; set; }
    public JupiterTables Table { get;  set; }
    public TableAction Action { get; set; }
    public Dictionary<string, string> PrimaryKeys { get; private set; }
    public List<Change> ChangeValues {get; private set;}

    public ChangeDescription(JupiterTables Table)
    {
      Comments = new List<string>();
      this.Table = Table;
      PrimaryKeys = JupiterDescription.GetPrimaryKeys(Table);
      ChangeValues = new List<Change>();
      Date = DateTime.Now;
     
    }


    public void FromXML(XElement ChangeElement)
    {
      User = ChangeElement.Element("User").Value;
      Project = ChangeElement.Element("Project").Value;

      //Could give problems with format
      DateTime d;
      if (DateTime.TryParse(ChangeElement.Element("Date").Value, out d))
        Date = d;

     
    }

    public XElement ToXML()
    {
      XElement ch = new XElement("Change",
        new XElement("User", User),
        new XElement("Project", Project),
        new XElement("Date", Date.ToShortDateString()),
        new XElement("Table", Table.ToString()),
        new XElement("Action", Action.ToString()));

      var el = new XElement("PrimaryKeys");
      foreach (var p in PrimaryKeys)
        el.Add(new XElement("PrimaryKey",
          new XElement("Key",p.Key),
          new XElement("Value", p.Value)));
      ch.Add(el);

      var cvEl = new XElement("ChangedValues");
      foreach (var cv in ChangeValues)
        cvEl.Add(new XElement("ChangedValue",
          new XElement("Column", cv.Column),
          new XElement("NewValue", cv.NewValue),
          new XElement("OldValue", cv.OldValue)));
      ch.Add(cvEl);

      var CommentElement = new XElement("Comments");
      foreach (var c in Comments)
        CommentElement.Add(new XElement("Comment", c));

      ch.Add(CommentElement);

      return ch;
    }

    public override string ToString()
    {
      return ToXML().ToString();
    }

  }
}
