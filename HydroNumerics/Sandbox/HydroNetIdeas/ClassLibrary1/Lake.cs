using System;
using System.Collections.Generic;
using System.Text;
using HydroNumerics.Geometry;
using HydroNumerics.Time.Core;

namespace ClassLibrary1
{
    public class Lake :IWaterBody
    {


        #region IWaterBody Members

        public string Id
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
            
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DateTime CurrentTime
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime ValidFrom
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime ValidTo
        {
            get { throw new NotImplementedException(); }
        }

        public IWaterBody DownstreamWaterBody
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<IWaterPacket> WaterPackets
        {
            get { throw new NotImplementedException(); }
        }

        public void InitializeAt(DateTime time)
        {
            throw new NotImplementedException();
        }

        public void UpdateTo(DateTime time)
        {
            throw new NotImplementedException();
        }

        #endregion

        public List<GroundwaterBoundary> GroundwaterBoundaries { get; private set; }

        public XYPolygon Shape { get; set; }

        public double Capacity { get; set; }


        public TimespanSeries Precipitation { get; set; }
        public TimespanSeries PotentialEvaporation { get; set; }
       
        
    }
}
