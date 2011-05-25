using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace HydroNumerics.Tough2.ViewModel
{
  public class DataColumn:Vector
  {
    public string Name { get;  private set; }
    public string Description { get; set; }
    public string Unit { get; set; }

    public DataColumn(string Name, int n)
      : base(n)
    {
      this.Name = Name;
    }
  }
}
