using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic;
using DHI.Mike1D.CrossSections;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class M11Setup
  {
    public Network network { get; private set; }

    public void ReadNetwork(string Nwk11FileName)
    {
      network = new Network(Nwk11FileName);
    }

    public void ReadCrossSections(string xnsFile)
    {
      CrossSectionCollection csc = new CrossSectionCollection();
      csc.Connection.FilePath = xnsFile;
      csc.Connection.Bridge = csc.Connection.AvailableBridges[0];

      foreach (var cs in csc.CrossSections)
      {

        CrossSection MyCs = new CrossSection(cs);
        M11Branch mb = network.Branches.FirstOrDefault(var => var.Name == MyCs.BranchName & var.TopoID == MyCs.TopoID & var.ChainageStart < MyCs.Chainage & var.ChainageEnd > MyCs.Chainage);
        if (mb != null)
          mb.AddCrossection(MyCs);

        
      }
      
    }
  }
}
