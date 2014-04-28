using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using HydroNumerics.Geometry;


namespace HydroNumerics.Geometry.Shapes
{
  public class ShapeReader:Shape
  {
    private DBFReader _data;
    private ShapeLib.ShapeType type = new ShapeLib.ShapeType();


    public ShapeReader(string FileName)
      : base()
    {
      _fileName = FileName;
      if (!File.Exists(FileName))
        throw new FileNotFoundException("Could not find the file: " + FileName);
      _data = new DBFReader(FileName);

      // Open shapefile
      _shapePointer = ShapeLib.SHPOpen(FileName, "rb");

      int nbentities = 0;
      double[] arr1 = new double[4];
      double[] arr2 = new double[4];

      ShapeLib.SHPGetInfo(_shapePointer, ref nbentities, ref type, arr1, arr2);

     
    }

    ///// <summary>
    ///// Reads next shape and increments counter
    ///// </summary>
    ///// <param name="X"></param>
    ///// <param name="Y"></param>
    //public void ReadNext(out double X, out double Y)
    //{
    //  IntPtr pShape = ShapeLib.SHPReadObject(_shapePointer, _recordPointer);
    //  ShapeLib.SHPObject shpObject = new ShapeLib.SHPObject();
    //  Marshal.PtrToStructure(pShape, shpObject);
    //  double[] x = new double[shpObject.nVertices];
    //  Marshal.Copy(shpObject.padfX, x, 0, x.Length);
    //  double[] y = new double[shpObject.nVertices];
    //  Marshal.Copy(shpObject.padfY, y, 0, y.Length);

    //  X= x[0];
    //  ShapeLib.SHPDestroyObject(pShape);
    //  Y = y[0];
    //  _recordPointer++;

    //}


    public override ProjNet.CoordinateSystems.ICoordinateSystem Projection
    {
      get
      {
        if (projection == null)
        {
          string prjfile = Path.ChangeExtension(_fileName, ".prj");
          if (File.Exists(prjfile))
          {
            using (System.IO.StreamReader sr = new System.IO.StreamReader(prjfile))
            {
              ProjNet.CoordinateSystems.CoordinateSystemFactory cs = new ProjNet.CoordinateSystems.CoordinateSystemFactory();
              projection = cs.CreateFromWkt(sr.ReadToEnd());
            }
          }
        }
        return projection;
      }
    }


    public IGeometry ReadNext()
    {
            IntPtr pShape = ShapeLib.SHPReadObject(_shapePointer, _recordPointer);
      ShapeLib.SHPObject shpObject = new ShapeLib.SHPObject();
      Marshal.PtrToStructure(pShape, shpObject);
      double[] x = new double[shpObject.nVertices];
      Marshal.Copy(shpObject.padfX, x, 0, x.Length);
      double[] y = new double[shpObject.nVertices];
      Marshal.Copy(shpObject.padfY, y, 0, y.Length);
      double[] z = new double[shpObject.nVertices];
      Marshal.Copy(shpObject.padfZ, z, 0, z.Length);

      int[] partstarts=null;
      if (shpObject.nParts > 0)
      {
        partstarts = new int[shpObject.nParts];
        Marshal.Copy(shpObject.paPartStart, partstarts, 0, partstarts.Length);
      }

      ShapeLib.SHPDestroyObject(pShape);
      _recordPointer++;

      IGeometry geom = null;

      switch (type)
      {
        case ShapeLib.ShapeType.NullShape:
          break;
        case ShapeLib.ShapeType.MultiPoint:
        case ShapeLib.ShapeType.MultiPointZ:
        case ShapeLib.ShapeType.MultiPointM:
        case ShapeLib.ShapeType.PointM:
        case ShapeLib.ShapeType.PointZ:
        case ShapeLib.ShapeType.Point:
          geom = new XYPoint(x[0], y[0]);
          break;
        case ShapeLib.ShapeType.PolyLineM:
        case ShapeLib.ShapeType.PolyLineZ:
        case ShapeLib.ShapeType.PolyLine:
          geom = new XYPolyline();
          for (int i = 0; i < x.Length; i++)
            ((XYPolyline)geom).Points.Add(new XYPoint(x[i], y[i]));
          break;
        case ShapeLib.ShapeType.PolygonM:
        case ShapeLib.ShapeType.PolygonZ:
        case ShapeLib.ShapeType.Polygon:

          if (partstarts.Count() == 1)
          {
            geom = new XYPolygon();
            
            for (int i = 0; i <x.Length; i++)
              ((XYPolygon)geom).Points.Add(new XYPoint(x[i], y[i]));
            ((XYPolygon)geom).Points.Reverse();
          }
          else
          {
            geom = new MultiPartPolygon();

            //foreach (var partstart in partstarts.Reverse())
            //{
            //  var poly = new XYPolygon();
            //  for (int i = end; i > partstart; i--)
            //    poly.Points.Add(new XYPoint(x[i], y[i]));
            //  end = partstart;
            //  ((MultiPartPolygon)geom).Polygons.Add(poly); 
            //}
            for (int j=0;j< partstarts.Count();j++)
            {
              int end;
              if (j < partstarts.Count() - 1)
                end = partstarts[j + 1];
              else
                end = x.Length;

              var poly = new XYPolygon();
              for (int i = partstarts[j]; i < end; i++)
                poly.Points.Add(new XYPoint(x[i], y[i]));
              poly.Points.Reverse();
              ((MultiPartPolygon)geom).Polygons.Add(poly);
            }

          }
          break;
        case ShapeLib.ShapeType.MultiPatch:
          break;
        default:
          break;
      }
      return geom;
    }

    public IEnumerable<GeoRefData> GeoData
    {
      get
      {
        while (!Data.EndOfData)
        {
          GeoRefData grd = new GeoRefData();
          grd.Geometry = ReadNext();
          grd.Data = Data.ReadNext();
          yield return grd;
        }
      }
    }


    /// <summary>
    /// Disposes the shapefile
    /// </summary>
    public override void Dispose()
    {
      _data.Dispose();
      base.Dispose();
    }

    /// <summary>
    /// Gets access to the data reader
    /// </summary>
    public DBFReader Data
    {
      get { return _data; }
    }

  }
}
