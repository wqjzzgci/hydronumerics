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

        private Timespan timespan;
        public Timespan TimeSpan
        {
            get { return timespan; }
            set { timespan = value; }
        }

        //private DateTime startTime;
        //public DateTime StartTime
        //{
        //    get { return startTime; }
        //    set { startTime = value; }
        //}

        //private DateTime endTime;
        //public DateTime EndTime
        //{
        //    get { return endTime; }
        //    set { endTime = value; }
        //}

        public TimespanValue(DateTime startTime, DateTime endTime, double value)
        {
            this.timespan = new Timespan(startTime, endTime);
            this.val = value; 
        }

        public TimespanValue(Timespan timespan, double value)
        {
            this.timespan = timespan;
            this.val = value;
        }
    }

}
