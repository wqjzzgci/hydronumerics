using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using DHI.Mike1D.CrossSections;

using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.MikeSheTools.PFS.MEX;
using HydroNumerics.MikeSheTools.PFS.NWK11;
using HydroNumerics.MikeSheTools.DFS;

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
        Xsecs.Add(xsec.CRSID, xsec);
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

    public List<MuBranch> Branches;

    Dictionary<string, MULink> RemainingLinks;

    public void CreateBranches()
    {
      Branches = new List<MuBranch>();

      RemainingLinks = new Dictionary<string, MULink>(Links); 

      //Move from most upstream
      foreach (var l in Links.Values.Where(l => l.UpstreamNode.Links.Count == 1))
      {
        TraverseLink(l);
      }

      foreach (var b in Branches.OrderByDescending(b => b.Links.Count).ToList())
      {
        MULink nextlink;
        while ((nextlink = b.Links.Last().DownstreamNode.Links.FirstOrDefault(l => l.UpstreamNode == b.Links.Last().DownstreamNode)) != null)
        {
          if (!RemainingLinks.ContainsKey(nextlink.pfslink.LinkID))
            break;
          b.Links.Add(nextlink);
          RemainingLinks.Remove(nextlink.pfslink.LinkID);
        }
      }

      
      while (RemainingLinks.Count > 0)
      {
        foreach (var l in RemainingLinks.Values.ToList().Where(l => l.UpstreamNode.Links.Count > 2))
        {
          TraverseLink(l);
        }
      }

      for (int i = 0; i < Branches.Count; i++)
        Branches[i].Name = "Br" + i;

    }

    private void TraverseLink(MULink link)
    {
      List<MULink> branch = new List<MULink>();
      Branches.Add(new MuBranch() { Links = branch });

      MUNode NextNode = link.DownstreamNode;
      MULink NextLink = link;

      while (NextNode != null)
      {
        RemainingLinks.Remove(NextLink.pfslink.LinkID);
        branch.Add(NextLink);
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
          var n = NextNode.Links.Where(L => L != NextLink).FirstOrDefault(L => L.Xsec != null && NextLink.Xsec != null && L.Xsec.CRSID.StartsWith(NextLink.Xsec.CRSID.Substring(0, 4)));
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
          NextNode = null;
        }
      }
    }


    public void SaveToMike11(string m11name)
    {
      NWK11File nwk = new NWK11File();

      double x0 = double.MaxValue;
      double x1 = double.MinValue;
      double y0 = double.MaxValue;
      double y1 = double.MinValue;

      int pointcount =1;

      //This is necessary because it fails if DHI.CrossSection.Dll tries to load UFS.dll
      DFS0 d = new DFS0(@"v");
      d.Dispose();



      CrossSectionCollection csc = new CrossSectionCollection();
      csc.Connection.FilePath = Path.ChangeExtension(m11name, ".xns11");
      csc.Connection.Bridge = csc.Connection.AvailableBridges[0];


      foreach (var b in Branches)
      {
        var newbranch = nwk.MIKE_11_Network_editor.BRANCHES.AddBranch();
        newbranch.definitions.Par1 = b.Name;

        double lastchainage = 0;
        for (int i = 0; i < b.Links.Count; i++)
        {
          var bp = nwk.MIKE_11_Network_editor.POINTS.AddPoint();
          bp.Par1 = pointcount;
          bp.Par2 = b.Links[i].UpstreamNode.Location.X;
          bp.Par3 = b.Links[i].UpstreamNode.Location.Y;

          x0 = Math.Min(b.Links[i].UpstreamNode.Location.X, x0);
          x1 = Math.Max(b.Links[i].UpstreamNode.Location.X, x1);
          y0 = Math.Min(b.Links[i].UpstreamNode.Location.Y, y0);
          y1 = Math.Max(b.Links[i].UpstreamNode.Location.Y, y1);

          if (i == 0)
          {
            bp.Par4 = 1;
          }
          else
          {
            bp.Par4 = 0;
            lastchainage += b.Links[i - 1].Length;
          }
          bp.Par5 = lastchainage;
          newbranch.points.AddValue(pointcount);


          //CrossSections

          CrossSection cs = new CrossSection(new DHI.Generic.RouteLocation(b.Name, "Topo-id", lastchainage, b.Links[i].pfslink.Par5));
          if (b.Links[i].Xsec != null && b.Links[i].Xsec.TypeNo == 4)
          {
            double bottom = double.MaxValue;
            int bottomindex = 0;
            int index = 0;
            foreach (var dat in b.Links[i].Xsec.Datas)
            {
              var z = dat.GetValue(1);
              if (z < bottom)
              {
                bottom = z;
                bottomindex = index;
              }
              cs.Points.AddPoint(new CrossSectionPoint(dat.GetValue(0), z));
              index++;
            }

            if (bottom == 0)
              cs.Datum = b.Links[i].UpstreamNode.pfsnode.InvertLevel;

            cs.Points.SetMarkerAt(1, 0);
            cs.Points.SetMarkerAt(3, b.Links[i].Xsec.Datas.Count - 1);
            cs.Points.SetMarkerAt(2, bottomindex);
            csc.Add(cs);
          }
          else if (b.Links[i].pfslink.Par4 == 1) //Assume circular
          {
            cs.Geometry = DHI.Mike1D.CrossSections.Geometry.ClosedCircular;
            cs.SetDiameter(b.Links[i].pfslink.Par7);
            cs.Datum = b.Links[i].UpstreamNode.pfsnode.InvertLevel;
            csc.Add(cs);
          }

          if (i == b.Links.Count - 1)
          {
            lastchainage += b.Links[i].Length;

            var connectionlink = b.Links[i].DownstreamNode.Links.FirstOrDefault(l => l.UpstreamNode == b.Links[i].DownstreamNode);
            if (connectionlink != null) //Create a connection
            {
              var branch = Branches.Single(br => br.Links.Contains(connectionlink));
              newbranch.connections.Par3 = branch.Name;
              newbranch.connections.Par4 = branch.GetChainage(connectionlink);
            }
            pointcount++;
            var bpn = nwk.MIKE_11_Network_editor.POINTS.AddPoint();
            bpn.Par1 = pointcount;
            bpn.Par2 = b.Links[i].DownstreamNode.Location.X;
            bpn.Par3 = b.Links[i].DownstreamNode.Location.Y;
            bpn.Par4 = 0;
            bpn.Par5 = lastchainage;
            newbranch.points.AddValue(pointcount);
          }
          pointcount++;
        }
        newbranch.definitions.Par3 = 0;
        newbranch.definitions.Par4 = (int) lastchainage;
        newbranch.definitions.Par6 = 1000;
        newbranch.definitions.Par7 = 3;
      }

      nwk.MIKE_11_Network_editor.DATA_AREA.x0 =(int) (x0- 0.1* (x1-x0));
      nwk.MIKE_11_Network_editor.DATA_AREA.x1 = (int)(x1 + 0.1 * (x1 - x0));
      nwk.MIKE_11_Network_editor.DATA_AREA.y0 = (int)(y0 - 0.1 * (y1 - y0));
      nwk.MIKE_11_Network_editor.DATA_AREA.y1 = (int)(y1 + 0.1 * (y1 - y0));

      nwk.FileName = m11name;
      nwk.Save();
      csc.Connection.Save();


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
          dt.Columns.Add("Name", typeof(string));
          dt.Columns.Add("Length", typeof(double));

          foreach (var b in Branches)
          {

            var line = new XYPolyline();
            line.Points.AddRange(b.Links.Select(p => p.UpstreamNode.Location));
            line.Points.Add(b.Links.Last().DownstreamNode.Location);
            GeoRefData grf = new GeoRefData();
            grf.Geometry = line; 
            grf.Data = dt.NewRow();
            grf.Data[0] = b.Name;
            grf.Data[1] = line.GetLength();
            
            sw.Write(grf);


          }



        }

      }
    }
  }
}
