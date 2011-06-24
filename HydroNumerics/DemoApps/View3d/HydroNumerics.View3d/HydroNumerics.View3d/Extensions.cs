using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using HydroNumerics.Geometry;
using HydroNumerics.JupiterTools;
using HelixToolkit;

namespace HydroNumerics.View3d
{
  public static class Extensions
  {
    public static List<Visual3D> Representation3D(this JupiterWell Well, IXYPoint refpoint)
    {
      List<Visual3D> wellrep = new List<Visual3D>();

      double x = refpoint.X - Well.X;
      double y = refpoint.Y - Well.Y;

      if (Well.Depth.HasValue)
      {
        TruncatedConeVisual3D tcv = new TruncatedConeVisual3D();
        tcv.TopRadius = 0.5;
        tcv.BaseRadius = 0.5;
        tcv.Origin = new System.Windows.Media.Media3D.Point3D(x, y, Well.Terrain - Well.Depth.Value);
        tcv.Height = Well.Depth.Value;

        var m = new SolidColorBrush(Colors.Gray);
        tcv.Fill = m;
        wellrep.Add(tcv);
      }

      foreach (var sc in Well.Intakes.SelectMany(var=>var.Screens))
      {
        if (sc.DepthToBottom.HasValue & sc.DepthToTop.HasValue)
        {
          TruncatedConeVisual3D tcv = new TruncatedConeVisual3D();
          tcv.TopRadius = 0.7;
          tcv.BaseRadius = 0.7;
          tcv.Origin = new System.Windows.Media.Media3D.Point3D(x, y, sc.BottomAsKote.Value);
          tcv.Height = sc.TopAsKote.Value - sc.BottomAsKote.Value;
          var m = new SolidColorBrush(Colors.Black);
          tcv.Fill = m;
          wellrep.Add(tcv);
        }
      }

      foreach (var l in Well.LithSamples)
      {
        if (l.Top != -999 & l.Bottom != -999)
        {
          TruncatedConeVisual3D tcv = new TruncatedConeVisual3D();
          tcv.TopRadius = 1;
          tcv.BaseRadius = 1;
          tcv.Origin = new System.Windows.Media.Media3D.Point3D(x, y, Well.Terrain - l.Bottom);
          tcv.Height = l.Bottom - l.Top;
          if (l.RockType.ToLower().Contains("s"))
          {
            var m = new SolidColorBrush(Colors.Blue);
            m.Opacity = 0.3;
            tcv.Fill = m;
          }
          else
          {
            var m = new SolidColorBrush(Colors.Red);
            m.Opacity = 0.3;
            tcv.Fill = m;
          }
          wellrep.Add(tcv);
        }

      }
      return wellrep;
    }



    public static ModelVisual3D Representation3D(this XYPolygon Poly, IXYPoint refpoint, double height)
    {
      Polygon3D pl = new Polygon3D();
      MeshBuilder mb = new MeshBuilder();
      var pts = new Point3DCollection();

      foreach (var p in Poly.Points)
      {
        pts.Add(new Point3D(refpoint.X - p.X, refpoint.Y - p.Y, height));
      }

      // POLYGONS (flat and convex)
      var poly3D = new Polygon3D(pts);
      // Transform the polygon to 2D
      var poly2D = poly3D.Flatten();
      // Triangulate
      var tri = poly2D.Triangulate();
      if (tri != null)
      {
        // Add the triangle indices with the 3D points
        mb.Append(pts, tri);
      }

      var m = MaterialHelper.CreateMaterial(Colors.Blue, 0.5);

      PlaneVisual3D pv = new PlaneVisual3D();
      TerrainVisual3D tv = new TerrainVisual3D();
      
      var mv3D = new ModelVisual3D();
      mv3D.Content = new GeometryModel3D(mb.ToMesh(), m);
      return mv3D;


    }
  }
}

