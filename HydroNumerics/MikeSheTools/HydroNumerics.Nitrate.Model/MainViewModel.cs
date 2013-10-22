using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DotSpatial.Data;

using HydroNumerics.Core.WPF;
using HydroNumerics.Geometry;

namespace HydroNumerics.Nitrate.Model
{
  public class MainViewModel : BaseViewModel
  {




    public ObservableCollection<Catchment> EndCatchments { get; private set; }


    public void LoadCatchments(string ShapeFileName)
    {
      IFeatureSet fs = FeatureSet.OpenFile(ShapeFileName);

      Dictionary<int, Catchment> allCatchments = new Dictionary<int, Catchment>();

      foreach (var c in fs.Features)
      {
        Catchment ca = new Catchment((int)c.DataRow[0]);
        if (!allCatchments.ContainsKey(ca.ID15))
          allCatchments.Add(ca.ID15, ca);

        XYPolygon geom = new XYPolygon();
        foreach (var p in c.Coordinates)
          geom.Points.Add(new XYPoint(p.X, p.Y));
       
        ca.Geometry = geom;
      }

      foreach (var c in fs.Features)
      {
        int catcid = ((int)c.DataRow[0]);
        int downid = ((int)c.DataRow[1]);
        Catchment DownStreamCatchment;
        if (allCatchments.TryGetValue(downid, out DownStreamCatchment))
        {
          allCatchments[catcid].DownstreamConnection = DownStreamCatchment;
          DownStreamCatchment.UpstreamConnections.Add(allCatchments[catcid]);
        }
      }
      fs.Close();
      fs.Dispose();
      EndCatchments = new ObservableCollection<Catchment>(allCatchments.Values.Where(c => c.DownstreamConnection == null));

    }

  }
}

