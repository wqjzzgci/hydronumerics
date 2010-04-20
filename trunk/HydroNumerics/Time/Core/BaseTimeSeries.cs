using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{
    public abstract class BaseTimeSeries : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// The data DataChanged event will be send whenever values of the timeseries are changed.
        /// The dataChanged event is not sent when timeseries properties are changed (such as TimeSeries.Name).
        /// However, the DataChanged event is sent, when the property: SelectedRecord is changed.
        /// </summary>
        public delegate void DataChanged();
        protected DataChanged dataChanged;

        protected string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        protected int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        protected Object tag;
        /// <summary>
        /// An object tag, that may be used for anything. Is used e.g. by the timeserieseditor to
        /// attach graphics specific objects to the individual time series. The tag object is not
        /// stored with the time series (not part of the xml seriallisation).
        /// </summary>
  
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

        protected int selectedRecord;

        /// <summary>
        /// Index of the user selected record. Used by the time series editor. Changing the selected record
        /// will trigger the DataChanged event to be sent. 
        /// </summary>
      
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

        protected string description;

        /// <summary>
        /// Description for the time series
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        protected Unit unit;

        /// <summary>
        /// Unit for all values in the time series
        /// </summary>
        public Unit Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        

        public abstract void ConvertUnit(Unit newUnit);


        public abstract int Count { get; }
        public abstract double GetValue(int index);
        public abstract double GetValue(int index, bool toSIUnit);
        public abstract double GetValue(int index, Unit toUnit);
        public abstract double ExtractValue(DateTime time);
        public abstract double ExtractValue(DateTime time, bool toSIUnit);
        public abstract double ExtractValue(DateTime time, Unit toUnit);
        public abstract double ExtractValue(DateTime fromTime, DateTime toTime);
        public abstract double ExtractValue(DateTime fromTime, DateTime toTime, bool toSIUnit);
        public abstract double ExtractValue(DateTime fromTime, DateTime toTime, Unit toUnit);

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
