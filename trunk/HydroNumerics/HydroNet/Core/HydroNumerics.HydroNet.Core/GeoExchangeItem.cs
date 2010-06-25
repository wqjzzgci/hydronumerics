using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

using HydroNumerics.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class GeoExchangeItem:ExchangeItem
  {
    public GeoExchangeItem(string location,string quantity, Unit unit, TimeType timetype, IGeometry geo):base(location,quantity,unit,timetype)
    {
      Geometry = geo;
    }

    [DataMember]
    public IGeometry Geometry { get; set; }
  }
}
