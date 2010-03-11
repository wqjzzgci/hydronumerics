using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HydroNumerics.Time.Core
{
    public class TimeSeriesGroup : System.ComponentModel.INotifyPropertyChanged
    {
        public delegate void DataChangedEventHandler(object sender, string info);
        public event DataChangedEventHandler DataChanged;

        private int current = 0; //The index of the timeseries currently being edited or viewed

        [XmlIgnore]
        public int Current
        {
            get { return current; }
            set 
            {
                if (value < 0)
                {
                    current = 0;
                }
                else if (value >= timeSeriesList.Count)
                {
                    current = timeSeriesList.Count - 1;
                }
                else
                {
                    current = value;
                }
                NotifyPropertyChanged("Current");
            }
        }
        private System.ComponentModel.BindingList<TimeSeries> timeSeriesList;

        public System.ComponentModel.BindingList<TimeSeries> TimeSeriesList
        {
            get { return timeSeriesList; }
            set 
            { 
                timeSeriesList = value;
                NotifyPropertyChanged("TimeSeriesList");
            }
        }

        public TimeSeriesGroup()
        {
            this.timeSeriesList = new System.ComponentModel.BindingList<TimeSeries>();
            current = 0;

            this.timeSeriesList.ListChanged += new System.ComponentModel.ListChangedEventHandler(timeSeriesDataList_ListChanged);
          
        }

        void timeSeriesDataList_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            if (DataChanged != null && DataChanged.GetInvocationList().Length > 0)
            {
                foreach (TimeSeries tsData in timeSeriesList)
                {
                    tsData.TimeValuesList.ListChanged -= new System.ComponentModel.ListChangedEventHandler(TimeValuesList_ListChanged);

                }

                foreach (TimeSeries tsData in timeSeriesList)
                {
                    tsData.TimeValuesList.ListChanged += new System.ComponentModel.ListChangedEventHandler(TimeValuesList_ListChanged);
                }
            }
            if (DataChanged != null)
            {
                DataChanged(this, "DataChanged");
            }
        }

        void TimeValuesList_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            DataChanged(this, "DataChanged");
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


        public void Save(FileStream fileStream)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TimeSeriesGroup));
            FileStream stream = fileStream;
            serializer.Serialize(stream, this);
            stream.Close();
        }

        public void Save(string filename)
        {
            FileStream stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
            Save(stream);
        }

      
    }
}
