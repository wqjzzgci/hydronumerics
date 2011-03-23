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

    public static void LayerSummationFunc(XElement OperationData)
    {
      string Dfs3File = OperationData.Element("DFS3FileName").Value;
      string DFS2OutPut = OperationData.Element("DFS2OutputFileName").Value;

      DFS3 input = new DFS3(Dfs3File);

      Matrix Sumdata = new Matrix(input.NumberOfRows, input.NumberOfColumns);

      int nitems = input.Items.Count();

      DFS2 output = new DFS2(DFS2OutPut, "LayerSum", nitems);
      output.NumberOfColumns = input.NumberOfColumns;
      output.NumberOfRows = input.NumberOfRows;
      output.GridSize = input.GridSize;

      
      for (int i = 0; i < input.NumberOfTimeSteps; i++)
      {
        for (int j = 1; j <= nitems; j++)
        {
          IMatrix3d data = input.GetData(i,j);

          Sumdata = data[0];

          for (int k = 1; k < input.NumberOfLayers; k++)
          {
            Sumdata = Sumdata + data[k];
          }

          output.SetData(i, j, Sumdata);
        }
      }
    }
  }
}
