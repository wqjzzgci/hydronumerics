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
using System.IO;
using System.Linq;

using HydroNumerics.Core;


namespace HydroNumerics.Time.Core
{
  [Serializable]
  [DataContract]
  public class TimestampSeries : BaseTimeSeries
  {

    [DataMember]
    private List<TimestampValue> items = new List<TimestampValue>();

    /// <summary>
    /// Constructor. Assigning default values for the timeseries properties.
    /// </summary>
    public TimestampSeries()
    {
      Items = new System.ComponentModel.BindingList<TimestampValue>(items);
      Items.ListChanged += new System.ComponentModel.ListChangedEventHandler(items_ListChanged);
      ExtrapolationMethod = ExtrapolationMethods.Linear;
    }

    [OnDeserialized]
    private void ReconnectEvents(StreamingContext context)
    {
      Items = new System.ComponentModel.BindingList<TimestampValue>(items);
      Items.ListChanged += new System.ComponentModel.ListChangedEventHandler(items_ListChanged);
    }



    public IEnumerable<TimestampValue> ItemsInPeriod(DateTime Start, DateTime End)
    {
      return Items.Where(var => var.Time>=Start & var.Time<End );
    }



    /// <summary>
    /// Copy constructor. Returns a deep clone
    /// </summary>
    /// <param name="TS"></param>
    public TimestampSeries(TimestampSeries TS)
    {
      foreach (var i in TS.Items)
        Items.Add(i);
      this.Id = TS.Id;
      this.IsSorted = TS.IsSorted;
      this.Name = TS.name;
      this.RelaxationFactor = TS.relaxationFactor;

      //Problem with the next two as they should really be cloned.
      this.Tag = TS.Tag;
      this.Unit = TS.Unit;

      this.AllowExtrapolation = TS.AllowExtrapolation;
      this.Description = TS.Description;
    }

    void items_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
      IsSorted = false;
      NotifyPropertyChanged("Items");
    }

      public TimestampSeries (string name, Unit unit) : this()
      {
          this.Unit = unit;
          this.name = name;
      }

    public TimestampSeries(string name, DateTime startTime, int numberOfTimesteps, int timestepLength, TimestepUnit timestepLengthUnit, double defaultValue)
      : this()
    {
      items.Add(new TimestampValue(startTime, defaultValue));

      this.name = name;

      for (int i = 1; i < numberOfTimesteps; i++)
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
          items.Add(new TimestampValue(startTime.AddDays(timestepLength * i), defaultValue));
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

    public TimestampSeries(string name, DateTime startTime, int numberOfTimesteps, int timestepLength, TimestepUnit timestepLengthUnit, double defaultValue, Unit unit) : this (name, startTime, numberOfTimesteps, timestepLength, timestepLengthUnit, defaultValue)
    {
        this.unit = unit;
    }

   

    /// <summary>
    /// The list holding all the TimeValues objects
    /// </summary>
    public System.ComponentModel.BindingList<TimestampValue> Items
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the last time of the time series
    /// </summary>
    public override DateTime EndTime
    {
      get
      {
        if (AllowExtrapolation)
          return DateTime.MaxValue;
        return items.Last().Time;
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

        return items.First().Time;
      }
    }



    /// <summary>
    /// Add time and values to the end of the timeseries. 
    /// Time is calculated based on the times for the last two existing records. If only
    /// one record exists in the time series, the time step length is set to one day. If 
    /// the timeseries is empty, the time for the added record will be January 1st 2020.
    /// </summary>
    public override void AppendValue(double value)
    {
      if (items.Count >= 2)
      {
        int yearDiff = items[items.Count - 1].Time.Year - items[items.Count - 2].Time.Year;
        int monthDiff = items[items.Count - 1].Time.Month - items[items.Count - 2].Time.Month + yearDiff * 12;
        int dayDiff = items[items.Count - 1].Time.Day - items[items.Count - 2].Time.Day;
        int hourDiff = items[items.Count - 1].Time.Hour - items[items.Count - 2].Time.Hour;
        int minuteDiff = items[items.Count - 1].Time.Minute - items[items.Count - 2].Time.Minute;
        int secondDiff = items[items.Count - 1].Time.Second - items[items.Count - 2].Time.Second;

        DateTime newDateTime;

        if (monthDiff != 0 && dayDiff == 0 && hourDiff == 0 && minuteDiff == 0 && secondDiff == 0)
        {
          newDateTime = items[items.Count - 1].Time.AddMonths(monthDiff);
        }
        else
        {
          newDateTime = items[items.Count - 1].Time.AddTicks(items[items.Count - 1].Time.Ticks - items[items.Count - 2].Time.Ticks);
        }
        Items.Add(new TimestampValue(newDateTime, value));
      }
      else if (items.Count == 1)
      {
        DateTime newDateTime = new DateTime(items[0].Time.Ticks);
        newDateTime = newDateTime.AddDays(1);
        Items.Add(new TimestampValue(newDateTime, value));
      }
      else if (items.Count == 0)
      {
        TimestampValue timeValue = new TimestampValue();
        timeValue.Value = value;
        Items.Add(timeValue);
      }
      else
      {
        throw new Exception("unexpected exception when adding time series record");
      }
    }

    public void Sort()
    {
      items.Sort(new Comparison<TimestampValue>((var1,var2)=>var1.Time.CompareTo(var2.Time)));
      IsSorted = true;
    }

    /// <summary>
    /// Add time and value to the timeseries. The added record will be placed according the the 
    /// specified time. If the time specified already exists the corresponding value will be overwritten
    /// with the new value
    /// </summary>
    /// <param name="timeValue">time and value to add</param>
    public void AddValue(DateTime time, double value)
    {
      //Comment by JAG: Var det ikke bedre at indsætte dem i en usorteret list og så sortere hvis det bliver nødvendigt. Lige som med TimeSpan.

      TimestampValue timeValue = new TimestampValue(time, value);

      if (this.Items.Count == 0 || this.Items[Items.Count - 1].Time < timeValue.Time)
      {
        Items.Add(timeValue);
      }
      else if (this.items[0].Time > timeValue.Time)  //add the record to the beginning of the list
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
        for (int i = 0; i < timeValuesListCount - 1; i++)
        {
          if (items[i].Time < timeValue.Time && timeValue.Time < items[i + 1].Time)
          {
            Items.Insert(i + 1, timeValue);
          }
        }
      }
    }

