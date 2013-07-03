using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.MikeUrban
{
  public class MuBranch
  {
    public string Name { get; set; }
    public List<MULink> Links { get; set; }

    public MuBranch()
    {
      Links = new List<MULink>();
    }

    public double GetChainage(MULink link)
    {
      int i=0;

      double chainage=0;
      while (Links[i] != link)
      {
        chainage += Links[i].Length;
        i++;
      }
      return chainage;

    }
  }
}
