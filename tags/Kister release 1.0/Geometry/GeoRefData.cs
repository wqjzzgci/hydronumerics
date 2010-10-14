using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Geometry
{
  public class GeoRefData
  {
    public IGeometry Geometry { get; set; }
    public DataRow Data {get;set;}
  }
}
