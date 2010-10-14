using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using HydroNumerics.Time.Core;
using HydroNumerics.Core;

namespace HydroNumerics.HydroCat.Core
{
    

    public class HydroCatEngine : System.ComponentModel.INotifyPropertyChanged
    {

        #region Input timeseries
        private BaseTimeSeries inputPrecipitation;
        private BaseTimeSeries inputPotentialEvaporation;
        private BaseTimeSeries inputTemperature;
        private BaseTimeSeries inputObservedRunoff;

        [BrowsableAttribute(false)]
        public BaseTimeSeries InputPrecipitation
        {
            get { return inputPrecipitation; }
            set
            {
                inputPrecipitation = value;
                inputPrecipitation.AllowExtrapolation = true;
                inputPrecipitation.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
                isConfigurated = false;
            }
        }

        [BrowsableAttribute(false)]
        public BaseTimeSeries InputPotentialEvaporation
        {
            get { return inputPotentialEvaporation; }
            set
            {
                inputPotentialEvaporation = value;
                inputPotentialEvaporation.AllowExtrapolation = true;
                inputPotentialEvaporation.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
                isConfigurated = false;
            }
        }

        [BrowsableAttribute(false)]
        public BaseTimeSeries InputTemperature
        {
            get { return inputTemperature; }
            set
            {
                inputTemperature = value;
                inputTemperature.AllowExtrapolation = true;
                inputTemperature.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
                isConfigurated = false;
            }
        }

        [BrowsableAttribute(false)]
        public BaseTimeSeries InputObservedRunoff
        {
            get { return inputObservedRunoff; }
            set
            {
                inputObservedRunoff = value;
                inputObservedRunoff.AllowExtrapolation = true;
                inputObservedRunoff.ExtrapolationMethod = ExtrapolationMethods.RecycleYear;
                isConfigurated = false;
            }
        }
        
        #endregion


        #region  ====== Initial values ======================================
        /// <summary>
        /// Snow storage [Unit: millimiters] (Ss)
        /// </summary>
        [DescriptionAttribute("Initial snow storage [Unit: millimiters] (Ss)"), CategoryAttribute("Initial values")]
        public double InitialSnowStorage { get; set; }

        /// <summary>
        /// Surface Storage [Unit: millimiters] (U)
        /// </summary>
        [DescriptionAttribute("Initial surface storage [Unit: millimiters] (U)"), CategoryAttribute("Initial values")]
        public double InitialSurfaceStorage { get; set; }

        /// <summary>
        /// Root zone storage [Unit: millimiters] (L)
        /// </summary>
        [DescriptionAttribute("Initial root zone storage [Unit: millimiters] (L)"), CategoryAttribute("Initial values")]
        public double InitialRootZoneStorage { get; set; }

        /// <summary>
        /// Overland flow rate [Unit: Millimiters / day]
        /// </summary>
        [DescriptionAttribute("Initial overland flow rate [Unit: Millimiters / day]"), CategoryAttribute("Initial values")]
        public double InitialOverlandFlow { get; set; }

        /// <summary>
        /// Inter flow rate (specific flow, before routing) [Unit: millimiters / day]
        /// </summary>
        [DescriptionAttribute("Initial inter flow rate (specific flow, before routing) [Unit: millimiters / day]"), CategoryAttribute("Initial values")]
        public double InitialInterFlow { get; set; }

        /// <summary>
        /// Base flow rate (specific flow, before routing) [Unit: millimiters / day]
        /// </summary>
        [DescriptionAttribute("Initial base flow rate (specific flow, before routing) [Unit: millimiters / day]"), CategoryAttribute("Initial values")]
        public double InitialBaseFlow { get; set; }
        #endregion

        #region =========  Calibration parameters =====================
        /// <summary>
        /// Catchment area [unit: m2] (Area)
        /// </summary>
        [DescriptionAttribute("Catchment area [unit: m2] (Area)"), CategoryAttribute("Calibration parameter")]
        public double CatchmentArea { get; set; }

        /// <summary>
        /// Surface water storage capacity (max capacity) [Unit: millimiters] (U*)
        /// </summary>
        [DescriptionAttribute("Surface water storage capacity (max capacity) [Unit: millimiters] (U*)"), CategoryAttribute("Calibration parameter")]
        public double SurfaceStorageCapacity { get; set; }

