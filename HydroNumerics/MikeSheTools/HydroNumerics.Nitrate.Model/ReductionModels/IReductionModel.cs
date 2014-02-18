using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace HydroNumerics.Nitrate.Model
{
  public interface IReductionModel
  {
    double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime);
    void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments);
    string Name { get; }
    bool Update { get; set; }
  }
}
