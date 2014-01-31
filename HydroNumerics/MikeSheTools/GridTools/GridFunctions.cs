using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.MikeSheTools.DFS;
using MathNet.Numerics.LinearAlgebra.Double;

namespace GridTools
{
  public class GridFunctions
  {

    #region Private methods

    /// <summary>
    /// Split a string into double. Splits on "," and ";".
    /// </summary>
    /// <param name="val"></param>
    /// <param name="MaxValue"></param>
    /// <returns></returns>
    private static double[] ParseString(string val)
    {
      string[] vals = val.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

      List<double> ToReturn = new List<double>();

      foreach (string s in vals)
      {
        ToReturn.Add(double.Parse(s));
      }
      return ToReturn.ToArray();
    }


    /// <summary>
    /// Split a string into ints. Splits on "," and ";". If string is empty an array with the values 0 ... MaxValue is returned
    /// </summary>
    /// <param name="val"></param>
    /// <param name="MaxValue"></param>
    /// <returns></returns>
    private static int[] ParseString(string val, int MinValue, int MaxValue)
    {
      string[] vals = val.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

      List<int> ToReturn = new List<int>();

      foreach (string s in vals)
      {
        string[] SeriesSplit = s.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

        int val1 = int.Parse(SeriesSplit[0]);

        if (SeriesSplit.Count() == 2)
        {
          int val2 = int.Parse(SeriesSplit[1]);
          for (int i = val1; i <= val2; i++)
          {
            ToReturn.Add(i);
          }
        }
        else
          ToReturn.Add(val1);
      }


      if (ToReturn.Count() == 0)
      {
        for (int i = MinValue; i <= MaxValue; i++)
          ToReturn.Add(i);
      }
      return ToReturn.ToArray();
    }

    /// <summary>
    /// Sets the same delete values in the created matrix as in the original
    /// </summary>
    /// <param name="Org"></param>
    /// <param name="Created"></param>
    /// <param name="DeleteValue"></param>
    private static void RecreateDeleteValues(Matrix Org, Matrix Created, double DeleteValue)
    {
      for (int l = 0; l < Org.ColumnCount; l++)
        for (int m = 0; m < Org.RowCount; m++)
          if (Org[m, l] == DeleteValue)
            Created[m, l] = DeleteValue;
    }


    #endregion

    /// <summary>
    /// Sums layers from a DFS3 into a DFS2
    /// </summary>
    /// <param name="OperationData"></param>
    public static void LayerSummation(XElement OperationData)
    {
      string Dfs3File = OperationData.Element("DFS3FileName").Value;
      string DFS2OutPut = OperationData.Element("DFS2OutputFileName").Value;

      DFS3.MaxEntriesInBuffer = 1;
      DFS2.MaxEntriesInBuffer = 1;

      DFS3 input = new DFS3(Dfs3File);

      var Items = ParseString(OperationData.Element("Items").Value, 1, input.Items.Count());
      var Layers = ParseString(OperationData.Element("Layers").Value, 0, input.NumberOfLayers - 1);

      DenseMatrix Sumdata = new DenseMatrix(input.NumberOfRows, input.NumberOfColumns);

      //Create the output file and copy info from input file
      DFS2 output = new DFS2(DFS2OutPut, Items.Count());
      output.CopyFromTemplate(input);
      int l = 0;
      //Create the items
      foreach (int j in Items)
      {
        int i = j - 1;
        output.Items[l].EumItem = input.Items[i].EumItem;
        output.Items[l].EumUnit = input.Items[i].EumUnit;
        output.Items[l].Name = input.Items[i].Name;
        l++;
      }

      for (int i = 0; i < input.NumberOfTimeSteps; i++)
      {
        foreach (int j in Items)
        {
          IMatrix3d data = input.GetData(i, j);

          Sumdata = data[Layers[0]];

          for (int k = 1; k < Layers.Count(); k++)
          {
            Sumdata = Sumdata + data[Layers[k]];
          }
          RecreateDeleteValues(data[Layers[0]], Sumdata, input.DeleteValue);

          output.SetData(i, j, Sumdata);
        }
      }
      input.Dispose();
      output.Dispose();
    }

