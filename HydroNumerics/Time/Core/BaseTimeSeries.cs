using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{
    public enum TimestepUnit
    {
        Years, Months ,Days, Hours, Minutes, Seconds
    }
    
  [DataContract]
    public abstract class BaseTimeSeries : System.ComponentModel.INotifyPropertyChanged
    {
      public BaseTimeSeries()
      {
          name = "No name defined";
          id = 0;
          description = "No description defined";
          relaxationFactor = 0.0;
          selectedRecord = 0;

          this.unit = new Unit("Default Unit", 1.0, 0.0, "Default Unit", new Dimension(0, 0, 0, 0, 0, 0, 0, 0));
          this.unit.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);
      }

      void unit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
          NotifyPropertyChanged(e.PropertyName);
      }

       [DataMember]
        protected string name;
        public string Name
        {
            get 
            {
                return name;
            }
            set 
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        [DataMember]
        protected int id;
        public int Id
        {
            get { return id; }
            set 
            {
                id = value;
                NotifyPropertyChanged("Id");
            }
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
                NotifyPropertyChanged("SelectedRecord");
            }
        }

        [DataMember]
        protected string description;

        /// <summary>
        /// Description for the time series
        /// </summary>
        public string Description
        {
            get { return description; }
            set 
            { 
                description = value;
                NotifyPropertyChanged("Description");
            }
          
        }

        [DataMember]
        protected Unit unit;

        /// <summary>
        /// Unit for all values in the time series
        /// </summary>
        public Unit Unit
        {
            get { return unit; }
            set 
            {
                unit = value;
                NotifyPropertyChanged("Unit");
                unit.PropertyChanged+=new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);
            }
        }


        [DataMember]
        protected double relaxationFactor;

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
                NotifyPropertyChanged("RelaxationFactor");
            }
        }

        public double GetValue(DateTime time, Unit toUnit)
        {
            double x = GetValue(time);
            return this.unit.FromThisUnitToUnit(x, toUnit);
        }
    
        public double GetValue(DateTime fromTime, DateTime toTime, Unit toUnit)
        {
            double x = GetValue(fromTime, toTime);
            return this.unit.FromThisUnitToUnit(x, toUnit);
        }

        public double GetSiValue(DateTime time)
        {
            return Unit.ToSiUnit(GetValue(time));
        }

        public double GetSiValue(DateTime fromTime, DateTime toTime)
        {
            return Unit.ToSiUnit(GetValue(fromTime, toTime));
        }

        public abstract void ConvertUnit(Unit newUnit);
 
        public abstract void AppendValue(double value);
        public abstract double GetValue(DateTime time);
        public abstract double GetValue(DateTime fromTime, DateTime toTime);
        public abstract void RemoveAfter(DateTime time);
 
        #region INotifyPropertyChanged Members

        /// <summary>
        /// The PropertyChanged event is raised when data in the timeseries are changed. This includes
        /// all data, also values in internal lists
        /// </summary>
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
