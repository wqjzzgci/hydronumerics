using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class TestFunctions
  {
    public static void BuildTotalLeachFile(string TemplateFile, string ShapeGrid, string OutputDirectory)
    {
      Dictionary<int, List<int>> SortedPoints = new Dictionary<int, List<int>>();

      using (ShapeReader sr = new ShapeReader(ShapeGrid))
      {
        foreach (var p in sr.GeoData)
        {
          int domain = int.Parse(p.Data["Model_Doma"].ToString());
          int gridcode = int.Parse(p.Data["GRIDID"].ToString());

          List<int> grids;
          if(!SortedPoints.TryGetValue(domain, out grids))
          {
            grids = new List<int>();
            SortedPoints.Add(domain, grids);
          }
          grids.Add(gridcode);
        }
      }

      List<string> lines = new List<string>();

      using (StreamReader sr = new StreamReader(TemplateFile))
      {
        sr.ReadLine();//headline
        var l = sr.ReadLine();
        int gridid = int.Parse(l.Substring(0, l.IndexOf(';')));
        lines.Add(l.Substring(l.IndexOf(';')));

        while (!sr.EndOfStream)
        {
          l = sr.ReadLine();
          int newgridid = int.Parse(l.Substring(0, l.IndexOf(';')));

          lines.Add(l.Substring(l.IndexOf(';')));

          if (newgridid != gridid)
            break;
        }
      }


      foreach (var domain in SortedPoints)
      {
        using (StreamWriter sr = new StreamWriter(Path.Combine(OutputDirectory, "Leaching_area_" + domain.Key + ".txt")))
        {
          foreach (var grid in domain.Value)
          {
            foreach (var year in lines)
            {
              sr.WriteLine(grid.ToString() + year);
            }
          }
        }
      }
    }


  }
}