    /// <summary>
    /// Makes a simple a mathematical operation on two items from .dfs2-files
    /// </summary>
    /// <param name="OperationData"></param>
    public static void GridMath(XElement OperationData)
    {
      DFS3.MaxEntriesInBuffer = 1;
      DFS2.MaxEntriesInBuffer = 1;

      string File1 = OperationData.Element("DFS2FileName1").Value;
      int Item1 = int.Parse(OperationData.Element("Item1").Value);

      string Operator = OperationData.Element("MathOperation").Value;

      string File2 = OperationData.Element("DFS2FileName2").Value;
      int Item2 = int.Parse(OperationData.Element("Item2").Value);

      string DFS2OutPut = OperationData.Element("DFS2OutputFileName").Value;

      DFS2 dfsFile1 = new DFS2(File1);
      DFS2 dfsFile2 = new DFS2(File2);

      DFS2 outputFile = new DFS2(DFS2OutPut, 1);
      outputFile.CopyFromTemplate(dfsFile1);

      outputFile.FirstItem.Name = dfsFile1.Items[Item1 - 1].Name + " " + Operator + " " + dfsFile2.Items[Item2 - 1].Name;
      outputFile.FirstItem.EumItem = dfsFile1.Items[Item1 - 1].EumItem;
      outputFile.FirstItem.EumUnit = dfsFile1.Items[Item1 - 1].EumUnit;


      for (int i = 0; i < dfsFile1.NumberOfTimeSteps; i++)
      {
        DenseMatrix M1 = dfsFile1.GetData(i, Item1);
        DenseMatrix M2 = dfsFile2.GetData(i, Item2);
        DenseMatrix M3 = new DenseMatrix(M1.RowCount, M1.ColumnCount);

        switch (Operator)
        {
          case "+":
            M3 = M1 + M2;
            break;
          case "-":
            M3 = M1 - M2;
            break;
          case "*":
            M1.PointwiseMultiply(M2, M3);
            break;
          case "/":
            M1.PointwiseDivide(M2, M3);
            break;
        }
        RecreateDeleteValues(M1, M3, dfsFile1.DeleteValue);
        outputFile.SetData(i, 1, M3);
      }
      dfsFile1.Dispose();
      dfsFile2.Dispose();
      outputFile.Dispose();
    }


    /// <summary>
    /// Does simple factor math on all or selected time steps and items in a dfs-file.
    /// </summary>
    /// <param name="OperationData"></param>
    public static void FactorMath(XElement OperationData)
    {
      DFS3.MaxEntriesInBuffer = 1;
      DFS2.MaxEntriesInBuffer = 1;

      string File1 = OperationData.Element("DFSFileName").Value;

      //DFSOutputFileName is optional. If it exists the input file is copied to this filename
      var outfile = OperationData.Element("DFSOutputFileName");
      if (outfile != null && outfile.Value != "")
      {
        if (File1.ToLower() != outfile.Value.ToLower())
          File.Copy(File1, outfile.Value, true);
        File1 = OperationData.Element("DFSOutputFileName").Value;
      }

      DFSBase dfs = DfsFileFactory.OpenFile(File1);

      int[] Items = ParseString(OperationData.Element("Items").Value, 1, dfs.Items.Count());
      int[] TimeSteps = ParseString(OperationData.Element("TimeSteps").Value, 0, dfs.NumberOfTimeSteps - 1);

      string Operator = OperationData.Element("MathOperation").Value;
      double Factor = double.Parse(OperationData.Element("Factor").Value);

      foreach (int j in TimeSteps)
        foreach (int i in Items)
        {
          switch (Operator)
          {
            case "+":
              dfs.AddToItemTimeStep(j, i, Factor);
              break;
            case "-":
              dfs.AddToItemTimeStep(j, i, -Factor);
              break;
            case "*":
              dfs.MultiplyItemTimeStep(j, i, Factor);
              break;
            case "/":
              dfs.MultiplyItemTimeStep(j, i, 1.0 / Factor);
              break;
            default:
              break;
          }
        }
      dfs.Dispose();
    }

