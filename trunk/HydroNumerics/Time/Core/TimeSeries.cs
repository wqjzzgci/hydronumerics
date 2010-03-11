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
        public delegate void DataChanged();
        DataChanged dataChanged;

        private string id;

        [XmlAttribute]
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private System.ComponentModel.BindingList<TimeValue> timeValuesList;

        public System.ComponentModel.BindingList<TimeValue> TimeValuesList
        {
            get { return timeValuesList; }
            set 
            {
                timeValuesList = value;
                NotifyPropertyChanged("TimeValuesList");
            }
        }

        private Object anyProperty;
        [XmlIgnore]
        public Object AnyProperty
        {
            get
            {
                return anyProperty;
            }
            set
            {
                anyProperty = value;
            }
        }
	

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private Quantity quantity;

        public Quantity Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        private int selectedRecord;

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
	
	

        public TimeSeries()
        {
            //this.Initialize();
            this.ID = "no ID";
            this.TimeSeriesType = TimeSeriesType.TimeStampBased;
            TimeValuesList = new System.ComponentModel.BindingList<TimeValue>();
            //TimeValuesList.ListChanged += new System.ComponentModel.ListChangedEventHandler(TimeValuesList_ListChanged);
            dataChanged = new DataChanged(DataChangedEventhandler);
            this.relaxationFactor = 0.0;

            Unit unit = new Unit("m", 1.0, 0.0, "meters");
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.Length, 1);
            quantity = new Quantity(unit, "water level", "WaterLevel", global::OpenMI.Standard.ValueType.Scalar, dimension);
            this.description = "no description";
            this.selectedRecord = 0;
            
         }

        //void TimeValuesList_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        //{
        //    NotifyPropertyChanged("TimeValuesList");
        //}

        private double relaxationFactor;

        public double RelaxationFactor
        {
            get { return relaxationFactor; }
            set { relaxationFactor = value; }
        }

        /// <summary>
        /// Add time and values to the end of the timeseries. The values is by default zero. Time is calculated based on the
        /// times for the last two existing records
        /// </summary>
        public void AddTimeValueRecord()
        {
            if (timeValuesList.Count >= 2)
            {
              //JAG: Kunne det ikke laves med TimeSpan?
              //System.TimeSpan ts = TimeValuesList[TimeValuesList.Count - 1].Time - TimeValuesList[TimeValuesList.Count - 2].Time;

                int yearDiff = timeValuesList[timeValuesList.Count - 1].Time.Year - timeValuesList[timeValuesList.Count -2].Time.Year;
                int monthDiff = timeValuesList[timeValuesList.Count - 1].Time.Month - timeValuesList[timeValuesList.Count - 2].Time.Month;
                int dayDiff = timeValuesList[timeValuesList.Count - 1].Time.Day - timeValuesList[timeValuesList.Count - 2].Time.Day;
                int hourDiff = timeValuesList[timeValuesList.Count - 1].Time.Hour - timeValuesList[timeValuesList.Count - 2].Time.Hour;
                int minuteDiff = timeValuesList[timeValuesList.Count - 1].Time.Minute - timeValuesList[timeValuesList.Count - 2].Time.Minute;
                int secondDiff = timeValuesList[timeValuesList.Count - 1].Time.Second - timeValuesList[timeValuesList.Count - 2].Time.Second;

                DateTime newDateTime;

                if (yearDiff == 0 && dayDiff == 0 && hourDiff == 0 && minuteDiff == 0 && secondDiff == 0)
                {
                    newDateTime = timeValuesList[timeValuesList.Count - 1].Time.AddMonths(monthDiff);
                }
                else 
                {
                    newDateTime = timeValuesList[timeValuesList.Count - 1].Time.AddTicks(timeValuesList[timeValuesList.Count - 1].Time.Ticks - timeValuesList[timeValuesList.Count - 2].Time.Ticks);
                }
                timeValuesList.Add(new TimeValue(newDateTime, 0));
            }
            else if(timeValuesList.Count == 1)
            {
                DateTime newDateTime = new DateTime(timeValuesList[0].Time.Ticks);
                newDateTime = newDateTime.AddDays(1);
                timeValuesList.Add(new TimeValue(newDateTime,0));
            }
            else if(timeValuesList.Count == 0)
            {
                timeValuesList.Add(new TimeValue());
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
            if (this.TimeValuesList.Count == 0 || this.TimeValuesList[TimeValuesList.Count - 1].Time < timeValue.Time)
            {
                TimeValuesList.Add(timeValue);
            }
            else if(this.timeValuesList[0].Time > timeValue.Time)  //add the record to the beginning of the list
            {
                TimeValuesList.Insert(0, timeValue);
            }
            else //overwrite values if time already exists in the list or insert according to the time.
            {
                foreach (TimeValue tv in timeValuesList) 
                {
                    if (tv.Time == timeValue.Time)
                    {
                        tv.Value = timeValue.Value;
                    }
                }
                int timeValuesListCount = timeValuesList.Count;
                for (int i = 0; i < timeValuesListCount - 2; i++)
                {
                    if (timeValuesList[i].Time < timeValue.Time && timeValue.Time < timeValuesList[i + 1].Time)
                    {
                        TimeValuesList.Insert(i+1, timeValue);
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


        public double GetValue(DateTime time)
        {
            if (timeValuesList.Count == 0)
            {
                throw new Exception("TimeSeries.GetValues() method was invoked for time series with zero records");
            }
            double tr = time.ToOADate();  // the requested time
            double xr = 0; // the value to return

            if (this.timeSeriesType == TimeSeriesType.TimeStampBased)
            {
               
                double ts0 = this.timeValuesList[0].Time.ToOADate();
                int count = this.timeValuesList.Count;
                double tsN2 = this.timeValuesList[count - 1].Time.ToOADate();
                

                if (count == 1)
                {
                    return this.timeValuesList[0].Value;
                }

                if (tr < ts0)
                {
                    double ts1 = this.timeValuesList[1].Time.ToOADate();
                    double xs0 = this.timeValuesList[0].Value;
                    double xs1 = this.timeValuesList[1].Value;
                    xr = ((xs0 - xs1) / (ts0 - ts1)) * (tr - ts0) * (1 - relaxationFactor) + xs0;
                    return xr;
                }

                else if (tr > tsN2)
                {
                    double tsN1 = this.timeValuesList[count - 2].Time.ToOADate();
                    double xsN1 = this.timeValuesList[count - 2].Value;
                    double xsN2 = this.timeValuesList[count - 1].Value;
                    xr = ((xsN1 - xsN2) / (tsN1 - tsN2)) * (tr - tsN2) * (1 - relaxationFactor) + xsN2;
                    return xr;
                }
                else
                {
                    for (int i = 0; i < count-1; i++)
                    {
                        double ts1 = this.timeValuesList[i].Time.ToOADate();
                        double ts2 = this.timeValuesList[i + 1].Time.ToOADate();
                        if (ts1 <= tr && tr <= ts2)
                        {
                            double xs1 = this.timeValuesList[i].Value;
                            double xs2 = this.timeValuesList[i + 1].Value;
                            xr = ((xs2 - xs1) / (ts2 - ts1)) * (tr - ts1) + xs1;
                            return xr;
                        }
                    }
                    throw new System.Exception("kurt");
                }

            }
            else// if (this.TimeSeriesType == TimeSeriesType.TimeSpanBased)
            {
                throw new System.Exception("Getvalues for timespanbased in not yet implemented");
                //TODO: implement
            }
        }

        public void AddData(double x)
        {
            AddTimeValueRecord();
            timeValuesList[timeValuesList.Count - 1].Value = x;

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
