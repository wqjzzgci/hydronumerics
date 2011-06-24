using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.View3d
{
  public class RegionViewModel:BaseViewModel
  {
    List<GeoRefData> sites;

    public RegionViewModel()
    {
      sites = new List<GeoRefData>();
      using (ShapeReader sr = new ShapeReader(@"C:\Jacob\Projects\OPI\ds_kortlaegninger_dkjord_v2_download.shp"))
      {
        sites.AddRange(sr.GeoData);
      }

    }

  }
}
