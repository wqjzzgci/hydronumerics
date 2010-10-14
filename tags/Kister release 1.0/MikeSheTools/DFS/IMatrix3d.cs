using System;
namespace HydroNumerics.MikeSheTools.DFS
{
  public interface IMatrix3d
  {
    int LayerCount { get; }
    MathNet.Numerics.LinearAlgebra.Matrix this[int Layer] { get; set; }
    MathNet.Numerics.LinearAlgebra.Vector this[int Row, int Column] { get; set; }
    double this[int Row, int Column, int Layer] { get; set; }
  }
}
