using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Core;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public enum TableAction
  {
    EditValue,
    DeleteRow,
    InsertRow
  }

  public class Change
  {
    public string User { get; set; }
    public string Project { get; set; }
    public DateTime Date { get; set; }
    public List<string> Comments { get; set; }
    public TableAction Action { get; set; }
    public string TableName { get; set; }
    public List<Tuple<string, string>> PrimaryKeys { get; set; }
    public string ColumnName { get; set; }
    public string NewValue { get; set; }
    public string OldValue { get; set; }

    public Change()
    {
      Comments = new List<string>();
      PrimaryKeys = new List<Tuple<string, string>>();
    }

    public XElement ToXML()
    {
      return new XElement("Change",
        new XElement("User", User),
        new XElement("Project", Project),
        new XElement("Date", Date.ToShortDateString()),
        new XElement("Comments", new XElement("Comment",Comments.FirstOrDefault())),
        new XElement("Action", Action.ToString()),
        new XElement("Table", TableName),
        new XElement("Column", ColumnName),
        new XElement("PrimaryKeys",new XElement(PrimaryKeys.First().First,PrimaryKeys.First().Second)),
        new XElement("OldValue", OldValue.ToString()),
        new XElement("NewValue", NewValue.ToString()));
    }
  }
}
