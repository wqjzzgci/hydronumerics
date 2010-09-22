using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroCat.Core
{
    public class Accumulated
    {
        public double AccumulatedPrecipitation { get; set; }
        public double AccumulatedSnowfall { get; set; }
        public double AccumulatedSnowMelt { get; set; }
        public double AccumulatedRainfall { get; set; }
        public double AccumulatedSurfaceEvaporation { get; set; }
        public double AccumulatedRootZoneEvaporation { get; set; }

        public double AccumulatedInflowToOverlandLinearReservoir { get; set; }
        public double AccumulatedInflowToInterflowLinearReservoir { get; set; }
        public double AccumulatedInflowToBaseflowLinearReservoir { get; set; }

        public double AccumulatedOverlandflow{ get; set; }
        public double AccumulatedInterflow { get; set; }
        public double AccumulatedBaseflow { get; set; }


        public void Initialize()
        {
            AccumulatedPrecipitation = 0;
            AccumulatedSnowfall = 0;
            AccumulatedSnowMelt = 0;
            AccumulatedRainfall = 0;
            AccumulatedSurfaceEvaporation = 0;
            AccumulatedRootZoneEvaporation = 0;
            AccumulatedInflowToOverlandLinearReservoir = 0;
            AccumulatedInflowToInterflowLinearReservoir = 0;
            AccumulatedInflowToBaseflowLinearReservoir = 0;
            AccumulatedOverlandflow = 0;
            AccumulatedInterflow = 0;
            AccumulatedBaseflow = 0;

        }
    }

 
}
