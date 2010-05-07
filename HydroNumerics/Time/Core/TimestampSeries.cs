#region Copyright
/*
* Copyright (c) 2010, Jan Gregersen (HydroInform) & Jacob Gudbjerg
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the names of Jan Gregersen (HydroInform) & Jacob Gudbjerg nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "Jan Gregersen (HydroInform) & Jacob Gudbjerg" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "Jan Gregersen (HydroInform) & Jacob Gudbjerg" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Text;
using HydroNumerics.Core;


namespace HydroNumerics.Time.Core
{
    //public enum TimeSeriesType : int
    //{
    //    TimeStampBased = 0,
    //    TimeSpanBased = 1
    //}

    [Serializable]
  [DataContract]
    public class TimestampSeries : BaseTimeSeries
    {
        /// <summary>
        /// Constructor. Assigning default values for the timeseries properties.
        /// </summary>
        public TimestampSeries()
        {
            this.Name = "no ID";
            //this.TimeSeriesType = TimeSeriesType.TimeStampBased;
            Items = new System.ComponentModel.BindingList<TimestampValue>();
            this.relaxationFactor = 0.0;
            this.unit = new Unit("Default Unit", 1.0, 0.0, "Default Unit",new Dimension(0,0,0,0,0,0,0,0));
            this.description = "no description";
            this.selectedRecord = 0;
            items.ListChanged += new System.ComponentModel.ListChangedEventHandler(timeValues_ListChanged);
        }

        public TimestampSeries(string name, DateTime startTime, int numberOfTimesteps, int timestepLength, TimestepUnit timestepLengthUnit, double defaultValue) : this()
        {
          
            items.Add(new TimestampValue(startTime, defaultValue));
                    
            this.name = name;

            for (int i = 0; i < numberOfTimesteps - 1; i++)
            {

                if (timestepLengthUnit == TimestepUnit.Years)
                {
                    items.Add(new TimestampValue(startTime.AddYears(timestepLength * i), defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Months)
                {
                    items.Add(new TimestampValue(startTime.AddMonths(timestepLength * i), defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Days)
                {
                    items.Add(new TimestampValue(startTime.AddDays(timestepLength *i), defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Hours)
                {
                    items.Add(new TimestampValue(startTime.AddHours(timestepLength * i), defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Minutes)
                {
                    items.Add(new TimestampValue(startTime.AddMinutes(timestepLength * i), defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Seconds)
                {
                    items.Add(new TimestampValue(startTime.AddSeconds(timestepLength * i), defaultValue));
                }
                else
                {
                    throw new Exception("Unexpected exception");
                }
            }
        }
    
        [DataMember]
        private System.ComponentModel.BindingList<TimestampValue> items;

        /// <summary>
        /// The list holding all the TimeValues objects
        /// </summary>
        public System.ComponentModel.BindingList<TimestampValue> Items
        {
            get { return items; }
            set 
            {
                items = value;
                NotifyPropertyChanged("TimeValuesList");
            }
        }
	
       

        void timeValues_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            NotifyPropertyChanged("TimeValues"); //notify also if values inside the list are changed
        }

        

        /// <summary>
        /// Add time and values to the end of the timeseries. The values are by default zero.
        /// Time is calculated based on the times for the last two existing records. If only
        /// one record exists in the time series, the time step length is set to one day. If 
        /// the timeseries is empty, the time for the added record will be January 1st 2020.
        /// </summary>
        public override void AppendValue(double value)
        {
            if (items.Count >= 2)
            {
              //JAG: Kunne det ikke laves med TimeSpan?
              //System.TimeSpan ts = TimeValuesList[TimeValuesList.Count - 1].Time - TimeValuesList[TimeValuesList.Count - 2].Time;

                int yearDiff = items[items.Count - 1].Time.Year - items[items.Count -2].Time.Year;
                int monthDiff = items[items.Count - 1].Time.Month - items[items.Count - 2].Time.Month;
                int dayDiff = items[items.Count - 1].Time.Day - items[items.Count - 2].Time.Day;
                int hourDiff = items[items.Count - 1].Time.Hour - items[items.Count - 2].Time.Hour;
                int minuteDiff = items[items.Count - 1].Time.Minute - items[items.Count - 2].Time.Minute;
                int secondDiff = items[items.Count - 1].Time.Second - items[items.Count - 2].Time.Second;

                DateTime newDateTime;

                if (yearDiff == 0 && dayDiff == 0 && hourDiff == 0 && minuteDiff == 0 && secondDiff == 0)
                {
                    newDateTime = items[items.Count - 1].Time.AddMonths(monthDiff);
                }
                else 
                {
                    newDateTime = items[items.Count - 1].Time.AddTicks(items[items.Count - 1].Time.Ticks - items[items.Count - 2].Time.Ticks);
                }
                items.Add(new TimestampValue(newDateTime, value));
            }
            else if(items.Count == 1)
            {
                DateTime newDateTime = new DateTime(items[0].Time.Ticks);
                newDateTime = newDateTime.AddDays(1);
                items.Add(new TimestampValue(newDateTime,value));
            }
            else if(items.Count == 0)
            {
                TimestampValue timeValue = new TimestampValue();
                timeValue.Value = value;
                items.Add(timeValue);
            }
            else
            {
                throw new Exception("unexpected exception when adding time series record");
            }
        }

        /// <summary>
        /// Add time and value to the timeseries. The added record will be placed according the the 
        /// specified time. If the time specified already exists the corresponding value will be overwritten
        /// with the new value
        /// </summary>
        /// <param name="timeValue">time and value to add</param>
        public void AddValue(DateTime time, double value)
        {
            TimestampValue timeValue = new TimestampValue(time, value);

            if (this.Items.Count == 0 || this.Items[Items.Count - 1].Time < timeValue.Time)
            {
                Items.Add(timeValue);
            }
            else if(this.items[0].Time > timeValue.Time)  //add the record to the beginning of the list
            {
                Items.Insert(0, timeValue);
            }
            else //overwrite values if time already exists in the list or insert according to the time.
            {
                foreach (TimestampValue tv in items) 
                {
                    if (tv.Time == timeValue.Time)
                    {
                        tv.Value = timeValue.Value;
                    }
                }
                int timeValuesListCount = items.Count;
                for (int i = 0; i < timeValuesListCount - 2; i++)
                {
                    if (items[i].Time < timeValue.Time && timeValue.Time < items[i + 1].Time)
                    {
                        Items.Insert(i+1, timeValue);
                    }
                }
            }
        }

        public void AddSiValue(DateTime time, double value)
        {
            AddValue(time, Unit.FromSiToThisUnit(value));
        }


        //[DataMember]
        //private TimeSeriesType timeSeriesType;
        //public TimeSeriesType TimeSeriesType
        //{
        //    get { return timeSeriesType; }
        //    set { timeSeriesType = value; }
        //}

        //public double GetValue(DateTime time)
        //{
        //    if (timeValues.Count == 0)
        //    {
        //        throw new Exception("TimeSeries.GetValues() method was invoked for time series with zero records");
        //    }

        //    if (timeValues.Count == 1)
        //    {
        //        return ToSI(timeValues[0].Value);
        //    }

        //    double tr = time.ToOADate();  // the requested time
        //    double xr = 0; // the value to return

        //    if (this.timeSeriesType == TimeSeriesType.TimeStampBased)
        //    {
               
        //        double ts0 = this.timeValues[0].Time.ToOADate();
        //        int count = this.timeValues.Count;
        //        double tsN2 = this.timeValues[count - 1].Time.ToOADate();
 
        //        if (count == 1)
        //        {
        //            return ToSI(this.timeValues[0].Value);
        //        }

        //        if (tr < ts0)
        //        {
        //            double ts1 = this.timeValues[1].Time.ToOADate();
        //            double xs0 = this.timeValues[0].Value;
        //            double xs1 = this.timeValues[1].Value;
        //            xr = ((xs0 - xs1) / (ts0 - ts1)) * (tr - ts0) * (1 - relaxationFactor) + xs0;
        //            return ToSI(xr);
        //        }

        //        else if (tr > tsN2)
        //        {
        //            double tsN1 = this.timeValues[count - 2].Time.ToOADate();
        //            double xsN1 = this.timeValues[count - 2].Value;
        //            double xsN2 = this.timeValues[count - 1].Value;
        //            xr = ((xsN1 - xsN2) / (tsN1 - tsN2)) * (tr - tsN2) * (1 - relaxationFactor) + xsN2;
        //            return ToSI(xr);
        //        }
        //        else
        //        {
        //            for (int i = 0; i < count-1; i++)
        //            {
        //                double ts1 = this.timeValues[i].Time.ToOADate();
        //                double ts2 = this.timeValues[i + 1].Time.ToOADate();
        //                if (ts1 <= tr && tr <= ts2)
        //                {
        //                    double xs1 = this.timeValues[i].Value;
        //                    double xs2 = this.timeValues[i + 1].Value;
        //                    xr = ((xs2 - xs1) / (ts2 - ts1)) * (tr - ts1) + xs1;
        //                    return ToSI(xr);
        //                }
        //            }
        //            throw new System.Exception("kurt");
        //        }

        //    }
        //    else   // if (this.TimeSeriesType == TimeSeriesType.TimeSpanBased)
        //    {
               
        //        //---------------------------------------------------------------------------
        //        //  Buffered TimesSpans:  |          >tbb0<  ..........  >tbbN<
        //        //  Requested TimeStamp:  |    >tr<
        //        //                         -----------------------------------------> t
        //        // --------------------------------------------------------------------------
        //        if (tr <= timeValues[0].Time.ToOADate())
        //        {
        //            double tbb0 = timeValues[0].Time.ToOADate();
        //            double tbb1 = timeValues[1].Time.ToOADate();
        //            double sbi0 = timeValues[0].Value;
        //            double sbi1 = timeValues[1].Value;
        //            xr = ((sbi0 - sbi1) / (tbb0 - tbb1)) * (tr - tbb0) * (1 - relaxationFactor) + sbi0;
        //        }

        //        //---------------------------------------------------------------------------
        //        //  Buffered TimesSpans:  |    >tbb0<   .................  >tbbN_1<
        //        //  Requested TimeStamp:  |                                             >tr<
        //        //                         ---------------------------------------------------> t
        //        // --------------------------------------------------------------------------
        //        else if (tr >= timeValues[timeValues.Count - 1].Time.ToOADate())//((ITimeSpan)_times[_times.Count - 1]).End.ModifiedJulianDay)
        //        {
        //            double tbeN_2 = timeValues[timeValues.Count - 2].Time.ToOADate(); //((ITimeSpan)_times[_times.Count - 2]).End.ModifiedJulianDay;
        //            double tbeN_1 = timeValues[timeValues.Count - 1].Time.ToOADate();//((ITimeSpan)_times[_times.Count - 1]).End.ModifiedJulianDay;

        //            if (timeValues.Count > 2)
        //            {
        //                double sbiN_2 = timeValues[timeValues.Count - 3].Value;//Support.GetVal((IValueSet)_values[_times.Count - 2], i, k);
        //                double sbiN_1 = timeValues[timeValues.Count - 2].Value;//Support.GetVal((IValueSet)_values[_times.Count - 1], i, k);

        //                xr = ((sbiN_1 - sbiN_2) / (tbeN_1 - tbeN_2)) * (tr - tbeN_1) * (1 - relaxationFactor) + sbiN_1;
        //            }
        //            else
        //            {
        //                xr = timeValues[0].Value;
        //            }
        //        }

        //        //---------------------------------------------------------------------------
        //        //  Availeble TimesSpans:  |    >tbb0<   ......................  >tbbN_1<
        //        //  Requested TimeStamp:   |                          >tr<
        //        //                         -------------------------------------------------> t
        //        // --------------------------------------------------------------------------
        //        else
        //        {
        //            for (int n = timeValues.Count - 1; n >= 0; n--) //for (int n = _times.Count - 1; n >= 0; n--)
        //            {
        //                double tbbn = timeValues[n - 1].Time.ToOADate();//((ITimeSpan)_times[n]).Start.ModifiedJulianDay;
        //                double tben = timeValues[n].Time.ToOADate();//((ITimeSpan)_times[n]).End.ModifiedJulianDay;

        //                if (tbbn <= tr && tr < tben)
        //                {
        //                    xr = timeValues[n - 1].Value;//xr[i][k - 1] = Support.GetVal((IValueSet)_values[n], i, k);
        //                    break;
        //                }
        //            }
        //        }
        //        return ToSI(xr);
        //    }
        //}

        //public double GetValue(DateTime fromTime, DateTime toTime)
        //{
        //    if (timeValues.Count == 0)
        //    {
        //        throw new Exception("TimeSeries.GetValues() method was invoked for time series with zero records");
        //    }

        //    if (timeValues.Count == 1) //if only one record in timeseries, always return that value
        //    {
        //        return ToSI(timeValues[0].Value);
        //    }

        //    double trFrom = fromTime.ToOADate();   // From time in requester time interval
        //    double trTo = toTime.ToOADate();     // To time in requester time interval

        //    if (trTo <= trFrom)
        //    {
        //        throw new Exception("Invalid arguments for GetValues method, toTime argument was smaller than or equal to fromTime argument");
        //    }

        //    double xr = 0; // return value;


        //    if (this.TimeSeriesType == TimeSeriesType.TimeSpanBased)
        //    {
        //        for (int n = 0; n < timeValues.Count - 1; n++)
        //        {
        //            double tsStepFrom = timeValues[n].Time.ToOADate(); //time series from time for n'th TimeValue record
        //            double tsStepTo = timeValues[n + 1].Time.ToOADate(); //time series to time for then'th TimeValue record
        //            double xTsStep = timeValues[n].Value; //time series value for the n'th timestep

        //            //---------------------------------------------------------------------------
        //            //    TS[n]:        <-------------------------->
        //            //    Requested: <------------------------------------->
        //            // --------------------------------------------------------------------------
        //            if (trFrom <= tsStepFrom && trTo >= tsStepTo)
        //            {
        //                xr += xTsStep * (tsStepTo - tsStepFrom) / (trTo - trFrom);
        //            }

        //              //---------------------------------------------------------------------------
        //            //           Times[n] Interval:        t1|-----------------------|t2
        //            //           Requested Interval:          rt1|--------------|rt2
        //            // --------------------------------------------------------------------------
        //            else if (tsStepFrom <= trFrom && trTo <= tsStepTo) //cover all
        //            {
        //                xr += timeValues[n].Value;
        //            }

        //              //---------------------------------------------------------------------------
        //            //           Times[n] Interval:       t1|-----------------|t2
        //            //           Requested Interval:                 rt1|--------------|rt2
        //            // --------------------------------------------------------------------------
        //            else if (tsStepFrom < trFrom && trFrom < tsStepTo && trTo > tsStepTo)
        //            {
        //                xr += xTsStep * (tsStepTo - trFrom) / (trTo - trFrom);
        //            }

        //              //---------------------------------------------------------------------------
        //            //           Times[i] Interval:             t1|-----------------|t2
        //            //           Requested Interval:      rt1|--------------|rt2
        //            // --------------------------------------------------------------------------
        //            else if (trFrom < tsStepFrom && trTo > tsStepFrom && trTo < tsStepTo)
        //            {
        //                xr += xTsStep * (trTo - tsStepFrom) / (trTo - trFrom);
        //            }
        //        }

        //        //--------------------------------------------------------------------------
        //        //              |--------|---------|--------| B
        //        //        |----------------|                  R
        //        //---------------------------------------------------------------------------
        //        double tsb0 = timeValues[0].Time.ToOADate(); //time series begine time for the first value
        //        double tse0 = timeValues[1].Time.ToOADate(); //time series end time for the first value

        //        if (trFrom < tsb0 && trTo > tsb0)
        //        {
        //            double xTs0 = timeValues[0].Value;
        //            double xTs1 = timeValues[1].Value;
        //            xr += ((tsb0 - trFrom) / (trTo - trFrom)) * (xTs0 - (1 - relaxationFactor) * ((tsb0 - trFrom) * (xTs1 - xTs0) / (tse0 - tsb0)));
        //        }

        //        //-------------------------------------------------------------------------------------
        //        //              |--------|---------|--------| B
        //        //                                    |----------------|                  R
        //        //-------------------------------------------------------------------------------------

        //        double tseN_1 = timeValues[timeValues.Count - 1].Time.ToOADate();

        //        if (trTo > tseN_1 && trFrom < tseN_1)
        //        {
        //            double tsbN_1 = timeValues[timeValues.Count - 2].Time.ToOADate();
        //            double xTSbN_1 = timeValues[timeValues.Count - 2].Value;
        //            double xTSbN_2 = timeValues[timeValues.Count - 3].Value;
        //            xr += ((trTo - tseN_1) / (trTo - trFrom)) * (xTSbN_1 + (1 - relaxationFactor) * ((trTo - tsbN_1) * (xTSbN_1 - xTSbN_2) / (tseN_1 - tsbN_1)));
        //        }
        //        //-------------------------------------------------------------------------------------
        //        //              |--------|---------|--------| B
        //        //                                              |----------------|   R
        //        //-------------------------------------------------------------------------------------
        //        if (trFrom >= tseN_1)
        //        {
        //            double tsbN_1 = timeValues[timeValues.Count - 2].Time.ToOADate();
        //            double xTSbN_1 = timeValues[timeValues.Count - 2].Value;
        //            double xTSbN_2 = timeValues[timeValues.Count - 3].Value;
        //            xr = xTSbN_1 + (1 - relaxationFactor) * ((xTSbN_1 - xTSbN_2) / (tseN_1 - tsbN_1)) * (trTo - tseN_1);
        //        }
        //        //-------------------------------------------------------------------------------------
        //        //                           |--------|---------|--------| B
        //        //        |----------------|   R
        //        //-------------------------------------------------------------------------------------
        //        if (trTo <= tsb0)
        //        {
        //            double xTs0 = timeValues[0].Value;
        //            double xTs1 = timeValues[1].Value;
        //            xr = xTs0 - (1 - relaxationFactor) * ((xTs1 - xTs0) / (tse0 - tsb0)) * (tsb0 - trFrom);
        //        }
        //    }
        //    else //this.timeSeriesType == TimeSeriesType.TimeStampBased
        //    {
        //        for (int n = 0; n < this.timeValues.Count - 1; n++)
        //        {
        //            double tbn = timeValues[n].Time.ToOADate();
        //            double tbnp1 = timeValues[n + 1].Time.ToOADate();
        //            double sbin = timeValues[n].Value;
        //            double sbinp1 = timeValues[n + 1].Value;

        //            //---------------------------------------------------------------------------
        //            //    B:           <-------------------------->
        //            //    R:        <------------------------------------->
        //            // --------------------------------------------------------------------------
        //            if (trFrom <= tbn && trTo >= tbnp1)
        //            {
        //                xr += 0.5 * (sbin + sbinp1) * (tbnp1 - tbn) / (trTo - trFrom);
        //            }

        //            //---------------------------------------------------------------------------
        //            //           Times[i] Interval:        t1|-----------------------|t2
        //            //           Requested Interval:          rt1|--------------|rt2
        //            // --------------------------------------------------------------------------
        //            else if (tbn <= trFrom && trTo <= tbnp1) //cover all
        //            {
        //                xr += sbin + ((sbinp1 - sbin) / (tbnp1 - tbn)) * ((trTo + trFrom) / 2 - tbn);
        //            }

        //            //---------------------------------------------------------------------------
        //            //           Times[i] Interval:       t1|-----------------|t2
        //            //           Requested Interval:                 rt1|--------------|rt2
        //            // --------------------------------------------------------------------------
        //            else if (tbn < trFrom && trFrom < tbnp1 && trTo > tbnp1)
        //            {
        //                xr += (sbinp1 - (sbinp1 - sbin) / (tbnp1 - tbn) * ((tbnp1 - trFrom) / 2)) * (tbnp1 - trFrom) / (trTo - trFrom);
        //            }

        //            //---------------------------------------------------------------------------
        //            //           Times[i] Interval:             t1|-----------------|t2
        //            //           Requested Interval:      rt1|--------------|rt2
        //            // --------------------------------------------------------------------------
        //            else if (trFrom < tbn && trTo > tbn && trTo < tbnp1)
        //            {
        //                xr += (sbin + (sbinp1 - sbin) / (tbnp1 - tbn) * ((trTo - tbn) / 2)) * (trTo - tbn) / (trTo - trFrom);
        //            }
        //        }
        //        //--------------------------------------------------------------------------
        //        //              |--------|---------|--------| B
        //        //        |----------------|                  R
        //        //---------------------------------------------------------------------------
        //        double tb0 = timeValues[0].Time.ToOADate();
        //        double tb1 = timeValues[1].Time.ToOADate();
        //        double tbN_1 = timeValues[timeValues.Count - 1].Time.ToOADate();
        //        double tbN_2 = timeValues[timeValues.Count - 2].Time.ToOADate();

        //        if (trFrom < tb0 && trTo > tb0)
        //        {
        //            double sbi0 = timeValues[0].Value;
        //            double sbi1 = timeValues[1].Value;
        //            xr += ((tb0 - trFrom) / (trTo - trFrom)) * (sbi0 - (1 - relaxationFactor) * 0.5 * ((tb0 - trFrom) * (sbi1 - sbi0) / (tb1 - tb0)));
        //        }
        //        //-------------------------------------------------------------------------------------
        //        //              |--------|---------|--------| B
        //        //                                    |----------------|                  R
        //        //-------------------------------------------------------------------------------------
        //        if (trTo > tbN_1 && trFrom < tbN_1)
        //        {
        //            double sbiN_1 = timeValues[timeValues.Count - 1].Value;
        //            double sbiN_2 = timeValues[timeValues.Count - 2].Value;
        //            xr += ((trTo - tbN_1) / (trTo - trFrom)) * (sbiN_1 + (1 - relaxationFactor) * 0.5 * ((trTo - tbN_1) * (sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)));
        //        }
        //        //-------------------------------------------------------------------------------------
        //        //              |--------|---------|--------| B
        //        //                                              |----------------|   R
        //        //-------------------------------------------------------------------------------------
        //        if (trFrom >= tbN_1)
        //        {

        //            double sbiN_1 = timeValues[timeValues.Count - 1].Value;
        //            double sbiN_2 = timeValues[timeValues.Count - 2].Value;
        //            xr = sbiN_1 + (1 - relaxationFactor) * ((sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)) * (0.5 * (trFrom + trTo) - tbN_1);
        //        }
        //        //-------------------------------------------------------------------------------------
        //        //                           |--------|---------|--------| B
        //        //        |----------------|   R
        //        //-------------------------------------------------------------------------------------
        //        if (trTo <= tb0)
        //        {
        //            double sbi0 = timeValues[0].Value;
        //            double sbi1 = timeValues[1].Value;
        //            xr = sbi0 - (1 - relaxationFactor) * ((sbi1 - sbi0) / (tb1 - tb0)) * (tb0 - 0.5 * (trFrom + trTo));
        //        }
        //    }

        //    return ToSI(xr);
        //}

        //public double GetValue(DateTime time, Unit unit)
        //{
        //    double x = GetValue(time);
        //    return (x - unit.OffSetToSI)/unit.ConversionFactorToSI;
        //}

       
        /// <summary>
        /// Returns the value corresponding to the timeperiod from fromTime to toTime. The value is 
        /// converted to the unit provided in the method arguments. If the period for which the value
        /// is requested is outside the period for the time series, the value is calculated by use of 
        /// linear extrapolation. Extrapolation depends on the relaxation factor (see definition of the relaxation factor).
        /// </summary>
        /// <param name="fromTime">start time for the period for which the value is requested</param>
        /// <param name="toTime">end time for the period for which the value is requested</param>
        /// <param name="unit">unit to which the returned value is converted</param>
        /// <returns>Returns the value corresponding to the timeperiod from fromTime to toTime. The value is converted to the unit provided in the method arguments </returns>
        //public double GetValue(DateTime fromTime, DateTime toTime, Unit unit)
        //{
        //    double x = GetValue(fromTime, toTime);
        //    return (x - unit.OffSetToSI) / unit.ConversionFactorToSI;
        //}

        //private double ToSI(double value)
        //{
        //    if (unit.ConversionFactorToSI == 1.0 && unit.OffSetToSI == 0.0)
        //    {
        //        return value;
        //    }
        //    else
        //    {
        //        return unit.ConversionFactorToSI * value + unit.OffSetToSI;
        //    }
        //}

       

      
        //#region INotifyPropertyChanged Members

        //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        //private void NotifyPropertyChanged(String propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        //    }
        //}

        //#endregion

        public override void ConvertUnit(Unit newUnit)
        {
            foreach (TimestampValue timeValue in items)
            {
                timeValue.Value = this.unit.FromThisUnitToUnit(timeValue.Value, newUnit);
            }
            this.unit = new HydroNumerics.Core.Unit(newUnit);
        }

        //public override int Count
        //{
        //    get
        //    {
        //        return timeValues.Count;
        //    }
        //}

        //public override double GetValue(int index)
        //{
        //    throw new NotImplementedException();
        //}

        //public override double GetValue(int index, bool toSIUnit)
        //{
        //    throw new NotImplementedException();
        //}

        //public override double GetValue(int index, Unit toUnit)
        //{
        //    throw new NotImplementedException();
        //}

        public override double GetValue(DateTime time)
        {
            if (items.Count == 0)
            {
                throw new Exception("TimeSeries.ExtractValue(DataTime time) method was invoked for time series with zero records");
            }

            if (items.Count == 1)
            {
                return items[0].Value;
            }

            double tr = time.ToOADate();  // the requested time
            double xr = 0; // the value to return

            double ts0 = this.items[0].Time.ToOADate();
            int count = this.items.Count;
            double tsN2 = this.items[count - 1].Time.ToOADate();

            if (count == 1)
            {
                return this.items[0].Value;
            }

            if (tr < ts0)
            {
                double ts1 = this.items[1].Time.ToOADate();
                double xs0 = this.items[0].Value;
                double xs1 = this.items[1].Value;
                xr = ((xs0 - xs1) / (ts0 - ts1)) * (tr - ts0) * (1 - relaxationFactor) + xs0;
                //return ToSI(xr);
                return xr;
            }

            else if (tr > tsN2)
            {
                double tsN1 = this.items[count - 2].Time.ToOADate();
                double xsN1 = this.items[count - 2].Value;
                double xsN2 = this.items[count - 1].Value;
                xr = ((xsN1 - xsN2) / (tsN1 - tsN2)) * (tr - tsN2) * (1 - relaxationFactor) + xsN2;
                //return ToSI(xr);
                return xr;
            }
            else
            {
                for (int i = 0; i < count - 1; i++)
                {
                    double ts1 = this.items[i].Time.ToOADate();
                    double ts2 = this.items[i + 1].Time.ToOADate();
                    if (ts1 <= tr && tr <= ts2)
                    {
                        double xs1 = this.items[i].Value;
                        double xs2 = this.items[i + 1].Value;
                        xr = ((xs2 - xs1) / (ts2 - ts1)) * (tr - ts1) + xs1;
                        //return ToSI(xr);
                        return xr;
                    }
                }
                throw new System.Exception("kurt");
            }
        }

        //public override double GetSiValue(DateTime time)
        //{
        //    return Unit.ToSiUnit(GetValue(time));
        //}




        public override double GetValue(DateTime fromTime, DateTime toTime)
        {
            if (items.Count == 0)
            {
                throw new Exception("TimeSeries.GetValues() method was invoked for time series with zero records");
            }

            if (items.Count == 1) //if only one record in timeseries, always return that value
            {
                return items[0].Value;
            }

            double trFrom = fromTime.ToOADate();   // From time in requester time interval
            double trTo = toTime.ToOADate();     // To time in requester time interval

            if (trTo <= trFrom)
            {
                throw new Exception("Invalid arguments for GetValues method, toTime argument was smaller than or equal to fromTime argument");
            }

            double xr = 0; // return value;
     
           
            for (int n = 0; n < this.items.Count - 1; n++)
            {
                double tbn = items[n].Time.ToOADate();
                double tbnp1 = items[n + 1].Time.ToOADate();
                double sbin = items[n].Value;
                double sbinp1 = items[n + 1].Value;

                //---------------------------------------------------------------------------
                //    B:           <-------------------------->
                //    R:        <------------------------------------->
                // --------------------------------------------------------------------------
                if (trFrom <= tbn && trTo >= tbnp1)
                {
                    xr += 0.5 * (sbin + sbinp1) * (tbnp1 - tbn) / (trTo - trFrom);
                }

                //---------------------------------------------------------------------------
                //           Times[i] Interval:        t1|-----------------------|t2
                //           Requested Interval:          rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (tbn <= trFrom && trTo <= tbnp1) //cover all
                {
                    xr += sbin + ((sbinp1 - sbin) / (tbnp1 - tbn)) * ((trTo + trFrom) / 2 - tbn);
                }

                //---------------------------------------------------------------------------
                //           Times[i] Interval:       t1|-----------------|t2
                //           Requested Interval:                 rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (tbn < trFrom && trFrom < tbnp1 && trTo > tbnp1)
                {
                    xr += (sbinp1 - (sbinp1 - sbin) / (tbnp1 - tbn) * ((tbnp1 - trFrom) / 2)) * (tbnp1 - trFrom) / (trTo - trFrom);
                }

                //---------------------------------------------------------------------------
                //           Times[i] Interval:             t1|-----------------|t2
                //           Requested Interval:      rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (trFrom < tbn && trTo > tbn && trTo < tbnp1)
                {
                    xr += (sbin + (sbinp1 - sbin) / (tbnp1 - tbn) * ((trTo - tbn) / 2)) * (trTo - tbn) / (trTo - trFrom);
                }
            }
            //--------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //        |----------------|                  R
            //---------------------------------------------------------------------------
            double tb0 = items[0].Time.ToOADate();
            double tb1 = items[1].Time.ToOADate();
            double tbN_1 = items[items.Count - 1].Time.ToOADate();
            double tbN_2 = items[items.Count - 2].Time.ToOADate();

            if (trFrom < tb0 && trTo > tb0)
            {
                double sbi0 = items[0].Value;
                double sbi1 = items[1].Value;
                xr += ((tb0 - trFrom) / (trTo - trFrom)) * (sbi0 - (1 - relaxationFactor) * 0.5 * ((tb0 - trFrom) * (sbi1 - sbi0) / (tb1 - tb0)));
            }
            //-------------------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //                                    |----------------|                  R
            //-------------------------------------------------------------------------------------
            if (trTo > tbN_1 && trFrom < tbN_1)
            {
                double sbiN_1 = items[items.Count - 1].Value;
                double sbiN_2 = items[items.Count - 2].Value;
                xr += ((trTo - tbN_1) / (trTo - trFrom)) * (sbiN_1 + (1 - relaxationFactor) * 0.5 * ((trTo - tbN_1) * (sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)));
            }
            //-------------------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //                                              |----------------|   R
            //-------------------------------------------------------------------------------------
            if (trFrom >= tbN_1)
            {

                double sbiN_1 = items[items.Count - 1].Value;
                double sbiN_2 = items[items.Count - 2].Value;
                xr = sbiN_1 + (1 - relaxationFactor) * ((sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)) * (0.5 * (trFrom + trTo) - tbN_1);
            }
            //-------------------------------------------------------------------------------------
            //                           |--------|---------|--------| B
            //        |----------------|   R
            //-------------------------------------------------------------------------------------
            if (trTo <= tb0)
            {
                double sbi0 = items[0].Value;
                double sbi1 = items[1].Value;
                xr = sbi0 - (1 - relaxationFactor) * ((sbi1 - sbi0) / (tb1 - tb0)) * (tb0 - 0.5 * (trFrom + trTo));
            }
            

            return xr;
        }

        //public override double GetSiValue(DateTime fromTime, DateTime toTime)
        //{
        //    return Unit.ToSiUnit(GetValue(fromTime, toTime));
        //}

        public override void RemoveAfter(DateTime time)
        {
            foreach (TimestampValue timestampValue in items)
            {
                if (timestampValue.Time > time)
                {
                    items.Remove(timestampValue);
                }
            }
        }
    }
}
