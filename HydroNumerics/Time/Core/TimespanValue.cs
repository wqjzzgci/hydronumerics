using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (endTime <= startTime)
            {
                throw new Exception("Attempt to contruct a TimespanValue object with an endTime that is smaller that or equal to startTime");
            }
            this.startTime = startTime;
            this.endTime = endTime;
            this.val = value; 
        }
    }

}
