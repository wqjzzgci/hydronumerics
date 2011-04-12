using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.MikeSheTools.DFS;
using MathNet.Numerics.LinearAlgebra;

namespace GridTools
{
  public class GridFunctions
  {

    #region Private methods

    /// <summary>
    /// Split a string into ints. Splits on "," and ";". If string is empty an array with the values 0 ... MaxValue is returned
    /// </summary>
    /// <param name="val"></param>
    /// <param name="MaxValue"></param>
    /// <returns></returns>
    private static int[] ParseString(string val, int MinValue, int MaxValue)
    {
      string[] vals = val.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

      int[] ToReturn = new int[vals.Count()];

      for (int i = 0; i < vals.Count(); i++)
      {
        ToReturn[i] = int.Parse(vals[i]);
      }

      if (ToReturn.Count() == 0)
      {
        ToReturn = new int[MaxValue];
        for (int i = 0; i < ToReturn.Count(); i++)
          ToReturn[i] = i+ MinValue;
      }
      return ToReturn;
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

      int DFS3savemaxentries = DFS3.MaxEntriesInBuffer;
      int DFS2savemaxentries = DFS2.MaxEntriesInBuffer;
      DFS3.MaxEntriesInBuffer = 1;
      DFS2.MaxEntriesInBuffer = 1;

      DFS3 input = new DFS3(Dfs3File);

      var Items = ParseString(OperationData.Element("Items").Value, 1, input.Items.Count());
      var Layers = ParseString(OperationData.Element("Layers").Value, 0, input.NumberOfLayers);

      Matrix Sumdata = new Matrix(input.NumberOfRows, input.NumberOfColumns);

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

          output.SetData(Sumdata);
        }
      }
      DFS3.MaxEntriesInBuffer = DFS3savemaxentries;
      DFS2.MaxEntriesInBuffer = DFS2savemaxentries;
      input.Dispose();
      output.Dispose();
    }

    /// <summary>
    /// Makes a simple a mathematical operation on two items from .dfs2-files
    /// </summary>
    /// <param name="OperationData"></param>
    public static void GridMath(XElement OperationData)
    {
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
        Matrix M1 = dfsFile1.GetData(i, Item1);
        Matrix M2 = dfsFile2.GetData(i, Item2);
        Matrix M3 = null;

        switch (Operator)
        {
          case "+":
            M3 = M1 + M2;
            break;
          case "-":
            M3 = M1 - M2;
            break;
          case "*":
            M3 = M1.Clone();
            M3.ArrayMultiply(M2);
            break;
          case "/":
            M3 = M1.Clone();
            M3.ArrayDivide(M2);
            break;
        }
        RecreateDeleteValues(M1, M3, dfsFile1.DeleteValue);
        outputFile.SetData(M3);
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
      string File1 = OperationData.Element("DFSFileName").Value;

      //DFSOutputFileName is optional. If it exists the input file is copied to this filename
      var outfile = OperationData.Element("DFSOutputFileName");
      if (outfile != null)
      {
        if (File1.ToLower()!=outfile.Value.ToLower())
          File.Copy(File1, outfile.Value, true);
        File1 = OperationData.Element("DFSOutputFileName").Value;
      }

      DFSBase dfs = DfsFileFactory.OpenFile(File1);

      int[] Items = ParseString(OperationData.Element("Items").Value,1, dfs.Items.Count());
      int[] TimeSteps = ParseString(OperationData.Element("TimeSteps").Value, 0, dfs.NumberOfTimeSteps);

      string Operator = OperationData.Element("MathOperation").Value;
      double Factor = double.Parse(OperationData.Element("Factor").Value);

      foreach (int j in TimeSteps)
        foreach (int i in Items)
        {
          switch (Operator)
          {
            case "+":
              dfs.AddItemTimeStep(j, i, Factor);
              break;
            case "-":
              dfs.AddItemTimeStep(j, i, -Factor);
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
    /// Sums all values on weekly, monthly or yearly basis
    /// </summary>
    /// <param name="OperationData"></param>
    public static void TimeSummation(XElement OperationData)
    {
      string File1 = OperationData.Element("DFSFileName").Value;
      DFSBase dfs = DfsFileFactory.OpenFile(File1);
      int[] Items = ParseString(OperationData.Element("Items").Value, 1, dfs.Items.Count());
      string timeinterval = OperationData.Element("TimeInterval").Value.ToLower();

      string File2 = OperationData.Element("DFSOutputFileName").Value;

      DFSBase outfile = DfsFileFactory.CreateFile(File2,Items.Count());

      outfile.CopyFromTemplate(dfs);

      int k = 0;
      //Create the items
      foreach (int j in Items)
      {
        int i =j-1;
        outfile.Items[k].EumItem = dfs.Items[i].EumItem;
        outfile.Items[k].EumUnit = dfs.Items[i].EumUnit;
        outfile.Items[k].Name = dfs.Items[i].Name;
        k++;
      }

      switch (timeinterval)
      {
        case "month":
          outfile.TimeOfFirstTimestep = new DateTime(dfs.TimeOfFirstTimestep.Year, dfs.TimeOfFirstTimestep.Month, 15);
          outfile.TimeStep = TimeSpan.FromDays(365.0 / 12);
          dfs.TimeSummation(Items, outfile, TimeInterval.Month);
          break;
        case "year":
          outfile.TimeOfFirstTimestep = new DateTime(dfs.TimeOfFirstTimestep.Year, 6, 1);
          outfile.TimeStep = TimeSpan.FromDays(365.0);
          dfs.TimeSummation(Items, outfile, TimeInterval.Year);
          break;
        case "week":
          outfile.TimeStep = TimeSpan.FromDays(7);
          dfs.TimeSummation(Items, outfile, TimeInterval.Week);
          break;
        default:
          break;
      }

      //Close the files
      dfs.Dispose();
      outfile.Dispose();
    }

    /// <summary>
    /// Does simple factor math on all time steps and of selected items in a dfs-file.
    /// A different factor can be used for each month
    /// </summary>
    /// <param name="OperationData"></param>
    public static void MonthlyMath(XElement OperationData)
    {
      string File1 = OperationData.Element("DFSFileName").Value;

      string Operator = OperationData.Element("MathOperation").Value;

      string[] FactorStrings = OperationData.Element("MonthlyValues").Value.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

      var outfile = OperationData.Element("DFSOutputFileName");
      if (outfile != null)
      {
        if (File1.ToLower() != outfile.Value.ToLower())
          File.Copy(File1, outfile.Value, true);
        File1 = OperationData.Element("DFSOutputFileName").Value;
      }

      DFSBase dfs = DfsFileFactory.OpenFile(File1);

      int[] Items = ParseString(OperationData.Element("Items").Value, 1, dfs.Items.Count());


      double[] Factors = new double[12];
      for (int i = 0; i < 12; i++)
        Factors[i] = double.Parse(FactorStrings[i]);

      for (int j = 0; j < dfs.TimeSteps.Count(); j++)
      {
        double Factor = Factors[dfs.TimeSteps[j].Month - 1];
        foreach (int i in Items)
        {
          switch (Operator)
          {
            case "+":
              dfs.AddItemTimeStep(j, i, Factor);
              break;
            case "-":
              dfs.AddItemTimeStep(j, i, -Factor);
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


  }
}
  
    
