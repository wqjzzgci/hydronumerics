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
      var v = new  SqlGeometryBuilder();
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
        v.BeginFigure(line.Points[0].X,line.Points[0].Y);
        for (int i = 1; i < line.Points.Count; i++)
          v.AddLine(line.Points[i].X, line.Points[i].Y);
        v.EndFigure();
        return v.ConstructedGeometry;
      }
      else
        return null;


    }

  }
}
