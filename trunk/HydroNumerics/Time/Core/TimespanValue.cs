using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{

    [Serializable]
    [DataContract]
    public class TimespanValue : System.ComponentModel.INotifyPropertyChanged
    {

    [DataMember]
        private double val;
        public double Value
        {
            get 
            { 
                return val; 
            }
            set
            {
                val = value;
                NotifyPropertyChanged("Value");
            }
        }

        public Timespan TimeSpan
        {
            get 
            { 
                return new Timespan(startTime, endTime); 
            }
            set 
            {
                startTime = value.Start;
                endTime = value.End;
                NotifyPropertyChanged("TimeSpan");
            }
        }

        [DataMember]
        private DateTime startTime;
        public DateTime StartTime
        {
            get 
            {
                return startTime; 
            }
            set
            {
                startTime = value;
                NotifyPropertyChanged("StartTime");
            }
        }

        [DataMember]
        private DateTime endTime;
        public DateTime EndTime
        {
            get 
            {
                return endTime;
            }
            set 
            {
                endTime = value;
                NotifyPropertyChanged("EndTime");
            }
        }

        public TimespanValue()
        {

        }

        public TimespanValue(DateTime startTime, DateTime endTime, double value)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.val = value; 
        }

        public TimespanValue(Timespan timespan, double value)
        {
            this.startTime = timespan.Start;
            this.endTime = timespan.End;
            this.val = value;
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
