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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{
    [Serializable]
    [DataContract]
    public class TimespanSeries : BaseTimeSeries
    {
        public TimespanSeries()
        {
          items = new List<TimespanValue>();
          Items = new System.ComponentModel.BindingList<TimespanValue>(items);
          Items.ListChanged += new System.ComponentModel.ListChangedEventHandler(items_ListChanged);
        }

        [OnDeserialized]
        private void ReconnectEvents(StreamingContext context)
        {
          Items = new System.ComponentModel.BindingList<TimespanValue>(items);
          Items.ListChanged += new System.ComponentModel.ListChangedEventHandler(items_ListChanged);
            ExtrapolationMethod = ExtrapolationMethods.Linear;
        }

        void items_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
          IsSorted = false;
            NotifyPropertyChanged("Items");
        }
      
        public TimespanSeries(string name, DateTime startTime, int numberOfTimesteps, int timestepLength, TimestepUnit timestepLengthUnit, double defaultValue) : this()
        {
            this.name = name;

            for (int i = 0; i < numberOfTimesteps; i++)
            {

                if (timestepLengthUnit == TimestepUnit.Years)
                {
                    items.Add(new TimespanValue(startTime.AddYears(i*timestepLength),startTime.AddYears((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Months)
                {
                    items.Add(new TimespanValue(startTime.AddMonths(i*timestepLength),startTime.AddMonths((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Days)
                {
                    items.Add(new TimespanValue(startTime.AddDays(i*timestepLength),startTime.AddDays((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Hours)
                {
                    items.Add(new TimespanValue(startTime.AddHours(i*timestepLength),startTime.AddHours((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Minutes)
                {
                    items.Add(new TimespanValue(startTime.AddMinutes(i*timestepLength),startTime.AddMinutes((i+1)*timestepLength),defaultValue));
                }
                else if (timestepLengthUnit == TimestepUnit.Seconds)
                {
                    items.Add(new TimespanValue(startTime.AddSeconds(i*timestepLength),startTime.AddSeconds((i+1)*timestepLength),defaultValue));
                }
                else
                {
                    throw new Exception("Unexpected exception");
                }
            }
        }

        [DataMember]
        private List<TimespanValue> items;

        public System.ComponentModel.BindingList<TimespanValue> Items { get; private set; }
      
      /// <summary>
      /// Gets the last time of the time series
      /// </summary>
      public override DateTime EndTime
        {
          get 
          {
            if (AllowExtrapolation)
              return DateTime.MaxValue;

            return items.Last().EndTime; 
          }
        }

      /// <summary>
      /// Gets the first time of the time series
      /// </summary>
      public override DateTime StartTime
      {
        get
        {
          if (AllowExtrapolation)
            return DateTime.MinValue;

          return items.First().StartTime;
        }
      }


        public override void ConvertUnit(HydroNumerics.Core.Unit newUnit)
        {
            foreach (TimespanValue timespanValue in items)
            {
                timespanValue.Value = this.unit.FromThisUnitToUnit(timespanValue.Value, newUnit);
            }
            this.unit = new Unit(newUnit);
        }
     
        public override void AppendValue(double value)
        {
            int count = items.Count;
            if (count >= 1)
            {
                int yearDiff = items[count - 1].EndTime.Year - items[count - 1].TimeSpan.Start.Year;
                int monthDiff = items[count - 1].EndTime.Month - items[count - 1].TimeSpan.Start.Month;
                int dayDiff = items[count - 1].EndTime.Day - items[count - 1].TimeSpan.Start.Day;
                int hourDiff = items[count - 1].EndTime.Hour - items[count - 1].TimeSpan.Start.Hour;
                int minuteDiff = items[count - 1].EndTime.Minute - items[count - 1].TimeSpan.Start.Minute;
                int secondDiff = items[count - 1].EndTime.Second - items[count - 1].TimeSpan.Start.Second;

                DateTime start = DateTime.FromOADate(items[count - 1].TimeSpan.Start.ToOADate());
                DateTime end;
                if (yearDiff == 0 && dayDiff == 0 && hourDiff == 0 && minuteDiff == 0 && secondDiff == 0)
                {
                    end = start.AddMonths(monthDiff);
                }
                else
                {
                    end = items[count - 1].TimeSpan.End.AddTicks(items[count - 1].TimeSpan.End.Ticks - items[count - 1].TimeSpan.Start.Ticks);
                }
                items.Add(new TimespanValue(items[count - 1].EndTime, end, value));
            }
            else 
            {
                throw new Exception("AppendValues method was invoked for empty TimespanSeries object");
            }
            
        }

        public void AddValue(DateTime startTime, DateTime endTime, double value)
        {
          //Finds entries that contain the starttime
          var val = items.FirstOrDefault(var => Contains(var, startTime));
          
          //entries found. Now add
          if (val != null)
          {
            val.Value += value;
          }
          else
            items.Add(new TimespanValue(startTime, endTime, value));
            //TODO: lav checks på om der er overlap osv.
        }
      
        public void AddSiValue(DateTime startTime, DateTime endTime, double value)
        {
            AddValue(startTime, endTime, Unit.FromSiToThisUnit(value));
        }

        public static Func<TimespanValue, DateTime, bool> Contains = (TSE, Time) => TSE.StartTime <= Time & TSE.EndTime > Time;

      

      /// <summary>
      /// Gets the value at the specified time. Returns false if there is no value.
      /// Extrapolates a value if Extrapolation is set to true
      /// </summary>
      /// <param name="time"></param>
      /// <param name="value"></param>
      /// <returns></returns>
        public bool TryGetValue(DateTime time, out double value)
        {
          //No values
          if (items.Count == 0)
            {
              value = double.NaN;
              return false;
            }

          if (ExtrapolationMethod == ExtrapolationMethods.RecycleYear)
          {
              DateTime tsStartTime = items.First().StartTime;
              DateTime tsEndTime = items.Last().EndTime;

              if (time < tsStartTime || time > tsEndTime)
              {
                  if (tsEndTime - tsStartTime < new System.TimeSpan(365, 0, 0))
                  {
                      throw new Exception("cannot use extrapolation method Recycle Year for timeseries with a duration less that one year");
                  }

                  while (time < tsStartTime)
                  {
                      time = time.AddYears(1);
                  }

                  while (time > tsEndTime)
                  {
                      time = time.AddYears(-1);
                  }
              }
          }


          //Try to find a value that contains the requested time
          var val = Items.FirstOrDefault(var => Contains(var, time));
          if (val != null)
          {
            value = val.Value;
            return true;
          }

          

          //Do not extrapolate
          if (!AllowExtrapolation)
          {
            value = double.NaN;
            return false;
          }

          //When only one item return that.  
          if (items.Count == 1)
          {
              value = items[0].Value;
              return true;
          }

          //If we reach this point we need to extrapolate.

          //Sort if necessary
          Sort();

          

          double tr = time.ToOADate();  // the requested time
            //---------------------------------------------------------------------------
            //  Buffered TimesSpans:  |          >tbb0<  ..........  >tbbN<
            //  Requested TimeStamp:  |    >tr<
            //                         -----------------------------------------> t
            // --------------------------------------------------------------------------
          //Time before  
          if (tr <= items[0].StartTime.ToOADate())
            {
              double tbb0 = items[0].StartTime.ToOADate();
              double tbb1 = items[1].StartTime.ToOADate();
              double sbi0 = items[0].Value;
              double sbi1 = items[1].Value;
              value = ((sbi0 - sbi1) / (tbb0 - tbb1)) * (tr - tbb0) * (1 - relaxationFactor) + sbi0;
            }

            //---------------------------------------------------------------------------
            //  Buffered TimesSpans:  |    >tbb0<   .................  >tbbN_1<
            //  Requested TimeStamp:  |                                             >tr<
            //                         ---------------------------------------------------> t
            // --------------------------------------------------------------------------
          //Time after  
          else if (tr >= items[items.Count - 1].EndTime.ToOADate())//((ITimeSpan)_times[_times.Count - 1]).End.ModifiedJulianDay)
          {
            double tbeN_2 = items[items.Count - 2].EndTime.ToOADate(); //((ITimeSpan)_times[_times.Count - 2]).End.ModifiedJulianDay;
            double tbeN_1 = items[items.Count - 1].EndTime.ToOADate();//((ITimeSpan)_times[_times.Count - 1]).End.ModifiedJulianDay;

            double sbiN_2 = items[items.Count - 2].Value;//Support.GetVal((IValueSet)_values[_times.Count - 2], i, k);
            double sbiN_1 = items[items.Count - 1].Value;//Support.GetVal((IValueSet)_values[_times.Count - 1], i, k);

            value = ((sbiN_1 - sbiN_2) / (tbeN_1 - tbeN_2)) * (tr - tbeN_1) * (1 - relaxationFactor) + sbiN_1;
          }
            //It would be necessary to interpolate, which is not allowed.
          else
          {
            value = double.NaN;
          }
          
      return true;

    }

      
      public override double GetValue(DateTime time)
        {
        double value;
        if (TryGetValue(time, out value))
          return value;
        else
          throw new Exception();

         
        }

      public void Sort()
      {
        if (!IsSorted)
        {
          items.Sort(new Comparison<TimespanValue>((var1, var2) => var1.StartTime.CompareTo(var2.StartTime)));
          IsSorted = true;
        }
      }


        public override double GetValue(DateTime fromTime, DateTime toTime)
        {
            if (items.Count == 0)
            {
                throw new Exception("ExtractValue method was invoked for time series with zero records");
            }

            if (items.Count == 1) //if only one record in timeseries, always return that value
            {
                return items[0].Value;
            }

            if (ExtrapolationMethod == ExtrapolationMethods.RecycleYear)
            {
                DateTime tsStartTime = items.First().StartTime;
                DateTime tsEndTime = items.Last().EndTime;

                if (fromTime < tsStartTime || toTime > tsEndTime)
                {
                    if (tsEndTime - tsStartTime < new System.TimeSpan(365, 0, 0))
                    {
                        throw new Exception("cannot use extrapolation method Recycle Year for timeseries with a duration less that one year");
                    }

                    System.TimeSpan timeSpan = toTime - fromTime;
                    while (fromTime < tsStartTime)
                    {
                        fromTime = fromTime.AddYears(1);
                        //toTime = toTime.AddYears(1);
                        toTime = fromTime + timeSpan;
                    }

                    while (toTime > tsEndTime)
                    {
                        toTime = toTime.AddYears(-1);
                        //fromTime = fromTime.AddYears(-1);
                        fromTime = toTime - timeSpan;
                    }
                }
            }

            double trFrom = fromTime.ToOADate();   // From time in requester time interval
            double trTo = toTime.ToOADate();     // To time in requester time interval

            if (trTo <= trFrom)
            {
                throw new Exception("Invalid arguments for ExtractValue method, toTime argument was smaller than or equal to fromTime argument");
            }

            double xr = 0; // return value;



            for (int n = 0; n < items.Count; n++)
            {
                double tsStepFrom = items[n].StartTime.ToOADate(); //time series from time for n'th TimeValue record
                double tsStepTo = items[n].EndTime.ToOADate(); //time series to time for then'th TimeValue record
                double xTsStep = items[n].Value; //time series value for the n'th timestep

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
            double tsb0 = items[0].StartTime.ToOADate(); //time series begine time for the first value
            double tse0 = items[0].EndTime.ToOADate(); //time series end time for the first value

            if (trFrom < tsb0 && trTo > tsb0)
            {
                double xTs0 = items[0].Value;
                double xTs1 = items[1].Value;
                xr += ((tsb0 - trFrom) / (trTo - trFrom)) * (xTs0 - (1 - relaxationFactor) * ((tsb0 - trFrom) * (xTs1 - xTs0) / (tse0 - tsb0)));
            }

            //-------------------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //                                    |----------------|                  R
            //-------------------------------------------------------------------------------------

            double tseN_1 = items[items.Count - 1].EndTime.ToOADate();

            if (trTo > tseN_1 && trFrom < tseN_1)
            {
                double tsbN_1 = items[items.Count - 1].StartTime.ToOADate();
                double xTSbN_1 = items[items.Count - 1].Value;
                double xTSbN_2 = items[items.Count - 2].Value;
                xr += ((trTo - tseN_1) / (trTo - trFrom)) * (xTSbN_1 + (1 - relaxationFactor) * ((trTo - tsbN_1) * (xTSbN_1 - xTSbN_2) / (tseN_1 - tsbN_1)));
            }
            //-------------------------------------------------------------------------------------
            //              |--------|---------|--------| B
            //                                              |----------------|   R
            //-------------------------------------------------------------------------------------
            if (trFrom >= tseN_1)
            {
                double tsbN_1 = items[items.Count - 1].StartTime.ToOADate();
                double xTSbN_1 = items[items.Count - 1].Value;
                double xTSbN_2 = items[items.Count - 2].Value;
                xr = xTSbN_1 + (1 - relaxationFactor) * ((xTSbN_1 - xTSbN_2) / (tseN_1 - tsbN_1)) * (trTo - tseN_1);
            }
            //-------------------------------------------------------------------------------------
            //                           |--------|---------|--------| B
            //        |----------------|   R
            //-------------------------------------------------------------------------------------
            if (trTo <= tsb0)
            {
                double xTs0 = items[0].Value;
                double xTs1 = items[1].Value;
                xr = xTs0 - (1 - relaxationFactor) * ((xTs1 - xTs0) / (tse0 - tsb0)) * (tsb0 - trFrom);
            }

            return xr;
        }

        public override void RemoveAfter(DateTime time)
        {
            bool foundItemToRemove;

            do
            {
                foundItemToRemove = false;
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (items[i].EndTime >= time)
                    {
                        items.RemoveAt(i);
                        foundItemToRemove = true;
                        break;
                    }
                }
            } while (foundItemToRemove);
        }

        public override void Load(FileStream fileStream)
        {
          DataContractSerializer dc = new DataContractSerializer(typeof(TimespanSeries));
//            XmlSerializer serializer = new XmlSerializer(typeof(TimespanSeries));
//            TimespanSeries ts = (TimespanSeries)serializer.Deserialize(fileStream);
            TimespanSeries ts = (TimespanSeries)dc.ReadObject(fileStream);
            this.name = ts.Name;
            this.id = ts.Id;
            this.relaxationFactor = ts.RelaxationFactor;
            this.unit = new Unit(ts.Unit);
            this.items.Clear();
            foreach (TimespanValue tsv in ts.Items)
            {
                items.Add(new TimespanValue(tsv));
            }
        }

        public override bool Equals(Object obj)
        {
            bool equals = true;
            if (obj == null || GetType() != obj.GetType()) return false;
            if (this.Id != ((TimespanSeries)obj).Id) equals = false;
            if (this.Name != ((TimespanSeries)obj).Name) equals = false;
            if (this.Description != ((TimespanSeries)obj).Description) equals = false;
            if (this.RelaxationFactor != ((TimespanSeries)obj).RelaxationFactor) equals = false;
            if (!this.Unit.Equals(((TimespanSeries)obj).Unit)) equals = false;
            if (this.Items.Count != ((TimespanSeries)obj).Items.Count)
            {
                return false;
            }
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (!this.Items[i].Equals(((TimespanSeries)obj).Items[i]))
                {
                    return false;
                }
            }

            return equals;
        }

        public override IEnumerable<double> Values
        {
          get
          {
            foreach (var v in Items)
              yield return v.Value;
          }
        }

    }
}
