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
using System.Text;
using OpenMI.Standard;
using HydroNumerics.OpenMI.Sdk.Backbone;

namespace HydroNumerics.Time.Core
{
    public enum TimeSeriesType : int
    {
        TimeStampBased = 0,
        TimeSpanBased = 1
    }

    [Serializable]
    public class TimeSeries : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// The data DataChanged event will be send whenever values of the timeseries are changed.
        /// The dataChanged event is not sent when timeseries properties are changed (such as TimeSeries.Name).
        /// However, the DataChanged event is sent, when the property: SelectedRecord is changed.
        /// </summary>
        public delegate void DataChanged();
        DataChanged dataChanged;

        private string name;

        [XmlAttribute]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int id;

        [XmlAttribute]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private System.ComponentModel.BindingList<TimeValue> timeValues;

        /// <summary>
        /// The list holding all the TimeValues objects
        /// </summary>
        public System.ComponentModel.BindingList<TimeValue> TimeValues
        {
            get { return timeValues; }
            set 
            {
                timeValues = value;
                NotifyPropertyChanged("TimeValuesList");
            }
        }

        private Object tag;
        /// <summary>
        /// An object tag, that may be used for anything. Is used e.g. by the timeserieseditor to
        /// attach graphics specific objects to the individual time series. The tag object is not
        /// stored with the time series (not part of the xml seriallisation).
        /// </summary>
        [XmlIgnore]
        public Object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }
	

        private string description;

        /// <summary>
        /// Description for the time series
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private Unit unit;

        /// <summary>
        /// Unit for all values in the time series
        /// </summary>
        public Unit Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        private Dimension dimension;

        /// <summary>
        /// Dimension of all values in the timeseries.
        /// </summary>
        public Dimension Dimension
        {
            get { return dimension; }
            set { dimension = value; }
        }



        private int selectedRecord;

        /// <summary>
        /// Index of the user selected record. Used by the time series editor. Changing the selected record
        /// will trigger the DataChanged event to be sent. 
        /// </summary>
        [XmlIgnore]
        public int SelectedRecord
        {   
            get { return selectedRecord; }
            set 
            { 
                selectedRecord = value;
                dataChanged();
                NotifyPropertyChanged("SelectedRecord");
            }
        }
	
        /// <summary>
        /// Constructor. Assigning default values for the timeseries properties.
        /// </summary>
        public TimeSeries()
        {
            this.Name = "no ID";
            this.TimeSeriesType = TimeSeriesType.TimeStampBased;
            TimeValues = new System.ComponentModel.BindingList<TimeValue>();
            dataChanged = new DataChanged(DataChangedEventhandler);
            this.relaxationFactor = 0.0;
            unit = new Unit("m", 1.0, 0.0, "meters");
            dimension = new Dimension();
            dimension.SetPower(DimensionBase.Length, 1);
            this.description = "no description";
            this.selectedRecord = 0;
            
         }
               
        private double relaxationFactor;

        /// <summary>
        /// The relaxationfactor is used when the GetValues method is invoked and extrapolation
        /// is required. If the relaxationfactor is zero full linear extrapolation based on the
        /// nearest two point is performed. If the relaxationfactor is one, the value for the
        /// nearest record is used unchanged. For relaxationfactor values between zero and one
        /// the linear extrapolation is dampen. The relaxationfactor is typically used in order
        /// to avoid numerical instabilities for numerical models using the time series as input.
        /// The relaxationfactor must always recide in the interval [0,1].
        /// </summary>
        public double RelaxationFactor
        {
            get { return relaxationFactor; }
            set 
            {
                if (value < 0 || value > 1.0)
                {
                    throw new Exception("Attempt to assign the relaxationfactor to a value outside the interval [0,1]");
                }
                relaxationFactor = value; 
            }
        }

        /// <summary>
        /// Add time and values to the end of the timeseries. The values are by default zero.
        /// Time is calculated based on the times for the last two existing records. If only
        /// one record exists in the time series, the time step length is set to one day. If 
        /// the timeseries is empty, the time for the added record will be January 1st 2020.
        /// </summary>
        public void AppendValue(double value)
        {
            if (timeValues.Count >= 2)
            {
              //JAG: Kunne det ikke laves med TimeSpan?
              //System.TimeSpan ts = TimeValuesList[TimeValuesList.Count - 1].Time - TimeValuesList[TimeValuesList.Count - 2].Time;

                int yearDiff = timeValues[timeValues.Count - 1].Time.Year - timeValues[timeValues.Count -2].Time.Year;
                int monthDiff = timeValues[timeValues.Count - 1].Time.Month - timeValues[timeValues.Count - 2].Time.Month;
                int dayDiff = timeValues[timeValues.Count - 1].Time.Day - timeValues[timeValues.Count - 2].Time.Day;
                int hourDiff = timeValues[timeValues.Count - 1].Time.Hour - timeValues[timeValues.Count - 2].Time.Hour;
                int minuteDiff = timeValues[timeValues.Count - 1].Time.Minute - timeValues[timeValues.Count - 2].Time.Minute;
                int secondDiff = timeValues[timeValues.Count - 1].Time.Second - timeValues[timeValues.Count - 2].Time.Second;

                DateTime newDateTime;

                if (yearDiff == 0 && dayDiff == 0 && hourDiff == 0 && minuteDiff == 0 && secondDiff == 0)
                {
                    newDateTime = timeValues[timeValues.Count - 1].Time.AddMonths(monthDiff);
                }
                else 
                {
                    newDateTime = timeValues[timeValues.Count - 1].Time.AddTicks(timeValues[timeValues.Count - 1].Time.Ticks - timeValues[timeValues.Count - 2].Time.Ticks);
                }
                timeValues.Add(new TimeValue(newDateTime, value));
            }
            else if(timeValues.Count == 1)
            {
                DateTime newDateTime = new DateTime(timeValues[0].Time.Ticks);
                newDateTime = newDateTime.AddDays(1);
                timeValues.Add(new TimeValue(newDateTime,0));
            }
            else if(timeValues.Count == 0)
            {
                TimeValue timeValue = new TimeValue();
                timeValue.Value = value;
                timeValues.Add(timeValue);
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
        public void AddTimeValueRecord(TimeValue timeValue)
        {
            if (this.TimeValues.Count == 0 || this.TimeValues[TimeValues.Count - 1].Time < timeValue.Time)
            {
                TimeValues.Add(timeValue);
            }
            else if(this.timeValues[0].Time > timeValue.Time)  //add the record to the beginning of the list
            {
                TimeValues.Insert(0, timeValue);
            }
            else //overwrite values if time already exists in the list or insert according to the time.
            {
                foreach (TimeValue tv in timeValues) 
                {
                    if (tv.Time == timeValue.Time)
                    {
                        tv.Value = timeValue.Value;
                    }
                }
                int timeValuesListCount = timeValues.Count;
                for (int i = 0; i < timeValuesListCount - 2; i++)
                {
                    if (timeValues[i].Time < timeValue.Time && timeValue.Time < timeValues[i + 1].Time)
                    {
                        TimeValues.Insert(i+1, timeValue);
                    }

                }
            }
        }


        private TimeSeriesType timeSeriesType;
        public TimeSeriesType TimeSeriesType
        {
            get { return timeSeriesType; }
            set { timeSeriesType = value; }
        }
	
	
        public static void DataChangedEventhandler()
        {
            // do nothihg
        }


        [XmlIgnore]
        public DataChanged DataChangedEvent
        {
            get { return this.dataChanged; }
            set { this.dataChanged = value; }

        }
	
        public void NotifyDataMayHaveChanged()
        {
            dataChanged();
        }

        //public double GetValue(int year, int month, int day, int hour, int minute, int second)
        //{
        //    return GetValue(new DateTime(year, month, day, hour, minute, second));
        //}

        //public double GetValue(int fromYear, int fromMonth, int fromDay, int fromHour, int fromMinute, int fromSecond, int toYear, int toMonth, int toDay, int toHour, int toMinute, int toSecond)
        //{
        //    return GetValue(new DateTime(fromYear, fromMonth, fromDay, fromHour, fromMinute, fromSecond), new DateTime(toYear, toMonth, toDay, toHour, toMinute, toSecond));
        //}

        public double GetValue(DateTime time)
        {
            if (timeValues.Count == 0)
            {
                throw new Exception("TimeSeries.GetValues() method was invoked for time series with zero records");
            }

            double tr = time.ToOADate();  // the requested time
            double xr = 0; // the value to return

            if (this.timeSeriesType == TimeSeriesType.TimeStampBased)
            {
               
                double ts0 = this.timeValues[0].Time.ToOADate();
                int count = this.timeValues.Count;
                double tsN2 = this.timeValues[count - 1].Time.ToOADate();
 
                if (count == 1)
                {
                    return ToSI(this.timeValues[0].Value);
                }

                if (tr < ts0)
                {
                    double ts1 = this.timeValues[1].Time.ToOADate();
                    double xs0 = this.timeValues[0].Value;
                    double xs1 = this.timeValues[1].Value;
                    xr = ((xs0 - xs1) / (ts0 - ts1)) * (tr - ts0) * (1 - relaxationFactor) + xs0;
                    return ToSI(xr);
                }

                else if (tr > tsN2)
                {
                    double tsN1 = this.timeValues[count - 2].Time.ToOADate();
                    double xsN1 = this.timeValues[count - 2].Value;
                    double xsN2 = this.timeValues[count - 1].Value;
                    xr = ((xsN1 - xsN2) / (tsN1 - tsN2)) * (tr - tsN2) * (1 - relaxationFactor) + xsN2;
                    return ToSI(xr);
                }
                else
                {
                    for (int i = 0; i < count-1; i++)
                    {
                        double ts1 = this.timeValues[i].Time.ToOADate();
                        double ts2 = this.timeValues[i + 1].Time.ToOADate();
                        if (ts1 <= tr && tr <= ts2)
                        {
                            double xs1 = this.timeValues[i].Value;
                            double xs2 = this.timeValues[i + 1].Value;
                            xr = ((xs2 - xs1) / (ts2 - ts1)) * (tr - ts1) + xs1;
                            return ToSI(xr);
                        }
                    }
                    throw new System.Exception("kurt");
                }

            }
            else   // if (this.TimeSeriesType == TimeSeriesType.TimeSpanBased)
            {
                if (timeValues.Count < 2)
                {
                    throw new Exception("GetValues was invoked for timestampbased timeseries with only on time defined");
                }
                //---------------------------------------------------------------------------
                //  Buffered TimesSpans:  |          >tbb0<  ..........  >tbbN<
                //  Requested TimeStamp:  |    >tr<
                //                         -----------------------------------------> t
                // --------------------------------------------------------------------------
                if (tr <= timeValues[0].Time.ToOADate())
                {
                    double tbb0 = timeValues[0].Time.ToOADate();
                    double tbb1 = timeValues[1].Time.ToOADate();
                    double sbi0 = timeValues[0].Value;
                    double sbi1 = timeValues[1].Value;
                    xr = ((sbi0 - sbi1) / (tbb0 - tbb1)) * (tr - tbb0) * (1 - relaxationFactor) + sbi0;
                }

                //---------------------------------------------------------------------------
                //  Buffered TimesSpans:  |    >tbb0<   .................  >tbbN_1<
                //  Requested TimeStamp:  |                                             >tr<
                //                         ---------------------------------------------------> t
                // --------------------------------------------------------------------------
                else if (tr >= timeValues[timeValues.Count - 1].Time.ToOADate())//((ITimeSpan)_times[_times.Count - 1]).End.ModifiedJulianDay)
                {
                    double tbeN_2 = timeValues[timeValues.Count - 2].Time.ToOADate(); //((ITimeSpan)_times[_times.Count - 2]).End.ModifiedJulianDay;
                    double tbeN_1 = timeValues[timeValues.Count - 1].Time.ToOADate();//((ITimeSpan)_times[_times.Count - 1]).End.ModifiedJulianDay;

                    if (timeValues.Count > 2)
                    {
                        double sbiN_2 = timeValues[timeValues.Count - 3].Value;//Support.GetVal((IValueSet)_values[_times.Count - 2], i, k);
                        double sbiN_1 = timeValues[timeValues.Count - 2].Value;//Support.GetVal((IValueSet)_values[_times.Count - 1], i, k);

                        xr = ((sbiN_1 - sbiN_2) / (tbeN_1 - tbeN_2)) * (tr - tbeN_1) * (1 - relaxationFactor) + sbiN_1;
                    }
                    else
                    {
                        xr = timeValues[0].Value;
                    }
                }

                //---------------------------------------------------------------------------
                //  Availeble TimesSpans:  |    >tbb0<   ......................  >tbbN_1<
                //  Requested TimeStamp:   |                          >tr<
                //                         -------------------------------------------------> t
                // --------------------------------------------------------------------------
                else
                {
                    for (int n = timeValues.Count - 1; n >= 0; n--) //for (int n = _times.Count - 1; n >= 0; n--)
                    {
                        double tbbn = timeValues[n - 1].Time.ToOADate();//((ITimeSpan)_times[n]).Start.ModifiedJulianDay;
                        double tben = timeValues[n].Time.ToOADate();//((ITimeSpan)_times[n]).End.ModifiedJulianDay;

                        if (tbbn <= tr && tr < tben)
                        {
                            xr = timeValues[n - 1].Value;//xr[i][k - 1] = Support.GetVal((IValueSet)_values[n], i, k);
                            break;
                        }
                    }
                }
                return ToSI(xr);
            }
        }

        public double GetValue(DateTime fromTime, DateTime toTime)
        {
            if (this.TimeSeriesType == TimeSeriesType.TimeSpanBased)
            {
                return FromTimeSpanToTimeSpan(fromTime, toTime);
                //throw new Exception("Getvalues method not yet implemented for timespan based TS'S");
            }

            if (timeValues.Count == 0)
            {
                throw new Exception("TimeSeries.GetValues() method was invoked for time series with zero records");
            }
            double trb = fromTime.ToOADate();   // Begin time in requester time interval
            double tre = toTime.ToOADate();     // End time in requester time interval
            double xr = 0; // return value;

            for (int n = 0; n < this.timeValues.Count - 1; n++)
            {
                double tbn = timeValues[n].Time.ToOADate();
                double tbnp1 = timeValues[n+1].Time.ToOADate();
                double sbin = timeValues[n].Value;
                double sbinp1 = timeValues[n + 1].Value;

                //---------------------------------------------------------------------------
                //    B:           <-------------------------->
                //    R:        <------------------------------------->
                // --------------------------------------------------------------------------
                if (trb <= tbn && tre >= tbnp1)
                {
                  xr += 0.5 * (sbin + sbinp1) * (tbnp1 - tbn) / (tre - trb);
                }

                //---------------------------------------------------------------------------
                //           Times[i] Interval:        t1|-----------------------|t2
                //           Requested Interval:          rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (tbn <= trb && tre <= tbnp1) //cover all
                {
                    xr += sbin + ((sbinp1 - sbin) / (tbnp1 - tbn)) * ((tre + trb) / 2 - tbn);
                }

                //---------------------------------------------------------------------------
                //           Times[i] Interval:       t1|-----------------|t2
                //           Requested Interval:                 rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (tbn < trb && trb < tbnp1 && tre > tbnp1)
                {
                   xr += (sbinp1 - (sbinp1 - sbin) / (tbnp1 - tbn) * ((tbnp1 - trb) / 2)) * (tbnp1 - trb) / (tre - trb);
                }

                //---------------------------------------------------------------------------
                //           Times[i] Interval:             t1|-----------------|t2
                //           Requested Interval:      rt1|--------------|rt2
                // --------------------------------------------------------------------------
                else if (trb < tbn && tre > tbn && tre < tbnp1)
                {
                   xr += (sbin + (sbinp1 - sbin) / (tbnp1 - tbn) * ((tre - tbn) / 2)) * (tre - tbn) / (tre - trb);
                 }
            }
            //--------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //        |----------------|                  R
            //---------------------------------------------------------------------------
            double tb0 = timeValues[0].Time.ToOADate(); 
            double tb1 = timeValues[1].Time.ToOADate();
            double tbN_1 = timeValues[timeValues.Count - 1].Time.ToOADate();
            double tbN_2 = timeValues[timeValues.Count - 2].Time.ToOADate();

            if (trb < tb0 && tre > tb0)
            {
                double sbi0 = timeValues[0].Value;
                double sbi1 = timeValues[1].Value;
                xr += ((tb0 - trb) / (tre - trb)) * (sbi0 - (1 - relaxationFactor) * 0.5 * ((tb0 - trb) * (sbi1 - sbi0) / (tb1 - tb0)));
            }
            //-------------------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //                                    |----------------|                  R
            //-------------------------------------------------------------------------------------
            if (tre > tbN_1 && trb < tbN_1)
            {
                double sbiN_1 = timeValues[timeValues.Count - 1].Value;
                double sbiN_2 = timeValues[timeValues.Count - 2].Value;
                xr += ((tre - tbN_1) / (tre - trb)) * (sbiN_1 + (1 - relaxationFactor) * 0.5 * ((tre - tbN_1) * (sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)));
            }
            //-------------------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //                                              |----------------|   R
            //-------------------------------------------------------------------------------------
            if (trb >= tbN_1)
            {

                double sbiN_1 = timeValues[timeValues.Count - 1].Value;
                double sbiN_2 = timeValues[timeValues.Count - 2].Value;
                xr = sbiN_1 + (1 - relaxationFactor) * ((sbiN_1 - sbiN_2) / (tbN_1 - tbN_2)) * (0.5 * (trb + tre) - tbN_1);
            }
            //-------------------------------------------------------------------------------------
            //                           |--------|---------|--------| B
            //        |----------------|   R
            //-------------------------------------------------------------------------------------
            if (tre <= tb0)
            {
                double sbi0 = timeValues[0].Value;
                double sbi1 = timeValues[1].Value;
                xr = sbi0 - (1 - relaxationFactor) * ((sbi1 - sbi0) / (tb1 - tb0)) * (tb0 - 0.5 * (trb + tre));
            }

            return ToSI(xr);
        }

        public double GetValue(DateTime time, Unit unit)
        {
            double x = GetValue(time);
            return (x - unit.OffSetToSI)/unit.ConversionFactorToSI;
        }

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
        public double GetValue(DateTime fromTime, DateTime toTime, Unit unit)
        {
            double x = GetValue(fromTime, toTime);
            return (x - unit.OffSetToSI) / unit.ConversionFactorToSI;
        }

        private double ToSI(double value)
        {
            if (unit.ConversionFactorToSI == 1.0 && unit.OffSetToSI == 0.0)
            {
                return value;
            }
            else
            {
                return unit.ConversionFactorToSI * value + unit.OffSetToSI;
            }
        }

        public double FromTimeSpanToTimeSpan(DateTime fromTime, DateTime toTime)
        {
      //      {
      //try
      //{
        int	       m  = timeValues.Count;//  ((IValueSet)_values[0]).Count;
        double xr = 0; //double[][] xr = new double[m][];                                       // Values to return
        double trb    = fromTime.ToOADate();//requestedTime.Start.ModifiedJulianDay;   // Begin time in requester time interval
        double tre    = toTime.ToOADate(); //requestedTime.End.ModifiedJulianDay;     // End time in requester time interval

        //int nk; // number of components (scalars has only 1 and vectors has 3 (3 axis))

        //if (_values[0] is IVectorSet)
        //{
        //  nk = 3;
        //}
        //else
        //{
        //  nk = 1;
        //}
				
        //for (int i = 0; i < m; i++)
        //{
        //  xr[i] = new double[nk];
        //}

        //for (int i = 0; i < m; i++)
        //{
        //  for (int k = 0; k < nk; k++)
        //  {
        //    xr[i][k] = 0;
        //  }
        //}

        for (int n = 0; n < timeValues.Count-1; n++)// _times.Count; n++)
        {
          double tbbn = timeValues[n].Time.ToOADate();//((ITimeSpan) _times[n]).Start.ModifiedJulianDay;
          double tben = timeValues[n+1].Time.ToOADate();//((ITimeSpan) _times[n]).End.ModifiedJulianDay;

          //---------------------------------------------------------------------------
          //    B:           <-------------------------->
          //    R:        <------------------------------------->
          // --------------------------------------------------------------------------
          if (trb <= tbbn && tre >= tben ) //Buffered TimeSpan fully included in requested TimeSpan
          {
            //for (int k = 1; k <= nk; k++)
            //{
            //  for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            //  {
                double sbin = timeValues[n].Value;//Support.GetVal((IValueSet)_values[n], i, k);
                xr += sbin * (tben - tbbn)/(tre - trb);
            //  }
            //}
          }

            //---------------------------------------------------------------------------
            //           Times[i] Interval:        t1|-----------------------|t2
            //           Requested Interval:          rt1|--------------|rt2
            // --------------------------------------------------------------------------
          else if (tbbn <= trb && tre <= tben) //cover all
          {
            //for (int k = 1; k <= nk; k++)
            //{
            //  for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            //  {
                xr += timeValues[n].Value;//Support.GetVal((IValueSet)_values[n], i, k);
            //  }
            //}
          }

            //---------------------------------------------------------------------------
            //           Times[i] Interval:       t1|-----------------|t2
            //           Requested Interval:                 rt1|--------------|rt2
            // --------------------------------------------------------------------------
          else if (tbbn < trb && trb < tben && tre > tben)
          {
            //for (int k = 1; k <= nk; k++)
            //{
            //  for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            //  {
                double sbin = timeValues[n].Value;//Support.GetVal((IValueSet)_values[n], i, k);
                xr += sbin * (tben - trb)/(tre - trb);
            //  }
            //}
          }

            //---------------------------------------------------------------------------
            //           Times[i] Interval:             t1|-----------------|t2
            //           Requested Interval:      rt1|--------------|rt2
            // --------------------------------------------------------------------------
          else if (trb < tbbn && tre > tbbn && tre < tben)
          {
            //for (int k = 1; k <= nk; k++)
            //{
            //  for (int i = 0; i < m; i++) // for all values coorsponding to the same time interval
            //  {
                double sbin = timeValues[n].Value;//Support.GetVal((IValueSet)_values[n], i, k);
                xr += sbin * (tre - tbbn)/(tre - trb);
            //  }
            //}
          }
        }

        //--------------------------------------------------------------------------
        //              |--------|---------|--------| B
        //        |----------------|                  R
        //---------------------------------------------------------------------------
        double tbb0 = timeValues[0].Time.ToOADate();//((ITimeSpan) _times[0]).Start.ModifiedJulianDay;
        double tbe0 = timeValues[1].Time.ToOADate();//((ITimeSpan) _times[0]).End.ModifiedJulianDay;
        //double tbb1 = ((ITimeSpan) _times[1]).Start.ModifiedJulianDay;
        double tbe1 = timeValues[2].Time.ToOADate();//((ITimeSpan) _times[1]).End.ModifiedJulianDay;

        if (trb < tbb0 && tre > tbb0)
        {
          //for (int k = 1; k <= nk; k++)
          //{
          //  for (int i = 0; i < m; i++)
          //  {
              double sbi0 = timeValues[0].Value;//Support.GetVal((IValueSet)_values[0], i, k);
              double sbi1 = timeValues[1].Value;//Support.GetVal((IValueSet)_values[1], i, k); 
              xr += ((tbb0 - trb)/(tre - trb)) * (sbi0 - (1 - relaxationFactor) * ((tbb0 - trb)*(sbi1 - sbi0)/(tbe1 - tbe0)));
          //  }
          //}
        }

        //-------------------------------------------------------------------------------------
        //              |--------|---------|--------| B
        //                                    |----------------|                  R
        //-------------------------------------------------------------------------------------

        double tbeN_1 = timeValues[timeValues.Count-1].Time.ToOADate();//((ITimeSpan) _times[_times.Count-1]).End.ModifiedJulianDay;
        double tbbN_2 = timeValues[timeValues.Count-3].Time.ToOADate();//((ITimeSpan) _times[_times.Count-2]).Start.ModifiedJulianDay;

        if (tre > tbeN_1 && trb < tbeN_1)
        {
          //double tbeN_2 = ((ITimeSpan) _times[_times.Count-2]).End.ModifiedJulianDay;
          double tbbN_1 = timeValues[timeValues.Count-2].Time.ToOADate();//((ITimeSpan) _times[_times.Count-1]).Start.ModifiedJulianDay;

          //for (int k = 1; k <= nk; k++)
          //{
          //  for (int i = 0; i < m; i++)
          //  {
              double sbiN_1 = timeValues[timeValues.Count-1].Value;//Support.GetVal((IValueSet)_values[_times.Count-1], i, k);
              double sbiN_2 = timeValues[timeValues.Count-2].Value;//Support.GetVal((IValueSet)_values[_times.Count-2], i,k);
              xr += ((tre - tbeN_1)/(tre - trb)) * (sbiN_1 + (1 - relaxationFactor) * ((tre - tbbN_1)*(sbiN_1 - sbiN_2)/(tbeN_1 - tbbN_2)));
          //  }
          //}
        }
        //-------------------------------------------------------------------------------------
        //              |--------|---------|--------| B
        //                                              |----------------|   R
        //-------------------------------------------------------------------------------------

        if (trb >= tbeN_1)
        {
          double tbeN_2 = timeValues[timeValues.Count-2].Time.ToOADate();//((ITimeSpan) _times[_times.Count-2]).End.ModifiedJulianDay;
          //double tbbN_1 = ((ITimeSpan) _times[_times.Count-1]).Start.ModifiedJulianDay;
			
          //for (int k = 1; k <= nk; k++)
          //{
          //  for (int i = 0; i < m; i++)
          //  {
              double sbiN_1 = timeValues[timeValues.Count-1].Value;//Support.GetVal((IValueSet)_values[_times.Count-1], i, k);
              double sbiN_2 = timeValues[timeValues.Count-2].Value;//Support.GetVal((IValueSet)_values[_times.Count-2], i, k);
              xr = sbiN_1 + (1 - relaxationFactor) * ((sbiN_1 - sbiN_2)/(tbeN_1 - tbbN_2)) * (trb + tre - tbeN_1 - tbeN_2);
          //  }
          //}
        }

        //-------------------------------------------------------------------------------------
        //                           |--------|---------|--------| B
        //        |----------------|   R
        //-------------------------------------------------------------------------------------

        if (tre <= tbb0)
        {
          //for (int k = 1; k <= nk; k++)
          //{
          //  for (int i = 0; i < m; i++)
          //  {
              double sbi0 = timeValues[0].Value;//Support.GetVal((IValueSet)_values[0], i, k);
              double sbi1 = timeValues[1].Value;//Support.GetVal((IValueSet)_values[1], i, k);
              xr = sbi0 - (1 - relaxationFactor) * ((sbi1 - sbi0)/(tbe1- tbb0))*(tbe0 + tbb0 - tre - trb);
          //  }
          //}
        }

        //-------------------------------------------------------------------------------------
        //if (_values[0] is IVectorSet)
        //{
        //  Vector [] vectors = new Vector[m]; 

        //  for (int i = 0; i < m; i++)
        //  {
        //    vectors[i] = new Vector(xr[i][0],xr[i][1],xr[i][2]);
        //  }

        //  VectorSet vectorSet = new VectorSet(vectors);

        //  return vectorSet;
        //}
        //else
        //{
        //  double[] xx = new double[m];

        //  for (int i = 0; i < m; i++)
        //  {
        //    xx[i] = xr[i][0];
        //  }
				
        //  ScalarSet scalarSet = new ScalarSet(xx);

        //  return scalarSet;
        //}
          return xr;
      //}
      //catch (Exception e)
      //{
      //  throw new Exception("MapFromTimeSpansToTimeSpan Failed",e);
      //}

        }
       

      
        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
