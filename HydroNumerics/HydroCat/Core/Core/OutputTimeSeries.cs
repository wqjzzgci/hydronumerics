using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroCat.Core
{
    public class OutputTimeSeries
    {
        // -- Met. input data --
        public TimestampSeries precipitationTs { get;  private set; }
        public TimestampSeries potentialEvaporationTs { get; private set; }
        public TimestampSeries temperatureTs { get; private set; }

        // -- outflows --
        public TimestampSeries surfaceEvaporationTs { get; private set; }
        public TimestampSeries rootZoneEvaporationTs { get; private set; }
        public TimestampSeries overlandFlowTs { get; private set; }
        public TimestampSeries interFlowTs { get; private set; }
        public TimestampSeries baseFlowTs { get; private set; }
        public TimestampSeries runoffTs { get; private set; }

        // -- observed runoff --
        public TimestampSeries observedRunoffTs { get; private set; }

        // -- Storages --
        public TimestampSeries snowStorageTs { get; private set; }
        public TimestampSeries surfaceStorageTs { get; private set; }
        public TimestampSeries rootZoneStorageTs { get; private set; }
        
        
        public TimeSeriesGroup TimeSeries { get; private set; }


        public OutputTimeSeries()
        {
            TimeSeries = new TimeSeriesGroup();
            // -- Met. input flows
            TimeSeries.Items.Add(precipitationTs = new TimestampSeries("Precipitation", Units.MmPrDay));
            TimeSeries.Items.Add(potentialEvaporationTs = new TimestampSeries("potentialEvaporation", Units.MmPrDay));
            TimeSeries.Items.Add(temperatureTs = new TimestampSeries("Temperature", Units.MmPrDay));

            // -- Outflows --
            TimeSeries.Items.Add(surfaceEvaporationTs = new TimestampSeries("SurfaceEvaporation", Units.MmPrDay));
            TimeSeries.Items.Add(rootZoneEvaporationTs = new TimestampSeries("RootZoneEvaporation", Units.MmPrDay));
            TimeSeries.Items.Add(overlandFlowTs = new TimestampSeries("OverlandFlow", Units.MmPrDay));
            TimeSeries.Items.Add(interFlowTs = new TimestampSeries("InterFlow", Units.MmPrDay));
            TimeSeries.Items.Add(baseFlowTs = new TimestampSeries("BaseFlow", Units.MmPrDay));
            TimeSeries.Items.Add(runoffTs = new TimestampSeries("Runoff", Units.M3PrSec));

            // -- observed runoff --
            TimeSeries.Items.Add(observedRunoffTs = new TimestampSeries("ObservedRunoff", Units.M3PrSec));
            
            // -- Storages --
            TimeSeries.Items.Add(snowStorageTs = new TimestampSeries("SnowStorage", Units.Millimiters));
            TimeSeries.Items.Add(surfaceStorageTs = new TimestampSeries("SurfaceStorage", Units.Millimiters));
            TimeSeries.Items.Add(rootZoneStorageTs = new TimestampSeries("RootZoneStorage", Units.Millimiters));
        }

        public void Initialize()
        {
            foreach (TimestampSeries timestampSeries in TimeSeries.Items)
            {
                timestampSeries.Items.Clear();
            }
        }
    }
}
