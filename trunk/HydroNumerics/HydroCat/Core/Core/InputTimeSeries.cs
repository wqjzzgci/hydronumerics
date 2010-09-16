using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroCat.Core
{
    public class InputTimeSeries
    {
        bool isConfigured;

        private HydroCatEngine hydroCatEngine;
        private BaseTimeSeries precipitationTs;
        private BaseTimeSeries potentialEvaporationTs;
        private BaseTimeSeries temperatureTs;
        private BaseTimeSeries observedRunoffTs;

        double[] precipitation; //time sereis are copied to these arrays for performance reasons.
        double[] potentialEvaporation;
        double[] temperature;
        double[] observedRunoff;

        public InputTimeSeries(HydroCatEngine hydroCatEngine)
        {

            this.hydroCatEngine = hydroCatEngine;
            hydroCatEngine.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(hydroCatEngine_PropertyChanged);

            isConfigured = false;
        }

        void hydroCatEngine_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SimulationStartTime" || e.PropertyName == "SimulationEndTime")
            {
                isConfigured = false;
            }
        }

        public BaseTimeSeries PrecipitationTs
        {
            set
            {
                precipitationTs = value;
                isConfigured = false;
            }
        }

        public BaseTimeSeries PotentialEvaporationTs
        {
           
            set
            {
                potentialEvaporationTs = value;
                isConfigured = false;
            }
        }

        public BaseTimeSeries TemperatureTs
        {
            set
            {
                temperatureTs = value;
                isConfigured = false;
            }
        }

        public BaseTimeSeries ObservedRunoffTs
        {
            set
            {
                observedRunoffTs = value;
                isConfigured = false;
            }
        }

       

        private void Configure()
        {
            Unit mmPrDayUnit = Units.MmPrDay;
            Unit centigradeUnit = Units.Centigrade;
            Unit m3PrSecUnit = Units.M3PrSec;

            int numberOfTimesteps = (int)(hydroCatEngine.SimulationEndTime.ToOADate() - hydroCatEngine.SimulationStartTime.ToOADate() - 0.5);

            precipitation = new double[numberOfTimesteps];
            potentialEvaporation = new double[numberOfTimesteps];
            temperature = new double[numberOfTimesteps];
            observedRunoff = new double[numberOfTimesteps];

            for (int i = 0; i < numberOfTimesteps; i++)
            {
                DateTime fromTime = hydroCatEngine.SimulationStartTime.AddDays(i);
                DateTime toTime = hydroCatEngine.SimulationStartTime.AddDays(i + 1);
                precipitation[i] = precipitationTs.GetValue(fromTime, toTime, mmPrDayUnit);
                potentialEvaporation[i] = potentialEvaporationTs.GetValue(fromTime, toTime, mmPrDayUnit);
                temperature[i] = temperatureTs.GetValue(fromTime, toTime, centigradeUnit);
                observedRunoff[i] = observedRunoffTs.GetValue(fromTime, toTime, m3PrSecUnit); 
            }
            isConfigured = true;
        }

        public double GetPrecipitation(int timestep)
        {
            if (!isConfigured)
            {
                Configure();
            }
            return precipitation[timestep];
        }

        public double GetPotentialEvaporation(int timestep)
        {
            if (!isConfigured)
            {
                Configure();
            }
            return potentialEvaporation[timestep];

        }

        public double GetTemperature(int timestep)
        {
            if (!isConfigured)
            {
                Configure();
            }
            return temperature[timestep];
        }

        public double GetObservedRunoff(int timestep)
        {
            if (!isConfigured)
            {
                Configure();
            }
            return observedRunoff[timestep];
        }
    }
}
