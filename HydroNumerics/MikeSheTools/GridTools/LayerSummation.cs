using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.MikeSheTools.DFS;
using MathNet.Numerics.LinearAlgebra;

namespace GridTools
{
  public class LayerSummation
  {

    /// <summary>
    /// Split a string into ints. Splits on "," and ";". If string is empty an array with the values 0 ... MaxValue is returned
    /// </summary>
    /// <param name="val"></param>
    /// <param name="MaxValue"></param>
    /// <returns></returns>
    public static int[] ParseString(string val, int MaxValue)
    {
      string[] vals = val.Split(new string[]{",",";"}, StringSplitOptions.RemoveEmptyEntries);

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
    /// Sums layers from a DFS3 into a DFS2
    /// </summary>
    /// <param name="OperationData"></param>
    public static void LayerSummationFunc(XElement OperationData)
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
          IMatrix3d data = input.GetData(i,j);

          Sumdata = data[Layers[0]];

          for (int k = 1; k < Layers.Count(); k++)
          {
            Sumdata = Sumdata + data[Layers[k]];
          }

          for (int l = 0; l < input.NumberOfColumns; l++)
            for (int m = 0; m < input.NumberOfRows; m++)
              if (data[m, l,0] == input.DeleteValue)
                Sumdata[m, l] = input.DeleteValue;

          output.SetData(i, j, Sumdata);
        }
      }
      DFS3.MaxEntriesInBuffer = DFS3savemaxentries;
      DFS2.MaxEntriesInBuffer = DFS2savemaxentries;
      input.Dispose();
      output.Dispose();
 
    }


    private static MathOperator GetOperator(string Operator)
    {
      return (MathOperator)Enum.Parse(typeof(MathOperator), Operator);
    }

    public static void GridMath(XElement OperationData)
    {
      string File1 = OperationData.Element("DFS2FileName1").Value;
      int Item1 = int.Parse(OperationData.Element("Item1").Value);
     
      MathOperator Operator = GetOperator( OperationData.Element("MathOperation").Value);
      
      string File2 = OperationData.Element("DFS2FileName2").Value;
      int Item2 = int.Parse(OperationData.Element("Item1").Value);

      string DFS2OutPut = OperationData.Element("DFS2OutputFileName").Value;

      DFS2 dfsFile1 = new DFS2(File1);
      DFS2 dfsFile2 = new DFS2(File2);

      DFS2 outputFile = new DFS2(DFS2OutPut, dfsFile1);


      for (int i = 0; i < dfsFile1.NumberOfTimeSteps; i++)
      {
        Matrix M1 = dfsFile1.GetData(i, Item1);
        Matrix M2 = dfsFile1.GetData(i, Item2);
        Matrix M3 =null;

        switch (Operator)
        {
          case MathOperator.Addition:
            M3 = M1 + M2;
            break;
          case MathOperator.Substraction:
            M3 = M1 - M2;
            break;
          case MathOperator.Multiplication:
            M3 = M1 * M2;
            break;
          case MathOperator.Division:
            M3 = M1 * M2.Inverse();
            break;
        }
        outputFile.SetData(M3);
      }

    }
  
  
  }
}
