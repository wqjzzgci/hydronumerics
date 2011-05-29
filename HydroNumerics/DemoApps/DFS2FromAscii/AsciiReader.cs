using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra.Double;

namespace DFS2FromAscii
{
  public class AsciiReader
  {
    public int NumberOfColumns { get; private set; }
    public int NumberOfRows { get; private set; }
    public double XOrigin { get; private set; }
    public double YOrigin { get; private set; }
    public double GridSize { get; private set; }
    public double DeleteValue { get; private set; }
    public DenseMatrix Data { get; private set; }
    public string FileName { get; private set; }
    
    public AsciiReader(string FileName)
    {
      this.FileName = FileName;
      using (StreamReader sr = new StreamReader(FileName))
      {
        string line = sr.ReadLine();
        NumberOfColumns = int.Parse(line.Split(new Char[]{' '},StringSplitOptions.RemoveEmptyEntries)[1]);
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
            Data[NumberOfRows-j-1, i] = double.Parse(DataRead[i]);
          }
        }

      }
    }
  }
}
