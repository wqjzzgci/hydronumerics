using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;


namespace HydroNumerics.Tough2.ViewModel
{
  public class TimesOutput : KeyedCollection<string, DataColumn>
  {

    public TimesOutput() : base() { }

    protected override string GetKeyForItem(DataColumn item)
    {
      // In this example, the key is the part number.
      return item.Name; 
    }

    /// <summary>
    /// Get the keys for the columns
    /// </summary>
    public IEnumerable<string> Keys
    {
      get
      {
        return this.Dictionary.Keys;
      }
    }

    /// <summary>
    /// Gets the print out time
    /// </summary>
    public TimeSpan TotalTime { get; private set; }


    public TimesOutput(TimeSpan TotalTime, ElementCollection elements)
    {
      this.TotalTime = TotalTime;
      Elements = elements;
    }

    ElementCollection Elements { get; set; }
    int _linesRead = 0;
    int _columnsOffSet = 0;

    /// <summary>
    /// Indicates which block of data is being read
    /// </summary>
    public BlockPointer Pointer { get; set; }

    /// <summary>
    /// Reads a line of the output matrix. Returns true when it wants the next line. Returns false when it does not expect more data
    /// </summary>
    /// <param name="Line"></param>
    /// <returns></returns>
    public bool ReadLine(string Line)
    {
      if (_linesRead == 0) //HeadLine
      {
        string[] Headers = Line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        if (Pointer == BlockPointer.FirstMatrixOutput)
        {
          _columnsOffSet = 0;
        }
        else
          _columnsOffSet = this.Count;

        for (int i = 2; i < Headers.Length; i++)
        {
          this.Add(new DataColumn(Headers[i], Elements.Count));
        }

      }
      else if (Line.StartsWith("     ")) //Units
      {
      }
      else if (Line.StartsWith(" ELEM")) //Header repeat
      {
      }
      else if (Line.Length < 3)
      {
      }
      else //data
      {
        string[] data = Line.Substring(12).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        int index = int.Parse(Line.Substring(7, 6));

        var dic = new Dictionary<string, double>();

        if (Pointer == BlockPointer.FirstMatrixOutput) 
          Elements[index - 1].PrintData.Add(TotalTime, dic);

        for (int i = 0; i < data.Length; i++)
        {
          double d = 0;
          double.TryParse(data[i], out d);
          this[i + _columnsOffSet][index - 1] = d;

          dic.Add(this.GetKeyForItem(this[i + _columnsOffSet]),d);
        }

        //No more data. Do not come back
        if (index == Elements.Count())
        {
          _linesRead = 0;
          return false;
        }
      }
      _linesRead++;

      return true;
    }
  }
}
