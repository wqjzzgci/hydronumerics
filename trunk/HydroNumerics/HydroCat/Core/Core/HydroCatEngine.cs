using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using HydroNumerics.Time.Core;
using HydroNumerics.Core;

namespace HydroNumerics.HydroCat.Core
{
    public class HydroCatEngine
    {

        #region ===== Time series ======
        public BaseTimeSeries PrecipitationTs{ get; set;}
        public BaseTimeSeries PotentialEvaporationTs { get; set; }  //Input
        public BaseTimeSeries TemperatureTs { get; set; } // Input
        public BaseTimeSeries ObservedRunoffTs { get; set; } // input

        public TimeSeriesGroup OutputTsg { get; private set; }

        private TimestampSeries runoffTs;
        private TimestampSeries overlandFlowTs;
        private TimestampSeries interFlowTs;
        private TimestampSeries baseFowTs;
        private TimestampSeries snowStorageTs;
        private TimestampSeries surfaceStorageTs;
        private TimestampSeries rootZoneStorageTs;

        public TimespanSeries RunoffTs { get; private set; }

        #endregion

        //public InitialValues InitialValues { get; private set; }

        //public Parameters Parameters { get; private set; }

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
        /// <summary>
        /// Start time for the simulation
        /// </summary>
        public DateTime SimulationStartTime { get; set; }

        /// <summary>
        /// End time for the simulation
        /// </summary>
        public DateTime SimulationEndTime { get; set; }
        #endregion

        // ============= output ================================

         /// <summary>
         /// The calculated specific runoff [Unit: mm/day]
         /// </summary>
        public double SpecificRunoff { get; private set; }
        
        /// <summary>
        /// The calculated riverflow [Unit: m3/sec]
        /// </summary>
        public double Runoff { get; private set; }


        #region ==== state variables =====
        /// <summary>
        /// Snow storage [Unit: millimiters] (Ss)
        /// </summary>
        [CategoryAttribute("State variables")]
        public double SnowStorage { get; private set; }

        /// <summary>
        /// Surface Storage [Unit: millimiters] (U)
        /// </summary>
        [CategoryAttribute("State variables")]
        public double SurfaceStorage { get; private set; }

        /// <summary>
        /// Root zone storage [Unit: millimiters] (L)
        /// </summary>
        [CategoryAttribute("State variables")]
        public double RootZoneStorage { get; private set; }

        /// <summary>
        /// Overland flow rate (specific flow, before routing) [Unit: Millimiters / day]
        /// </summary>
        [CategoryAttribute("State variables")]
        public double OverlandFlow { get; private set; }

        /// <summary>
        /// Inter flow rate (specific flow, before routing) [Unit: millimiters / day]
        /// </summary>
        [CategoryAttribute("State variables")]
        public double InterFlow { get; private set; }

        /// <summary>
        /// Base flow rate (specific flow, before routing) [Unit: millimiters / day]
        /// </summary>
        [CategoryAttribute("State variables")]
        public double BaseFlow { get; private set; }
        #endregion

        // ----------
        Unit mmUnit;
        Unit mmPrDayUnit; //
        Unit centigradeUnit;
        Unit m3PrSecUnit;

        int timestep = 0;
        double[] precipitation; //time sereis are copied to these arrays for performance reasons.
        double[] potentialEvaporation;
        double[] temperature;
 
        public bool IsInitialized { get; private set; }
        private bool isConfigurated = false;
       
        //public DateTime CurrentTime { get; private set; }
        int numberOfTimesteps;

