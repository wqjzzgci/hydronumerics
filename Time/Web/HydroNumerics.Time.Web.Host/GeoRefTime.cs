using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.Time.Core
{
  [DataContract]
  public class GeoXYPointTime : TimestampSeries
  {
    [DataMember]
    public XYPoint Geometry { get; set; }
    
  }
}