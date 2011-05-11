using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra;

namespace HydroNumerics.Geometry.ASCII
{

  public class ASCIIGrid
  {
    public int NumberOfColumns { get; set; }
    public int NumberOfRows { get; set; }
    public double GridSize { get; set; }
    public double DeleteValue { get; set; }
    public double XOrigin {get;set;}
    public double YOrigin {get;set;}
    public Matrix Data { get; set; }
    public bool OriginAtCenter {get;set;}

    public ASCIIGrid()
    {
      OriginAtCenter = false;
    }


    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("<NCOLS " + NumberOfColumns + ">");
      sb.AppendLine("<NROWS " + NumberOfRows + ">");
      if (OriginAtCenter)
      {
        sb.AppendLine("<XLLCENTER " + XOrigin + ">");
        sb.AppendLine("<YLLCENTER " + YOrigin + ">");
      }
      else
      {
        sb.AppendLine("<XLLCORNER " + XOrigin + ">");
        sb.AppendLine("<YLLCORNER " + YOrigin + ">");
      }
      sb.AppendLine("<CELLSIZE " + GridSize + ">");
      sb.AppendLine("<NODATA_VALUE " + DeleteValue + ">");

      for (int i=0;i<Data.ColumnCount;i++)
        for (int j = Data.RowCount -1 ; j>=0; j--)
          sb.AppendLine(Data[j, i].ToString());

      return sb.ToString();
    }
  }
}
