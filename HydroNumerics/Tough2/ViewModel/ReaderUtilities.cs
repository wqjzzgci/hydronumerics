using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  /// <summary>
  /// Extension of stream reader class that stores everything read through ReadLine()-method
  /// </summary>
  public class ReaderUtilities:StreamReader
  {
    public StringBuilder FileContent { get; private set; }

    public ReaderUtilities(string FileName) : base(FileName) 
    {
      FileContent = new StringBuilder();
    }

    public override string ReadLine()
    {
      string line = base.ReadLine();
      FileContent.AppendLine(line);
      return line;
    }

    public static string JoinIntoString(double[] data, int FieldLength)
    {
      string FormatString="";

      if (FieldLength == 20)
        FormatString = "0.0000000000000E+00";
      if (FieldLength == 10)
        FormatString = "0.000E+00";

      StringBuilder outline = new StringBuilder();

      foreach (var v in data)
        outline.Append(" "+ v.ToString(FormatString));

      return outline.ToString();

    }


    public static double[] SplitIntoDoubles(string Line,int startindex, int FieldLength)
    {
      int ndata;
      if (startindex>Line.Length)
        ndata = 0;
      else
        ndata =Line.Substring(startindex).TrimEnd().Length / FieldLength;
      
      double[] data = new double[ndata];

      for (int i = 0; i < ndata; i++)
      {
        double d = 0;

        double.TryParse(Line.Substring(i * FieldLength + startindex, FieldLength), out d);
        data[i] = d;
      }

      return data;
    }
  }
}
