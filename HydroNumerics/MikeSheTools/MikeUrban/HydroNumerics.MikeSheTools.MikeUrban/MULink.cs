using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.PFS.MEX;


namespace HydroNumerics.MikeSheTools.MikeUrban
{
  public class MULink
  {

    public MUNode UpstreamNode { get; set; }

    public MUNode DownstreamNode { get; set; }

    public Link pfslink { get; private set; }

    public Cross_Section Xsec { get; set; }

    public MULink(Link L)
    {
      pfslink = L;
    }


    public override string ToString()
    {
      return pfslink.LinkID;
    }

    public override bool Equals(object obj)
    {
      return pfslink.LinkID == ((MULink)obj).pfslink.LinkID;
    }

    public override int GetHashCode()
    {
      return pfslink.LinkID.GetHashCode();
    }


  }
}
