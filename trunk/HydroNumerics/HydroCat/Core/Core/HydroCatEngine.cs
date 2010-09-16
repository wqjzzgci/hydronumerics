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
        public InputTimeSeries InputTimeSeries { get; set; }
        public OutputTimeSeries OutputTimeSeries { get; private set; }
        public SurfaceStorageMassBalance SurfaceMassBalance { get; private set; }
        public RootZoneMassBalance RootZoneMassBalance { get; private set; }
        public SnowStorageMassBalance SnowStorageMassBalance { get; private set; }
        
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

        #region === accumulated output values
        [DescriptionAttribute("Accumulated precipitation [Unit: millimeters"), CategoryAttribute("Accumulated values")]
        public double Accprecipitation { get; private set; }
        #endregion

        int timestep = 0;
       
        public bool IsInitialized { get; private set; }
        private bool isConfigurated = false;

        double surfaceEvaporation;
        double rootZoneEvaporation;
       
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

            InputTimeSeries = new InputTimeSeries(this);
            OutputTimeSeries = new OutputTimeSeries();

            SurfaceMassBalance = new SurfaceStorageMassBalance();
            RootZoneMassBalance = new RootZoneMassBalance();
            SnowStorageMassBalance = new SnowStorageMassBalance();

        }

        private void Configurate()
        {
            
            
            isConfigurated = true;
        }

        public void Initialize()
        {

            numberOfTimesteps = (int)(SimulationEndTime.ToOADate() - SimulationStartTime.ToOADate() - 0.5);
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

            Accprecipitation = 0;


            OutputTimeSeries.Initialize();
            SurfaceMassBalance.Initialize(InitialSurfaceStorage);
            RootZoneMassBalance.Initialize(InitialRootZoneStorage);
            SnowStorageMassBalance.Initialize(InitialSnowStorage);
                                    
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
            double precipitation = InputTimeSeries.GetPrecipitation(timestep);
            double potentialEvaporation = InputTimeSeries.GetPotentialEvaporation(timestep);
            double temperature = InputTimeSeries.GetTemperature(timestep);

            // -- Do the timestep ---
            Step(InputTimeSeries.GetPrecipitation(timestep), InputTimeSeries.GetPotentialEvaporation(timestep), InputTimeSeries.GetTemperature(timestep));

            // == update output timeseries ===
            DateTime currentTime = simulationStartTime.AddDays(timestep);

            //  Met. Input data copied to output ----
            OutputTimeSeries.precipitationTs.Items.Add(new TimestampValue(currentTime, precipitation));
            OutputTimeSeries.potentialEvaporationTs.Items.Add(new TimestampValue(currentTime, potentialEvaporation));
            OutputTimeSeries.temperatureTs.Items.Add(new TimestampValue(currentTime, temperature));

             // -- outflows --
            OutputTimeSeries.surfaceEvaporationTs.Items.Add(new TimestampValue(currentTime, surfaceEvaporation));
            OutputTimeSeries.rootZoneEvaporationTs.Items.Add(new TimestampValue(currentTime, rootZoneEvaporation));
            OutputTimeSeries.overlandFlowTs.Items.Add(new TimestampValue(currentTime, OverlandFlow));
            OutputTimeSeries.interFlowTs.Items.Add(new TimestampValue(currentTime, InterFlow));
            OutputTimeSeries.baseFlowTs.Items.Add(new TimestampValue(currentTime, BaseFlow));
            OutputTimeSeries.runoffTs.Items.Add(new TimestampValue(currentTime, Runoff));
            OutputTimeSeries.specificRunoffTs.Items.Add(new TimestampValue(currentTime, SpecificRunoff));

            // -- observed runoff --
            OutputTimeSeries.observedRunoffTs.Items.Add(new TimestampValue(currentTime, InputTimeSeries.GetObservedRunoff(timestep)));
            OutputTimeSeries.observedSpecificRunoffTs.Items.Add(new TimestampValue(currentTime, InputTimeSeries.GetObservedRunoff(timestep) * 1000 * 3600 * 24.0/CatchmentArea));

            // -- Storages --
            OutputTimeSeries.snowStorageTs.Items.Add(new TimestampValue(currentTime, SnowStorage));
            OutputTimeSeries.surfaceStorageTs.Items.Add(new TimestampValue(currentTime, SurfaceStorage));
            OutputTimeSeries.rootZoneStorageTs.Items.Add(new TimestampValue(currentTime, RootZoneStorage));
                                
            timestep++;
        }
        
        public void Step(double precipitation, double potentialEvaporation, double temperature)
        {
            // extract values for current time step from the timeseries
           

            double yesterdaysOverlandFlow = OverlandFlow;
            double yesterdaysInterFlow = InterFlow;
            double yesterdaysBaseflow = BaseFlow;

            
            // 1) -- Precipitation, snowstorage, snow melt --
            double snowMelt = 0;
            double rainfall = 0;
            double snowfall = 0;
            
            if (temperature < 0)
            {
                snowfall = precipitation;
            }
            else //temperature above zero
            {
                rainfall = precipitation;
                snowMelt = Math.Min(SnowStorage, temperature * SnowmeltCoefficient);
             }

            SnowStorage += snowfall;
            SnowStorage -= snowMelt;
            SurfaceStorage += rainfall + snowMelt;

            // 2) -- Surface evaporation --
            surfaceEvaporation = Math.Min(SurfaceStorage, potentialEvaporation);
            SurfaceStorage -= surfaceEvaporation;


            // 3) -- Evaporation (evapotranspiration) from root zone
            rootZoneEvaporation = 0;
            if (surfaceEvaporation < potentialEvaporation)
            {
                rootZoneEvaporation = (potentialEvaporation - surfaceEvaporation) * (RootZoneStorage / RootZoneStorageCapacity);
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
                excessRainfall = SurfaceStorage - SurfaceStorageCapacity ;
                SurfaceStorage = SurfaceStorageCapacity;
            }
            else
            {
                excessRainfall = 0;
            }

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
            double infiltrationIntoRootZone = (excessRainfall - OverlandFlow) / (1 - RootZoneStorage / RootZoneStorageCapacity);
            RootZoneStorage += infiltrationIntoRootZone;

            // 8) infiltration into the ground water zone
            double groundwaterInfiltration = excessRainfall - OverlandFlow - infiltrationIntoRootZone;
          
            // 9) Mass balance calculation
            SurfaceMassBalance.SetValues(snowMelt, rainfall, surfaceEvaporation, OverlandFlow, InterFlow, infiltrationIntoRootZone, groundwaterInfiltration, SurfaceStorage);
            RootZoneMassBalance.SetValues(infiltrationIntoRootZone, rootZoneEvaporation, RootZoneStorage);
            SnowStorageMassBalance.SetValues(snowfall, snowMelt, SnowStorage);

            // 10) Routing
            OverlandFlow = yesterdaysOverlandFlow * Math.Exp(-1 / OverlandFlowTimeConstant) + OverlandFlow * (1 - Math.Exp(-1 / OverlandFlowTimeConstant));

            InterFlow = yesterdaysInterFlow * Math.Exp(-1 /InterflowTimeConstant ) + InterFlow * (1 - Math.Exp(-1 / InterflowTimeConstant));

            BaseFlow = yesterdaysBaseflow * Math.Exp(-1 / BaseflowTimeConstant) + groundwaterInfiltration * (1 - Math.Exp(-1 / BaseflowTimeConstant));

            // 11) Runoff
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
