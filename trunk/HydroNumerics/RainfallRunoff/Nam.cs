using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Time.Core;

namespace HydroNumerics.RainfallRunoff
{
    public class Nam
    {


        // Input tidsserier
        public BaseTimeSeries PrecipitationTs { get; set; }
        public BaseTimeSeries PotentialEvaporationTs { get; set; }
        public BaseTimeSeries TemperatureTs { get; set; }

        public InitialValues InitialValues { get; private set; }

        // ============== Initial values ===================================
        ///// <summary>
        ///// Initial snow storage [Unit: millimters] (Ss0)
        ///// </summary>
        //public double InitialSnowStorage     { get; set; } 

        ///// <summary>
        ///// Initial surface water storage [Unit: millimiters] (U0)
        ///// </summary>
        //public double InitialSurfaceStorage  { get; set; } 

        ///// <summary>
        ///// Initial Root zone storage [Unit: millimiters] (L0)
        ///// </summary>
        //public double InitialRootZoneStorage { get; set; }  //L0

        ///// <summary>
        ///// Initial overland flow rate [Unit: m3 / sec.] (QR10)
        ///// </summary>
        //public double InitialOverlandFlow    { get; set; }

        ///// <summary>
        ///// Initial interflow rage [Unit: m3 / sec]  (QR20)
        ///// </summary>
        //public double InitialInterflow       { get; set; } 

        ///// <summary>
        ///// Initial base flow rage [Unit: m3 / sec:] (BF0)
        ///// </summary>
        //public double InitialBaseFlow        { get; set; }  //BF0

        public Parameters Parameters { get; private set; }
        // ================ Input parametre ===============================
        /// <summary>
        /// Catchment area [unit: m2] (Area)
        /// </summary>
        public double CatchmentArea           { get; set; }

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
        /// Overland flow tresshold [Unit: millimiters] (TOF) (Cl2)
        /// If the relative moisture content of the roor zone is above the overland flow treshold
        /// overland flow is generated.
        /// </summary>
        public double OverlandFlowTreshold { get; set; }

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
        /// Interflow coefficient. [Unit: dimensionless] (= 1/CKIF) (Cif)
        /// </summary>
        public double InterflowCoefficient { get; set; }

        /// <summary>
        /// Interflow treshold [Unit: millimiters] (TIF) (CL1)
        /// </summary>
        public double InterflowTreshold { get; set; }

        /// <summary>
        /// Interflow routing time constant [unit: Days]
        /// </summary>
        public double InterflowTimeConstant { get; set; }
 
        /// <summary>
        /// Base flow routing time constant[Unit: Days] (CKBF) (kb)
        /// </summary>
        public double BaseflowTimeConstant { get; set; } // (CKBF) (kb)

        /// <summary>
        /// Baseflow treshold [Unit: millimiters] (TG)
        /// </summary>
        public double BaseFlowTreshold { get; set; } 

        // ======   Simulation control input parameters ================
        /// <summary>
        /// Start time for the simulation
        /// </summary>
        public DateTime SimulationStartTime { get; set; }

        /// <summary>
        /// End time for the simulation
        /// </summary>
        public DateTime SimulationEndTime { get; set; }
      

        // ============= output ================================

        /// <summary>
        /// The calculated specific runoff [Unit: mm/day]
        /// </summary>
        public double SpecificRunoff { get; private set; }
        
        /// <summary>
        /// The calculated riverflow [Unit: m3/sec]
        /// </summary>
        public double Runoff { get; private set; }

        
        // state variables
        double snowStorage;
        double surfaceStorage;
        double rootZoneStorage;

        double overlandFlow;
        double interFlow;
        double baseFlow;

        // ----------
        HydroNumerics.Core.Unit mmPrDayUnit; //
        HydroNumerics.Core.Unit centigradeUnit;

        // Output tidsserier
        public TimespanSeries RunoffTs { get; private set; }

        public bool IsInitialized { get; private set; }
       
        public DateTime CurrentTime { get; private set; }


        public Nam()
        {
            IsInitialized = false;
            
            //--- Default values ----
            SimulationStartTime = new DateTime(2010, 1, 1);
            SimulationEndTime = new DateTime(2011, 1, 1);
            CurrentTime = SimulationStartTime.AddDays(0);

            // -- Units --
            mmPrDayUnit = new HydroNumerics.Core.Unit("mm pr day", 1.0/(1000*3600*24), 0);
            centigradeUnit = new HydroNumerics.Core.Unit("Centigrade", 1.0, -273.15);

            // --- 
            InitialValues = new InitialValues();
            Parameters = new Parameters();

 
        }

