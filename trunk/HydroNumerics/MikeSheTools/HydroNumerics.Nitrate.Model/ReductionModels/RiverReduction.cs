using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;


namespace HydroNumerics.Nitrate.Model
{
  public class RiverReduction : BaseViewModel, IReductionModel
  {


    public RiverReduction(XElement Configuration)
    {
      Name = Configuration.Attribute("Type").Value;
      Update = bool.Parse(Configuration.Element("Update").Value);
    }


    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      return CurrentMass * 0.1; ;
    }

    public bool Update { get; set; }
  }
}
