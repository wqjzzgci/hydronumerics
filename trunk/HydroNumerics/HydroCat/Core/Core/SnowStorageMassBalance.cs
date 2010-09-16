using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroCat.Core
{
    public class SnowStorageMassBalance
    {
        [DescriptionAttribute("Accumulated snowfall [Unit: Millimiters"), CategoryAttribute("Inflow")]
        public double Snowfall { get; private set; }
        [DescriptionAttribute("Accumulated Snowmelt (Evapotranspiration) [Unit: Millimiters"), CategoryAttribute("Outflow")]
        public double Snowmelt { get; private set; }

        [DescriptionAttribute("Initial storage [Unit: Millimiters"), CategoryAttribute("Storage")]
        public double InitialStorage { get; private set; }
        [DescriptionAttribute("Current storage [Unit: Millimiters"), CategoryAttribute("Storage")]
        public double Storage { get; private set; }
        [DescriptionAttribute("Massbalance error [Unit: Millimiters"), CategoryAttribute("Mass balance Error")]
        public double MassBalanceError
        {
            get
            {
                return Snowfall - Snowmelt - (Storage - InitialStorage);
            }
        }
        [DescriptionAttribute("Maximum mass balance error during the simulation [Unit: Millimiters"), CategoryAttribute("Mass balance Error")]
        public double MaxMassBalanceError { get; private set; }

        public void SetValues(double snowfall, double snowmelt, double storage)
        {
            Snowfall += snowfall;
            Snowmelt += snowmelt;
            Storage = storage;
            MaxMassBalanceError = Math.Max(MaxMassBalanceError, MassBalanceError);
        }

        public void Initialize(double initialSnowStorage)
        {
            InitialStorage = initialSnowStorage;
            Snowfall = 0;
            Snowmelt = 0;
            MaxMassBalanceError = 0;

        }
    }
}
