using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public interface IFluid
  {
     double Pressure { get;}
     double Viscosity {get;}
     double Density { get;}
  }
}
