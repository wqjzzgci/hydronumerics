using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
    public class Timespan
    {
        private DateTime start;
        private DateTime end;

        public Timespan(DateTime start, DateTime end)
        {
            if (end <= start)
            {
                throw new Exception("Attempt to contruct a TimespanValue object with an endTime that is smaller that or equal to startTime");
            }
            this.start = start;
            this.end = end;
        }

        public DateTime Start
        {
            get { return start; }
            set { start = value; }
        }

        public DateTime End
        {
            get { return end; }
            set { end = value; }
        }

        public bool Includes(Timespan timespan)
        {
            if (this.start <= timespan.start && this.end >= timespan.end)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Includes(DateTime time)
        {
            if (this.start <= time && time <= this.end)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsOverlapping(Timespan timespan)
        {
            if ((this.start > timespan.start && timespan.start < this.end) || (this.start < timespan.end && timespan.end < this.end))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
