using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media.Media3D;

using HydroNumerics.MikeSheTools.ViewModel;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.JupiterTools;
using HydroNumerics.Wells;

using HelixToolkit;

namespace HydroNumerics.View3d
{
  public class SiteViewModel:BaseViewModel
  {
    private GeoRefData area;
    private XYPolygon site;
    private List<JupiterWell> closestWells;


    public SiteViewModel( GeoRefData Area, IWellCollection  Wells)
    {
      area = Area;
      site = area.Geometry as XYPolygon;

      foreach (var v in Wells)
      {
        if (XYGeometryTools.CalculatePointToPointDistance(site.Points.First(), v) < 500)
          closestWells.Add((JupiterWell) v); 
      }

      double? height = 40;
      HydroNumerics.Geometry.Net.KMSData.TryGetHeight(site.Points.First(), 32, out height);
      
      var plant = site.Representation3D(site.Points.First(), height.Value);
    }

  }
}
