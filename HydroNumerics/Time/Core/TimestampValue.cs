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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

namespace HydroNumerics.Time.Core
{
    [Serializable]
  [DataContract]
    public class TimestampValue : System.ComponentModel.INotifyPropertyChanged
    {

        public TimestampValue()
        {
            val = 0;
            time = new DateTime(2020, 1, 1);
        }

        public TimestampValue(DateTime time, double val): this()
        {
            this.time = time;
            this.Value = val;
        }

        public TimestampValue(TimestampValue obj):this()
        {
            val = obj.Value;
            time = obj.Time;
        }

        private DateTime time;

        [XmlAttribute]
        [DataMember]
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
        [DataMember]
        public double Value
        {
            get { return val; }
            set 
            {
                val = value;
                NotifyPropertyChanged("Value");
            }
        }

        public override string ToString()
        {
          return "Time: " + Time.ToShortDateString() + " and value: " + val;
        }

        public override bool Equals(Object obj)
        {
            bool equals = true;
            if (obj == null || GetType() != obj.GetType()) return false;
            if (this.Time != ((TimestampValue)obj).Time) equals = false;
            if (this.Value != ((TimestampValue)obj).Value) equals = false;
            
            return equals;
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
