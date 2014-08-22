using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.MikeSheTools.PFS.NWK11;
using HydroNumerics.Core;


namespace HydroNumerics.MikeSheTools.Mike11
{
  public class Network:BaseViewModel
  {

    private SortedList<BranchID, M11Branch> _branchsort = new SortedList<BranchID, M11Branch>();


    private ObservableCollection<M11Branch> branches = new ObservableCollection<M11Branch>();

    /// <summary>
    /// Gets and sets Branches;
    /// </summary>
    public ObservableCollection<M11Branch> Branches
    {
      get { return branches; }
      set
      {
        if (value != branches)
        {
          branches = value;
          RaisePropertyChanged("Branches");
        }
      }
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
        branches.Add(new M11Branch(b, Points));
        _branchsort.Add(branches.Last().ID, branches.Last());
      }

      foreach (var b in branches)
      {
        if (!b.IsEndPoint)
        {
          b.DownstreamBranch = branches.FirstOrDefault(br => br.Name == b.DownStreamConnection.Branchname & br.ChainageEnd >= b.DownStreamConnection.StartChainage & br.ChainageStart <= b.DownStreamConnection.StartChainage);
          if (b.DownstreamBranch != null)
            _branchsort[b.DownstreamBranch.ID].UpstreamBranches.Add(b);
        }
      }
    }

    public M11Branch GetBranch(string BranchName, double chainage)
    {
      return branches.FirstOrDefault(b => b.Name.ToLower() == BranchName.ToLower() & b.ChainageStart <= chainage & b.ChainageEnd >= chainage);
    }


    /// <summary>
    /// Writes a polyline shape file with the network
    /// </summary>
    /// <param name="shapefilename"></param>
    public void WriteToShape(string shapefilename)
    {
      using (ShapeWriter swc = new ShapeWriter(shapefilename + "_QHPoints"))
      {
        DataTable dat = new DataTable();
        dat.Columns.Add("BranchName", typeof(string));
        dat.Columns.Add("Chainage", typeof(double));
        dat.Columns.Add("Type", typeof(string));
        foreach (var b in nwkfile.MIKE_11_Network_editor.COMPUTATIONAL_SETUP.branchs)
        {
          foreach (var p in b.points.points)
          {
            GeoRefData gd = new GeoRefData();
            gd.Data = dat.NewRow();
            gd.Data["BranchName"] = b.name;
            gd.Data["Chainage"] = p.Par1;

            if(p.Par3 ==0)
            gd.Data["Type"] = "h";
            else
              gd.Data["Type"] = "q";

            var bran = Branches.FirstOrDefault(br => br.Name == b.name);
            if (bran != null)
            {
              gd.Geometry = bran.GetPointAtChainage(p.Par1);
              swc.Write(gd);
            }
          }
        }
      }

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

      foreach (var b in branches)
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
