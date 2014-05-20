using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class Gener
  {

    public List<DeliverabilityWell> Delvs = new List<DeliverabilityWell>();


    public void ReadFromStream(StreamReader Input)
    {
      string sr = "";
      while ((sr = Input.ReadLine().TrimEnd()) != string.Empty)
      {
        string Element = sr.Substring(0, 5);
        string Type = sr.Substring(35, 4);
        double par1 = double.Parse( sr.Substring(40, 10));
        double par2 = double.Parse(sr.Substring(50, 10));

        if (Type == "DELV")
          Delvs.Add(new DeliverabilityWell(){Eleme = new Element(Element,1,1), Resistance = par1, Pressure =par2});
      }
    }

    public override string ToString()
    {
      StringBuilder Output = new StringBuilder();
      Output.AppendLine("GENER----1----*----2----*----3----*----4----*----5----*----6----*----7----*----8");

      foreach (var W in Delvs)
      {
        Output.AppendLine(W.ToString());
      }
      Output.AppendLine();
      return Output.ToString();
    }
  }
}
