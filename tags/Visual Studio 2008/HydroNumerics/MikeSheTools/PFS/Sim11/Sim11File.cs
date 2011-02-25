using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.Sim11
{
  public class Sim11File:PFSFile 
  {
    public Input FileNames { get; private set; }

    public Sim11File(string Sim11FileName):base(Sim11FileName)
    {
      FileNames = new Input(_pfsClass.GetTarget(1).GetSection("Input", 1));
    }
 }
}
