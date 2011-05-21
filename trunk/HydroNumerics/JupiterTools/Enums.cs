using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.JupiterTools
{

  public enum JupiterTables
  {
    BOREHOLE,
    SCREEN,
    DRWPLANTINTAKE,
    WATLEVEL,
  }

  public class JupiterDescription
  {
    public static Dictionary<string, string> GetPrimaryKeys(JupiterTables Table)
    {
      Dictionary<string, string> pks = new Dictionary<string, string>();

      switch (Table)
      {
        case JupiterTables.BOREHOLE:
          pks.Add("BOREHOLENO", "");
          break;
        case JupiterTables.SCREEN:
          pks.Add("BOREHOLENO", "");
          pks.Add("SCREENNO", "");
          break;
        case JupiterTables.DRWPLANTINTAKE:
          pks.Add("INTAKEPLANTID", "");
          break;
        default:
          break;
      }

      return pks;
    }



  }

 
}