    /// <summary>
    /// Min of values on weekly, monthly or yearly basis
    /// </summary>
    /// <param name="OperationData"></param>
    public static void TimeMin(XElement OperationData)
    {
      TimeAggregation(OperationData, MathType.Min);
    }

    /// <summary>
    /// Max of values on weekly, monthly or yearly basis
    /// </summary>
    /// <param name="OperationData"></param>
    public static void TimeMax(XElement OperationData)
    {
      TimeAggregation(OperationData, MathType.Max);
    }

    /// <summary>
    /// Averages all values on weekly, monthly or yearly basis
    /// </summary>
    /// <param name="OperationData"></param>
    public static void TimeAverage(XElement OperationData)
    {
      TimeAggregation(OperationData, MathType.Average);
    }

    /// <summary>
    /// Sums all values on weekly, monthly or yearly basis
    /// </summary>
    /// <param name="OperationData"></param>
    public static void TimeSummation(XElement OperationData)
    {
      TimeAggregation(OperationData, MathType.Sum);
    }

    /// <summary>
    /// Does either summation or average on weekly, monthly or yearly basis
    /// </summary>
    /// <param name="OperationData"></param>
    /// <param name="sum"></param>
    private static void TimeAggregation(XElement OperationData, MathType mathtype)
    {
      DFS3.MaxEntriesInBuffer = 1;
      DFS2.MaxEntriesInBuffer = 1;

      string File1 = OperationData.Element("DFSFileName").Value;
      DFSBase dfs = DfsFileFactory.OpenFile(File1);
      int[] Items = ParseString(OperationData.Element("Items").Value, 1, dfs.Items.Count());
      string timeinterval = OperationData.Element("TimeInterval").Value.ToLower();
      var Tstep = OperationData.Element("TimeIntervalSteps");
      int timesteps = 1;
      if (Tstep != null)
        timesteps = int.Parse(Tstep.Value);

      string File2;
      bool samefile = true;
      if (OperationData.Element("DFSOutputFileName") != null)
      {
        File2 = OperationData.Element("DFSOutputFileName").Value;
        samefile = false;
      }
      else
      {
        File2 = Path.Combine(Path.GetFileNameWithoutExtension(File1) + "_temp", Path.GetExtension(File1));
      }

      DFSBase outfile = DfsFileFactory.CreateFile(File2, Items.Count());

      outfile.CopyFromTemplate(dfs);

      int k = 0;
      //Create the items
      foreach (int j in Items)
      {
        int i = j - 1;
        outfile.Items[k].EumItem = dfs.Items[i].EumItem;
        outfile.Items[k].EumUnit = dfs.Items[i].EumUnit;
        outfile.Items[k].Name = dfs.Items[i].Name;
        k++;
      }

      switch (timeinterval)
      {
        case "month":
          outfile.TimeOfFirstTimestep = new DateTime(dfs.TimeOfFirstTimestep.Year, dfs.TimeOfFirstTimestep.Month, 15);
          outfile.TimeStep = TimeSpan.FromDays(365.0 / 12 * timesteps);
          dfs.TimeAggregation(Items, outfile, TimeInterval.Month, timesteps, mathtype);
          break;
        case "year":
          outfile.TimeOfFirstTimestep = new DateTime(dfs.TimeOfFirstTimestep.Year, 6, 1);
          outfile.TimeStep = TimeSpan.FromDays(365.0 * timesteps);
          dfs.TimeAggregation(Items, outfile, TimeInterval.Year, timesteps, mathtype);
          break;
        case "day":
          outfile.TimeStep = TimeSpan.FromDays(timesteps);
          dfs.TimeAggregation(Items, outfile, TimeInterval.Day, timesteps, mathtype);
          break;
        default:
          break;
      }

      //Close the files
      dfs.Dispose();
     
      outfile.Dispose();

      if (samefile)
      {
        File.Delete(File1);
        FileInfo f = new FileInfo(File2);
        File.Move(File2, File1);
      }
    }

