using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Time.Core;
using HydroNumerics.OpenMI.Sdk.Backbone;

namespace HydroNumerics.Time.OpenMI
{
    public class TsQuantity : Quantity
    {
        BaseTimeSeries baseTimeSeries;

        public BaseTimeSeries BaseTimeSeries
        {
            get
            {
                return baseTimeSeries;
            }
            set
            {
                baseTimeSeries = value;
            }
        }
    }
}