        /// <summary>
        /// Root zone capacity [Unit: millimiters] (L*)
        /// </summary>
        [DescriptionAttribute("Root zone capacity [Unit: millimiters] (L*)"), CategoryAttribute("Calibration parameter")]
        public double RootZoneStorageCapacity { get; set; }

        /// <summary>
        /// Snow melting coefficient [Unit: dimensionless] (Cs)
        /// </summary>
        [DescriptionAttribute("Snow melting coefficient [Unit: dimensionless] (Cs), Allowed range[0,1]"), CategoryAttribute("Calibration parameter")]
        public double SnowmeltCoefficient { get; set; }

        /// <summary>
        /// Overland flow coefficient [Unit: dimensionless] (Cof)
        /// Determins the fraction of excess water that runs off as overland flow
        /// </summary>
        [DescriptionAttribute("Overland flow coefficient [Unit: dimensionless] (Cof), Allowed range[0,1]"), CategoryAttribute("Calibration parameter")]
        public double OverlandFlowCoefficient { get; set; }

        /// <summary>
        /// Overland flow routing time constant [Unit: Days]  (Ko)
        /// </summary>
        [DescriptionAttribute("Overland flow routing time constant [Unit: Days]  (Ko)"), CategoryAttribute("Calibration parameter")]
        public double OverlandFlowTimeConstant { get; set; }

        /// <summary>
        /// Overland flow treshold [Unit: dimensionless] (TOF) (CL2)
        /// If the relative moisture content of the root zone is above the overland flow treshold
        /// overland flow is generated. The overland flow treshold must be in the interval [0,1].
        /// </summary>
        [DescriptionAttribute("Overland flow treshold [Unit: dimensionless] (TOF) (CL2), allowed range [0,1]"), CategoryAttribute("Calibration parameter")]
        public double OverlandFlowTreshold { get; set; }

        /// <summary>
        /// Interflow coefficient. [Unit: dimensionless] (CIf)
        /// Must be in the interval [0,1]
        /// </summary>
        [DescriptionAttribute("Interflow coefficient. [Unit: dimensionless] (CIf), allowed range [0,1]"), CategoryAttribute("Calibration parameter")]
        public double InterflowCoefficient { get; set; }

        /// <summary>
        /// Interflow routing time constant [unit: Days]
        /// </summary>
        [DescriptionAttribute("Interflow routing time constant [unit: Days]"), CategoryAttribute("Calibration parameter")]
        public double InterflowTimeConstant { get; set; }

        /// <summary>
        /// Interflow treshold [Unit: millimiters] (CL1)
        /// Must be in the interval [0,1]
        /// </summary>
        [DescriptionAttribute("Interflow treshold [Unit: dimensionless] (CL1), allowed range [0,1["), CategoryAttribute("Calibration parameter")]
        public double InterflowTreshold { get; set; }

        /// <summary>
        /// Base flow routing time constant[Unit: Days] (CKBF) (kb)
        /// </summary>
        [DescriptionAttribute("Base flow routing time constant[Unit: Days] (CKBF) (kb)"), CategoryAttribute("Calibration parameter")]
        public double BaseflowTimeConstant { get; set; }

        #endregion --- Calibration parameters ------

        #region ======   Simulation control input parameters ================
        DateTime simulationStartTime;
        /// <summary>
        /// Start time for the simulation
        /// </summary>
        [DescriptionAttribute("Simulation start time"), CategoryAttribute("Simulation control")]
        public DateTime SimulationStartTime
        {
            get { return simulationStartTime; }
            set 
            {
                simulationStartTime = value;
                NotifyPropertyChanged("SimulationStartTime");
            } 
        }

        DateTime simulationEndTime;
        /// <summary>
        /// End time for the simulation
        /// </summary>
        [DescriptionAttribute("Simulation end time"), CategoryAttribute("Simulation control")]
        public DateTime SimulationEndTime 
        {
            get { return simulationEndTime; }
            set
            {
                simulationEndTime = value;
                NotifyPropertyChanged("SimulationEndTime");
            }
            
        }
        #endregion

        // ============= output ================================
       