    /// <summary>
    /// Does simple factor math on all time steps and of selected items in a dfs-file.
    /// A different factor can be used for each month
    /// </summary>
    /// <param name="OperationData"></param>
    public static void MonthlyMath(XElement OperationData)
    {
      DFS3.MaxEntriesInBuffer = 1;
      DFS2.MaxEntriesInBuffer = 1;

      string File1 = OperationData.Element("DFSFileName").Value;

      string Operator = OperationData.Element("MathOperation").Value;

      string[] FactorStrings = OperationData.Element("MonthlyValues").Value.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

      var outfile = OperationData.Element("DFSOutputFileName");
      if (outfile != null && outfile.Value != "")
      {
        if (File1.ToLower() != outfile.Value.ToLower())
          File.Copy(File1, outfile.Value, true);
        File1 = OperationData.Element("DFSOutputFileName").Value;
      }

      DFSBase dfs = DfsFileFactory.OpenFile(File1);

      int[] Items = ParseString(OperationData.Element("Items").Value, 1, dfs.Items.Count());
      int[] TimeSteps = ParseString(OperationData.Element("TimeSteps").Value, 0, dfs.NumberOfTimeSteps - 1);

      double[] Factors = new double[12];
      for (int i = 0; i < 12; i++)
        Factors[i] = double.Parse(FactorStrings[i]);

      foreach (var j in TimeSteps)
      {
        double Factor = Factors[dfs.TimeSteps[j].Month - 1];
        foreach (int i in Items)
        {
          switch (Operator)
          {
            case "+":
              dfs.AddToItemTimeStep(j, i, Factor);
              break;
            case "-":
              dfs.AddToItemTimeStep(j, i, -Factor);
              break;
            case "*":
              dfs.MultiplyItemTimeStep(j, i, Factor);
              break;
            case "/":
              dfs.MultiplyItemTimeStep(j, i, 1.0 / Factor);
              break;
            default:
              break;
          }
        }
      }
      dfs.Dispose();
    }
    /// <summary>
    /// Does simple factor math on all time steps and of selected items in a dfs-file.
    /// A different factor can be used for each month
    /// </summary>
    /// <param name="OperationData"></param>
    public static void Percentile(XElement OperationData)
    {
      DFS3.MaxEntriesInBuffer = 1;
      DFS2.MaxEntriesInBuffer = 1;

      int maxmem = 300; //Uses 300 mB of memory

      string File1 = OperationData.Element("DFSFileName").Value;
      string outfile = OperationData.Element("DFSOutputFileName").Value;

      DFSBase dfsinput = DfsFileFactory.OpenFile(File1);

      double[] Percentiles = ParseString(OperationData.Element("Percentiles").Value);
      int Item = int.Parse((OperationData.Element("Item").Value));
      int[] TimeSteps = ParseString(OperationData.Element("TimeSteps").Value, 0, dfsinput.NumberOfTimeSteps - 1);

      var TimeintervalElement = OperationData.Element("TimeInterval");


      string timeinterval = "";

      //Percentiles are wanted for either each month or each year.
      if (TimeintervalElement != null)
        timeinterval = TimeintervalElement.Value.ToLower();

      List<int> timesteps = new List<int>();
      string ext = Path.GetExtension(outfile);

      switch (timeinterval)
      {

        case "month":
          for (int i = 1; i <= 12; i++)
          {
            timesteps.Clear();
            foreach (int j in TimeSteps)
              if (dfsinput.TimeSteps[j].Month == i)
                timesteps.Add(j);

            if (timesteps.Count > 3)
            {
              string FileName = outfile.Substring(0, outfile.Length - ext.Length) + "_Month_" + i + ext;
              var dfsoutm = DfsFileFactory.CreateFile(FileName, Percentiles.Count());
              dfsoutm.CopyFromTemplate(dfsinput);
              dfsinput.Percentile(1, timesteps.ToArray(), dfsoutm, Percentiles, maxmem);
              dfsoutm.Dispose();
            }
          }
          break;
        case "year":
          int CurrentYear = dfsinput.TimeSteps[TimeSteps.First()].Year;
          foreach (int j in TimeSteps)
          {
            if (CurrentYear == dfsinput.TimeSteps[j].Year)
              timesteps.Add(j);
            else
            {
              if (timesteps.Count > 3)
              {
                string FileName = outfile.Substring(0, outfile.Length - ext.Length) + "_Year_" + CurrentYear + ext;
                var dfsoutm = DfsFileFactory.CreateFile(FileName, Percentiles.Count());
                dfsoutm.CopyFromTemplate(dfsinput);
                dfsinput.Percentile(1, timesteps.ToArray(), dfsoutm, Percentiles, maxmem);
                dfsoutm.Dispose();
              }
              timesteps.Clear();
              CurrentYear = dfsinput.TimeSteps[j].Year;
              timesteps.Add(j);
            }
          }
          break;
        default: //Just do percentile on everything when not month or year
          DFSBase dfs = DfsFileFactory.CreateFile(outfile, Percentiles.Count());
          dfs.CopyFromTemplate(dfsinput);
          dfsinput.Percentile(Item, TimeSteps, dfs, Percentiles, maxmem);
          dfs.Dispose();
          break;
      }
      dfsinput.Dispose();
    }


