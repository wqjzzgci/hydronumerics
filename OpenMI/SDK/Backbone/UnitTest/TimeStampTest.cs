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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMI.Standard;
using HydroNumerics.OpenMI.Sdk.Backbone;

namespace HydroNumerics.OpenMI.Sdk.Backbone.UnitTest
{
    [TestClass()]
	public class TimeStampTest
	{
		TimeStamp timeStamp;
        [TestInitialize()]
		public void Init()
		{
			timeStamp = new TimeStamp(12345.3);
		}

		[TestMethod()]
		public void Constructor()
		{
			TimeStamp timeStamp2 = new TimeStamp(timeStamp);
			Assert.AreEqual(timeStamp,timeStamp2);

            TimeStamp timeStamp3 = new TimeStamp(new DateTime(1858, 11, 17)); // the zero Julian time
            Assert.AreEqual(0.0, timeStamp3.ModifiedJulianDay);
		}
		[TestMethod()]
		public void ModifiedJulianDay()
		{
			Assert.AreEqual(12345.3,timeStamp.ModifiedJulianDay);
			timeStamp.ModifiedJulianDay = 54321.7;
			Assert.AreEqual(54321.7,timeStamp.ModifiedJulianDay);
		}
		[TestMethod()]
		public void Equals()
		{
			TimeStamp timeStamp1 = new TimeStamp(12345.3);
			Assert.IsTrue(timeStamp.Equals(timeStamp1));
			timeStamp1.ModifiedJulianDay = 34.0;
			Assert.IsFalse(timeStamp.Equals(timeStamp1));

			Assert.IsFalse(timeStamp.Equals(null));
			Assert.IsFalse(timeStamp.Equals("string"));
		}

		[TestMethod()]
		public void CompareTo()
		{
			TimeStamp timeStamp1 = new TimeStamp(12345.3);
			Assert.AreEqual(0.0,timeStamp.CompareTo(timeStamp1));
			timeStamp1.ModifiedJulianDay = 10000;
			Assert.IsTrue(timeStamp.CompareTo(timeStamp1)>0.0);
			Assert.IsTrue(timeStamp1.CompareTo(timeStamp)<0.0);
		}

		[TestMethod()]
		public void String()
		{
			Assert.AreEqual("12345.3",timeStamp.ToString());
		}

        [TestMethod()]
        public void ToLongString()
        {
            Assert.AreEqual("(ITimeStamp) 4. september 1892 07:12:00  (JulianTime: 12345.3)", timeStamp.ToLongString());
        }

        [TestMethod()]
        public void ToDateTime()
        {
            DateTime testDate = new DateTime(2008, 12, 31);
            TimeStamp timeStamp = new TimeStamp(testDate);
            Assert.AreEqual(testDate, timeStamp.ToDateTime());
        }
	}
}