        private TimestampSeries precipitation;
        private TimestampSeries potentialEvaporation;
        private TimestampSeries temperature;
        private TimestampSeries observedSpecificRunoff;
        private TimestampSeries observedRunoff;
        private TimestampSeries surfaceEvaporation;
        private TimestampSeries rootZoneEvaporation;
        private TimestampSeries rainfall;
        private TimestampSeries snowfall;
        private TimestampSeries snowMelt;
        private TimestampSeries snowStorage;
        private TimestampSeries surfaceStorage;
        private TimestampSeries rootZoneStorage;
        private TimestampSeries overlandflow;
        private TimestampSeries routedOverlandflow;
        private TimestampSeries interflow;
        private TimestampSeries routedInterflow;
        private TimestampSeries baseflow;
        private TimestampSeries routedBaseflow;
        private TimestampSeries specificRunoff;
        private TimestampSeries runoff;
        [BrowsableAttribute(false)]
        public TimeSeriesGroup OutputTimeSeries { get; private set; }
      

      

        int timestep = 0;

        private bool isConfigurated = false;

        //public DateTime CurrentTime { get; private set; }
        int numberOfTimesteps;

        public HydroCatEngine()
        {
            isConfigurated = false;

            //--- Default values ----
            SimulationStartTime = new DateTime(2010, 1, 1);
            SimulationEndTime = new DateTime(2011, 1, 1);

            OutputTimeSeries = new TimeSeriesGroup();

            //-- Default values (parameters)
            this.CatchmentArea = 160000000;
            this.SnowmeltCoefficient = 2.0;
            this.SurfaceStorageCapacity = 18;
            this.RootZoneStorageCapacity = 250;
            this.OverlandFlowCoefficient = 0.61;
            this.InterflowCoefficient = 0.6;
            this.OverlandFlowTreshold = 0.38;
            this.InterflowTreshold = 0.08;
            this.OverlandFlowTimeConstant = 0.3;
            this.InterflowTimeConstant = 30;
            this.BaseflowTimeConstant = 2800;

        }

