using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace HydroNumerics.Nitrate.Model
{
  public interface ISink:INitrateModel
  {
    double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime);
  }
}
