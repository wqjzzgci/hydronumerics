using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroCat.Core
{
    public class MassBalance
    {
        private OutputTimeSeries outputTimeSeries;
        public MassBalance(OutputTimeSeries outputTimeSeries)
        {
            this.outputTimeSeries = outputTimeSeries;

        }

        private double Accumulate(TimestampSeries timeStampSeries)
        {
            double accumulatedValue = 0;
            foreach (TimestampValue timestampValue in timeStampSeries.Items)
            {
                accumulatedValue += timestampValue.Value;
            }
            return accumulatedValue;
        }

        //// ============ Met. data =========
        public double AccumulatedPrecipitation { get {return Accumulate(outputTimeSeries.precipitationTs);}}
        public double AccumulatedPotentialEvaporation { get { return Accumulate(outputTimeSeries.potentialEvaporationTs); } }

        ////============= Snow ============

        //public double AccumulatedSnowFall { get { return Accumulate(outputTimeSeries.sn); } }
        //public double AccumulatedSnowMelt { get { return Accumulate(outputTimeSeries.sno); } }
        //double SnowStorageInitial;
        //double SnowStorage;
        //double SnowStorageChange;

        ////====== Surface =====
        //double AccumulatedRainFall;
        //double SnowMelt;
        //double SurfaceStorageCurrent;
        //double SurfaceStorageInitial;
        //double SurfaceStorageChange;
        //double Interflow;
    }
}
