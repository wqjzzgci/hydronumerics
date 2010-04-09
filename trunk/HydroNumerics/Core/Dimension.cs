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
	public class Dimension
	{
        //[System.Xml.Serialization.XmlAttribute()]
        public double AmountOfSubstance
        {
            get { return GetPower(DimensionBase.AmountOfSubstance); }
            set { SetPower(DimensionBase.AmountOfSubstance, value); }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        public double Currency
        {
            get { return GetPower(DimensionBase.Currency); }
            set { SetPower(DimensionBase.Currency, value); }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        public double ElectricCurrent
        {
            get { return GetPower(DimensionBase.ElectricCurrent); }
            set { SetPower(DimensionBase.ElectricCurrent, value); }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        public double Length
        {
            get { return GetPower(DimensionBase.Length); }
            set { SetPower(DimensionBase.Length, value); }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        public double LuminousIntensity
        {
            get { return GetPower(DimensionBase.LuminousIntensity); }
            set { SetPower(DimensionBase.LuminousIntensity, value); }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        public double Mass
        {
            get { return GetPower(DimensionBase.Mass); }
            set { SetPower(DimensionBase.Mass, value); }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        public double Temperature
        {
            get { return GetPower(DimensionBase.Temperature); }
            set { SetPower(DimensionBase.Temperature, value); }
        }

        //[System.Xml.Serialization.XmlAttribute()]
        public double Time
        {
            get { return GetPower(DimensionBase.Time); }
            set { SetPower(DimensionBase.Time, value); }
        }



        [DataMember]
        double[] _count;


		public Dimension()
		{
			_count = new double[ (int) DimensionBase.NUM_BASE_DIMENSIONS];
		}

        public Dimension(double length, double mass, double time, double electricCurrent, double temperature, double amountOfSubstance, double luminousIntensity, double currency) : this()
        {
            this.Length = length;
            this.Mass = mass;
            this.Time = time;
            this.ElectricCurrent = electricCurrent;
            this.Temperature = temperature;
            this.AmountOfSubstance = amountOfSubstance;
            this.LuminousIntensity = luminousIntensity;
            this.Currency = currency;
        }

        public Dimension(Dimension obj):this()
        {
            this.AmountOfSubstance = obj.AmountOfSubstance;
            this.Currency = obj.Currency;
            this.ElectricCurrent = obj.ElectricCurrent;
            this.Length = obj.Length;
            this.LuminousIntensity = obj.LuminousIntensity;
            this.Mass = obj.Mass;
            this.Temperature = obj.Temperature;
            this.Time = obj.Time;
        }

		/// <summary>
		/// Returns the power of a base quantity
		/// </summary>
		/// <param name="baseQuantity">The base quantity</param>
		/// <returns>The power</returns>
		public double GetPower(DimensionBase baseQuantity)
		{
			return _count[(int) baseQuantity];
		}

		/// <summary>
		/// Sets a power for a base quantity
		/// </summary>
		/// <param name="baseQuantity">The base quantity</param>
		/// <param name="power">The power</param>
		public void SetPower(DimensionBase baseQuantity,double power)
		{
			_count[(int) baseQuantity] = power;
		}

		///<summary>
		/// Check if the current instance equals another instance of this class.
		///</summary>
		///<param name="obj">The instance to compare the current instance with.</param>
		///<returns><code>true</code> if the instances are the same instance or have the same content.</returns>
		public bool Equals(Dimension obj) 
		{
			if (obj == null || GetType() != obj.GetType()) 
			 return false;
			Dimension d = (Dimension) obj;
			for (int i=0;i<(int)DimensionBase.NUM_BASE_DIMENSIONS;i++) 
			{
				if (_count[i]!=d._count[i])
					return false;
			}
			return true;
		}
	}
}
