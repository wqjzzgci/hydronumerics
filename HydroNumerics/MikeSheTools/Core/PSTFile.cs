using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.MikeSheTools.Core
{
  public class PSTFile:FileClass
  {
    public List<CalibrationParameter> Parameters { get; private set; }
    public List<ParameterGroup> Groups { get; private set; }

    private StringBuilder Upper = new StringBuilder();
    private StringBuilder Lower = new StringBuilder();


    public PSTFile(string FileName):base(FileName)
    {
      Parameters = new List<CalibrationParameter>();
      Groups = new List<ParameterGroup>();
      Load();
    }

    private void Load()
    {
      int noOfParameters;

      using (StreamReader sr = new StreamReader(FileName))
      {
        Upper.AppendLine(sr.ReadLine());
        Upper.AppendLine(sr.ReadLine());
        Upper.AppendLine(sr.ReadLine());
        string s = sr.ReadLine();
        Upper.AppendLine(s);
        noOfParameters = int.Parse(s.Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0]);
        
        while ((s = sr.ReadLine())!="* parameter data")
          Upper.AppendLine(s);

        Upper.AppendLine(s);

        for (int i = 0; i < noOfParameters; i++)
        {
          string[] splitted = sr.ReadLine().Split(new string[] { "\t" }, StringSplitOptions.None);
          CalibrationParameter cp = new CalibrationParameter();
          cp.ShortName = splitted[0];

          ParameterType p;
           if (Enum.TryParse<ParameterType>(splitted[1], true, out p ))
             cp.ParType = p;
           cp.ParChgLim = splitted[2];
           cp.CurrentValue = double.Parse(splitted[3]);
           cp.MinValue = double.Parse(splitted[4]);
           cp.MaxValue = double.Parse(splitted[5]);

           var g = Groups.FirstOrDefault(var => var.Name.ToLower() == splitted[6].ToLower());
           if (g == null)
           {
             g = new ParameterGroup();
             g.Name = splitted[6];
             Groups.Add(g);
           }
           cp.Group = g;

          cp.Scale = double.Parse(splitted[7]);
          cp.Offset = double.Parse(splitted[8]);
          cp.Dercom = double.Parse(splitted[9]);
          Parameters.Add(cp);
        }

        for (int i=0;i< Parameters.Count(var=>var.ParType == ParameterType.tied);i++)
        {
          string[] splitted = sr.ReadLine().Split(new string[] { " ","\t" }, StringSplitOptions.None);
          var cp = Parameters.Single(var => var.ShortName == splitted[0]);
          cp.TiedTo = Parameters.Single(var => var.ShortName == splitted[1]);
        }

        Lower.Append(sr.ReadToEnd());
      }
    }
    public override void Save()
    {
      using (StreamWriter sw = new StreamWriter(FileName))
      {
        sw.Write(Upper.ToString());

        foreach (var P in Parameters)
        {
          StringBuilder tostring = new StringBuilder();
          tostring.Append(P.ShortName + "\t");
          tostring.Append(P.ParType + "\t");
          tostring.Append(P.ParChgLim + "\t");
          tostring.Append(P.CurrentValue + "\t");
          tostring.Append(P.MinValue + "\t");
          tostring.Append(P.MaxValue + "\t");
          tostring.Append(P.Group.Name + "\t");
          tostring.Append(P.Scale + "\t");
          tostring.Append(P.Offset + "\t");
          tostring.Append(P.Dercom );
          sw.WriteLine(tostring.ToString());
        }
        foreach (var P in Parameters.Where(var=>var.ParType== ParameterType.tied))
        {
          sw.WriteLine(P.ShortName + " " + P.TiedTo.ShortName);
        }

        sw.Write(Lower.ToString());
      }

    }

  }
}
