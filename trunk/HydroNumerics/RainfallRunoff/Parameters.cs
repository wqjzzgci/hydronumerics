using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.RainfallRunoff
{
    public class Parameters
    {
        double catchmentArea;
        double baseFlowTreshold;
        double baseflowTimeConstant;
        double interflowCoefficient;
        double interflowTimeConstant;
        double interflowTreshold;
        double overlandFlowCoefficient;
        double overlandFlowTimeConstant;
        double overlandFlowTreshold;
        double rootZoneStorageCapacity;
        double snowmeltCoefficient;
        double surfaceStorageCapacity;

        bool catchmentAreaIsDefined = false;
        bool baseFlowTresholdIsDefined = false;
        bool baseflowTimeConstantIsDefined = false;
        bool interflowCoefficientIsDefined = false;
        bool interflowTimeConstantIsDefined = false;
        bool interflowTresholdIsDefined = false;
        bool overlandFlowCoefficientIsDefined = false;
        bool overlandFlowTimeConstantIsDefined = false;
        bool overlandFlowTresholdIsDefined = false;
        bool rootZoneStorageCapacityIsDefined = false;
        bool snowmeltCoefficientIsDefined = false;
        bool surfaceStorageCapacityIsDefined = false;

        /// <summary>
        /// Catchment area [unit: m2] (Area)
        /// </summary>
        public double CatchmentArea
        {
            get
            {
                if (!catchmentAreaIsDefined)
                {
                    throw new Exception("Value for catchmentArea is undefined");
                }
                return catchmentArea;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("catchmentArea must be greater than zero. Initial value was: " + value.ToString());
                }
                catchmentAreaIsDefined = true;
                catchmentArea = value;
            }
        }

        /// <summary>
        /// Surface water storage capacity (max capacity) [Unit: millimiters] (U*)
        /// </summary>
        public double SurfaceStorageCapacity
        {
            get
            {
                if (!surfaceStorageCapacityIsDefined)
                {
                    throw new Exception("Value for surfaceStorageCapacity is undefined");
                }
                return surfaceStorageCapacity;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("surfaceStorageCapacity must be greater than zero. Initial value was: " + value.ToString());
                }
                surfaceStorageCapacityIsDefined = true;
                surfaceStorageCapacity = value;
            }
        }

        /// <summary>
        /// Root zone capacity [Unit: millimiters] (L*)
        /// </summary>
        public double RootZoneStorageCapacity
        {
            get
            {
                if (!rootZoneStorageCapacityIsDefined)
                {
                    throw new Exception("Value for rootZoneStorageCapacity is undefined");
                }
                return rootZoneStorageCapacity;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("rootZoneStorageCapacity must be greater than zero. Initial value was: " + value.ToString());
                }
                rootZoneStorageCapacityIsDefined = true;
                rootZoneStorageCapacity = value;
            }
        }

        /// <summary>
        /// Snow melting coefficient [Unit: dimensionless] (Cs)
        /// </summary>
        public double SnowmeltCoefficient
        {
            get
            {
                if (!snowmeltCoefficientIsDefined)
                {
                    throw new Exception("Value for snowmeltCoefficient is undefined");
                }
                return snowmeltCoefficient;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("snowmeltCoefficient must be greater than zero. Initial value was: " + value.ToString());
                }
                snowmeltCoefficientIsDefined = true;
                snowmeltCoefficient = value;
            }
        }

        /// <summary>
        /// Overland flow treshold [Unit: dimensionless] (TOF) (CL2)
        /// If the relative moisture content of the roor zone is above the overland flow treshold
        /// overland flow is generated. The overland flow treshold must be in the interval [0,1].
        /// </summary>
        public double OverlandFlowTreshold
        {
            get
            {
                if (!overlandFlowTresholdIsDefined)
                {
                    throw new Exception("Value for overlandFlowTreshold is undefined");
                }
                return overlandFlowTreshold;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("overlandFlowTreshold must be greater than zero. The value was: " + value.ToString());
                }
                if (value > 1)
                {
                    throw new Exception("overlandFlowTreshold must be less than or equal to one. The value was: " + value.ToString());
                }
                overlandFlowTresholdIsDefined = true;
                overlandFlowTreshold = value;
            }
        }

        /// <summary>
        /// Overland flow coefficient [Unit: dimensionless] (Cof)
        /// Determins the fraction of excess water that runs off as overland flow
        /// </summary>
        public double OverlandFlowCoefficient
        {
            get
            {
                if (!overlandFlowCoefficientIsDefined)
                {
                    throw new Exception("Value for overlandFlowCoefficient is undefined");
                }
                return overlandFlowCoefficient;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("overlandFlowCoefficient must be greater than zero. The value was: " + value.ToString());
                }
                if (value > 1.0)
                {
                    throw new Exception("overlandFlowCoefficient must be less than or equal to one. The value was: " + value.ToString());
                }
                overlandFlowCoefficientIsDefined = true;
                overlandFlowCoefficient = value;
            }
        }

        /// <summary>
        /// Overland flow routing time constant [Unit: Days]  (Ko)
        /// </summary>
        public double OverlandFlowTimeConstant
        {
            get
            {
                if (!overlandFlowTimeConstantIsDefined)
                {
                    throw new Exception("Value for overlandFlowTimeConstant is undefined");
                }
                return overlandFlowTimeConstant;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("overlandFlowTimeConstant must be greater than zero. The value was: " + value.ToString());
                }
                overlandFlowTimeConstantIsDefined = true;
                overlandFlowTimeConstant = value;
            }
        }



        /// <summary>
        /// Interflow coefficient. [Unit: dimensionless] (CIf)
        /// Must be in the interval [0,1]
        /// </summary>
        public double InterflowCoefficient
        {
            get
            {
                if (!interflowCoefficientIsDefined)
                {
                    throw new Exception("Value for interflowCoefficient is undefined");
                }
                return interflowCoefficient;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("interflowCoefficient must be greater than zero. The value was: " + value.ToString());
                }
                if (value > 1)
                {
                    throw new Exception("interflowCoefficient must be smaller than or equal to one. The value was: " + value.ToString());
                }
                interflowCoefficientIsDefined = true;
                interflowCoefficient = value;
            }
        }

        /// <summary>
        /// Interflow treshold [Unit: millimiters] (CL1)
        /// Must be in the interval [0,1]
        /// </summary>
        public double InterflowTreshold
        {
            get
            {
                if (!interflowTresholdIsDefined)
                {
                    throw new Exception("Value for interflowTreshold is undefined");
                }
                return interflowTreshold;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("interflowTreshold must be greater than zero. The value was: " + value.ToString());
                }
                if (value > 1)
                {
                    throw new Exception("interflowTreshold must be smaler than or equal to one. The value was: " + value.ToString());
                }
                interflowTresholdIsDefined = true;
                interflowTreshold = value;
            }
        }

        /// <summary>
        /// Interflow routing time constant [unit: Days]
        /// </summary>
        public double InterflowTimeConstant
        {
            get
            {
                if (!interflowTimeConstantIsDefined)
                {
                    throw new Exception("Value for interflowTimeConstant is undefined");
                }
                return interflowTimeConstant;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("interflowTimeConstant must be greater than zero. The value was: " + value.ToString());
                }
                interflowTimeConstantIsDefined = true;
                interflowTimeConstant = value;
            }
        }

        /// <summary>
        /// Base flow routing time constant[Unit: Days] (CKBF) (kb)
        /// </summary>
        public double BaseflowTimeConstant
        {
            get
            {
                if (!baseflowTimeConstantIsDefined)
                {
                    throw new Exception("Value for baseflowTimeConstant is undefined");
                }
                return baseflowTimeConstant;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("baseflowTimeConstant must be greater than zero. The value was: " + value.ToString());
                }
                baseflowTimeConstantIsDefined = true;
                baseflowTimeConstant = value;
            }
        }

        ///// <summary>
        ///// Baseflow treshold [Unit: millimiters] (TG)
        ///// </summary>
        //public double BaseFlowTreshold
        //{
        //    get
        //    {
        //        if (!baseFlowTresholdIsDefined)
        //        {
        //            throw new Exception("Value for baseFlowTreshold is undefined");
        //        }
        //        return baseFlowTreshold;
        //    }
        //    set
        //    {
        //        if (value < 0)
        //        {
        //            throw new Exception("baseFlowTreshold must be greater than zero. The value was: " + value.ToString());
        //        }
        //        baseFlowTresholdIsDefined = true;
        //        baseFlowTreshold = value;
        //    }
        //}

        
    }
}
