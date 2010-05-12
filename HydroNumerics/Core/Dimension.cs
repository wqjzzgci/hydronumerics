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
using System.Xml;
using System.Runtime.Serialization;


namespace HydroNumerics.Core
{
	/// <summary>
	/// The Dimension class contains the dimension for a unit.
    /// </summary>

    public enum DimensionBase : int
    {
        /// <summary>
        /// Base dimension length.
        /// </summary>

        Length = 0,

        /// <summary>
        /// Base dimension mass.
        /// </summary>

        Mass = 1,


        /// <summary>
        /// Base dimension time.
        /// </summary>

        Time = 2,


        /// <summary>
        /// Base dimension electric current.
        /// </summary>

        ElectricCurrent = 3,


        /// <summary>
        /// Base dimension temperature.
        /// </summary>

        Temperature = 4,


        /// <summary>
        /// Base dimension amount of substance.
        /// </summary>

        AmountOfSubstance = 5,


        /// <summary>
        /// Base dimension luminous intensity.
        /// </summary>

        LuminousIntensity = 6,


        /// <summary>
        /// Base dimension currency.
        /// </summary>

        Currency = 7,

        /// <summary>
        /// Total number of base dimensions.
        /// </summary>

        NUM_BASE_DIMENSIONS

    }

	[Serializable]
  [DataContract]
    public class Dimension : System.ComponentModel.INotifyPropertyChanged
	{
        //[System.Xml.Serialization.XmlAttribute()]
        double amountOfSubstance;
        public double AmountOfSubstance
        {
            get { return amountOfSubstance; }
            set 
            { 
                amountOfSubstance = value;
                NotifyPropertyChanged("AmountOfSubstance");
            }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        double currency;
        public double Currency
        {
            get { return currency; }
            set 
            {
                currency = value;
                NotifyPropertyChanged("Currency");
            }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        double electricCurrent;
        public double ElectricCurrent
        {
            get { return electricCurrent; }
            set 
            {
                electricCurrent = value;
                NotifyPropertyChanged("ElectricCurrent");
            }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        double length;
        public double Length
        {
            get { return length; }
            set 
            {
                length = value;
                NotifyPropertyChanged("Length");
            }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        double luminousIntensity;
        public double LuminousIntensity
        {
            get { return luminousIntensity; }
            set 
            {
                luminousIntensity = value;
                NotifyPropertyChanged("LuminousIntensity");
            }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        double mass;
        public double Mass
        {
            get { return mass; }
            set 
            {
                mass = value;
                NotifyPropertyChanged("Mass");
            }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        double temperature;
        public double Temperature
        {
            get { return temperature; }
            set 
            {
                temperature = value;
                NotifyPropertyChanged("Temperature");
            }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        double time;
        public double Time
        {
            get { return time; }
            set 
            {
                time = value;
                NotifyPropertyChanged("Time");
            }
        }

		public Dimension()
		{
            amountOfSubstance = 0;
            currency = 0;
            electricCurrent = 0;
            length = 0;
            luminousIntensity = 0;
            mass = 0;
            temperature = 0;
            time = 0;
		}

        public Dimension(double length, double mass, double time, double electricCurrent, double temperature, double amountOfSubstance, double luminousIntensity, double currency) : this()
        {
            this.length = length;
            this.mass = mass;
            this.time = time;
            this.electricCurrent = electricCurrent;
            this.temperature = temperature;
            this.amountOfSubstance = amountOfSubstance;
            this.luminousIntensity = luminousIntensity;
            this.currency = currency;
        }

        public Dimension(Dimension obj):this()
        {
            this.amountOfSubstance = obj.AmountOfSubstance;
            this.currency = obj.Currency;
            this.electricCurrent = obj.ElectricCurrent;
            this.length = obj.Length;
            this.luminousIntensity = obj.LuminousIntensity;
            this.mass = obj.Mass;
            this.temperature = obj.Temperature;
            this.time = obj.Time;
        }

		/// <summary>
		/// Returns the power of a base quantity
		/// </summary>
		/// <param name="baseQuantity">The base quantity</param>
		/// <returns>The power</returns>
		public double GetPower(DimensionBase baseQuantity)
		{
            double power;

            switch (baseQuantity)
            {
                case DimensionBase.Length:
                    return length;
                case DimensionBase.Mass:
                    return mass;
                case DimensionBase.Time:
                    return time;
                case DimensionBase.ElectricCurrent:
                    return electricCurrent;
                case DimensionBase.Temperature:
                    return temperature;
                case DimensionBase.AmountOfSubstance:
                    return amountOfSubstance;
                case DimensionBase.LuminousIntensity:
                    return luminousIntensity;
                case DimensionBase.Currency:
                    return currency;
                default:
                    throw new Exception("unexpected exception");
                    break;
            }
		}

		/// <summary>
		/// Sets a power for a base quantity
		/// </summary>
		/// <param name="baseQuantity">The base quantity</param>
		/// <param name="power">The power</param>
		public void SetPower(DimensionBase baseQuantity,double power)
		{
            switch (baseQuantity)
            {
                case DimensionBase.Length:
                    length = power;
                    break;
                case DimensionBase.Mass:
                    mass = power;
                    break;
                case DimensionBase.Time:
                    time = power;
                    break;
                case DimensionBase.ElectricCurrent:
                    electricCurrent = power;
                    break;
                case DimensionBase.Temperature:
                    temperature = power;
                    break;
                case DimensionBase.AmountOfSubstance:
                    amountOfSubstance = power;
                    break;
                case DimensionBase.LuminousIntensity:
                    luminousIntensity = power;
                    break;
                case DimensionBase.Currency:
                    currency = power;
                    break;
                default:
                    throw new Exception("Unexpected exception");
            }

		}

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public bool Equals(Dimension obj) 
		{
            if (obj.AmountOfSubstance == this.AmountOfSubstance &&
                obj.Currency == this.currency &&
                obj.electricCurrent == this.electricCurrent &&
                obj.length == this.length &&
                obj.LuminousIntensity == this.LuminousIntensity &&
                obj.Temperature == this.Temperature &&
                obj.Time == this.Time &&
                obj.Mass == this.Mass)
            {
                return true;
            }
            else
            {
                return false;
            }
  		}

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
