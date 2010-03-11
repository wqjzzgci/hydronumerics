using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;

namespace HydroNumerics.Time.Core
{
    [Serializable]
    public class TimeValue : System.ComponentModel.INotifyPropertyChanged
    {

        public TimeValue()
        {
            val = 0;
            time = new DateTime(2020, 1, 1);
        }

        public TimeValue(DateTime time, double val): this()
        {
            this.time = time;
            this.Value = val;
        }

        private DateTime time;

        [XmlAttribute]
        public DateTime Time
        {
            get { return time; }
            set 
            {
                time = value;
                NotifyPropertyChanged("Time");
            }
        }

        private double val;

        [XmlAttribute]
        public double Value
        {
            get { return val; }
            set 
            {
                val = value;
                NotifyPropertyChanged("Value");
            }
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
