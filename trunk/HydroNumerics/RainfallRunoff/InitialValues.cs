using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.RainfallRunoff
{
    public class InitialValues
    {
        double snowStorage; 
        bool snowStorageIsAssigned = false;
        double surfaceStorage; 
        bool surfaceStorageIsAssigned = false;
        double rootZoneStorage; 
        bool rootZoneStorageIsAssigned = false;
        double overlandFlow; 
        bool overlandFlowIsAssigned = false;
        double interFlow; 
        bool interFlowIsAssigned = false;
        double baseFlow; 
        bool baseFlowIsAssigned = false;
        
        /// <summary>
        /// Initial snow storage [Unit: millimters] (Ss0)
        /// </summary>
        public double SnowStorage  
        {
            get
            {
                if (!snowStorageIsAssigned)
                {
                    throw new Exception("Initial value for SnowStorage is undefined");
                }
                return snowStorage; 
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Initial snow storage must be greater than zero. Initial value was: " + value.ToString());
                }
                snowStorageIsAssigned = true;
                snowStorage = value;
            }
        }

        /// <summary>
        /// Initial surface water storage [Unit: millimiters] (U0)
        /// </summary>
        public double SurfaceStorage 
        {
            get
            {
                if (!surfaceStorageIsAssigned)
                {
                    throw new Exception("Initial value for surface storage is undefined");
                }
                return surfaceStorage;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Initial surface storage must be greater than zero. Initial value was: " + value.ToString());
                }
                surfaceStorageIsAssigned = true;
                surfaceStorage = value;
            }
        }

        /// <summary>
        /// Initial Root zone storage [Unit: millimiters] (L0)
        /// </summary>
        public double RootZoneStorage  //L0
        {
            get
            {
                if (!rootZoneStorageIsAssigned)
                {
                    throw new Exception("Initial value for root zone storage is undefined");
                }
                return rootZoneStorage;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Initial root zone storage must be greater than zero. Initial value was: " + value.ToString());
                }
                rootZoneStorageIsAssigned = true;
                rootZoneStorage = value;
            }
        }


        /// <summary>
        /// Initial specific overlandflow rate [Unit: millimiters/day] (QR10)
        /// </summary>
        public double OverlandFlow
        {
            get
            {
                if (!overlandFlowIsAssigned)
                {
                    throw new Exception("Initial value for overlandflow is undefined");
                }
                return overlandFlow;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Initial overlandflow must be greater than zero. Initial value was: " + value.ToString());
                }
                overlandFlowIsAssigned = true;
                overlandFlow = value;
            }
        }


        /// <summary>
        /// Initial specific interflow rate [Unit: millimeters/day]  (QR20)
        /// </summary>
        public double InterFlow 
        {
            get
            {
                if (!interFlowIsAssigned)
                {
                    throw new Exception("Initial value for interflow is undefined");
                }
                return interFlow;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Initial interflow must be greater than zero. Initial value was: " + value.ToString());
                }
                interFlowIsAssigned = true;
                interFlow = value;
            }
        }


        /// <summary>
        /// Initial specific baseflow rate [Unit: millimiters/day] (BF0)
        /// </summary>
        public double BaseFlow
        {
            get
            {
                if (!baseFlowIsAssigned)
                {
                    throw new Exception("Initial value for baseflow is undefined");
                }
                return baseFlow;
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Initial baseflow must be greater than zero. Initial value was: " + value.ToString());
                }
                baseFlowIsAssigned = true;
                baseFlow = value;
            }
        }


       

    }
}
