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

    [DataMember]
    public TimeSeriesGroup Output { get; protected set; }

    [DataMember]
    protected TimespanSeries ts;

    [DataMember]
    protected List<GeoExchangeItem> _exchangeItems = new List<GeoExchangeItem>();

    [DataMember]
    //protected GeoExchangeItem _flow;
    public double Flow { get; set; }


    public AbstractBoundary()
    {
      Output = new TimeSeriesGroup();
      ts = new TimespanSeries();
      
      ts.Name = "Flow";
      Output.Items.Add(ts);
    }

  
    /// <summary>
    /// Gets and sets the Contact polygon for the boundary
    /// </summary>
    [DataMember]
    public IGeometry ContactGeometry {get;set;}

    public List<GeoExchangeItem> ExchangeItems
    {
      get
      {
        return _exchangeItems;
      }
    }

    public void ResetOutputTo(DateTime Time)
    {
      Output.ResetToTime(Time);
    }


    public virtual void Initialize()
    {
    }

    public virtual void ReceiveSinkWater(DateTime Start, TimeSpan TimeStep, IWaterPacket Water)
    {
      Flow = -Water.Volume/TimeStep.TotalSeconds;
      ts.AddSiValue(Start, Start.Add(TimeStep), Flow);
    }


  }
}
