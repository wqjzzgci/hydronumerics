using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{
    public abstract class BaseTimeSeries
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private Object tag;
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
    }
}
