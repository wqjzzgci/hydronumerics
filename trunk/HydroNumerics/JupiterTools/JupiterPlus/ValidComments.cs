using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ValidComments
  {

    public static IList<ICollection<string>> GetValidComments(ChangeDescription change)
    {
      List<ICollection<string>> comments = new List<ICollection<string>>();

      switch (change.Table)
      {
        case JupiterTables.BOREHOLE:
          break;
        case JupiterTables.SCREEN:
          break;
        case JupiterTables.DRWPLANTINTAKE:
          comments.Add(new List<string>() { "Oplysning fra Vandværk","Oplysning fra kommune", "Oplysning fra Miljøcenter", "Skøn" });

          break;
        default:
          break;
      }

      return comments;
    }

    public void WriteValidComments(string FileName)
    {
      XDocument x = new XDocument();

      x.Add(new XElement(JupiterTables.BOREHOLE.ToString(), new XElement("XUTM", new XElement("Lists", new XElement("List", new XElement("ValidString", "Skøn"))))));

      x.Save(FileName);
    }

  }
}
