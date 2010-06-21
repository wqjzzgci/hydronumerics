using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace HydroNumerics.Core
{
  [DataContract]
    public class ExchangeItem
    {
    [DataMember]
      public Unit Unit { get; private set; }
    [DataMember]
    private string description;
    [DataMember]
    private string location;
    [DataMember]
    private string quantity;
    [DataMember]
    private double exchangeValue;
    [DataMember]
    public bool IsInput { get; set; }
    [DataMember]
    public bool IsOutput { get; set; }


        public ExchangeItem(string location, string quantity, Unit unit)
        {
          Location = location;
          Quantity = quantity;
          this.Unit = unit;
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public string Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public virtual double ExchangeValue
        {
            get { return exchangeValue; }
            set { exchangeValue = value; }
        }

    }
}
