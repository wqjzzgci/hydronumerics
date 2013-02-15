using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

using Microsoft.SqlServer.Types;

namespace HydroNumerics.Geometry
{
  public static class Extensions
  {

    /// <summary>
    /// Returns a geometry. Sets srid to 25832 (UTM32N) and does not do any conversions
    /// </summary>
    /// <param name="geom"></param>
    /// <returns></returns>
    public static SqlGeometry GetSqlGeometry(this IGeometry geom)
    {
      var v = new SqlGeometryBuilder();
      v.SetSrid(25832);
      
      Type t = geom.GetType();

      if (t is IXYPoint)
      {
        v.BeginFigure(((IXYPoint)geom).X, ((IXYPoint)geom).Y);
        v.EndFigure();
        return v.ConstructedGeometry;
      }
      else if (t == typeof(XYPolyline))
      {
        XYPolyline line = geom as XYPolyline;
        v.BeginGeometry(OpenGisGeometryType.LineString);
        v.BeginFigure(line.Points[0].X, line.Points[0].Y);
        for (int i = 0; i < line.Points.Count; i++)
          v.AddLine(line.Points[i].X, line.Points[i].Y);
        v.EndFigure();
        v.EndGeometry();
        return v.ConstructedGeometry;
      }
      else if (t == typeof(XYPolygon))
      {
        XYPolygon poly = geom as XYPolygon;
        v.BeginGeometry(OpenGisGeometryType.Polygon);

        v.BeginFigure(poly.Points[0].X, poly.Points[0].Y);
        for (int i = 1; i < poly.Points.Count; i++)
          v.AddLine(poly.Points[i].X, poly.Points[i].Y);
        v.AddLine(poly.Points[0].X, poly.Points[0].Y);
        v.EndFigure();
        v.EndGeometry();  
        
        return v.ConstructedGeometry;
      }
      else
        return null;
    }

    /// <summary>
    /// Returns a geopgraph and converts from the provided projection.
    /// </summary>
    /// <param name="geom"></param>
    /// <param name="ConvertFromThis"></param>
    /// <returns></returns>
    public static SqlGeography GetSqlGeography(this IGeometry geom, ICoordinateSystem ConvertFromThis)
    {
      var v = new SqlGeographyBuilder();

      Type t = geom.GetType();
      v.SetSrid(4326);

      ICoordinateTransformation trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(ConvertFromThis, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84);

      if (t == typeof(XYPoint))
      {
        double[] p1 = trans.MathTransform.Transform(new double[] { ((XYPoint)geom).X, ((XYPoint)geom).Y });

        v.BeginGeography(OpenGisGeographyType.Point);
        v.BeginFigure(p1[1],p1[0]);
        v.EndFigure();
        v.EndGeography();
        return v.ConstructedGeography;
      }
      else if (t == typeof(XYPolyline))
      {
        XYPolyline line = geom as XYPolyline;
        bool FirstPoint =true;
        v.BeginGeography(OpenGisGeographyType.LineString);
        foreach(var p in line.Points)
        {
          double[] p1 = trans.MathTransform.Transform(new double[] {p.X, p.Y });
          if (!FirstPoint)
          {
            v.BeginFigure(p1[1],p1[0]);
            FirstPoint=false;
          }
          else
            v.AddLine(p1[1],p1[0]);
        }
        v.EndFigure();
        v.EndGeography();
        return v.ConstructedGeography;
      }
      else if (t == typeof(XYPolygon))
      {
        XYPolygon poly = geom as XYPolygon;
        v.BeginGeography(OpenGisGeographyType.Polygon);

        double[] p0 = trans.MathTransform.Transform(new double[] { poly.Points[0].X, poly.Points[0].Y });
        v.BeginFigure(p0[1], p0[0]);

        for (int i = 1; i < poly.Points.Count; i++)
        {
          double[] pn = trans.MathTransform.Transform(new double[] { poly.Points[i].X, poly.Points[i].Y });
          v.AddLine(pn[1],pn[0]);
        }
        v.AddLine(p0[1], p0[0]);
        v.EndFigure();
        v.EndGeography();

        return v.ConstructedGeography;
      }
      else
        return null;
    }
  }
}