    public static void InsertPointValues(XElement OperationData)
    {
      string filename = OperationData.Element("DFSFileName").Value;

      int Item = OperationData.Element("Item") == null ? 1 : int.Parse(OperationData.Element("Item").Value);
      bool ClearValues = OperationData.Element("ClearValues") == null ? true: bool.Parse(OperationData.Element("ClearValues").Value);

      List<Tuple<double, double, int, int, double>> points = new List<Tuple<double, double, int, int, double>>();

      foreach (var p in OperationData.Element("Points").Elements())
      {
        Tuple<double, double, int, int, double> point = new Tuple<double, double, int, int, double>(
          p.Element("X") == null ? -1 : double.Parse(p.Element("X").Value),
          p.Element("Y") == null ? -1 : double.Parse(p.Element("Y").Value),
          p.Element("Z") == null ? 0 : int.Parse(p.Element("Z").Value),
          p.Element("TimeStep") == null ? 0 : int.Parse(p.Element("TimeStep").Value),
          double.Parse(p.Element("Value").Value));
        points.Add(point);
      }

      if (Path.GetExtension(filename).EndsWith("0"))
      {
        using (DFS0 dfs = new DFS0(filename))
        {
          if (ClearValues)
          {
            for (int i = 0; i < dfs.NumberOfTimeSteps; i++)
            {
              dfs.SetData(i, Item, 0);
            }
          }
          foreach (var p in points)
          {
            dfs.SetData(p.Item4, Item, p.Item5);
          }
        }
      }
      else if (Path.GetExtension(filename).EndsWith("2"))
      {
        using (DFS2 dfs = new DFS2(filename))
        {
          if (ClearValues)
          {
            for (int i = 0; i < dfs.NumberOfTimeSteps; i++)
            {
              dfs.SetData(i, Item, new DenseMatrix(dfs.NumberOfRows, dfs.NumberOfColumns));
            }
          }
          foreach (var p in points)
          {
            var data = dfs.GetData(p.Item4, Item);
            int column = dfs.GetColumnIndex(p.Item1);
            int row = dfs.GetRowIndex(p.Item2);

            if (column >= 0 & row >= 0)
              data[row, column] = p.Item5;

            dfs.SetData(p.Item4, Item, data);
          }
        }
      }
      else if (Path.GetExtension(filename).EndsWith("3"))
      {
        using (DFS3 dfs = new DFS3(filename))
        {
          if (ClearValues)
          {
            for (int i = 0; i < dfs.NumberOfTimeSteps; i++)
            {
              dfs.SetData(i, Item, new Matrix3d(dfs.NumberOfRows, dfs.NumberOfColumns, dfs.NumberOfLayers));
            }
          }
          foreach (var p in points)
          {
            var data = dfs.GetData(p.Item4, Item);
            int column = dfs.GetColumnIndex(p.Item1);
            int row = dfs.GetRowIndex(p.Item2);

            if (column >= 0 & row >= 0)
              data[row, column, p.Item3] = p.Item5;

            dfs.SetData(p.Item4, Item, data);
          }
        }
      }
    }
  }
}
  
    
