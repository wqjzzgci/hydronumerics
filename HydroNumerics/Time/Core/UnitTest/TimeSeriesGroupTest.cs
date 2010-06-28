﻿#region Copyright
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
using System.IO;
using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.Core;
namespace HydroNumerics.Time.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TimeSeriesGroupTest and is intended
    ///to contain all TimeSeriesGroupTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimeSeriesGroupTest
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
        public void PropertyChangedEvent()
        {
            TimeSeriesGroup timeSeriesGroup = new TimeSeriesGroup();
            timeSeriesGroup.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(timeSeriesGroup_PropertyChanged);
            eventWasRaised = false;
            timeSeriesGroup.Items.Add(new TimestampSeries());
            Assert.IsTrue(eventWasRaised); eventWasRaised = false;
            ((TimestampSeries)timeSeriesGroup.Items[0]).AppendValue(4.3);

            Assert.IsTrue(eventWasRaised); eventWasRaised = false;
            ((TimestampSeries)timeSeriesGroup.Items[0]).Items[0].Value = 2.1;
            Assert.IsTrue(eventWasRaised); eventWasRaised = false;
            ((TimestampSeries)timeSeriesGroup.Items[0]).Items[0].Time = new System.DateTime(2010, 1, 1, 0, 0, 0);
            Assert.IsTrue(eventWasRaised); eventWasRaised = false;
            //TODO: more testing needed
        }

        void timeSeriesGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            eventWasRaised = true;
        }

        [TestMethod()]
        public void Name()
        {
            TimeSeriesGroup timeSeriesGroup = new TimeSeriesGroup();
            timeSeriesGroup.Name = "test name";
            Assert.AreEqual("test name", timeSeriesGroup.Name);
        }

        [TestMethod()]
        public void Current()
        {
            TimeSeriesGroup timeSeriesGroup = new TimeSeriesGroup();
            timeSeriesGroup.Items.Add(new TimestampSeries());
            timeSeriesGroup.Items.Add(new TimestampSeries());
            timeSeriesGroup.Items.Add(new TimestampSeries());
            Assert.AreEqual(0, timeSeriesGroup.Current);
            timeSeriesGroup.Current = 2;
            Assert.AreEqual(2, timeSeriesGroup.Current);
            timeSeriesGroup.Current = 3;
            Assert.AreEqual(2, timeSeriesGroup.Current);
            timeSeriesGroup.Current = -1;
            Assert.AreEqual(0, timeSeriesGroup.Current);
        }

        [TestMethod()]
        public void Save()
        {
            TimestampSeries ts = new TimestampSeries("TsName", new System.DateTime(2010, 1, 1), 10, 1, TimestepUnit.Days, 4.3);
            TimeSeriesGroup tsg = new TimeSeriesGroup();
            tsg.Items.Add(ts);
            tsg.Save(@"c:\tmp\tsg.xlm");
        }

       //[TestMethod()]
       // public void Example()
       // {
       //     // Load timeseries file and assign first timeseries to timeseries object --
       //     TimeSeriesGroup timeSeriesGroup = TimeSeriesGroupFactory.Create(@"c:\tmp\flow.xts");
       //     TimestampSeries timeSeries = new TimestampSeries();
       //     timeSeriesGroup.TimeSeriesList.Add((BaseTimeSeries)timeSeries);

       //     // change the unit
       //     timeSeries.Name = "Flow";
       //     timeSeries.Unit.ID = "m3/sec";
       //     timeSeries.Unit.ConversionFactorToSI = 1.0;

       //     // Add more data. The values are automatically inserted at the correct location in the timeseries.
       //     timeSeries.AddTimeValueRecord(new TimestampValue(new System.DateTime(2010, 1, 1, 12, 0, 0), 0.2));
       //     timeSeries.AddTimeValueRecord(new TimestampValue(new System.DateTime(2010, 1, 2, 12, 0, 0), 0.3));

       //     // Append data to end of the timeseries file. The corresponding time is automatically calculated
       //     // by encrementing the time for the last record by the timeperiod between the last two records.
       //     timeSeries.AppendValue(0.23);
       //     timeSeries.AppendValue(0.3);

       //     // Get value for a specific time. In this case the value is interpolated between the nearest 
       //     // records in the time series. The returned value is in SI units regardless of which unit the
       //     // values inside the timeseries are using. 
       //     double x1 = timeSeries.GetValue(new System.DateTime(2010,1,1,18,0,0));

       //     // Get value for a specic time period. The returned value corresponds to the mean value for the
       //     // specified time period. The value is in SI units regardless of which unit the values inside the
       //     // timeseries are using.
       //     System.DateTime fromTime = new System.DateTime(2010, 1, 1, 12, 0, 0);
       //     System.DateTime toTime = new System.DateTime(2010, 1, 4, 0, 0, 0);
       //     double x2 = timeSeries.GetValue(fromTime, toTime);

       //     // Get a value that is converted to a specific unit.
       //     Unit myUnit = new Unit("l/sec",0.001,0.0); 
       //     double x3 = timeSeries.GetValue(new System.DateTime(2010,1,1,18,0,0),myUnit); 

       //     //Save the timeseries to a XML file.
       //     timeSeriesGroup.Save(@"c:\tmp\flow01.xts");
       // }
    }
}
