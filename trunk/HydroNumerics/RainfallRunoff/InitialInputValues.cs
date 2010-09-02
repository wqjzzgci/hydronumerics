using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.RainfallRunoff
{
    public class InitialInputValues
    {
        // ============== Initial values ===================================
        /// <summary>
        /// Initial snow storage [Unit: millimters] (Ss0)
        /// </summary>
        public double SnowStorage { get; set; }

        /// <summary>
        /// Initial surface water storage [Unit: millimiters] (U0)
        /// </summary>
        public double SurfaceStorage { get; set; }

        /// <summary>
        /// Initial Root zone storage [Unit: millimiters] (L0)
        /// </summary>
        public double RootZoneStorage { get; set; }  //L0

        /// <summary>
        /// Initial overland flow rate [Unit: m3 / sec.] (QR10)
        /// </summary>
        public double OverlandFlow { get; set; }

        /// <summary>
        /// Initial interflow rage [Unit: m3 / sec]  (QR20)
        /// </summary>
        public double Interflow { get; set; }

        /// <summary>
        /// Initial base flow rage [Unit: m3 / sec:] (BF0)
        /// </summary>
        public double BaseFlow { get; set; }  //BF0
    }
}
