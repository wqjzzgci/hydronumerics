using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public interface IReductionModel
  {

    double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime);
    string Name { get;}
    bool Update { get; set; }

  }
}
