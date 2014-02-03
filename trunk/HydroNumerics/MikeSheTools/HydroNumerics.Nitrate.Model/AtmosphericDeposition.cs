using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class AtmosphericDeposition:BaseViewModel,ISource
  {

    public AtmosphericDeposition(XElement Configuration)
    {
      //FileNames
      Name = "Atmospheric deposition";
    }

    public bool Calculate { get; set; }

    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      return c.Geometry.GetArea() * 1.1;
    }

  }
}
