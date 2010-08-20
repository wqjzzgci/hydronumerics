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
    private List<M11Branch> _branches = new List<M11Branch>();

    public IEnumerable<M11Branch> Branches
    {
      get { return _branches; }
    }

    public Network(string NWK11FileName)
    {
      NWK11File nfile = new NWK11File(NWK11FileName);

      foreach(Branch b in nfile.MIKE_11_Network_editor.Branches)
        _branches.Add(new M11Branch(b, nfile.MIKE_11_Network_editor.Points));
    }

    public void WriteToShape(string shapefilename)
    {
      ShapeWriter sw = new ShapeWriter(shapefilename);

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
      }
      sw.Dispose();
    }

  }
}
