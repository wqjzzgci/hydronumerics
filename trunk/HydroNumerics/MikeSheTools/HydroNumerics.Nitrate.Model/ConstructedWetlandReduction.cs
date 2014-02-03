using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class ConstructedWetlandReduction:BaseViewModel, IReductionModel
  {

    public ConstructedWetlandReduction(XElement Configuration)
    {
      Name = "Reduction in constructed wetlands";
    }
    
    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      throw new NotImplementedException();
    }

    public bool Calculate{get;set;}
  }
}
