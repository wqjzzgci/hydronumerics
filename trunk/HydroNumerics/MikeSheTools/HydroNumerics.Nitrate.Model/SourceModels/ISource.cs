using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public interface ISource
  {

    /// <summary>
    /// Should return the source rate to the catchment in kg/s at the current time
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>
    double GetValue(Catchment c, DateTime CurrentTime);
    string Name { get;}
    bool Update { get; set; }
    void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments);
  }
}
