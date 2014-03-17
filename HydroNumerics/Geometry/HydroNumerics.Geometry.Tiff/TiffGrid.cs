using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;

using BitMiracle.LibTiff.Classic;
//using Microsoft.SqlServer.Types;


namespace HydroInform.Catchment
{
  public class TiffGrid:BaseGrid,IDisposable
  {

    Tiff tiff;


    Dictionary<int, byte[]> ScanLineCache;


    private byte[] scanline;
    private int bits;

    public TiffGrid(string FileName)
    {

      string fname = Path.GetFullPath(FileName);

      tiff = Tiff.Open(FileName, "r");
      var val = tiff.GetField((TiffTag)33922)[1].ToDoubleArray();
      XOrigin = val[3];
      YOrigin = val[4]; //Upper basegrid assumes Lower
      val = tiff.GetField((TiffTag)33550)[1].ToDoubleArray();
      GridSize = val[0];
      GridSize = val[1];
      NumberOfColumns = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
      NumberOfRows = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
      scanline = new byte[tiff.ScanlineSize()];
      bits = tiff.ScanlineSize() / NumberOfColumns;
      ScanLineCache = new Dictionary<int, byte[]>();

      
    }


    /// <summary>
    /// Gets the value from the GridCell containing X and Y. Returns null if X and Y is not within the grid.
    /// </summary>
    /// <param name="X"></param>
    /// <param name="Y"></param>
    /// <returns></returns>
    public double? GetValue(double X, double Y)
    {
      int column = GetColumnIndex(X);
      int row = GetRowIndex(Y);

      if (column < NumberOfColumns & row < NumberOfRows)
      {
        SetCurrentScanline(row);
        return GetColumnValue(column);
      }
      return null;
    }


    public double GetVolumeBelow(double grade)
    {
      double vol =0;
      for (int i = 0; i < NumberOfRows; i++)
      {
        SetCurrentScanline(i);
        for (int j = 0; j < NumberOfColumns; j++)
        {
          double height = GetColumnValue(j);
          if (height!= -9999 &  height < grade)
            vol += GridSize * GridSize * (grade - height);
        }
      }
      return vol;
    }

    //public double GetAverage(SqlGeometry polygon)
    //{
    //  int nvals = 0;
    //  double average = 0;


    //  double xmax = double.MinValue;
    //  double ymax =double.MinValue;
    //  double ymin = double.MaxValue;
    //  double xmin = double.MaxValue;

    //  var bound = polygon.STBoundary();

    //  for(int i =1;i<=bound.STNumPoints();i++)
    //  {
    //   xmax = Math.Max(xmax, (double) bound.STPointN(i).STX);
    //   xmin = Math.Min(xmin, (double) bound.STPointN(i).STX);
    //   ymax = Math.Max(ymax, (double) bound.STPointN(i).STY);
    //   ymin = Math.Min(ymin, (double) bound.STPointN(i).STY);
    //  }


    //  for (int i = GetRow(ymax); i <= GetRow(ymin); i++)
    //  {
    //    SetCurrentScanline(i);
    //    for (int j = GetColumn(xmin); j <= GetColumn(xmax); j++)
    //    {
    //      if ((bool)polygon.STContains(SqlGeometry.Point(XOrigin + j * GridSizeX, YOrigin - i * GridSizeY, (int)polygon.STSrid)))
    //      {
    //        int d = BitConverter.ToInt32(scanline, j * bits);
    //        average += d;
    //        nvals++;
    //      }
    //    }
    //  }
    //  return average / nvals;
    //}



    //public SqlGeometry GetCountur(double value)
    //{
    //  SqlGeometry catchdat = null;
    //  for (int i = 0; i < NumberOfRows; i++)
    //  {
    //    SetCurrentScanline(i);
    //    SqlGeometry line = null;
    //    for (int j = 0; j < NumberOfColumns; j++)
    //    {
    //      int d = BitConverter.ToInt32(scanline, j * bits);
    //      if (d > value)
    //      {
    //        var b = new SqlGeometryBuilder();
    //        b.SetSrid(25832);
    //        b.BeginGeometry(OpenGisGeometryType.Polygon);

