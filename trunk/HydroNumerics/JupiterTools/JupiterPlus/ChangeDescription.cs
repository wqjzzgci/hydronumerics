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
      this.Table = Table;
      PrimaryKeys = JupiterDescription.GetPrimaryKeys(Table);
      ChangeValues = new List<Change>();
      Comments = new List<string>();
      Date = DateTime.Now;
    }


    public ChangeDescription(XElement ChangeElement)
    {
      PrimaryKeys = new Dictionary<string, string>();
      ChangeValues = new List<Change>();
      Comments = new List<string>();

      User = ChangeElement.Element("User").Value;
      Project = ChangeElement.Element("Project").Value;

      //Could give problems with format
      DateTime d;
      if (DateTime.TryParse(ChangeElement.Element("Date").Value, out d))
        Date = d;

      Table = (JupiterTables)Enum.Parse(typeof(JupiterTables), ChangeElement.Element("Table").Value);

      Action = (TableAction) Enum.Parse(typeof(TableAction), ChangeElement.Element("Action").Value);

      foreach (var ele in ChangeElement.Element("PrimaryKeys").Elements())
      {
        PrimaryKeys.Add(ele.Element("Key").Value, ele.Element("Value").Value);
      }

      foreach (var ele in ChangeElement.Element("ChangedValues").Elements())
      {
        Change c = new Change(ele.Element("Column").Value, ele.Element("NewValue").Value, ele.Element("OldValue").Value);
        this.ChangeValues.Add(c);
      }

      foreach (var ele in ChangeElement.Element("Comments").Elements())
      {
        Comments.Add(ele.Value);
      }
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
