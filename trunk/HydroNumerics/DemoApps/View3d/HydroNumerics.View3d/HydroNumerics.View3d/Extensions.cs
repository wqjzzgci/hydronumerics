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

      TruncatedConeVisual3D tcvw = new TruncatedConeVisual3D();
      tcvw.TopRadius = 0.5;
      tcvw.BaseRadius = 0.5;
      tcvw.Origin = new System.Windows.Media.Media3D.Point3D(x, y, Well.Terrain - Well.Depth.Value);
      if (Well.Depth.HasValue)
        tcvw.Height = Well.Depth.Value;
      else if (Well.Intakes.SelectMany(var => var.Screens).Count() > 0)
        tcvw.Height = Well.Intakes.SelectMany(var => var.Screens).Max(var2 => var2.DepthToBottom.Value);
      else
        tcvw.Height = Well.LithSamples.Max(var => var.Bottom);

      tcvw.Fill = new SolidColorBrush(Colors.Gray);
      wellrep.Add(tcvw);


      foreach (var sc in Well.Intakes.SelectMany(var => var.Screens))
      {
        if (sc.DepthToBottom.HasValue & sc.DepthToTop.HasValue)
        {
          TruncatedConeVisual3D tcv = new TruncatedConeVisual3D();
          tcv.TopRadius = 0.7;
          tcv.BaseRadius = 0.7;
          tcv.Origin = new System.Windows.Media.Media3D.Point3D(x, y, sc.BottomAsKote.Value);
          tcv.Height = sc.TopAsKote.Value - sc.BottomAsKote.Value;
          tcv.Fill = new SolidColorBrush(Colors.Black);
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
          SolidColorBrush m;
          if (l.RockSymbol.ToLower().Contains("s"))
          {
            m = new SolidColorBrush(Colors.Blue);
          }
          else if (l.RockSymbol.ToLower().Contains("l"))
          {
            m = new SolidColorBrush(Colors.Red);
          }
          else
            m = new SolidColorBrush(Colors.Green);

          m.Opacity = 0.3;
          tcv.Fill = m;

          wellrep.Add(tcv);
          TextVisual3D txt = new TextVisual3D();
          txt.Center = new Point3D(x+3, y+3, Well.Terrain - (l.Bottom + l.Top)/2.0);
          txt.Text = l.RockSymbol;
          txt.Height = 1;
          wellrep.Add(txt);

        }

      }
      return wellrep;
    }



    public static ModelVisual3D Representation3D(this XYPolygon Poly, IXYPoint refpoint, double height)
    {
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

      var m = MaterialHelper.CreateMaterial(Colors.DimGray, 0.5);

      var mv3D = new ModelVisual3D();
      mv3D.Content = new GeometryModel3D(mb.ToMesh(), m);
      return mv3D;

    }
  }
}

