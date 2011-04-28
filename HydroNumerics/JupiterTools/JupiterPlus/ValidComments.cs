using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
  }
}