        private void Configurate()
        {
            numberOfTimesteps = (int)(SimulationEndTime.ToOADate() - SimulationStartTime.ToOADate() - 0.5);
            precipitation = new TimestampSeries("Precipitation", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            potentialEvaporation = new TimestampSeries("PotentialEvaportion", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            temperature = new TimestampSeries("Temperature", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.Centigrade);
            int numberOfObservedRunoffValues = 0;
            if (inputObservedRunoff is TimespanSeries)
            {
                numberOfObservedRunoffValues = ((TimespanSeries) inputObservedRunoff).Items.Count;
            }
            else if (inputObservedRunoff is TimestampSeries)
            {
                numberOfObservedRunoffValues = ((TimestampSeries)inputObservedRunoff).Items.Count;
            }
            else
            {
                throw new Exception("Unexpected exception");
            }
            observedSpecificRunoff = new TimestampSeries("Observed specific runoff", simulationStartTime, numberOfObservedRunoffValues, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            observedRunoff = new TimestampSeries("Observed Runoff", simulationStartTime, numberOfObservedRunoffValues, 1, TimestepUnit.Days, 0, Units.M3PrSec);

            for (int i = 0; i < numberOfTimesteps; i++)
            {
                DateTime fromTime = SimulationStartTime.AddDays(i);
                DateTime toTime = SimulationStartTime.AddDays(i + 1);
                precipitation.Items[i].Value = InputPrecipitation.GetValue(fromTime, toTime, precipitation.Unit);
                potentialEvaporation.Items[i].Value = InputPotentialEvaporation.GetValue(fromTime, toTime, potentialEvaporation.Unit);
                temperature.Items[i].Value = InputTemperature.GetValue(fromTime, toTime, temperature.Unit);
            }

            for (int i = 0; i < numberOfObservedRunoffValues; i++)
            {
                DateTime fromTime = SimulationStartTime.AddDays(i);
                DateTime toTime = SimulationStartTime.AddDays(i + 1);
                observedSpecificRunoff.Items[i].Value = InputObservedRunoff.GetValue(fromTime, toTime, observedSpecificRunoff.Unit) * (1000.0 * 3600 * 24 / CatchmentArea);
                observedRunoff.Items[i].Value = InputObservedRunoff.GetValue(fromTime, toTime, observedRunoff.Unit);
            }

            surfaceEvaporation = new TimestampSeries("SurfaceEvaporation", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            rootZoneEvaporation = new TimestampSeries("RootZoneEvaporation", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            rainfall = new TimestampSeries("Rainfall", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            snowfall = new TimestampSeries("Snowfall", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            snowMelt = new TimestampSeries("Snowmelt", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            snowStorage = new TimestampSeries("Snow storage", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.Millimiters);
            surfaceStorage = new TimestampSeries("Surface storage", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.Millimiters);
            rootZoneStorage = new TimestampSeries("Root zone storage", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.Millimiters);
            overlandflow = new TimestampSeries("Overlandflow", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            routedOverlandflow = new TimestampSeries("Routed overlandflow", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            interflow = new TimestampSeries("Inteflow", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            routedInterflow = new TimestampSeries("Routed interflow", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            baseflow = new TimestampSeries("Baseflow", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            routedBaseflow = new TimestampSeries("Routed baseflow", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            specificRunoff = new TimestampSeries("Specific runoff", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.MmPrDay);
            runoff = new TimestampSeries("Runoff", simulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0, Units.M3PrSec);

            OutputTimeSeries.Items.Clear();
            OutputTimeSeries.Items.Add(precipitation);
            OutputTimeSeries.Items.Add(potentialEvaporation);
            OutputTimeSeries.Items.Add(temperature);
            OutputTimeSeries.Items.Add(observedSpecificRunoff);
            OutputTimeSeries.Items.Add(observedRunoff);
            OutputTimeSeries.Items.Add(surfaceEvaporation);
            OutputTimeSeries.Items.Add(rootZoneEvaporation);
            OutputTimeSeries.Items.Add(rainfall);
            OutputTimeSeries.Items.Add(snowfall);
            OutputTimeSeries.Items.Add(snowMelt);
            OutputTimeSeries.Items.Add(snowStorage);
            OutputTimeSeries.Items.Add(surfaceStorage);
            OutputTimeSeries.Items.Add(rootZoneStorage);
            OutputTimeSeries.Items.Add(overlandflow);
            OutputTimeSeries.Items.Add(routedOverlandflow);
            OutputTimeSeries.Items.Add(interflow);
            OutputTimeSeries.Items.Add(routedInterflow);
            OutputTimeSeries.Items.Add(baseflow);
            OutputTimeSeries.Items.Add(routedBaseflow);
            OutputTimeSeries.Items.Add(specificRunoff);
            OutputTimeSeries.Items.Add(runoff);

            foreach (TimestampSeries ts in OutputTimeSeries.Items)
            {
                ts.IsVisible = false;
            }
            observedRunoff.IsVisible = true;
            runoff.IsVisible = true;


            isConfigurated = true;
        }

        public void Initialize()
        {
            if (inputObservedRunoff == null)
            {
                throw new Exception("InputObservedRunoff is undefined");
            }

            if (inputPrecipitation == null)
            {
                throw new Exception("IputPrecipitation is undefined");
            }

            if (inputPotentialEvaporation == null)
            {
                throw new Exception("InputPotentialEvaporation is undefined");
            }

            if (inputTemperature == null)
            {
                throw new Exception("InputTemperature is undefined");
            }

            if (!isConfigurated)
            {
                Configurate();
            }
            // -- reset initial values ---
            snowStorage.Items[0].Value = InitialSnowStorage;
            surfaceStorage.Items[0].Value = InitialSurfaceStorage;
            rootZoneStorage.Items[0].Value = InitialRootZoneStorage;
            routedOverlandflow.Items[0].Value = InitialOverlandFlow;
            routedInterflow.Items[0].Value = 0.5 * InitialOverlandFlow; //I know this is strange...
            routedBaseflow.Items[0].Value = InitialBaseFlow;
            runoff.Items[0].Value = InitialOverlandFlow + InitialInterFlow + InitialBaseFlow;
            specificRunoff.Items[0].Value = (InitialOverlandFlow + InitialInterFlow + InitialBaseFlow) * (1000.0 * 3600 * 24 / CatchmentArea);

            timestep = 1;
            
        }

        public void RunSimulation()
        {
            Initialize();
            ValidateParametersAndInitialValues();

            for (int i = 0; i < numberOfTimesteps-1; i++)
            {
                PerformTimeStep();
            }
        }
    
        
        public void PerformTimeStep()
        {
            // extract values for current time step from the timeseries
            int i = timestep;
            snowStorage.Items[i].Value = snowStorage.Items[i - 1].Value;
            surfaceStorage.Items[i].Value = surfaceStorage.Items[i - 1].Value;
            rootZoneStorage.Items[i].Value = rootZoneStorage.Items[i - 1].Value;

            
            if (temperature.Items[i].Value < 0)
            {
                snowfall.Items[i].Value = precipitation.Items[i].Value;
            }
            else //temperature above zero
            {
                rainfall.Items[i].Value = precipitation.Items[i].Value;
                snowMelt.Items[i].Value = Math.Min(snowStorage.Items[i].Value, temperature.Items[i].Value * SnowmeltCoefficient);
            }



            snowStorage.Items[i].Value += snowfall.Items[i].Value; ;
            snowStorage.Items[i].Value -= snowMelt.Items[i].Value;
            surfaceStorage.Items[i].Value += rainfall.Items[i].Value + snowMelt.Items[i].Value;

            // 2) -- Surface evaporation --
            surfaceEvaporation.Items[i].Value = Math.Min(surfaceStorage.Items[i].Value, potentialEvaporation.Items[i].Value);
            surfaceStorage.Items[i].Value -= surfaceEvaporation.Items[i].Value;


            // 3) -- Evaporation (evapotranspiration) from root zone
            rootZoneEvaporation.Items[i].Value = 0;
            if (surfaceEvaporation.Items[i].Value < potentialEvaporation.Items[i].Value)
            {
                rootZoneEvaporation.Items[i].Value = (potentialEvaporation.Items[i].Value - surfaceEvaporation.Items[i].Value) * (rootZoneStorage.Items[i-1].Value / RootZoneStorageCapacity);
                rootZoneStorage.Items[i].Value -= rootZoneEvaporation.Items[i].Value;
            }


            // 4) --- Interflow ---
            interflow.Items[i].Value = 0;
            if ((rootZoneStorage.Items[i-1].Value / RootZoneStorageCapacity) > InterflowTreshold)
            {
                interflow.Items[i].Value = InterflowCoefficient * Math.Min(surfaceStorage.Items[i].Value, SurfaceStorageCapacity) * ((rootZoneStorage.Items[i-1].Value / RootZoneStorageCapacity) - InterflowTreshold) / (1 - InterflowTreshold);
                surfaceStorage.Items[i].Value -= interflow.Items[i].Value;
            }
            

            // 5) Calculating Pn (Excess rainfall)
            double excessRainfall; //(Pn)
            if (surfaceStorage.Items[i].Value > SurfaceStorageCapacity)
            {
                excessRainfall = surfaceStorage.Items[i].Value - SurfaceStorageCapacity ;
                surfaceStorage.Items[i].Value = SurfaceStorageCapacity;
            }
            else
            {
                excessRainfall = 0;
            }

            // 6) Overland flow calculation
            if ((rootZoneStorage.Items[i-1].Value / RootZoneStorageCapacity) > OverlandFlowTreshold)
            {
                overlandflow.Items[i].Value = OverlandFlowCoefficient * excessRainfall * ((rootZoneStorage.Items[i-1].Value / RootZoneStorageCapacity) - OverlandFlowTreshold) / (1 - OverlandFlowTreshold);
            }
            else
            {
                overlandflow.Items[i].Value = 0;
            }

            // 7) infiltration into the root zone (DL)
            double infiltrationIntoRootZone = (excessRainfall - overlandflow.Items[i].Value) * (1 - rootZoneStorage.Items[i-1].Value / RootZoneStorageCapacity);
            rootZoneStorage.Items[i].Value += infiltrationIntoRootZone;

            // 8) infiltration into the ground water zone
            double groundwaterInfiltration = (excessRainfall - overlandflow.Items[i].Value) * (rootZoneStorage.Items[i-1].Value / RootZoneStorageCapacity);
            baseflow.Items[i].Value = groundwaterInfiltration;
 
         
            // 10) Routing
            routedOverlandflow.Items[i].Value = routedOverlandflow.Items[i-1].Value * Math.Exp(-1 / OverlandFlowTimeConstant) + overlandflow.Items[i].Value * (1 - Math.Exp(-1 / OverlandFlowTimeConstant));
          
            routedInterflow.Items[i].Value = routedInterflow.Items[i-1].Value * Math.Exp(-1 /InterflowTimeConstant ) + interflow.Items[i].Value * (1 - Math.Exp(-1 / InterflowTimeConstant));
            
            routedBaseflow.Items[i].Value = routedBaseflow.Items[i-1].Value * Math.Exp(-1 / BaseflowTimeConstant) + baseflow.Items[i].Value * (1 - Math.Exp(-1 / BaseflowTimeConstant));
                       
            // 11) Runoff
            specificRunoff.Items[i].Value = routedOverlandflow.Items[i].Value + routedInterflow.Items[i].Value + routedBaseflow.Items[i].Value;
            runoff.Items[i].Value  = CatchmentArea * specificRunoff.Items[i].Value / (1000.0 * 24 * 3600) ;

            timestep++;
        }

        public void ValidateParametersAndInitialValues()
        {
            //-- Initial variable validation --
            GreaterThanOrEqualToZeroValidation(InitialSnowStorage, "InitialSnowStorage");

            GreaterThanOrEqualToZeroValidation(InitialSurfaceStorage, "InitialSurfaceStorage");
            UpperLimitValidation(InitialSurfaceStorage, "InitialSurfaceStorage", SurfaceStorageCapacity);
 
            GreaterThanOrEqualToZeroValidation(InitialRootZoneStorage, "InitialRootZoneStorage");
            UpperLimitValidation(InitialRootZoneStorage, "InitialRootZoneStorage", RootZoneStorageCapacity);

            GreaterThanOrEqualToZeroValidation(InitialOverlandFlow, "InitialOverlandFlow");

            GreaterThanOrEqualToZeroValidation(InitialInterFlow, "InitialInterFlow");

            GreaterThanOrEqualToZeroValidation(InitialBaseFlow, "InitialBaseFlow");

            //-- Parameter validation --
            GreaterThanOrEqualToZeroValidation(CatchmentArea, "CatchmentArea");

            GreaterThanOrEqualToZeroValidation(SurfaceStorageCapacity, "SurfaceStorageCapacity");

            GreaterThanOrEqualToZeroValidation(RootZoneStorageCapacity, "RootZoneStorageCapacity");

            GreaterThanOrEqualToZeroValidation(SnowmeltCoefficient, "SnowmeltCoefficient");

            GreaterThanOrEqualToZeroValidation(OverlandFlowTreshold, "OverlandFlowTreshold");
            UpperLimitValidation(OverlandFlowTreshold, "OverlandFlowTreshold", 1.0);

            GreaterThanOrEqualToZeroValidation(OverlandFlowCoefficient, "OverlandFlowCoefficient");
            UpperLimitValidation(OverlandFlowCoefficient, "OverlandFlowCoefficient", 1.0);

            GreaterThanOrEqualToZeroValidation(OverlandFlowTimeConstant, "OverlandFlowTimeConstant");

            GreaterThanOrEqualToZeroValidation(InterflowCoefficient, "InterflowCoefficient");
            UpperLimitValidation(InterflowCoefficient, "InterflowCoefficient", 1.0);

            GreaterThanOrEqualToZeroValidation(InterflowTreshold, "InterflowTreshold");
            UpperLimitValidation(InterflowTreshold, "InterflowTreshold", 1.0);

            GreaterThanOrEqualToZeroValidation(InterflowTimeConstant, "InterflowTimeConstant");

            GreaterThanOrEqualToZeroValidation(BaseflowTimeConstant, "BaseflowTimeConstant");

            
        }

        private void GreaterThanOrEqualToZeroValidation(double x, string variableName)
        {
            if (x < 0)
            {
                throw new Exception("The property <" + variableName + "> must be greather than zero. " + variableName + " = " + x.ToString());
            }
        }

        private void UpperLimitValidation(double x, string variableName, double upperLimit)
        {
            if (x >= upperLimit)
            {
                throw new Exception("The property <" + variableName + "> must be less than  " + upperLimit.ToString() + " " + variableName + " = " + x.ToString());
            }
        }

        public string GetMassBalanceReport()
        {
            string report = "Flows\n\n";

            foreach (TimestampSeries ts in OutputTimeSeries.Items)
            {
                if (ts.Unit.Dimension.Time == -1)
                {
                    double acc = 0;
                    foreach (TimestampValue tsv in ts.Items)
                    {
                        acc += tsv.Value;
                    }

                    report += "Accumulated " + ts.Name + " = " + acc.ToString();

                    if (ts.Unit.ID == Units.M3PrSec.ID)
                    {
                        report += " Cubic meters\n";
                    }
                    else
                    {
                        report += " millimiters\n";
                    }
                }
            }

            report += "\n\nStorage changes\n\n";

            double totalStorageChange = 0;
            foreach (TimestampSeries ts in OutputTimeSeries.Items)
            {
                if (ts.Unit.Dimension.Length == 1 && ts.Unit.Dimension.Time == 0)
                {
                    double storageChange = ts.Items[ts.Items.Count - 1].Value - ts.Items[0].Value;
                    totalStorageChange += storageChange;
                    report += "Storage change for " + ts.Name + " = " + storageChange.ToString() + " millimiters\n";

                    
                }
            }

            return report;

        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                
            }
            isConfigurated = false;
        }

        #endregion
    }
}
