using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public interface ISource
  {

    double GetValue(Catchment c, DateTime CurrentTime);
    string Name { get;}
    bool Update { get; set; }
  }
}
