using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  public class Branch:PFSMapper 
  {
    private PFSKeyword _definitions;
    private PFSKeyword _connections;

    internal Branch(PFSSection Keyword)
    {
      _pfsHandle = Keyword;

      _definitions = _pfsHandle.GetKeyword("definitions",1);
      _connections = _pfsHandle.GetKeyword("connections",1);
      PFSKeyword _points = _pfsHandle.GetKeyword("points", 1);
      PointNumbers = new List<int>();

      for (int i =1;i<=_points.GetParametersNo();i++)
        PointNumbers.Add(_points.GetParameter(i).ToInt());

    }

    public string BranchID
    {
      get { return _definitions.GetParameter(1).ToString(); }
      set { _definitions.GetParameter(1).Value = value; }
    }

    public string TopoID
    {
      get { return _definitions.GetParameter(2).ToString(); }
      set { _definitions.GetParameter(2).Value = value; }
    }

    public double UpstreamChainage
    {
      get { return _definitions.GetParameter(3).ToDouble(); }
      set { _definitions.GetParameter(3).Value = value; }
    }

    public double DownstreamChainage
    {
      get { return _definitions.GetParameter(4).ToDouble(); }
      set { _definitions.GetParameter(4).Value = value; }
    }

    public int FlowDirection
    {
      get { return _definitions.GetParameter(5).ToInt(); }
      set { _definitions.GetParameter(5).Value = value; }
    }

    
    public double MaximumDX
    {
      get { return _definitions.GetParameter(6).ToDouble(); }
      set { _definitions.GetParameter(6).Value = value; }
    }

    public int BranchType
    {
      get { return _definitions.GetParameter(7).ToInt(); }
      set { _definitions.GetParameter(7).Value = value; }
    }

    public string UpstreamConnectionName
    {
      get { return _connections.GetParameter(1).ToString(); }
      set { _connections.GetParameter(1).Value = value; }
    }

    public double UpstreamConnectionChainage
    {
      get { return _connections.GetParameter(2).ToDouble(); }
      set { _connections.GetParameter(2).Value = value; }
    }

    public string DownstreamConnectionName
    {
      get { return _connections.GetParameter(3).ToString(); }
      set { _connections.GetParameter(3).Value = value; }
    }

    public double DownstreamConnectionChainage
    {
      get { return _connections.GetParameter(4).ToDouble(); }
      set { _connections.GetParameter(4).Value = value; }
    }

    public List<int> PointNumbers { get; private set; }

  }
}
