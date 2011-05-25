using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class Rocks : KeyedCollection<string, Rock>
  {
    public Rocks() : base() { }

    protected override string GetKeyForItem(Rock item)
    {
      // In this example, the key is the part number.
      return item.Name;
    }



    public override string ToString()
    {
      StringBuilder Output = new StringBuilder();


      Output.AppendLine("ROCKS----1----*----2----*----3----*----4----*----5----*----6----*----7----*----8");

      foreach (var r in this)
      {
        Output.Append(r.Name);
        int writemore = 0;
        if (r.RelativePermeabilityModel > 0)
          if (r.CapillaryPressureModel > 0)
          {
            writemore = 2;
            Output.Append("    2");
          }
          else
          {
            writemore = 1;
            Output.Append("    1");
          }
        else
          Output.Append("    0");

        Output.AppendLine(ReaderUtilities.JoinIntoString(new double[] { r.Density, r.Porosity, r.PermX, r.PermY, r.PermZ, r.WetHeatConductivity, r.HeatCapacity }, 10));

        Output.AppendLine(ReaderUtilities.JoinIntoString(new double[] { r.Compressibility }, 10));
        if (writemore > 0)
        {
          if (r.RelativePermeabilityModel > 9)
            Output.Append("   ");
          else
            Output.Append("    ");
          Output.Append(r.RelativePermeabilityModel);
          Output.Append("     ");
          Output.AppendLine(ReaderUtilities.JoinIntoString(r.RelativePermebilityParameters, 10));
          
        }

        if (writemore>1)
        {
          if (r.CapillaryPressureModel > 9)
            Output.Append("   ");
          else
            Output.Append("    ");
          Output.Append(r.CapillaryPressureModel);
          Output.Append("     ");
          Output.AppendLine(ReaderUtilities.JoinIntoString(r.CapillaryPressureParameters, 10));
        }

      }
      Output.AppendLine();
      Output.AppendLine();
      return Output.ToString();
    }

    public void ReadFromStream(StreamReader Input)
    {
      string sr = "";
      while ((sr = Input.ReadLine().TrimEnd()) != string.Empty)
      {

        Rock rock = new Rock();
        rock.Name = sr.Substring(0, 5);
        int ReadMore = int.Parse(sr.Substring(5, 5));
        rock.Density = Double.Parse(sr.Substring(10, 10));
        rock.Porosity = Double.Parse(sr.Substring(20, 10));
        rock.PermX = Double.Parse(sr.Substring(30, 10));
        rock.PermY = Double.Parse(sr.Substring(40, 10));
        rock.PermZ = Double.Parse(sr.Substring(50, 10));
        rock.WetHeatConductivity = Double.Parse(sr.Substring(60, 10));
        rock.HeatCapacity = Double.Parse(sr.Substring(70, 10));

        sr = Input.ReadLine();
        rock.Compressibility = Double.Parse(sr.Substring(0, 10));

        if (ReadMore >= 1)
        {
          sr = Input.ReadLine().TrimEnd();
          rock.RelativePermeabilityModel = int.Parse(sr.Substring(0, 5));
          rock.RelativePermebilityParameters = ReaderUtilities.SplitIntoDoubles(sr,10,10);
        }
        if (ReadMore >= 2)
        {
          sr = Input.ReadLine().TrimEnd();
          rock.CapillaryPressureModel = int.Parse(sr.Substring(0, 5));
          rock.CapillaryPressureParameters = ReaderUtilities.SplitIntoDoubles(sr,10, 10);
        }

        this.Add(rock);

      }
    }
  }
}
