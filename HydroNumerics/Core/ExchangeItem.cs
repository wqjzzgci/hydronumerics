using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
    public class ExchangeItem
    {
        private Unit unit;
        private string description;
        private string location;
        private string quantity;
        private double exchangeValue;
                
        public Unit Unit 
        {
            get { return unit;}
            set { unit = value;}
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

        public string Qauntity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public double ExchangeValue
        {
            get { return exchangeValue; }
            set { exchangeValue = value; }
        }

    }
}
