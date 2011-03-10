using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Core;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public enum Action
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
    public Action Action { get; set; }
    public List<Tuple<string, string>> PrimaryKeys { get; set; }
    public string ColumnName { get; set; }
    public string NewValue { get; set; }
    public string OldValue { get; set; }

    public Change()
    {
      Comments = new List<string>();
      PrimaryKeys = new List<Tuple<string, string>>();
    }

    public XElement[] ToXML()
    {
      return new XElement[]{
        new XElement("User", User),
        new XElement("Project", Project),
        new XElement("Date", Date.ToShortDateString()),
        new XElement("OldValue", OldValue.ToString()),
        new XElement("NewValue", NewValue.ToString())};
    }
  }
}
