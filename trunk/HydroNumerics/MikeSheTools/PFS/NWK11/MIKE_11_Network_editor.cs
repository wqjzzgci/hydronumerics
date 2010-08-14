using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public class MIKE_11_Network_editor : PFSMapper
  {
    public List<Point> Points { get; private set; }
    public List<Branch> Branches { get; private set; }

    internal MIKE_11_Network_editor(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);
        switch (sub.Name)
        {
          case "POINTS":
            int np = sub.GetKeywordsNo("point");
            Points = new List<Point>();
            for (int j = 1; j <= np; j++)
            {
              Points.Add(new Point(sub.GetKeyword(j)));
            }
            break;
          case "BRANCHES":
            int nb = sub.GetSectionsNo("branch");
            Branches = new List<Branch>();
            for (int j = 1; j <= nb; j++)
            {
              Branches.Add(new Branch(sub.GetSection(j)));
            }
            break;

          default:
            _unMappedSections.Add(sub.Name);
            break;

        }
      }
    }
  }
}
