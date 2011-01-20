#region Copyright
/*
* Copyright (c) HydroInform ApS & Jacob Gudbjerg
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the HydroInform ApS & Jacob Gudbjerg nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "HydroInform ApS & Jacob Gudbjerg" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "HydroInform ApS & Jacob Gudbjerg" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Backbone
{
    public class TimeSet : ITimeSet, IEnumerable<ITime>
    {       
        #region ITimeSet Members

        IList<ITime> times = new List<ITime>();
        private bool hasDurations;
        private ITime timeHorizon = new Time();
        private double offsetFromUtcInHours;

        public IList<ITime> Times
        {
            get { return times; }
            set { times = value; }
        }

        public bool HasDurations
        {
            get { return hasDurations; }
            set { hasDurations = value; }
        }

        public double OffsetFromUtcInHours
        {
            get { return offsetFromUtcInHours; }
            set { offsetFromUtcInHours = value; }
        }

        public ITime TimeHorizon
        {
            get { return timeHorizon; }
            set { timeHorizon = value; }
        }

        public void Add(ITime time)
        {
            times.Add(time);
        }

        public void RemoveRange(int index, int count)
        {
            List<ITime> timesList = times as List<ITime>;
            if (timesList != null)
            {
                timesList.RemoveRange(index, count);
                return;
            }
            for (int i = index+count-1; i >= index; i--)
            {
                times.RemoveAt(i);                
            }
        }

        public ITime this[int timeIndex]
        {
            get { return times[timeIndex]; }
            set { times[timeIndex] = value; }
        }

        public int Count { get { return (times.Count); } }

        #endregion

        #region Convenience Functions

		public void SetTimeHorizonFromTimes()
        {
            TimeHorizon = new Time(Time.Start(Times[0]), Time.End(Times[Times.Count - 1]));
        }

        #endregion

        public IEnumerator<ITime> GetEnumerator()
        {
            return (times.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