    public void AddSiValue(DateTime time, double value)
    {
      AddValue(time, Unit.FromSiToThisUnit(value));
    }

    public override void ConvertUnit(Unit newUnit)
    {
      foreach (TimestampValue timeValue in items)
      {
        timeValue.Value = this.unit.FromThisUnitToUnit(timeValue.Value, newUnit);
      }
      this.unit = new HydroNumerics.Core.Unit(newUnit);
    }

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

      if (ExtrapolationMethod == ExtrapolationMethods.RecycleYear)
      {
          DateTime tsStartTime = items.First().Time;
          DateTime tsEndTime = items.Last().Time;

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
        return xr;
      }

      else if (tr > tsN2)
      {
        double tsN1 = this.items[count - 2].Time.ToOADate();
        double xsN1 = this.items[count - 2].Value;
        double xsN2 = this.items[count - 1].Value;
        xr = ((xsN1 - xsN2) / (tsN1 - tsN2)) * (tr - tsN2) * (1 - relaxationFactor) + xsN2;
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
            return xr;
          }
        }
        throw new System.Exception("kurt");
      }
    }

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

      if (ExtrapolationMethod == ExtrapolationMethods.RecycleYear)
      {
          DateTime tsStartTime = items.First().Time;
          DateTime tsEndTime = items.Last().Time;

          if (fromTime < tsStartTime || toTime > tsEndTime)
          {
              if (tsEndTime - tsStartTime < new System.TimeSpan(365, 0, 0))
              {
                  throw new Exception("cannot use extrapolation method Recycle Year for timeseries with a duration less that one year");
              }

              while (fromTime < tsStartTime)
              {
                  fromTime = fromTime.AddYears(1);
                  toTime = toTime.AddYears(1);
              }

              while (toTime > tsEndTime)
              {
                  toTime = toTime.AddYears(-1);
                  fromTime = fromTime.AddYears(-1);
              }
          }
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

    public override void RemoveAfter(DateTime time)
    {
      bool foundItemToRemove;

      do
      {
        foundItemToRemove = false;
        for (int i = items.Count - 1; i >= 0; i--)
        {
          if (items[i].Time > time)
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
      //XmlSerializer serializer = new XmlSerializer(typeof(TimestampSeries));
      //TimestampSeries ts = (TimestampSeries)serializer.Deserialize(fileStream);
      DataContractSerializer dc = new DataContractSerializer(typeof(TimestampSeries));
      TimestampSeries ts = (TimestampSeries)dc.ReadObject(fileStream);
      this.name = ts.Name;
      this.id = ts.Id;
      this.relaxationFactor = ts.RelaxationFactor;
      this.unit = new Unit(ts.unit);
      this.items.Clear();
      foreach (TimestampValue tsv in ts.Items)
      {
        items.Add(new TimestampValue(tsv));
      }
    }

    public override bool Equals(Object obj)
    {
      bool equals = true;
      if (obj == null || GetType() != obj.GetType()) return false;
      if (this.Id != ((TimestampSeries)obj).Id) equals = false;
      if (this.Name != ((TimestampSeries)obj).Name) equals = false;
      if (this.Description != ((TimestampSeries)obj).Description) equals = false;
      if (this.RelaxationFactor != ((TimestampSeries)obj).RelaxationFactor) equals = false;
      if (!this.Unit.Equals(((TimestampSeries)obj).Unit)) equals = false;
      if (this.Items.Count != ((TimestampSeries)obj).Items.Count)
      {
        return false;
      }
      for (int i = 0; i < this.Items.Count; i++)
      {
        if (!this.Items[i].Equals(((TimestampSeries)obj).Items[i]))
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
