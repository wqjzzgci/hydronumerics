using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.View3d
{
  public class RegionViewModel:BaseViewModel
  {
    public List<SiteViewModel> Sites{get;set;}

    public RegionViewModel()
    {
      IWellCollection Wells;
      using (Reader R = new Reader(@"C:\Jacob\Projects\OPI\sjælland.mdb"))
      {
        Wells = R.ReadWellsInSteps();
        R.ReadLithology(Wells);
      }
      Sites = new List<SiteViewModel>();
      using (ShapeReader sr = new ShapeReader(@"C:\Jacob\Projects\OPI\ds_kortlaegninger_dkjord_v2_download.shp"))
      {
        foreach(var s in sr.GeoData)
          Sites.Add(new SiteViewModel(s, Wells));
      }
      RaisePropertyChanged("Sites");
    }
  }
}
