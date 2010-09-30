using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;


namespace HydroNumerics.HydroCat.Core
{
    public static class Units
    {
        public static Unit Millimiters
        {
            get
            {
                return new HydroNumerics.Core.Unit("millimiters", 0.001, 0.0, "millimiters", new Dimension(1, 0, 0, 0, 0, 0, 0, 0));
            }
        }

        public static Unit MmPrDay
        {
            get
            {
                return new HydroNumerics.Core.Unit("mm pr day", 1.0 / (1000 * 3600 * 24), 0, "millimiters pr day", new Dimension(1, 0, -1, 0, 0, 0, 0, 0));
            }
        }

        public static Unit M3PrSec
        {
            get
            {
                return new HydroNumerics.Core.Unit("m3 pr sec.", 1.0, 0.0, "cubic meters pr second", new Dimension(3, 0, -1, 0, 0, 0, 0, 0));
            }
        }

        public static Unit Centigrade
        {
            get
            {
                return new HydroNumerics.Core.Unit("Centigrade", 1.0, -273.15, "degree centigrade", new Dimension(0, 0, 0, 0, 1, 0, 0, 0));
            }
        }

    }
}
