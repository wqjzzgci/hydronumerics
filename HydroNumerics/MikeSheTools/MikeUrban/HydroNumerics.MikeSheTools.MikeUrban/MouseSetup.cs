using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.MikeSheTools.PFS.MEX;

namespace HydroNumerics.MikeSheTools.MikeUrban
{
  public class MouseSetup : FileClass
  {
    private MexFile data;

    public Dictionary<string, MUNode> Nodes = new Dictionary<string, MUNode>();
    public Dictionary<string, MULink> Links = new Dictionary<string, MULink>();
    public Dictionary<string, Cross_Section> Xsecs = new Dictionary<string, Cross_Section>();

    public MouseSetup(String FileName)
      : base(FileName)
    {
      data = new MexFile(FileName);

      foreach (var n in data.MOUSE_NODES.Nodes)
      {
        Nodes.Add(n.NodeID, new MUNode(n));
      }

      foreach (var xsec in data.MOUSE_CROSS_SECTIONS.Cross_Sections)
      {
        Xsecs.Add(xsec.CRSID1, xsec);
      }

      foreach (var l in data.MOUSE_LINKS.Links)
      {
        MULink newlink = new MULink(l);
        Links.Add(l.LinkID, newlink);

        newlink.UpstreamNode = Nodes[newlink.pfslink.FromNode];
        newlink.DownstreamNode = Nodes[newlink.pfslink.ToNode];

        Nodes[newlink.UpstreamNode.pfsnode.NodeID].Links.Add(newlink);
        Nodes[newlink.DownstreamNode.pfsnode.NodeID].Links.Add(newlink);

        Cross_Section xsec;
        if(Xsecs.TryGetValue(l.CrsID, out xsec))
          newlink.Xsec = xsec;
      }
    }

    public List<XYPolyline> Branches;
    Dictionary<string, MULink> RemainingLinks;

    public void CreateBranches()
    {
      Branches = new List<XYPolyline>();

      RemainingLinks = new Dictionary<string, MULink>(Links); 

      //Move from most upstream
      foreach (var l in Links.Values.Where(l => l.UpstreamNode.Links.Count == 1))
      {
        TraverseLink(l);
      }


      while (RemainingLinks.Count > 0)
      {
        foreach (var l in RemainingLinks.Values.ToList().Where(l => l.UpstreamNode.Links.Count > 2))
        {
          TraverseLink(l);


        }
      }



    }
    private void TraverseLink(MULink link)
    {

      XYPolyline branch = new XYPolyline();

      MUNode NextNode = link.DownstreamNode;
      MULink NextLink = link;
      branch.Points.Add(link.UpstreamNode.Location);

      while (NextNode != null)
      {
        RemainingLinks.Remove(NextLink.pfslink.LinkID);
        branch.Points.Add(NextNode.Location);
        if (NextNode.Links.Count == 2)
        {
          NextLink = NextNode.Links.First(LL => LL != NextLink);
          if (NextLink.DownstreamNode != NextNode) //To avoid loop when two branches end in the same point
            NextNode = NextLink.DownstreamNode;
          else
          {
            NextNode = null;
          }
        }
        else if (NextNode.Links.Count > 2)
        {
          var n = NextNode.Links.Where(L => L != NextLink).FirstOrDefault(L => L.Xsec != null && NextLink.Xsec != null && L.Xsec.CRSID1.StartsWith(NextLink.Xsec.CRSID1.Substring(0, 4)));
          if (n != null)
          {
            NextLink = n;
            NextNode = NextLink.DownstreamNode;
          }
          else
            NextNode = null;
        }
        else
        {
          branch.Points.Add(NextNode.Links[0].DownstreamNode.Location);
          NextNode = null;
        }
      }
      Branches.Add(branch);

    }



    public void SaveToShape(string ShapeFileName)
    {
      using (ShapeWriter sw = new ShapeWriter(ShapeFileName))
      {
        DataTable dt = new DataTable();
        dt.Columns.Add("LinkID", typeof(string));
        dt.Columns.Add("FromNode", typeof(string));
        dt.Columns.Add("ToNode", typeof(string));
        dt.Columns.Add("SpecifiedLength", typeof(double));

        foreach (var b in Links.Values)
        {
          GeoRefData grf = new GeoRefData();
          var l = new XYPolyline();
          l.Points.Add(new XYPoint(b.UpstreamNode.pfsnode.X, b.UpstreamNode.pfsnode.Y));
          l.Points.Add(new XYPoint(b.DownstreamNode.pfsnode.X, b.DownstreamNode.pfsnode.Y));
          grf.Geometry = l;
          grf.Data = dt.NewRow();
          grf.Data[0] = b.pfslink.LinkID;
          grf.Data[1] = b.pfslink.FromNode;
          grf.Data[2] = b.pfslink.ToNode;
          grf.Data[3] = b.pfslink.SpecifiedLength;
          sw.Write(grf);
        }
      }

      if (Branches != null && Branches.Count > 0)
      {
        using (ShapeWriter sw = new ShapeWriter(Path.Combine(Path.GetDirectoryName(ShapeFileName), Path.GetFileNameWithoutExtension(ShapeFileName)+"_branches.shp")))
        {
          DataTable dt = new DataTable();
          dt.Columns.Add("Length", typeof(double));

          foreach (var b in Branches)
          {
            GeoRefData grf = new GeoRefData();
            grf.Geometry = b;
            grf.Data = dt.NewRow();
            grf.Data[0] = b.GetLength();
            
            sw.Write(grf);


          }



        }

      }
    }
  }
}
