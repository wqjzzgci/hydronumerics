using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Geometry;


namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public abstract class AbstractBoundary:IDObject
  {
    /// <summary>
    /// Gets the output from this boundary
    /// </summary>
    [DataMember]
    public TimeSeriesGroup Output { get; protected set; }

    /// <summary>
    /// Gets the list of exchange items
    /// </summary>
    [DataMember]
    public List<GeoExchangeItem> ExchangeItems { get; protected set; }
     
    /// <summary>
    /// Gets and sets the Contact polygon for the boundary
    /// </summary>
    [DataMember]
    public IGeometry ContactGeometry {get;set;}

    /// <summary>
    /// Default constructor. Must be called.
    /// </summary>
    public AbstractBoundary()
    {
      Output = new TimeSeriesGroup();
      ExchangeItems = new List<GeoExchangeItem>();
    }


    /// <summary>
    /// Resets the ouput
    /// </summary>
    /// <param name="Time"></param>
    public void ResetOutputTo(DateTime Time)
    {
      Output.ResetToTime(Time);
    }


  }
}
