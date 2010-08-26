using System;
using System.Collections.Generic;
using System.Text;
using HydroNumerics.Time.Core;

namespace ClassLibrary1
{
    public class MyModel
    {

        public void ConfigAndRun()
        {
            DateTime simulationStart = new DateTime(2010, 1, 1);
            DateTime simulationEnd = new DateTime(2010,2,1);
            TimeSpan dt = new TimeSpan(1, 0, 0);
            
            Lake upperLake = new Lake();

            upperLake.Shape = new HydroNumerics.Geometry.XYPolygon();
            upperLake.Name = "UpperLake";
            upperLake.Id = "dsdkek123";
            upperLake.Capacity = 2000;
            upperLake.Precipitation = new TimespanSeries();
            upperLake.PotentialEvaporation = new TimespanSeries();

            WaterPacket waterPacket = new WaterPacket();
            waterPacket.WaterVolume = upperLake.Capacity;
            waterPacket.AddedFrom = simulationStart;
            waterPacket.AddedUntil = simulationStart + dt;
            upperLake.WaterPackets.Add(waterPacket);

            Lake lowerLake = new Lake();
            lowerLake.Shape = new HydroNumerics.Geometry.XYPolygon();
            lowerLake.Name = "Lower lake";
            lowerLake.Id = "2";

            GroundwaterBoundary groundwaterBoundary = new GroundwaterBoundary();
            groundwaterBoundary.ContactArea = upperLake.Shape;
            groundwaterBoundary.Distance = 2.3;
            groundwaterBoundary.HydraulicConductance = 1e-4;

            upperLake.GroundwaterBoundaries.Add(groundwaterBoundary);
            upperLake.DownstreamWaterBody = lowerLake;

            List<IWaterBody> waterBodies = new List<IWaterBody>();

            waterBodies.Add(upperLake);
            waterBodies.Add(lowerLake);

            foreach (IWaterBody waterBody in waterBodies)
            {
                waterBody.InitializeAt(simulationStart);
            }

            for (DateTime time = simulationStart; time < simulationEnd; time += dt)
            {
                foreach (IWaterBody waterBody in waterBodies)
                {
                    waterBody.UpdateTo(time);
                }
            }

                        
        }

    }
}
