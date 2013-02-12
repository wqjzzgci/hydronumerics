using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Types;

namespace HydroNumerics.Geometry
{
  public static class Extensions
  {

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


    public static SqlGeography GetSqlGeography(this IGeometry geom)
    {
      var v = new SqlGeographyBuilder();

      Type t = geom.GetType();
      v.SetSrid(4326);

      if (t == typeof(XYPoint))
      {
        v.BeginGeography(OpenGisGeographyType.Point);
        v.BeginFigure(((XYPoint)geom).Latitude, ((XYPoint)geom).Longitude);
        v.EndFigure();
        v.EndGeography();
        return v.ConstructedGeography;
      }
      else if (t == typeof(XYPolyline))
      {
        XYPolyline line = geom as XYPolyline;
        v.BeginGeography(OpenGisGeographyType.LineString);
        v.BeginFigure( ((XYPoint) line.Points[0]).Latitude, ((XYPoint)line.Points[0]).Longitude);
        for (int i = 0; i < line.Points.Count; i++)
          v.AddLine(((XYPoint)line.Points[i]).Latitude, ((XYPoint)line.Points[i]).Longitude);
        v.EndFigure();
        v.EndGeography();
        return v.ConstructedGeography;
      }
      else if (t == typeof(XYPolygon))
      {
        XYPolygon poly = geom as XYPolygon;
        v.BeginGeography(OpenGisGeographyType.Polygon);

        v.BeginFigure(((XYPoint)poly.Points[0]).Latitude, ((XYPoint)poly.Points[0]).Longitude);
        for (int i = 1; i < poly.Points.Count; i++)
          v.AddLine(((XYPoint)poly.Points[i]).Latitude, ((XYPoint)poly.Points[i]).Longitude);
        v.AddLine(((XYPoint)poly.Points[0]).Latitude, ((XYPoint)poly.Points[0]).Longitude);
        v.EndFigure();
        v.EndGeography();

        return v.ConstructedGeography;
      }
      else
        return null;


    }


  }
}
