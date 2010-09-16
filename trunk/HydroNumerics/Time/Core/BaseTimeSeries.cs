using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
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
    [XmlInclude(typeof(TimestampSeries))]
    [XmlInclude(typeof(TimespanSeries))]
    [Serializable]
    [KnownType(typeof(TimestampSeries))]
    [KnownType(typeof(TimespanSeries))]
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
          AllowExtrapolation = false;
          isVisible = true;

          this.unit = new Unit("Default Unit", 1.0, 0.0, "Default Unit", new Dimension(0, 0, 0, 0, 0, 0, 0, 0));
          this.unit.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);
      }

      public BaseTimeSeries(string name, Unit unit) : this()
      {
          this.Unit = unit;
          this.name = name;
      }

      void unit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
          NotifyPropertyChanged(e.PropertyName);
      }

      [DataMember]
      public bool AllowExtrapolation { get; set; }

      /// <summary>
      /// Indicates whether the entries are sorted according to time.
      /// </summary>
      [DataMember]
      public bool IsSorted { get; internal set; }

        protected string name;
        [DataMember]
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

        [DataMember]
        protected bool isVisible;
        /// <summary>
        /// Used by timeseries plotting
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                NotifyPropertyChanged("IsVisible");
            }
        }

        protected Object tag;
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

        protected int selectedRecord;

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
        [DataMember]
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

        public void Save(FileStream fileStream)
        {
          DataContractSerializer dc = new DataContractSerializer(this.GetType(), null, int.MaxValue, false, true, null);

            //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
            FileStream stream = fileStream;
            dc.WriteObject(stream, this);
            //serializer.Serialize(stream, this);
            stream.Close();
        }

        public void Save(string filename)
        {
            FileStream stream = new FileStream(filename, FileMode.Create);
            Save(stream);
        }

        public void Load(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            Load(fileStream);
            fileStream.Close();
        }

        public abstract void ConvertUnit(Unit newUnit);
        public abstract void AppendValue(double value);
        public abstract double GetValue(DateTime time);
        public abstract double GetValue(DateTime fromTime, DateTime toTime);
        public abstract void RemoveAfter(DateTime time);
        public abstract void Load(FileStream fileStream);
        public abstract IEnumerable<double> Values {get;}
     

        public abstract DateTime EndTime { get; }
        public abstract DateTime StartTime { get; }
 
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
