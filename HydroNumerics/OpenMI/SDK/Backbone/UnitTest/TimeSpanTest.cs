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
	public class TimeSpanTest
	{
		TimeSpan timeSpan;

        [TestInitialize()]
		public void Init()
		{	
			timeSpan = new TimeSpan(new TimeStamp(1.0),new TimeStamp(2.0));
		}

		[TestMethod()]
		public void Constructor ()
		{
			TimeSpan timeSpan2 = new TimeSpan(timeSpan);
			Assert.AreEqual(timeSpan,timeSpan2);
		}

		[TestMethod()]
		public void Start()
		{
			Assert.AreEqual(new TimeStamp(1.0),timeSpan.Start);
			timeSpan.Start = new TimeStamp(2.0);
			Assert.AreEqual(new TimeStamp(2.0),timeSpan.Start);
		}

		[TestMethod()]
		public void End()
		{
			Assert.AreEqual(new TimeStamp(2.0),timeSpan.End);
			timeSpan.End = new TimeStamp(3.0);
			Assert.AreEqual(new TimeStamp(3.0),timeSpan.End);
		}

		[TestMethod()]
		public void Equals()
		{
			Assert.IsTrue(timeSpan.Equals(new TimeSpan(new TimeStamp(1.0),new TimeStamp(2.0))));
			Assert.IsFalse(timeSpan.Equals(null));
			Assert.IsFalse(timeSpan.Equals("string"));
		}

		[TestMethod()]
		public void EqualsStart()
		{
			Assert.IsFalse(timeSpan.Equals(new TimeSpan(new TimeStamp(1.1),new TimeStamp(2.0))));
		}

		[TestMethod()]
		public void EqualsEnd()
		{
			Assert.IsFalse(timeSpan.Equals(new TimeSpan(new TimeStamp(1.0),new TimeStamp(2.1))));
		}

        [TestMethod()]
        public void ToLongString()
        {
            string str = timeSpan.ToLongString();
            Assert.AreEqual("(ITimeSpan) From: (ITimeStamp) 18. november 1858 00:00:00  (JulianTime: 1) To: (ITimeStamp) 19. november 1858 00:00:00  (JulianTime: 2)", str);
        }

	}
}
