using System;
using System.Collections.Generic;
using System.Text;
using HydroNumerics.Geometry;

namespace ClassLibrary1
{
    public class GroundwaterBoundary
    {
        public void Initialize() { }
        public double GetGroundwaterExchangeRate(double waterLevel)
        {
            throw new NotImplementedException();
        }

        public double HydraulicConductance { get; set; }
        public double Distance { get; set; }
        public XYPolygon ContactArea { get; set; }
    }
}
