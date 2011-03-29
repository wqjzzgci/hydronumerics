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
    private static int[] ParseString(string val, int MaxValue)
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
          ToReturn[i] = i;
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

      var Layers = ParseString(OperationData.Element("Layers").Value, input.NumberOfLayers);

      Matrix Sumdata = new Matrix(input.NumberOfRows, input.NumberOfColumns);


      DFS2 output = new DFS2(DFS2OutPut, input);

      for (int i = 0; i < input.NumberOfTimeSteps; i++)
      {
        for (int j = 1; j <= input.Items.Count(); j++)
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

      outputFile.FirstItem.Name = dfsFile1.Items[Item1 - 1].Name + " " + Operator + " " + dfsFile2.Items[Item2 - 1];
      outputFile.FirstItem.EumItem = dfsFile1.Items[Item1 - 1].EumItem;
      outputFile.FirstItem.EumUnit = dfsFile1.Items[Item1 - 1].EumUnit;


      for (int i = 0; i < dfsFile1.NumberOfTimeSteps; i++)
      {
        Matrix M1 = dfsFile1.GetData(i, Item1);
        Matrix M2 = dfsFile1.GetData(i, Item2);
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


    public static void FactorMath(XElement OperationData)
    {
      string File1 = OperationData.Element("DFSFileName").Value;
      DFSBase dfs = DfsFileFactory.OpenFile(File1);

      int[] Items = ParseString(OperationData.Element("Items").Value, dfs.Items.Count());
      int[] TimeSteps = ParseString(OperationData.Element("TimeSteps").Value, dfs.NumberOfTimeSteps);

      string Operator = OperationData.Element("MathOperation").Value;
      double Factor = double.Parse(OperationData.Element("Factor").Value);

      foreach (int j in TimeSteps)
        foreach (int i in Items)
        {
          switch (Operator)
          {
            case "+":
              dfs.AddItemTimeStep(j, i +1, Factor);
              break;
            case "-":
              dfs.AddItemTimeStep(j, i +1, -Factor);
              break;
            case "*":
              dfs.MultiplyItemTimeStep(j, i +1, Factor);
              break;
            case "/":
              dfs.MultiplyItemTimeStep(j, i+1, 1.0 / Factor);
              break;
            default:
              break;
          }
        }
      dfs.Dispose();
    }
  }
}
  
    