        public HydroCatEngine()
        {
            IsInitialized = false;
            isConfigurated = false;
            
            //--- Default values ----
            SimulationStartTime = new DateTime(2010, 1, 1);
            SimulationEndTime = new DateTime(2011, 1, 1);
            //CurrentTime = SimulationStartTime.AddDays(0);

            //-- Default values (state variables)
            SnowStorage = InitialSnowStorage = 0;
            SurfaceStorage = InitialSurfaceStorage = 0;
            RootZoneStorage = InitialRootZoneStorage = 220;
            OverlandFlow = InitialOverlandFlow = 0;
            InterFlow = InitialInterFlow = 0;
            BaseFlow = InitialBaseFlow = 0.6;

            //-- Default values (parameters)
            this.CatchmentArea = 160000000;
            this.SnowmeltCoefficient = 2.0;
            this.SurfaceStorageCapacity = 18;
            this.RootZoneStorageCapacity = 250;
            this.OverlandFlowCoefficient = 0.61;
            this.InterflowCoefficient = 0.6; //??
            this.OverlandFlowTreshold = 0.38;
            this.InterflowTreshold = 0.08;
            this.OverlandFlowTimeConstant = 0.3;
            this.InterflowTimeConstant = 30;
            this.BaseflowTimeConstant = 2800;

            // -- Units --
            mmUnit = new HydroNumerics.Core.Unit("millimiters", 0.001, 0.0, "millimiters", new Dimension(1,0,0,0,0,0,0,0));
            mmPrDayUnit = new HydroNumerics.Core.Unit("mm pr day", 1.0/(1000*3600*24), 0, "millimiters pr day", new Dimension(1,0,-1,0,0,0,0,0));
            centigradeUnit = new HydroNumerics.Core.Unit("Centigrade", 1.0, -273.15,"degree centigrade", new Dimension(0,0,0,0, 1,0,0,0));
            m3PrSecUnit = new HydroNumerics.Core.Unit("m3 pr sec.", 1.0, 0.0, "cubic meters pr second",new Dimension(3,0,-1,0,0,0,0,0));
            
            // --- 
            //InitialValues = new InitialValues();
            //Parameters = new Parameters();

 
        }

        private void Configurate()
        {
            // -- prepare input arrays ---
            numberOfTimesteps = (int)(SimulationEndTime.ToOADate() - SimulationStartTime.ToOADate() - 0.5);
            precipitation = new double[numberOfTimesteps];
            potentialEvaporation = new double[numberOfTimesteps];
            temperature = new double[numberOfTimesteps];
            for (int i = 0; i < numberOfTimesteps; i++)
            {
                DateTime fromTime = SimulationStartTime.AddDays(i);
                DateTime toTime = SimulationStartTime.AddDays(i + 1);
                precipitation[i] = PrecipitationTs.GetValue(fromTime, toTime, mmPrDayUnit);
                potentialEvaporation[i] = PotentialEvaporationTs.GetValue(fromTime, toTime, mmPrDayUnit);
                temperature[i] = TemperatureTs.GetValue(fromTime, toTime, centigradeUnit);
            }

            //CurrentTime = SimulationStartTime.AddDays(0);  //TODO : current time kan vist godt fjernes.

            // -- prepare output ---
            overlandFlowTs = new TimestampSeries("Overlandflow", SimulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0);
            overlandFlowTs.Unit = mmPrDayUnit;
            interFlowTs = new TimestampSeries("Interflow", SimulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0);
            interFlowTs.Unit = mmPrDayUnit;
            baseFowTs = new TimestampSeries("Baseflow", SimulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0);
            baseFowTs.Unit = mmPrDayUnit;
            snowStorageTs = new TimestampSeries("Snow storage", SimulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0);
            snowStorageTs.Unit = mmUnit;
            surfaceStorageTs = new TimestampSeries("Surface storage", SimulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0);
            surfaceStorageTs.Unit = mmUnit;
            rootZoneStorageTs = new TimestampSeries("Rootzone storage", SimulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0);
            rootZoneStorageTs.Unit = mmUnit;
            runoffTs = new TimestampSeries("Runoff", SimulationStartTime, numberOfTimesteps, 1, TimestepUnit.Days, 0);
            runoffTs.Unit = m3PrSecUnit;
            OutputTsg = new TimeSeriesGroup();

            OutputTsg.Items.Add(PrecipitationTs);
            OutputTsg.Items.Add(PotentialEvaporationTs);
            OutputTsg.Items.Add(TemperatureTs);
            OutputTsg.Items.Add(runoffTs);
            OutputTsg.Items.Add(ObservedRunoffTs);
            OutputTsg.Items.Add(overlandFlowTs);
            OutputTsg.Items.Add(interFlowTs);
            OutputTsg.Items.Add(baseFowTs);
            OutputTsg.Items.Add(snowStorageTs);
            OutputTsg.Items.Add(surfaceStorageTs);
            OutputTsg.Items.Add(rootZoneStorageTs);
            
            isConfigurated = true;
        }

        public void Initialize()
        {
            // -- reset initial values ---
            SnowStorage = InitialSnowStorage;
            SurfaceStorage = InitialSurfaceStorage;
            RootZoneStorage = InitialRootZoneStorage;
            OverlandFlow = InitialOverlandFlow;
            InterFlow = InitialInterFlow;
            BaseFlow = InitialBaseFlow;
            timestep = 0;
            if (!isConfigurated)
            {
                Configurate();
            }

           
                                    
            IsInitialized = true;
        }

