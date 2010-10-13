using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class Change<T>
  {
    public string User { get; set; }
    public string Project { get; set; }
    public DateTime Date { get; set; }
    public T NewValue { get; set; }
    public T OldValue { get; set; }

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
