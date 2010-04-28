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
            timespanValues = new System.ComponentModel.BindingList<TimespanValue>();
            timespanValues.ListChanged += new System.ComponentModel.ListChangedEventHandler(timespanValues_ListChanged);
        }

        public TimespanSeries(string name, DateTime startTime, int numberOfTimesteps, int timestepLength, TimestepUnit timestepLengthUnit, double defaultValue) : this()
        {
            this.name = name;

            for (int i = 0; i < numberOfTimesteps; i++)
            {

                if (timestepLengthUnit == TimestepUnit.Years)
                {
                    timespanValues.Add(new TimespanValue(startTime.AddYears(i*timestepLength),startTime.AddYears((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Months)
                {
                    timespanValues.Add(new TimespanValue(startTime.AddMonths(i*timestepLength),startTime.AddMonths((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Days)
                {
                    timespanValues.Add(new TimespanValue(startTime.AddDays(i*timestepLength),startTime.AddDays((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Hours)
                {
                    timespanValues.Add(new TimespanValue(startTime.AddHours(i*timestepLength),startTime.AddHours((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Minutes)
                {
                    timespanValues.Add(new TimespanValue(startTime.AddMinutes(i*timestepLength),startTime.AddMinutes((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Seconds)
                {
                    timespanValues.Add(new TimespanValue(startTime.AddSeconds(i*timestepLength),startTime.AddSeconds((i+1)*timestepLength),defaultValue));
                }
                else
                {
                    throw new Exception("Unexpected exception");
                }
            }
        }

        void timespanValues_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            NotifyPropertyChanged("TimespanValues");
        }
        private System.ComponentModel.BindingList<TimespanValue> timespanValues;

        public System.ComponentModel.BindingList<TimespanValue> TimespanValues
        {
            get { return timespanValues; }
            set { timespanValues = value; }
        }
        
        //public override int Count
        //{
        //    get { return timespanValues.Count; }
        //}

        public override void ConvertUnit(HydroNumerics.Core.Unit newUnit)
        {
            throw new NotImplementedException();
        }
        
        //public void SetValue(int index, double value)
        //{
        //    if (index < 0 || index >= Count)
        //    {
        //        throw new Exception("Index was out of range");
        //    }
        //    timespanValues[index].Value = value;
        //}

        //public void SetValue(int index, double value, bool fromSiUnit)
        //{
        //    if (index < 0 || index >= Count)
        //    {
        //        throw new Exception("Index was out of range");
        //    }

        //    timespanValues[index].Value = this.Unit.FromSiToThisUnit(value);
            
        //}

        //public void SetValue(int index, double value, Unit fromUnit)
        //{
        //    if (index < 0 || index >= Count)
        //    {
        //        throw new Exception("Index was out of range");
        //    }

        //    timespanValues[index].Value = this.Unit.FromUnitToThisUnit(value, fromUnit);
        //}
        
        public void AppendValue(double value)
        {
            int count = timespanValues.Count;
            if (count >= 1)
            {
                int yearDiff = timespanValues[count - 1].EndTime.Year - timespanValues[count - 1].TimeSpan.Start.Year;
                int monthDiff = timespanValues[count - 1].EndTime.Month - timespanValues[count - 1].TimeSpan.Start.Month;
                int dayDiff = timespanValues[count - 1].EndTime.Day - timespanValues[count - 1].TimeSpan.Start.Day;
                int hourDiff = timespanValues[count - 1].EndTime.Hour - timespanValues[count - 1].TimeSpan.Start.Hour;
                int minuteDiff = timespanValues[count - 1].EndTime.Minute - timespanValues[count - 1].TimeSpan.Start.Minute;
                int secondDiff = timespanValues[count - 1].EndTime.Second - timespanValues[count - 1].TimeSpan.Start.Second;

                DateTime start = DateTime.FromOADate(timespanValues[count - 1].TimeSpan.Start.ToOADate());
                DateTime end;
                if (yearDiff == 0 && dayDiff == 0 && hourDiff == 0 && minuteDiff == 0 && secondDiff == 0)
                {
                    end = start.AddMonths(monthDiff);
                }
                else
                {
                    end = timespanValues[count - 1].TimeSpan.End.AddTicks(timespanValues[count - 1].TimeSpan.End.Ticks - timespanValues[count - 1].TimeSpan.Start.Ticks);
                }
                timespanValues.Add(new TimespanValue(timespanValues[count - 1].EndTime, end, value));
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
        
        public override double ExtractValue(DateTime time)
        {
            if (timespanValues.Count == 0)
            {
                throw new Exception("ExtractValues() method was invoked for time series with zero records");
            }

            if (timespanValues.Count == 1)
            {
                return timespanValues[0].Value;
            }

            double tr = time.ToOADate();  // the requested time
            double xr = 0; // the value to return

            

            //---------------------------------------------------------------------------
            //  Buffered TimesSpans:  |          >tbb0<  ..........  >tbbN<
            //  Requested TimeStamp:  |    >tr<
            //                         -----------------------------------------> t
            // --------------------------------------------------------------------------
            if (tr <= timespanValues[0].StartTime.ToOADate())
            {
                double tbb0 = timespanValues[0].StartTime.ToOADate();
                double tbb1 = timespanValues[1].StartTime.ToOADate();
                double sbi0 = timespanValues[0].Value;
                double sbi1 = timespanValues[1].Value;
                xr = ((sbi0 - sbi1) / (tbb0 - tbb1)) * (tr - tbb0) * (1 - relaxationFactor) + sbi0;
            }

            //---------------------------------------------------------------------------
            //  Buffered TimesSpans:  |    >tbb0<   .................  >tbbN_1<
            //  Requested TimeStamp:  |                                             >tr<
            //                         ---------------------------------------------------> t
            // --------------------------------------------------------------------------
            else if (tr >= timespanValues[timespanValues.Count - 1].EndTime.ToOADate())//((ITimeSpan)_times[_times.Count - 1]).End.ModifiedJulianDay)
            {
                double tbeN_2 = timespanValues[timespanValues.Count - 2].EndTime.ToOADate(); //((ITimeSpan)_times[_times.Count - 2]).End.ModifiedJulianDay;
                double tbeN_1 = timespanValues[timespanValues.Count - 1].EndTime.ToOADate();//((ITimeSpan)_times[_times.Count - 1]).End.ModifiedJulianDay;

                if (timespanValues.Count > 2)
                {
                    double sbiN_2 = timespanValues[timespanValues.Count - 2].Value;//Support.GetVal((IValueSet)_values[_times.Count - 2], i, k);
                    double sbiN_1 = timespanValues[timespanValues.Count - 1].Value;//Support.GetVal((IValueSet)_values[_times.Count - 1], i, k);

                    xr = ((sbiN_1 - sbiN_2) / (tbeN_1 - tbeN_2)) * (tr - tbeN_1) * (1 - relaxationFactor) + sbiN_1;
                }
                else
                {
                    xr = timespanValues[0].Value;
                }
            }

            //---------------------------------------------------------------------------
            //  Availeble TimesSpans:  |    >tbb0<   ......................  >tbbN_1<
            //  Requested TimeStamp:   |                          >tr<
            //                         -------------------------------------------------> t
            // --------------------------------------------------------------------------
            else
            {
                for (int n = timespanValues.Count - 1; n >= 0; n--) //for (int n = _times.Count - 1; n >= 0; n--)
                {
                    double tbbn = timespanValues[n].StartTime.ToOADate();//((ITimeSpan)_times[n]).Start.ModifiedJulianDay;
                    double tben = timespanValues[n].EndTime.ToOADate();//((ITimeSpan)_times[n]).End.ModifiedJulianDay;

                    if (tbbn <= tr && tr < tben)
                    {
                        xr = timespanValues[n].Value;//xr[i][k - 1] = Support.GetVal((IValueSet)_values[n], i, k);
                        break;
                    }
                }
            }
            return xr;
         
        }

        public override double ExtractValue(DateTime fromTime, DateTime toTime)
        {
            if (timespanValues.Count == 0)
            {
                throw new Exception("ExtractValue method was invoked for time series with zero records");
            }

            if (timespanValues.Count == 1) //if only one record in timeseries, always return that value
            {
                return timespanValues[0].Value;
            }

            double trFrom = fromTime.ToOADate();   // From time in requester time interval
            double trTo = toTime.ToOADate();     // To time in requester time interval

            if (trTo <= trFrom)
            {
                throw new Exception("Invalid arguments for ExtractValue method, toTime argument was smaller than or equal to fromTime argument");
            }

            double xr = 0; // return value;



            for (int n = 0; n < timespanValues.Count - 1; n++)
            {
                double tsStepFrom = timespanValues[n].StartTime.ToOADate(); //time series from time for n'th TimeValue record
                double tsStepTo = timespanValues[n].EndTime.ToOADate(); //time series to time for then'th TimeValue record
                double xTsStep = timespanValues[n].Value; //time series value for the n'th timestep

                //---------------------------------------------------------------------------
                //    TS[n]:        <-------------------------->
                //    Requested: <------------------------------------->
                // --------------------------------------------------------------------------
                if (trFrom <= tsStepFrom && trTo >= tsStepTo)
                {
                    xr += xTsStep * (tsStepTo - tsStepFrom) / (trTo - trFrom);
                }

                  //---------------------------------------------------------------------------
                //           Times[n] Interval:        t1|-----------------------|t2
                //           Requested Interval:          rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (tsStepFrom <= trFrom && trTo <= tsStepTo) //cover all
                {
                    xr += xTsStep; //timeValues[n].Value;
                }

                  //---------------------------------------------------------------------------
                //           Times[n] Interval:       t1|-----------------|t2
                //           Requested Interval:                 rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (tsStepFrom < trFrom && trFrom < tsStepTo && trTo > tsStepTo)
                {
                    xr += xTsStep * (tsStepTo - trFrom) / (trTo - trFrom);
                }

                  //---------------------------------------------------------------------------
                //           Times[i] Interval:             t1|-----------------|t2
                //           Requested Interval:      rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (trFrom < tsStepFrom && trTo > tsStepFrom && trTo < tsStepTo)
                {
                    xr += xTsStep * (trTo - tsStepFrom) / (trTo - trFrom);
                }
            }

            //--------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //        |----------------|                  R
            //---------------------------------------------------------------------------
            double tsb0 = timespanValues[0].StartTime.ToOADate(); //time series begine time for the first value
            double tse0 = timespanValues[0].EndTime.ToOADate(); //time series end time for the first value

            if (trFrom < tsb0 && trTo > tsb0)
            {
                double xTs0 = timespanValues[0].Value;
                double xTs1 = timespanValues[1].Value;
                xr += ((tsb0 - trFrom) / (trTo - trFrom)) * (xTs0 - (1 - relaxationFactor) * ((tsb0 - trFrom) * (xTs1 - xTs0) / (tse0 - tsb0)));
            }

            //-------------------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //                                    |----------------|                  R
            //-------------------------------------------------------------------------------------

            double tseN_1 = timespanValues[timespanValues.Count - 1].EndTime.ToOADate();

            if (trTo > tseN_1 && trFrom < tseN_1)
            {
                double tsbN_1 = timespanValues[timespanValues.Count - 2].StartTime.ToOADate();
                double xTSbN_1 = timespanValues[timespanValues.Count - 1].Value;
                double xTSbN_2 = timespanValues[timespanValues.Count - 2].Value;
                xr += ((trTo - tseN_1) / (trTo - trFrom)) * (xTSbN_1 + (1 - relaxationFactor) * ((trTo - tsbN_1) * (xTSbN_1 - xTSbN_2) / (tseN_1 - tsbN_1)));
            }
            //-------------------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //                                              |----------------|   R
            //-------------------------------------------------------------------------------------
            if (trFrom >= tseN_1)
            {
                double tsbN_1 = timespanValues[timespanValues.Count - 1].StartTime.ToOADate();
                double xTSbN_1 = timespanValues[timespanValues.Count - 1].Value;
                double xTSbN_2 = timespanValues[timespanValues.Count - 2].Value;
                xr = xTSbN_1 + (1 - relaxationFactor) * ((xTSbN_1 - xTSbN_2) / (tseN_1 - tsbN_1)) * (trTo - tseN_1);
            }
            //-------------------------------------------------------------------------------------
            //                           |--------|---------|--------| B
            //        |----------------|   R
            //-------------------------------------------------------------------------------------
            if (trTo <= tsb0)
            {
                double xTs0 = timespanValues[0].Value;
                double xTs1 = timespanValues[1].Value;
                xr = xTs0 - (1 - relaxationFactor) * ((xTs1 - xTs0) / (tse0 - tsb0)) * (tsb0 - trFrom);
            }

            return xr;
        }
        
    }
}
