using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HydroNumerics.Tough2.ViewModel
{
  public class DataColumn:Vector
  {
    public string Name { get;  private set; }
    public string Description { get; set; }
    public string Unit { get; set; }

    public DataColumn(string Name, int n):base(n)
    {
      
      this.Name = Name;
    }

    protected override void At(int index, double value)
    {
     
    }

    protected override double At(int index)
    {
      throw new NotImplementedException();
    }

    public override MathNet.Numerics.LinearAlgebra.Generic.Matrix<double> CreateMatrix(int rows, int columns)
    {
      throw new NotImplementedException();
    }

    public override MathNet.Numerics.LinearAlgebra.Generic.Vector<double> CreateVector(int size)
    {
      throw new NotImplementedException();
    }
  }
}
