using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;


namespace HydroNumerics.MikeSheTools.DFS
{
  public class Matrix3d : IMatrix3d
  {
    private DenseMatrix[] _data;

    public Matrix3d(int NumberOfRows, int NumberOfColumns, int NumberOfLayers)
    {
      _data = new DenseMatrix[NumberOfLayers];
      
      for (int i = 0; i < NumberOfLayers; i++)
        _data[i] = new DenseMatrix(NumberOfRows, NumberOfColumns);
    }

    /// <summary>
    /// Gets the number of layers
    /// </summary>
    public int LayerCount
    {
      get { return _data.Length; }
    }

    /// <summary>
    /// Gets and sets a value using indeces.
    /// </summary>
    /// <param name="Row"></param>
    /// <param name="Column"></param>
    /// <param name="Layer"></param>
    /// <returns></returns>
    public double this[int Row, int Column, int Layer]
    {
      get
      {
        return _data[Layer][Row, Column];
      }
      set
      {
        _data[Layer][Row, Column] = value;
      }
    }
    
    /// <summary>
    /// Gets and sets data in a column using indeces 
    /// </summary>
    /// <param name="Row"></param>
    /// <param name="Column"></param>
    /// <returns></returns>
    public DenseVector this[int Row, int Column]
    {
      get
      {
        DenseVector V = new DenseVector(_data.Length);

        for (int i = 0; i < _data.Length; i++)
          V[i] = this[Row, Column, i];
        return V;
      }
      set
      {
        if (value.Count != _data.Length)
          throw new Exception("Number of elements in Vector is not equal to the number of layers in the 3D object");
        for (int i = 0; i < _data.Length; i++)
          this[Row, Column, i]=value[i];
      }
    }


    /// <summary>
    /// Gets and sets a matrix using index 
    /// </summary>
    /// <param name="Layer"></param>
    /// <returns></returns>
    public DenseMatrix this[int Layer]
    {
      get
      {
        return _data[Layer];
      }
      set
      {
        _data[Layer] = value;
      }
    }
  }
}
