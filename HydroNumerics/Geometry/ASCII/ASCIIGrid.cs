using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra.Double;

using HydroNumerics.Geometry;

namespace HydroNumerics.Geometry.ASCII
{

  public class ASCIIGrid:IGrid
  {
    public int NumberOfColumns { get; set; }
    public int NumberOfRows { get; set; }
    public double GridSize { get; set; }
    public double DeleteValue { get; set; }
    public double XOrigin {get;set;}
    public double YOrigin {get;set;}
    public DenseMatrix Data { get; set; }
    public bool OriginAtCenter {get;set;}
    public double Orientation { get; set; }

    public ASCIIGrid()
    {
      OriginAtCenter = false;
      Orientation = 0;
    }

    public void Load(string FileName)
    {
      using (StreamReader sr = new StreamReader(FileName))
      {
        string line = sr.ReadLine();
        NumberOfColumns = int.Parse(line.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        line = sr.ReadLine();
        NumberOfRows = int.Parse(line.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        line = sr.ReadLine();
        XOrigin = double.Parse(line.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        line = sr.ReadLine();
        YOrigin = double.Parse(line.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        line = sr.ReadLine();
        GridSize = double.Parse(line.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        line = sr.ReadLine();
        DeleteValue = double.Parse(line.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]);

        Data = new DenseMatrix(NumberOfRows, NumberOfColumns);


        string[] DataRead;
        for (int j = 0; j < NumberOfRows; j++)
        {
          DataRead = sr.ReadLine().Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
          for (int i = 0; i < NumberOfColumns; i++)
          {
            Data[NumberOfRows - j - 1, i] = double.Parse(DataRead[i]);
          }
        }
      }
    }

    public void Save(string FileName)
    {
      using (StreamWriter sw = new StreamWriter(FileName, false, Encoding.Default))
      {
        sw.Write(this.ToString());
      }
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
