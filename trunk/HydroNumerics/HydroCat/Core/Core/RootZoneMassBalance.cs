using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroCat.Core
{
    public class RootZoneMassBalance
    {
       
        [DescriptionAttribute("Accumulated infiltration from surface storage [Unit: Millimiters"), CategoryAttribute("Inflow")]
        public double Infiltration { get; private set; }
        [DescriptionAttribute("Accumulated evaporation (Evapotranspiration) [Unit: Millimiters"), CategoryAttribute("Outflow")]
        public double Evaporation { get; private set; }
        
        [DescriptionAttribute("Initial storage [Unit: Millimiters"), CategoryAttribute("Storage")]
        public double InitialStorage { get; private set; }
        [DescriptionAttribute("Current storage [Unit: Millimiters"), CategoryAttribute("Storage")]
        public double Storage { get; private set; }
        [DescriptionAttribute("Massbalance error [Unit: Millimiters"), CategoryAttribute("Mass balance Error")]
        public double MassBalanceError 
        {
            get
            {
                return Infiltration - Evaporation - (Storage - InitialStorage);
            }
        }
        [DescriptionAttribute("Maximum mass balance error during the simulation [Unit: Millimiters"), CategoryAttribute("Mass balance Error")]
        public double MaxMassBalanceError { get; private set; }

        public void SetValues(double infiltration, double evaporation, double storage)
        {
            Infiltration += infiltration;
            Evaporation += evaporation;
            Storage = storage;
            MaxMassBalanceError = Math.Max(MaxMassBalanceError, MassBalanceError);
        }

        public void Initialize(double initialRootZoneStorage)
        {
            InitialStorage = initialRootZoneStorage;
            Infiltration = 0;
            Evaporation = 0;
            MaxMassBalanceError = 0;

        }
    }
}
