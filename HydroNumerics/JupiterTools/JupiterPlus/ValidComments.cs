using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ValidComments
  {
    static XElement tables;

    static ValidComments()
    {
      string fullPath = System.Reflection.Assembly.GetAssembly(typeof(ValidComments)).Location;

      //get the folder that's in
      string theDirectory = Path.GetDirectoryName(fullPath);
      tables = XDocument.Load(Path.Combine(theDirectory,"ValidComments.xml")).Element("Tables");
    }

    public static IList<ICollection<string>> GetValidComments(ChangeDescription change)
    {
      List<ICollection<string>> comments = new List<ICollection<string>>();
      var el = tables.Element(change.Table.ToString());
      if (el == null)
        return comments;
       el =el.Element(change.Action.ToString());
      if (el == null)
        return comments;


      if (change.Action == TableAction.EditValue)
      {
        el = el.Element(change.ChangeValues.First().Column);
      }

        foreach (var v in el.Elements("ValidComments"))
        {
          List<string> sublist = new List<string>();

          foreach (var s in v.Elements("ValidComment"))
            sublist.Add(s.Value);

          comments.Add(sublist);
        }

      return comments;
    }

    public void WriteValidComments(string FileName)
    {
      XDocument x = new XDocument();

      var el = new XElement("Tables", new XElement(JupiterTables.BOREHOLE.ToString(),
      new XElement(TableAction.EditValue.ToString(),
      new XElement("Columns", new XElement("UMTX",
      new XElement("ValidComments", new XElement("ValidComment", "Skøn")))))));
      x.Add(el);
      x.Save(FileName);
    }

  }
}
