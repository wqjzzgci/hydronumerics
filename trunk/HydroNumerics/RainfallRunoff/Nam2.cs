using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Time.Core;

namespace HydroNumerics.RainfallRunoff
{
    public class Nam2
    {

        public DateTime SimulationStartTime { get; set; }
        public DateTime SimulationEndTime { get; set; }
        public System.TimeSpan Dt { get; set; }

        public DateTime CurrentTime { get; private set; }


        // Input tidsserier
        public TimespanSeries PrecipitationTs { get; set; }
        public TimespanSeries PotentialEvaporationTs { get; set; }
        public TimestampSeries TemperatureTs { get; set; }
        
        double Area = 160;
        double Carea = 1;
        double Csnow = 2;
        double Umax = 18;
        double Lmax = 250;
        double CQOF = 0.61;
        double CKIF = 870;
        double TOF = 0.38;
        double TIF = 0.08;
        double TG = 0.25;
        double CK12 = 30;
        double CKBF = 2800;

        // Initial values
        double Ss0 = 0;
        double U0 = 0;
        double L0 = 260;
        double QR10 = 0;
        double QR20 = 0;
        double BF0 = 0.6;

        // --------
        double p;
        double Epot;
        double Temp;
        double Ss;
        double Qs;
        double U1;
        double EAU;
        double EAL;
        double U2;
        double QIF;
        double U3;
        double PN;
        double Ut;
        double QOF;
        double PN_QOF;
        double G_;
        double DL_;
        double G;
        double DL;
        double Lt;
        double LtOverLmax;
        double CKQOF;
        double OF1;
        double OF2;
        double IF1;
        double IF2;
        double BF;
        double Qsim;
        double U_hat_IF;

        public void Initialize()
        {
            Ss = Ss0;
            Ut = U0;
            Lt = L0;
            LtOverLmax = Lt / Lmax;
            OF1 = 0.5 * QR10;
            OF2 = 0.5 * QR20;
            IF1 = 0.5 * QR10;
            IF2 = 0.5 * QR20;
            BF = BF0;
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
            p = PrecipitationTs.GetValue(CurrentTime, CurrentTime + Dt); // precipitation for this time step
            Epot = PotentialEvaporationTs.GetValue(CurrentTime, CurrentTime + Dt);   // Potential evaporation for this time step
            Temp = TemperatureTs.GetValue(CurrentTime, CurrentTime + Dt);  // Temperatuer for this time step


            // -- Qs ---------
            double melt;
            if (Temp < 0)
            {
                melt = Ss * Csnow;
            }
            else
            {
                melt = 0;
            }
            Qs = Math.Min(Ss, melt);


            // -- Ss -------------
            if (Temp < 0)
            {
                Ss += p;
            }
            else
            {
                Ss -= Qs;
            }

            // -- U1 --
            if (Temp > 0)
            {
                U1 += p + Qs;
            }

            // -- Eau ---
            if (U1 > Epot)
            {
                EAU = U1;
            }
            else
            {
                EAU = Epot;
            }

            // -- EAL
            if (EAU < Epot)
            {
                EAL = (Epot - EAU) * LtOverLmax;
            }
            else
            {
                EAL = 0;
            }

            // -- U2
            U2 = U1 - EAU;

            // -- U_hat_IF
            U_hat_IF = Math.Min(Umax, U2);

            // -- QIF --
            if (LtOverLmax > TIF)
            {
                //QIF = CKIF * (LtOverLmax - TIF)/(1-TIF)
            }


            
        }

    }
}
