using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroCat.Core
{
    public class SurfaceStorageMassBalance
    {
        [DescriptionAttribute("Accumulated snow melt [Unit: Millimiters"), CategoryAttribute("Inflow")]
        public double SnowMelt { get; private set; }
        [DescriptionAttribute("Accumulated rainfall [Unit: Millimiters"), CategoryAttribute("Inflow")]
        public double Rainfall { get; private set; }
        [DescriptionAttribute("Accumulated evaporation [Unit: Millimiters"), CategoryAttribute("Outflow")]
        public double Evaporation { get; private set; }
        [DescriptionAttribute("Accumulated overlandflow [Unit: Millimiters"), CategoryAttribute("Outflow")]
        public double OverlandFlow { get; private set; }
        [DescriptionAttribute("Accumulated interflow [Unit: Millimiters"), CategoryAttribute("Outflow")]
        public double InterFlow { get; private set; }
        [DescriptionAttribute("Accumulated infiltration to the root zone [Unit: Millimiters"), CategoryAttribute("Outflow")]
        public double InfiltrationToRootZone { get; private set; }
        [DescriptionAttribute("Accumulated infiltration to the ground water [Unit: Millimiters"), CategoryAttribute("Outflow")]
        public double InfiltrationToGroundWater { get; private set; }
        [DescriptionAttribute("Initial storage [Unit: Millimiters"), CategoryAttribute("Storage")]
        public double InitialStorage { get; private set; }
        [DescriptionAttribute("Current storage [Unit: Millimiters"), CategoryAttribute("Storage")]
        public double Storage { get; private set; }
        [DescriptionAttribute("Massbalance error [Unit: Millimiters"), CategoryAttribute("Mass balance Error")]
        public double MassBalanceError 
        {
            get
            {
                return SnowMelt + Rainfall - Evaporation - OverlandFlow - InterFlow - InfiltrationToRootZone - InfiltrationToGroundWater - (Storage - InitialStorage);
            }
        }
        [DescriptionAttribute("Maximum mass balance error during the simulation [Unit: Millimiters"), CategoryAttribute("Mass balance Error")]
        public double MaxMassBalanceError { get; private set; }

        public void SetValues(double snowMelt, double rainfall, double evaporation, double overlandFlow, double interFlow, double infiltrationToRootZone, double infiltrationToGroundWater, double storage)
        {
            SnowMelt += snowMelt;
            Rainfall += rainfall;
            Evaporation += evaporation;
            OverlandFlow += overlandFlow;
            InterFlow += interFlow;
            InfiltrationToRootZone += infiltrationToRootZone;
            InfiltrationToGroundWater += infiltrationToGroundWater;
            Storage = storage;
            MaxMassBalanceError = Math.Max(MaxMassBalanceError, MassBalanceError);
        }

        public void Initialize(double initialSurfaceStorage)
        {
            InitialStorage = initialSurfaceStorage;
            SnowMelt = 0;
            Rainfall = 0;
            Evaporation = 0;
            OverlandFlow = 0;
            InterFlow = 0;
            InfiltrationToRootZone = 0;
            InfiltrationToGroundWater = 0;
            MaxMassBalanceError = 0;
        }
    }
}
