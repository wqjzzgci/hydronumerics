using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroNumerics.MikeSheTools.MikeUrban
{
  class Program
  {
    static void Main(string[] args)
    {
      MouseSetup ms = new MouseSetup(args[0]);

      ms.CreateBranches();
      ms.SaveToShape(Path.Combine(Path.GetDirectoryName(args[0]), "setup.shp"));
      ms.SaveToMike11(Path.Combine(Path.GetDirectoryName(args[0]),"m11.nwk11"));
    }
  }
}
