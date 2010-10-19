using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

using HydroNumerics.Core;
using HydroNumerics.Geometry;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class GeoExchangeItem : ExchangeItem, INotifyPropertyChanged
  {
    public GeoExchangeItem()
      : base()
    {
      Geometry = null;
    }

    public GeoExchangeItem(string location, string quantity, Unit unit, TimeType timetype, IGeometry geo)
      : base(location, quantity, unit, timetype)
    {
      Geometry = geo;
    }

    [DataMember]
    public IGeometry Geometry { get; set; }


    public override double ExchangeValue
    {
      get
      {
        return base.ExchangeValue;
      }
      set
      {
        if (value != base.ExchangeValue)
        {
          base.ExchangeValue = value;
          NotifyPropertyChanged("ExchangeValue");
        }
      }
    }



    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion
  }
}
