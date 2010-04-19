using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{
    public class TimespanSeries : BaseTimeSeries
    {
        public TimespanSeries()
        {
            //TODO: call base constructor
            timespanValues = new System.ComponentModel.BindingList<TimespanValue>();
        }
        private System.ComponentModel.BindingList<TimespanValue> timespanValues;

        public System.ComponentModel.BindingList<TimespanValue> TimespanValues
        {
            get { return timespanValues; }
            set { timespanValues = value; }
        }
        
        public override int Count
        {
            get { return timespanValues.Count; }
        }

        public override void ConvertUnit(HydroNumerics.Core.Unit newUnit)
        {
            throw new NotImplementedException();
        }
        
        public void SetValue(int index, double value)
        {
            if (index < 0 || index >= Count)
            {
                throw new Exception("Index was out of range");
            }
            timespanValues[index].Value = value;
        }

        public void SetValue(int index, double value, bool fromSiUnit)
        {
            if (index < 0 || index >= Count)
            {
                throw new Exception("Index was out of range");
            }

            timespanValues[index].Value = this.Unit.FromSiToThisUnit(value);
            
        }

        public void SetValue(int index, double value, Unit fromUnit)
        {
            if (index < 0 || index >= Count)
            {
                throw new Exception("Index was out of range");
            }

            timespanValues[index].Value = this.Unit.FromUnitToThisUnit(value, fromUnit);
        }
        
        public void AppendValue(double value)
        {
            if (timespanValues.Count >= 1)
            {
                int yearDiff = timespanValues[Count - 1].TimeSpan.End.Year - timespanValues[Count - 1].TimeSpan.Start.Year;
                int monthDiff = timespanValues[Count - 1].TimeSpan.End.Month - timespanValues[Count - 1].TimeSpan.Start.Month;
                int dayDiff = timespanValues[Count - 1].TimeSpan.End.Day - timespanValues[Count - 1].TimeSpan.Start.Day;
                int hourDiff = timespanValues[Count - 1].TimeSpan.End.Hour - timespanValues[Count - 1].TimeSpan.Start.Hour;
                int minuteDiff = timespanValues[Count - 1].TimeSpan.End.Minute - timespanValues[Count - 1].TimeSpan.Start.Minute;
                int secondDiff = timespanValues[Count - 1].TimeSpan.End.Second - timespanValues[Count - 1].TimeSpan.Start.Second;

                DateTime start = DateTime.FromOADate(timespanValues[Count - 1].TimeSpan.Start.ToOADate());
                DateTime end;
                if (yearDiff == 0 && dayDiff == 0 && hourDiff == 0 && minuteDiff == 0 && secondDiff == 0)
                {
                    end = start.AddMonths(monthDiff);
                }
                else
                {
                    end = timespanValues[Count - 1].TimeSpan.End.AddTicks(timespanValues[Count - 1].TimeSpan.End.Ticks - timespanValues[Count - 1].TimeSpan.Start.Ticks);
                }
                timespanValues.Add(new TimespanValue(start, end, value));
            }
            else 
            {
                throw new Exception("AppendValues method was invoked for empty TimespanSeries object");
            }
            
        }

        public void AppendValue(double value, bool fromSiUnit)
        {
            if (fromSiUnit)
            {
                AppendValue(Unit.ToSiUnit(value));
            }
            else
            {
                AppendValue(value);
            }
        }

        public void AppendValue(double value, Unit fromUnit)
        {
            AppendValue(Unit.FromUnitToThisUnit(value,fromUnit));
        }

        public void AddValue(Timespan timespan, double value, bool allowOverwrite)
        {
            throw new NotImplementedException();
        }

        public void AddValue(DateTime startTime, DateTime endTime, double value, bool allowOverwrite, bool fromSiUnit)
        {
            throw new NotImplementedException();
        }

        public void AddValue(DateTime startTime, DateTime endTime, double value, bool allowOverwrite, Unit fromUnit)
        {
            throw new NotImplementedException();
        }
        
       

        public override double GetValue(int index)
        {
            throw new NotImplementedException();
        }

        public override double GetValue(int index, bool toSIUnit)
        {
            throw new NotImplementedException();
        }

        public override double GetValue(int index, HydroNumerics.Core.Unit toUnit)
        {
            throw new NotImplementedException();
        }

        public override double ExtractValue(DateTime time)
        {
            throw new NotImplementedException();
        }

        public override double ExtractValue(DateTime time, bool toSIUnit)
        {
            throw new NotImplementedException();
        }

        public override double ExtractValue(DateTime time, HydroNumerics.Core.Unit toUnit)
        {
            throw new NotImplementedException();
        }

        public override double ExtractValue(DateTime fromTime, DateTime toTime)
        {
            throw new NotImplementedException();
        }

        public override double ExtractValue(DateTime fromTime, DateTime toTime, bool toSIUnit)
        {
            throw new NotImplementedException();
        }

        public override double ExtractValue(DateTime fromTime, DateTime toTime, HydroNumerics.Core.Unit toUnit)
        {
            throw new NotImplementedException();
        }
    }
}
