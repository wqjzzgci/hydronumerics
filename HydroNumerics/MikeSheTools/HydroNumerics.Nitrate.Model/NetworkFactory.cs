using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotSpatial.Data;

namespace HydroNumerics.Nitrate.Model
{
  public class NetworkFactory
  {

    public static List<Catchment> EndCatchments(string ShapeFileName)
    {
      IFeatureSet fs = FeatureSet.OpenFile(ShapeFileName);

      Dictionary<int, Catchment> allCatchments = new Dictionary<int,Catchment>();

      foreach (var c in fs.Features)
      {
        Catchment ca = new Catchment( (int) c.DataRow[0]);
        if (!allCatchments.ContainsKey(ca.ID15))
          allCatchments.Add(ca.ID15,ca);
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

      return allCatchments.Values.Where(c => c.DownstreamConnection == null).ToList();

    }


  }
}
