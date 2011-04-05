using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Core;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{

  public class Change
  {
    public string User { get; set; }
    public string Project { get; set; }
    public DateTime Date { get; set; }

    public List<string> Comments { get; set; }
    public JupiterTables Table { get; set; }
    public TableAction Action { get; set; }
    public List<Tuple<string, string>> PrimaryKeys { get; private set; }
    public List<Treple<string,string,string>> ChangeValues {get; private set;}

    public Change()
    {
      Comments = new List<string>();
      PrimaryKeys = new List<Tuple<string, string>>();
      ChangeValues = new List<Treple<string, string, string>>();
    }

    public XElement ToXML()
    {
      XElement ch = new XElement("Change",
        new XElement("Table", Table.ToString()),
        new XElement("Action", Action.ToString()));

      var el = new XElement("PrimaryKeys");
      foreach (var p in PrimaryKeys)
        el.Add(new XElement("PrimaryKey",
          new XElement("Key",p.First),
          new XElement("Value", p.Second)));
      ch.Add(el);

      var cvEl = new XElement("ChangedValues");
      foreach (var cv in ChangeValues)
        cvEl.Add(new XElement("ChangedValue",
          new XElement("Column", cv.First),
          new XElement("NewValue", cv.Second),
          new XElement("OldValue", cv.Third)));
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
