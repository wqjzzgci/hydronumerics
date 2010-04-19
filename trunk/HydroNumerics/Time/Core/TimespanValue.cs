using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{
    public class TimespanValue
    {

        private double val;
        public double Value
        {
            get { return val; }
            set { val = value;}
        }

        public Timespan TimeSpan
        {
            get { return new Timespan(startTime, endTime); }
            set 
            {
                startTime = value.Start;
                endTime = value.End;
            }
        }

        private DateTime startTime;
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        private DateTime endTime;
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        public TimespanValue(DateTime startTime, DateTime endTime, double value)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.val = value; 
        }

        public TimespanValue(Timespan timespan, double value)
        {
            this.startTime = timespan.Start;
            this.endTime = timespan.End;
            this.val = value;
        }
    }

}
