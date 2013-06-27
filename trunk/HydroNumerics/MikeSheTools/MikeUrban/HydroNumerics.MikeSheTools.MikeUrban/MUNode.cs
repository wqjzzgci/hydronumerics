using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;
using HydroNumerics.MikeSheTools.PFS.MEX;

namespace HydroNumerics.MikeSheTools.MikeUrban
{
  public class MUNode
  {

    public List<MULink> Links { get; private set; }

    public Node pfsnode { get; private set; }

    public MUNode(Node N)
    {
      Links = new List<MULink>();
      pfsnode = N;
      Location = new XYPoint(pfsnode.X, pfsnode.Y);
    }

    public XYPoint Location { get; private set; }


    public override string ToString()
    {
      return pfsnode.NodeID;
    }

    public override bool Equals(object obj)
    {
      return pfsnode.NodeID == ((MUNode)obj).pfsnode.NodeID;
    }

    public override int GetHashCode()
    {
      return pfsnode.NodeID.GetHashCode();
    }

  }
}
