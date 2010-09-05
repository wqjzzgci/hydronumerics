using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Time.Core;

namespace HydroNumerics.RainfallRunoff
{
    public class HydroCat
    {


        // Input tidsserier
        public BaseTimeSeries PrecipitationTs { get; set; }
        public BaseTimeSeries PotentialEvaporationTs { get; set; }
        public BaseTimeSeries TemperatureTs { get; set; }

        //public InitialValues InitialValues { get; private set; }

        //public Parameters Parameters { get; private set; }

        // ====== Initial values ======================================

        
        #region =========  Calibration parameters =====================
        /// <summary>
        /// Catchment area [unit: m2] (Area)
        /// </summary>
        public double CatchmentArea { get; set; }

        /// <summary>
        /// Surface water storage capacity (max capacity) [Unit: millimiters] (U*)
        /// </summary>
        public double SurfaceStorageCapacity { get; set; }

        /// <summary>
        /// Root zone capacity [Unit: millimiters] (L*)
        /// </summary>
        public double RootZoneStorageCapacity { get; set; }

        /// <summary>
        /// Snow melting coefficient [Unit: dimensionless] (Cs)
        /// </summary>
        public double SnowmeltCoefficient { get; set; }

        /// <summary>
        /// Overland flow coefficient [Unit: dimensionless] (Cof)
        /// Determins the fraction of excess water that runs off as overland flow
        /// </summary>
        public double OverlandFlowCoefficient { get; set; }

        /// <summary>
        /// Overland flow routing time constant [Unit: Days]  (Ko)
        /// </summary>
        public double OverlandFlowTimeConstant { get; set; }

        /// <summary>
        /// Overland flow treshold [Unit: dimensionless] (TOF) (CL2)
        /// If the relative moisture content of the roor zone is above the overland flow treshold
        /// overland flow is generated. The overland flow treshold must be in the interval [0,1].
        /// </summary>
        public double OverlandFlowTreshold { get; set; }

        /// <summary>
        /// Interflow coefficient. [Unit: dimensionless] (CIf)
        /// Must be in the interval [0,1]
        /// </summary>
        public double InterflowCoefficient { get; set; }

        /// <summary>
        /// Interflow routing time constant [unit: Days]
        /// </summary>
        public double InterflowTimeConstant { get; set; }

        /// <summary>
        /// Interflow treshold [Unit: millimiters] (CL1)
        /// Must be in the interval [0,1]
        /// </summary>
        public double InterflowTreshold { get; set; }

        /// <summary>
        /// Base flow routing time constant[Unit: Days] (CKBF) (kb)
        /// </summary>
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
        public double SnowStorage { get; set; }

        /// <summary>
        /// Surface Storage [Unit: millimiters] (U)
        /// </summary>
        public double SurfaceStorage { get; set; }

        /// <summary>
        /// Root zone storage [Unit: millimiters] (L)
        /// </summary>
        public double RootZoneStorage { get; set; }

        /// <summary>
        /// Overland flow rate (specific flow, before routing) [Unit: Millimiters / day]
        /// </summary>
        public double OverlandFlow { get; set; }

        /// <summary>
        /// Inter flow rate (specific flow, before routing) [Unit: millimiters / day]
        /// </summary>
        public double InterFlow { get; set; }

        /// <summary>
        /// Base flow rate (specific flow, before routing) [Unit: millimiters / day]
        /// </summary>
        public double BaseFlow { get; set; }
        #endregion

        // ----------
        HydroNumerics.Core.Unit mmPrDayUnit; //
        HydroNumerics.Core.Unit centigradeUnit;

        // Output tidsserier
        public TimespanSeries RunoffTs { get; private set; }

        public bool IsInitialized { get; private set; }
       
        public DateTime CurrentTime { get; private set; }


        public HydroCat()
        {
            IsInitialized = false;
            
            //--- Default values ----
            SimulationStartTime = new DateTime(2010, 1, 1);
            SimulationEndTime = new DateTime(2011, 1, 1);
            CurrentTime = SimulationStartTime.AddDays(0);

            //-- Default values (state variables)
            this.SnowStorage = 0;
            this.SurfaceStorage = 0;
            this.RootZoneStorage = 220;
            this.OverlandFlow = 0;
            this.InterFlow = 0;
            this.BaseFlow = 0.6;

            //-- Default values (parameters)
            this.CatchmentArea = 1600000;
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
            mmPrDayUnit = new HydroNumerics.Core.Unit("mm pr day", 1.0/(1000*3600*24), 0);
            centigradeUnit = new HydroNumerics.Core.Unit("Centigrade", 1.0, -273.15);

            // --- 
            //InitialValues = new InitialValues();
            //Parameters = new Parameters();

 
        }

        public void Initialize()
        {
            // -- Initial values
            //SnowStorage = InitialValues.SnowStorage;
            //SurfaceStorage = InitialValues.SurfaceStorage;
            //RootZoneStorage = InitialValues.RootZoneStorage;
            //OverlandFlow = InitialValues.OverlandFlow;
            //InterFlow = InitialValues.InterFlow;
            //BaseFlow = InitialValues.BaseFlow;

            

            RunoffTs = new TimespanSeries();

            CurrentTime = SimulationStartTime.AddDays(0);

                        
            IsInitialized = true;
        }

        public void RunSimulation()
        {
            ValidateParametersAndInitialValues();

            while (CurrentTime < SimulationEndTime) 
            {
                PerformTimeStep();
            }
            
        }

        public void PerformTimeStep()
        {
            double precipitation = PrecipitationTs.GetValue(CurrentTime, CurrentTime.AddDays(1), mmPrDayUnit); // precipitation for this time step
            double potentialEvaporation = PotentialEvaporationTs.GetValue(CurrentTime, CurrentTime.AddDays(1), mmPrDayUnit);   // Potential evaporation for this time step
            double temperature = TemperatureTs.GetValue(CurrentTime, CurrentTime.AddDays(1), centigradeUnit);  // Temperatuer for this time step

            Step(precipitation, potentialEvaporation, temperature);
            RunoffTs.AddSiValue(CurrentTime, CurrentTime.AddDays(1), Runoff);
            CurrentTime = CurrentTime.AddDays(1);
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
            Runoff = CatchmentArea * SpecificRunoff * 24 * 3600 / 1000.0;
        }

        public void ValidateParametersAndInitialValues()
        {
            //-- State variable validation --
            GreaterThanOrEqualToZeroValidation(SnowStorage, "SnowStorage");
            GreaterThanOrEqualToZeroValidation(SurfaceStorage, "SurfaceStorage");
            GreaterThanOrEqualToZeroValidation(RootZoneStorage, "RootZoneStorage");
            GreaterThanOrEqualToZeroValidation(OverlandFlow, "OverlandFlow");
            GreaterThanOrEqualToZeroValidation(InterFlow, "InterFlow");
            GreaterThanOrEqualToZeroValidation(BaseFlow, "BaseFlow");

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
            if (x > upperLimit)
            {
                throw new Exception("The property <" + variableName + "> must be less than or equal to " + upperLimit.ToString() + " " + variableName + " = " + x.ToString());
            }
        }

      


       

    }
}
