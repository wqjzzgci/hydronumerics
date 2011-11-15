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

      ShapeLib.SHPDestroyObject(pShape);
      _recordPointer++;

      IGeometry geom = null;

      switch (type)
      {
        case ShapeLib.ShapeType.NullShape:
          break;
        case ShapeLib.ShapeType.Point:
          geom = new XYPoint(x[0], y[0]);
          break;
        case ShapeLib.ShapeType.PolyLine:
          break;
        case ShapeLib.ShapeType.Polygon:
          geom = new XYPolygon();
          for (int i = x.Length-1; i >0; i--)
            ((XYPolygon)geom).Points.Add(new XYPoint(x[i], y[i]));
          break;
        case ShapeLib.ShapeType.MultiPoint:
          break;
        case ShapeLib.ShapeType.PointZ:
          break;
        case ShapeLib.ShapeType.PolyLineZ:
          geom = new XYPolyline();
          for (int i = x.Length - 1; i > 0; i--)
            ((XYPolyline)geom).Points.Add(new XYPoint(x[i], y[i]));
          break;
        case ShapeLib.ShapeType.PolygonZ:
          break;
        case ShapeLib.ShapeType.MultiPointZ:
          break;
        case ShapeLib.ShapeType.PointM:
          break;
        case ShapeLib.ShapeType.PolyLineM:
          break;
        case ShapeLib.ShapeType.PolygonM:
          break;
        case ShapeLib.ShapeType.MultiPointM:
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