        public void RunSimulation()
        {
            Initialize();
            ValidateParametersAndInitialValues();

            for (int i = 0; i < numberOfTimesteps; i++)
            {
                PerformTimeStep();
            }

            
            
        }

        public void PerformTimeStep()
        {
 
            Step(precipitation[timestep], potentialEvaporation[timestep], temperature[timestep]);

            runoffTs.Items[timestep].Value = Runoff;
            overlandFlowTs.Items[timestep].Value = OverlandFlow;
            interFlowTs.Items[timestep].Value = InterFlow;
            baseFowTs.Items[timestep].Value = BaseFlow;
            snowStorageTs.Items[timestep].Value = SnowStorage;
            surfaceStorageTs.Items[timestep].Value = SurfaceStorage;
            rootZoneStorageTs.Items[timestep].Value = RootZoneStorage;

            
            timestep++;
        }
        
        public void Step(double precipitation, double potentialEvaporation, double temperature)
        {
            // extract values for current time step from the timeseries
           

            double yesterdaysOverlandFlow = OverlandFlow;
            double yesterdaysInterFlow = InterFlow;
            double yesterdaysBaseflow = BaseFlow;

            
            // 1) -- Precipitation, snowstorage, snow melt --
            if (temperature < 0)  
            {
                SnowStorage += precipitation;
            }
            else
            {   
                double snowMelt = Math.Min(SnowStorage, temperature * SnowmeltCoefficient);
                SnowStorage -= snowMelt;
                SurfaceStorage += (precipitation + snowMelt);
            }

            // 2) -- Surface evaporation --
            double surfaceEvaporation = Math.Min(SurfaceStorage, potentialEvaporation);
            SurfaceStorage -= surfaceEvaporation;


            // 3) -- Evaporation (evapotranspiration) from root zone
            if (surfaceEvaporation < potentialEvaporation)
            {
                double rootZoneEvaporation = (potentialEvaporation - surfaceEvaporation) * (RootZoneStorage / RootZoneStorageCapacity);
                RootZoneStorage -= rootZoneEvaporation;
            }


            // 4) --- Interflow ---
            if ((RootZoneStorage / RootZoneStorageCapacity) > InterflowTreshold)
            {
                InterFlow = InterflowCoefficient * Math.Min(SurfaceStorage, SurfaceStorageCapacity) * ((RootZoneStorage / RootZoneStorageCapacity) - InterflowTreshold) / (1 - InterflowTreshold);
            }
            SurfaceStorage -= InterFlow;

            // 5) Calculating Pn (Excess rainfall)
            double excessRainfall; //(Pn)
            if (SurfaceStorage > SurfaceStorageCapacity)
            {
                excessRainfall = SurfaceStorageCapacity - SurfaceStorage;
            }
            else
            {
                excessRainfall = 0;
            }

            SurfaceStorage -= excessRainfall;

            // 6) Overland flow calculation
            if ((RootZoneStorage / RootZoneStorageCapacity) > OverlandFlowTreshold)
            {
                OverlandFlow = OverlandFlowCoefficient * excessRainfall * ((RootZoneStorage / RootZoneStorageCapacity) - OverlandFlowTreshold) / (1 - OverlandFlowTreshold);
            }
            else
            {
                OverlandFlow = 0;
            }

            // 7) infiltration into the root zone (DL)
            double dl = (excessRainfall - OverlandFlow) / (1 - RootZoneStorage / RootZoneStorageCapacity);
            RootZoneStorage += dl;

            // 8) infiltration into the ground water zone
            double groundwaterInfiltration = excessRainfall - OverlandFlow - dl;
          
            // 9) Routing
            OverlandFlow = yesterdaysOverlandFlow * Math.Exp(-1 / OverlandFlowTimeConstant) + OverlandFlow * (1 - Math.Exp(-1 / OverlandFlowTimeConstant));

            InterFlow = yesterdaysInterFlow * Math.Exp(-1 /InterflowTimeConstant ) + InterFlow * (1 - Math.Exp(-1 / InterflowTimeConstant));

            BaseFlow = yesterdaysBaseflow * Math.Exp(-1 / BaseflowTimeConstant) + BaseFlow * (1 - Math.Exp(-1 / BaseflowTimeConstant));

            // 10) Runoff
            SpecificRunoff = OverlandFlow + InterFlow + BaseFlow;
            Runoff = CatchmentArea * SpecificRunoff / (1000.0 * 24 * 3600) ;
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

      


       

    }
}
