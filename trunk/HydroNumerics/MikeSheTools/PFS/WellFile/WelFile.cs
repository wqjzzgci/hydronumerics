using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.WellFile
{
  public class WelFile:PFSFile 
  {
    public WELLDATA WELLDATA { get; private set; }

    public WelFile(string Sim11FileName)
      : base(Sim11FileName)
    {
      WELLDATA = new WELLDATA(_pfsClass.GetTarget("WEL_CFG", 1).GetSection("WELLDATA", 1));
    }
 }
}
