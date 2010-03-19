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
using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Time.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TimeValueTest and is intended
    ///to contain all TimeValueTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimeValueTest
    {
        private TestContext testContextInstance;
        private bool eventWasRaised = false;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void ValueTest()
        {
            TimeValue timeValue = new TimeValue();
            timeValue.Value = 4.3;
            Assert.AreEqual(4.3, timeValue.Value);
            
        }

        [TestMethod()]
        public void TimeTest()
        {
            TimeValue timeValue = new TimeValue(); 
            DateTime dateTime = new DateTime(2010,12,03,20,34,23);
            timeValue.Time = dateTime;
            Assert.AreEqual(dateTime, timeValue.Time);
        }

        [TestMethod()]
        public void PropertyChangedEvent()
        {
            TimeValue timeValue = new TimeValue(new DateTime(2010, 12, 03, 20, 34, 23), 4.3);
            timeValue.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(timeValue_PropertyChanged);
            eventWasRaised = false;
            timeValue.Value = 22.2;
            Assert.IsTrue(eventWasRaised);
            eventWasRaised = false;
            timeValue.Time = new DateTime(2010, 01, 01, 12, 12, 12);
            Assert.IsTrue(eventWasRaised);
            

        }

        void timeValue_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            eventWasRaised = true;
        }

        [TestMethod()]
        public void TimeValueConstructorTest()
        {
            // Default constructor
            TimeValue timeValue = new TimeValue();
            DateTime dateTime = new DateTime(2010, 01, 01, 17, 18, 35);
            timeValue.Time = dateTime;
            timeValue.Value = 5.4;
            Assert.AreEqual(5.4, timeValue.Value);
            Assert.AreEqual(2010, timeValue.Time.Year);
            Assert.AreEqual(35, timeValue.Time.Second);

            // Second constructor
            timeValue = new TimeValue(dateTime, 5.4);
            Assert.AreEqual(5.4, timeValue.Value);
            Assert.AreEqual(2010, timeValue.Time.Year);
            Assert.AreEqual(35, timeValue.Time.Second);
        }
  
    }
}