    //        b.BeginFigure(j * GridSizeX + XOrigin, YOrigin - i * GridSizeY + 0.01);
    //        b.AddLine((j) * GridSizeX + XOrigin, YOrigin - (i + 1) * GridSizeY);
    //        b.AddLine((j + 1) * GridSizeX + XOrigin + 0.01, YOrigin - (i + 1) * GridSizeY);
    //        b.AddLine((j + 1) * GridSizeX + XOrigin + 0.01, YOrigin - i * GridSizeY + 0.01);
    //        b.AddLine((j) * GridSizeX + XOrigin, YOrigin - i * GridSizeY + 0.01);

    //        b.EndFigure();
    //        b.EndGeometry();
    //        if (line == null)
    //          line = b.ConstructedGeometry;
    //        else
    //          line = line.STUnion(b.ConstructedGeometry);
    //      }
    //    }
    //    if (line != null)
    //    {
    //      if (catchdat == null)
    //        catchdat = line;
    //      else
    //        catchdat = catchdat.STUnion(line);
    //    }
    //  }
    //  return catchdat;
    //}




   

    private void SetCurrentScanline(int row)
    {

      byte[] temp;
      if (!ScanLineCache.TryGetValue(row, out temp))
      {
        temp = new byte[tiff.ScanlineSize()];
        tiff.ReadScanline(temp, row);
        ScanLineCache.Add(row, temp);
      }
      scanline = temp;
    }

    private float GetColumnValue(int Column)
    {
      return BitConverter.ToSingle(scanline, Column * bits);
    }



    private bool TryAdd(int column, int row)
    {
      List<int> rows;
      if (!BeenThere.TryGetValue(column, out rows))
      {
        rows = new List<int>();
        rows.Add(row);
        BeenThere.Add(column, rows);
        return true;
      }
      else
      {
        if (rows.Contains(row))
          return false;
        else
        {
          rows.Add(row);
          return true;
        }
      }
    }

    private Dictionary<int,List<int>> BeenThere;

    private Queue<Tuple<int,int>> ToProcess;

    public Dictionary<int,List<int>> GetFloodedArea(double X, double Y, double Height)
    {

      BeenThere = new Dictionary<int,List<int>>();
      ToProcess = new Queue<Tuple<int,int>>();

      if (Height > GetValue(X, Y))
      {
        Tuple<int,int> center = new Tuple<int,int>(GetColumnIndex (X), GetRowIndex(Y));
        TryAdd(center.Item1, center.Item2);

        ToProcess.Enqueue(center);

        while(ToProcess.Count>0)
          explode(ToProcess.Dequeue(), Height);
      }
      return BeenThere;
    }

    private void explode(Tuple<int,int> point, double height)
    {

      int column  = point.Item1;
      int row = point.Item2;    

      //left
      SetCurrentScanline(row);
      if (column>0)
        if (GetColumnValue(column-1)<=height)
          if (TryAdd(column -1, row))
            ToProcess.Enqueue(new Tuple<int,int>(column-1, row));

      //right
      if (column<NumberOfColumns-1)
        if (GetColumnValue(column+1)<=height)
          if (TryAdd(column +1, row))
            ToProcess.Enqueue(new Tuple<int,int>(column+1, row));

      //up
      if (row>1)
      {
      SetCurrentScanline(row-1);
        if (GetColumnValue(column)<=height)
          if (TryAdd(column, row-1))
            ToProcess.Enqueue(new Tuple<int,int>(column, row-1));
      }

      //down
      if (row<NumberOfRows -1)
      {
      SetCurrentScanline(row+1);
        if (GetColumnValue(column)<=height)
          if (TryAdd(column, row+1))
            ToProcess.Enqueue(new Tuple<int,int>(column, row+1));
      }

    }





    #region IDisposable Members

    public void Dispose()
    {
      if (tiff != null)
        tiff.Dispose();
    }

    #endregion
  }
}
