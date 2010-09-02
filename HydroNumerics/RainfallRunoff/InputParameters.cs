using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.RainfallRunoff
{
    public class InputParameters
    {
        /// <summary>
        /// Catchment area [unit: m2] (Area)
        /// </summary>
        public double CatchmentArea { get; set; }

        /// <summary>
        /// Surface water storage capacity (max capacity) [Unit: millimiters] (U*)
        /// </summary>
        public double SurfaceStorageCapacity { get; set; }

        /// <summary>
        /// Root zone capacity [Unit: millimiters] (L*)
        /// </summary>
        public double RootZoneStorageCapacity { get; set; }

        /// <summary>
        /// Snow melting coefficient [Unit: dimensionless] (Cs)
        /// </summary>
        public double SnowmeltCoefficient { get; set; }

        /// <summary>
        /// Overland flow tresshold [Unit: millimiters] (TOF) (Cl2)
        /// If the relative moisture content of the roor zone is above the overland flow treshold
        /// overland flow is generated.
        /// </summary>
        public double OverlandFlowTreshold { get; set; }

        /// <summary>
        /// Overland flow coefficient [Unit: dimensionless] (Cof)
        /// Determins the fraction of excess water that runs off as overland flow
        /// </summary>
        public double OverlandFlowCoefficient { get; set; }

        /// <summary>
        /// Overland flow routing time constant [Unit: Days]  (Ko)
        /// </summary>
        public double OverlandFlowTimeConstant { get; set; }



        /// <summary>
        /// Interflow coefficient. [Unit: dimensionless] (= 1/CKIF) (Cif)
        /// </summary>
        public double InterflowCoefficient { get; set; }

        /// <summary>
        /// Interflow treshold [Unit: millimiters] (TIF) (CL1)
        /// </summary>
        public double InterflowTreshold { get; set; }

        /// <summary>
        /// Interflow routing time constant [unit: Days]
        /// </summary>
        public double InterflowTimeConstant { get; set; }

        /// <summary>
        /// Base flow routing time constant[Unit: Days] (CKBF) (kb)
        /// </summary>
        public double BaseflowTimeConstant { get; set; } // (CKBF) (kb)

        /// <summary>
        /// Baseflow treshold [Unit: millimiters] (TG)
        /// </summary>
        public double BaseFlowTreshold { get; set; } 
    }
}
