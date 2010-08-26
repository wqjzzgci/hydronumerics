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
        public TimespanSeries PrecipitationTs { get; set; }
        public TimespanSeries PotentialEvaporationTs { get; set; }
        public TimestampSeries TemperatureTs { get; set; }

        // Input parametre
        double lowerZoneStorageCapacity;
        double surfaceStorageCapacity;

        double cL1; // when the relative storage in the lower zone is above this parameter, interflow is generated.
        double cLi; // Interflow coefficient

        double cl2;
        double cloF;

        double CatchmentArea;


        double CSnow;   //Catchment specifik melting coefficient for snow 



        // Output tidsserier
        public TimespanSeries Runoff { get; private set; }


        // Storage variables
        double surfaceStorage;
        double snowStorage;
        double lowerZoneStorage;


        double snowMelt;
        double upperZoneEvaporation;
       
        double lowerZoneEvaporation;

        double interFlow;

        //public double Area {get; set;}

        //public double Carea {get; set;}
        //public double Csnow {get; set;}
        //public double Umax {get; set;}
        //public double Lmax {get; set;}
        //public double CQOF {get; set;}
        //public double CKIF {get; set;}
        //public double TOF {get; set;}
        //public double TIF {get; set;}
        //public double TG {get; set;}
        //public double CK12 {get; set;}
        //public double CKBF {get; set;}

        //public double Ss0 {get; set;}
        //public double U0 {get; set;}
        //public double L0 {get; set;}
        //public double QR10 {get; set;}
        //public double QR20 {get; set;}
        //public double BF0 {get; set;}




        public bool IsInitialized { get; private set; }
        public DateTime SimulationStartTime { get; set; }
        public DateTime SimulationEndTime { get; set; }
        public System.TimeSpan Dt { get; set; }

        public DateTime CurrentTime { get; private set; }

        //private double Ss, Qs, U1, EAU, EAL, U2, QIF, U3, PN, Ut, QOF, PN_QOF, GX, DLX, G, DL, Lt, LtOverLmax, CKQOF, OF1, OF2, IF1, IF2, BF, Qsim;




        public Nam()
        {
            IsInitialized = false;
            
            SimulationStartTime = new DateTime(2010, 1, 1);
            SimulationEndTime = new DateTime(2011, 1, 1);

            CurrentTime = SimulationStartTime.AddDays(0);
        }

        public void Initialize()
        {
            Dt = new TimeSpan(24, 0, 0);

            IsInitialized = true;
            

        }

        public void RunModel()
        {
            for (CurrentTime = SimulationStartTime; CurrentTime < SimulationStartTime; CurrentTime += Dt)
            {
                PerformTimeStep();
            }
            
        }



        public void PerformTimeStep()
        {
            // extract values for current time step from the timeseries
            double precip = PrecipitationTs.GetValue(CurrentTime, CurrentTime + Dt); // precipitation for this time step
            double potentialEvaporation = PotentialEvaporationTs.GetValue(CurrentTime, CurrentTime + Dt);   // Potential evaporation for this time step
            double temperature = TemperatureTs.GetValue(CurrentTime, CurrentTime + Dt);  // Temperatuer for this time step
            
            
            
            double dt = Dt.TotalSeconds; // timesteplength in seconds;
            

            
            // 1) -- Precipitation, snowstorage, snow melt --
            if (temperature < 0)  //TODO: sørg for at temperaturen er i centigrades
            {
                snowStorage += precip * dt;
            }
            else
            {   
                snowMelt = Math.Min(snowStorage, temperature * CSnow);
                surfaceStorage += (precip * snowMelt) * dt;

                
            }

            // 2) -- Upper zone evaporation --
            upperZoneEvaporation = Math.Min(surfaceStorage, potentialEvaporation) * dt;
            surfaceStorage -= upperZoneEvaporation;

            
            // 3) -- Evaporation (evapotranspiration) from lower zone
            double lowerZoneRelativeStorage = lowerZoneStorage / lowerZoneStorageCapacity;
            if (upperZoneEvaporation < potentialEvaporation)
            {
                lowerZoneEvaporation = (potentialEvaporation - upperZoneEvaporation) * lowerZoneRelativeStorage * dt;
            }
            else
            {
                lowerZoneEvaporation = 0;
            }

            // 4) Interflow
            if (lowerZoneRelativeStorage <= cL1)
            {
                interFlow = 0;
            }
            else
            {
                interFlow = cLi * surfaceStorage * (lowerZoneRelativeStorage - cL1) / (1 - cL1);
            }

            surfaceStorage -= interFlow;

            // 5) Calculating Pn
            double pn;
            if (surfaceStorage > surfaceStorageCapacity)
            {
                pn = surfaceStorageCapacity - surfaceStorage;
            }
            else
            {
                pn = 0;
            }

            surfaceStorage -= pn;

            double overlandFlow;

            // 6) Overland flow calculation
            if (lowerZoneRelativeStorage > cl2)
            {
                overlandFlow = cloF * pn * (lowerZoneRelativeStorage - cl2) / (1 - cl2);
            }
            else
            {
                overlandFlow = 0;
            }


            

        }



       

    }
}
