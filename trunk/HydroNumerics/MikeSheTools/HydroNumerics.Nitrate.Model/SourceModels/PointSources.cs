using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class PointSources : BaseViewModel, ISource
  {

    public PointSources(XElement Configuration)
    {
      Name = Configuration.Attribute("Type").Value;
      Update = bool.Parse(Configuration.Element("Update").Value);

      string filename = Configuration.Element("FileName").Value;
    }

    public bool Update { get; set; }

    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      return c.Geometry.GetArea() * 1.1;
    }
    public void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
    }

  }
}