        public void Initialize()
        {
            //Dt = new TimeSpan(24, 0, 0);

            // -- Initial values


            snowStorage = InitialValues.SnowStorage;
            surfaceStorage = InitialValues.SurfaceStorage;
            rootZoneStorage = InitialValues.RootZoneStorage;

            overlandFlow = InitialValues.OverlandFlow;
            interFlow = InitialValues.InterFlow;
            baseFlow = InitialValues.BaseFlow;

            RunoffTs = new TimespanSeries();

            CurrentTime = SimulationStartTime.AddDays(0);

                        
            IsInitialized = true;
        }

        public void RunModel()
        {
           
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
            CurrentTime = CurrentTime.AddDays(1);
        }
        
        public void Step(double precipitation, double potentialEvaporation, double temperature)
        {
            // extract values for current time step from the timeseries
           

            double yesterdaysOverlandFlow = overlandFlow;
            double yesterdaysInterFlow = interFlow;
            double yesterdaysBaseflow = baseFlow;

            
            // 1) -- Precipitation, snowstorage, snow melt --
            if (temperature < 0)  
            {
                snowStorage += precipitation;
            }
            else
            {   
                double snowMelt = Math.Min(snowStorage, temperature * SnowmeltCoefficient);
                snowStorage -= snowMelt;
                surfaceStorage += (precipitation + snowMelt);
            }

            // 2) -- Surface evaporation --
            double surfaceEvaporation = Math.Min(surfaceStorage, potentialEvaporation);
            surfaceStorage -= surfaceEvaporation;


            // 3) -- Evaporation (evapotranspiration) from root zone
            if (surfaceEvaporation < potentialEvaporation)
            {
                double rootZoneEvaporation = (potentialEvaporation - surfaceEvaporation) * (rootZoneStorage / RootZoneStorageCapacity);
                rootZoneStorage -= rootZoneEvaporation;
            }


            // 4) --- Interflow ---
            if ((rootZoneStorage / RootZoneStorageCapacity) > InterflowTreshold)
            {
                interFlow = InterflowCoefficient * Math.Min(surfaceStorage, SurfaceStorageCapacity) * ((rootZoneStorage / RootZoneStorageCapacity) - InterflowTreshold) / (1 - InterflowTreshold);
            }
            surfaceStorage -= interFlow;

            // 5) Calculating Pn (Excess rainfall)
            double excessRainfall; //(Pn)
            if (surfaceStorage > SurfaceStorageCapacity)
            {
                excessRainfall = SurfaceStorageCapacity - surfaceStorage;
            }
            else
            {
                excessRainfall = 0;
            }

            surfaceStorage -= excessRainfall;

            // 6) Overland flow calculation
            if ((rootZoneStorage / RootZoneStorageCapacity) > OverlandFlowTreshold)
            {
                overlandFlow = OverlandFlowCoefficient * excessRainfall * ((rootZoneStorage / RootZoneStorageCapacity) - OverlandFlowTreshold) / (1 - OverlandFlowTreshold);
            }
            else
            {
                overlandFlow = 0;
            }

            // 7) infiltration into the root zone (DL)
            double dl = (excessRainfall - overlandFlow) / (1 - rootZoneStorage / RootZoneStorageCapacity);
            rootZoneStorage += dl;

            // 8) infiltration into the ground water zone
            double groundwaterInfiltration = excessRainfall - overlandFlow - dl;
          
            // 9) Routing
            overlandFlow = yesterdaysOverlandFlow * Math.Exp(-1 / OverlandFlowTimeConstant) + overlandFlow * (1 - Math.Exp(-1 / OverlandFlowTimeConstant));

            interFlow = yesterdaysInterFlow * Math.Exp(-1 /InterflowTimeConstant ) + interFlow * (1 - Math.Exp(-1 / InterflowTimeConstant));

            baseFlow = yesterdaysBaseflow * Math.Exp(-1 / BaseflowTimeConstant) + baseFlow * (1 - Math.Exp(-1 / BaseflowTimeConstant));

            // 10) Runoff
            SpecificRunoff = overlandFlow + interFlow + baseFlow;
            Runoff = CatchmentArea * SpecificRunoff * 24 * 3600 / 1000.0;
                        
 

            
        }

      


       

    }
}
