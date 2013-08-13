using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.MikeSheTools.PFS.NWK11;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class Network
  {

    private SortedList<BranchID, M11Branch> _branchsort = new SortedList<BranchID, M11Branch>();
    private List<M11Branch> _branches = new List<M11Branch>();

    public List<M11Branch> Branches
    {
      get { return _branches; }
    }

    public Network()
    {
    }

    public NWK11File nwkfile { get; private set; }

    public void Load(string NWK11FileName)
    {
      nwkfile = new NWK11File(NWK11FileName);

      SortedDictionary<int, point> Points = new SortedDictionary<int, point>();
      foreach (var p in nwkfile.MIKE_11_Network_editor.POINTS.points)
      {
        Points.Add(p.Par1, p);
      }

      foreach (var b in nwkfile.MIKE_11_Network_editor.BRANCHES.branchs)
      {
        _branches.Add(new M11Branch(b, Points));
        _branchsort.Add(_branches.Last().ID, _branches.Last());
      }

      foreach (var b in _branches)
      {
        if (!b.IsEndPoint)
        {
          b.DownStreamBranch = _branches.FirstOrDefault(br => br.Name == b.DownStreamConnection.Branchname & br.ChainageEnd >= b.DownStreamConnection.StartChainage & br.ChainageStart <= b.DownStreamConnection.StartChainage);
          if (b.DownStreamBranch!=null)
            _branchsort[b.DownStreamBranch.ID].UpstreamBranches.Add(b);
        }
      }
    }


    /// <summary>
    /// Writes a polyline shape file with the network
    /// </summary>
    /// <param name="shapefilename"></param>
    public void WriteToShape(string shapefilename)
    {
      ShapeWriter sw = new ShapeWriter(shapefilename);


      ShapeWriter swCsc = new ShapeWriter(shapefilename + "_CrossSections");
      DataTable dtCsc = new DataTable();
      dtCsc.Columns.Add("Name", typeof(string));
      dtCsc.Columns.Add("TopoID", typeof(string));
      dtCsc.Columns.Add("Chainage", typeof(double));

      DataTable dt = new DataTable();
      dt.Columns.Add("Name", typeof(string));
      dt.Columns.Add("TopoID", typeof(string));
      dt.Columns.Add("ChainageStart", typeof(double));
      dt.Columns.Add("ChainageEnd", typeof(double));

      foreach (var b in _branches)
      {
        GeoRefData grf = new GeoRefData();
        grf.Geometry = b.Line;
        grf.Data=dt.NewRow();
        grf.Data[0] = b.Name;
        grf.Data[1] = b.TopoID;
        grf.Data[2] = b.ChainageStart;
        grf.Data[3] = b.ChainageEnd;
        sw.Write(grf);

        foreach (var Csc in b.CrossSections)
        {
          GeoRefData csc_data = new GeoRefData();
          csc_data.Geometry = Csc.Line;
          csc_data.Data = dtCsc.NewRow();
          csc_data.Data[0] = Csc.BranchName;
          csc_data.Data[1] = Csc.TopoID;
          csc_data.Data[2] = Csc.Chainage;

          swCsc.Write(csc_data);
        }
      }
      sw.Dispose();
      swCsc.Dispose();
    }

  }
}
