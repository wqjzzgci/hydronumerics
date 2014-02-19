using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public interface ISource:INitrateModel
  {

    /// <summary>
    /// Should return the source rate to the catchment in kg/s at the current time
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>
    double GetValue(Catchment c, DateTime CurrentTime);
  }
}
