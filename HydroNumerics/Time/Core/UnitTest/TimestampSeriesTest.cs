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
    ///This is a test class for TimeSeriesTest and is intended
    ///to contain all TimeSeriesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimestampSeriesTest
    {


        private TestContext testContextInstance;
        private bool propertyChanged = false;

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
            TimestampSeries timeSeries = new TimestampSeries();
            timeSeries.Unit = new HydroNumerics.Core.Unit("liter/sec", 0.001, 0.0, "liters pr. second");
            timeSeries.AddValue(new DateTime(2010, 1, 1, 0, 0, 0), 5.0);
            timeSeries.AddValue(new DateTime(2010, 1, 2, 0, 0, 0), 5.0);
            timeSeries.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(timeSeries_PropertyChanged);
            propertyChanged = false;
            timeSeries.Name = "something else";
            Assert.IsTrue(propertyChanged);
            //TODO: implement test like above for the remaining properties
            propertyChanged = false;
            timeSeries.Items[0].Value = 7.3;
            Assert.IsTrue(propertyChanged);
        }

        [TestMethod()]
        public void GetValue01()  //GetValue(DateTime time)
        {
            TimestampSeries timeSeries = new TimestampSeries();

            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0));
            }
            catch (Exception ex)
            {
                //-- Expected exception when GetValues is invoked on an empty timeseries. --
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }

            //-- When only one record in time series --
            timeSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 0, 0), 3.0));
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2011, 1, 1, 0, 0, 0)));
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2009, 1, 1, 0, 0, 0)));

            //-- timeseries with two records ---
            timeSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 2, 0, 0, 0), 6.0));
            timeSeries.RelaxationFactor = 1.0;
            Assert.AreEqual(4.5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0))); //Inbetween
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));  //Hit first time
            Assert.AreEqual(6.0, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));  // Hit last time
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0)));  // one day before 
            Assert.AreEqual(6.0, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));  // one day after 
            timeSeries.RelaxationFactor = 0.0;
            Assert.AreEqual(4.5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0))); //Inbetween
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));  // Hit first time
            Assert.AreEqual(6.0, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));  // Hit last time
            Assert.AreEqual(0.0, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0)));  // one day before 
            Assert.AreEqual(9.0, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));  // one day after 
            timeSeries.RelaxationFactor = 0.5;
            Assert.AreEqual(4.5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0))); //Inbetween
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));  // Hit first time
            Assert.AreEqual(6.0, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));  // Hit last time
            Assert.AreEqual(1.5, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0)));  // one day before 
            Assert.AreEqual(7.5, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));  // one day after

            // -- timeseries with 4 records ---
            timeSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 3, 0, 0, 0), 6.0));
            timeSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 4, 0, 0, 0), 4.0));
            timeSeries.RelaxationFactor = 0.0;
            Assert.AreEqual(4.5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0))); //Inbetween
            Assert.AreEqual(6.0, timeSeries.GetValue(new DateTime(2010, 1, 2, 12, 0, 0))); //Inbetween
            Assert.AreEqual(5.0, timeSeries.GetValue(new DateTime(2010, 1, 3, 12, 0, 0))); //Inbetween
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));  //Hit first time
            Assert.AreEqual(6.0, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));  // Hit Second
            Assert.AreEqual(6.0, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));  // Hit third time
            Assert.AreEqual(4.0, timeSeries.GetValue(new DateTime(2010, 1, 4, 0, 0, 0)));  // Hit last time
            Assert.AreEqual(0.0, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0)));  // one day before 
            Assert.AreEqual(2.0, timeSeries.GetValue(new DateTime(2010, 1, 5, 0, 0, 0)));  // one day after 
        }

        [TestMethod()]
        public void GetValue02() // GetValue(DateTime fromTime, DateTime toTime)
        {
            //-- Expected exception when GetValues is invoked on an empty timeseries. --
            TimestampSeries timeSeries = new TimestampSeries();
           
            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }

            timeSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 0, 0), 3.0));

            //Testing when only one record in timeseries
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2010, 11, 1, 0, 0, 0), new DateTime(2010, 12, 1, 0, 0, 0)));

            timeSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 2, 0, 0, 0), 6.0));
            timeSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 3, 0, 0, 0), 6.0));
            timeSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 4, 0, 0, 0), 4.0));

            timeSeries.RelaxationFactor = 1.0;

            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 1, 0, 0, 0));

            }
            catch (Exception ex)
            {
                //Testing invalid argument for timespan
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }

            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 1, 0, 0, 0));

            }
            catch (Exception ex)
            {
                //Testing invalid argument for timespan
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }

            timeSeries.RelaxationFactor = 0.0;

            // v--------------------------------------V
            // |------------|------------|------------|
            // 3            6            6            4 
            Assert.AreEqual(15.5 / 3.0, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 4, 0, 0, 0))); //interval same as the full timeseries

            //        v-----------v
            // |------------|------------|------------|
            Assert.AreEqual(5.625, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0), new DateTime(2010, 1, 2, 12, 0, 0)));

            //        v-------------------------v
            // |------------|------------|------------|
            Assert.AreEqual(11.375 / 2.0, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0), new DateTime(2010, 1, 3, 12, 0, 0)));

            //     v----v
            // |------------|------------|------------|
            Assert.AreEqual(4.5, timeSeries.GetValue(new DateTime(2010, 1, 1, 6, 0, 0), new DateTime(2010, 1, 1, 18, 0, 0)));

            // v--------------------------------------------------------------v
            // ------------|------------|------------|------------|------------
            Assert.AreEqual(4.0, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0), new DateTime(2010, 1, 5, 0, 0, 0)));  //Extrapolating outside timeseries
        }

        [TestMethod]
        public void GetSiValue01()  //GetSiValues(DateTime time)
        {
            TimestampSeries ts = new TimestampSeries();
            ts.Unit = new HydroNumerics.Core.Unit("cm/sec", 0.01, 0.0);
            ts.Items.Add(new TimestampValue(new DateTime(2010,1,1,0,0,0), 3));
            ts.Items.Add(new TimestampValue(new DateTime(2010,1,2,0,0,0), 3));
            Assert.AreEqual(0.03, ts.GetSiValue(new DateTime(2010, 1, 1, 0, 0, 0)));
        }

        [TestMethod]
        public void GetSiValue02()  //GetSiValues(DateTime startTimem, DateTime endTime)
        {
            TimestampSeries ts = new TimestampSeries();
            ts.Unit = new HydroNumerics.Core.Unit("cm/sec", 0.01, 0.0);
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 0, 0), 3));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 2, 0, 0, 0), 3));
            Assert.AreEqual(0.03, ts.GetSiValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010,1,2,0,0,0)));
        }

        [TestMethod]
        public void AddValue()
        {
            TimestampSeries ts = new TimestampSeries();
            ts.AddValue(new DateTime(2010, 1, 2), 1);
            ts.AddValue(new DateTime(2010, 1, 1), 0);
            ts.AddValue(new DateTime(2010, 1, 3), 2);
            ts.AddValue(new DateTime(2010, 1, 4), 3);

            Assert.AreEqual(4, ts.Items.Count);
            Assert.AreEqual(0, ts.Items[0].Value);
            Assert.AreEqual(1, ts.Items[1].Value);
            Assert.AreEqual(2, ts.Items[2].Value);
            Assert.AreEqual(3, ts.Items[3].Value);

            ts.AddValue(new DateTime(2010, 1, 1, 12, 0, 0), 0.5);  //testing isert

            Assert.AreEqual(5, ts.Items.Count);
            Assert.AreEqual(0, ts.Items[0].Value);
            Assert.AreEqual(0.5, ts.Items[1].Value);
            Assert.AreEqual(1, ts.Items[2].Value);
            Assert.AreEqual(2, ts.Items[3].Value);
            Assert.AreEqual(3, ts.Items[4].Value);

            ts.AddValue(new DateTime(2010, 1, 3, 0, 0, 0), 77);  // testing overwrite

            Assert.AreEqual(5, ts.Items.Count);
            Assert.AreEqual(0, ts.Items[0].Value);
            Assert.AreEqual(0.5, ts.Items[1].Value);
            Assert.AreEqual(1, ts.Items[2].Value);
            Assert.AreEqual(77, ts.Items[3].Value);
            Assert.AreEqual(3, ts.Items[4].Value);
        }

        [TestMethod]
        public void AddSiValue()
        {
            TimestampSeries ts = new TimestampSeries();
            ts.Unit = new HydroNumerics.Core.Unit("cm/sec", 0.01, 0.0);
            ts.AddSiValue(new DateTime(2010, 1, 1), 2);
            Assert.AreEqual(200, ts.Items[0].Value);

        }

        [TestMethod]
        public void AppendValue()
        {
            TimestampSeries ts = new TimestampSeries();
            ts.AppendValue(1);
            ts.AppendValue(2);
            Assert.AreEqual(new DateTime(2020, 1, 1), ts.Items[0].Time);
            Assert.AreEqual(1, ts.Items[0].Value);
            Assert.AreEqual(new DateTime(2020, 1, 2), ts.Items[1].Time);
            Assert.AreEqual(2, ts.Items[1].Value);

            ts = new TimestampSeries(); //testing when timestep is two seconds
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 0, 1), 10));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 0, 3), 20));
            ts.AppendValue(30);
            Assert.AreEqual(new DateTime(2010, 1, 1, 0, 0, 5), ts.Items[2].Time);
            
            ts = new TimestampSeries(); //testing when timestep is two minutes
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 1, 0), 10));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 3,0 ), 20));
            ts.AppendValue(30);
            Assert.AreEqual(new DateTime(2010, 1, 1, 0, 5, 0), ts.Items[2].Time);
            
            ts = new TimestampSeries(); //testing when timestep is two days
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 1), 10));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 3), 20));
            ts.AppendValue(30);
            Assert.AreEqual(new DateTime(2010, 1, 5), ts.Items[2].Time);

            ts = new TimestampSeries(); //testing when timestep is two days, when passing end of month
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 31), 10));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 2, 2), 20));
            ts.AppendValue(30);
            Assert.AreEqual(new DateTime(2010, 2, 4), ts.Items[2].Time);

            ts = new TimestampSeries(); //testing when timestep two months
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 0, 0), 10));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 3, 1, 0, 0, 0), 20));
            ts.AppendValue(30);
            Assert.AreEqual(new DateTime(2010, 5, 1, 0, 0, 0), ts.Items[2].Time);

            ts = new TimestampSeries(); //testing when timestep two months, when passing january 1st
            ts.Items.Add(new TimestampValue(new DateTime(2010, 12, 1, 0, 0, 0), 10));
            ts.Items.Add(new TimestampValue(new DateTime(2011, 2, 1, 0, 0, 0), 20));
            ts.AppendValue(30);
            Assert.AreEqual(new DateTime(2011, 4, 1, 0, 0, 0), ts.Items[2].Time);

            ts = new TimestampSeries(); //testing when timestep is two years
            ts.Items.Add(new TimestampValue(new DateTime(2010, 6, 17), 10));
            ts.Items.Add(new TimestampValue(new DateTime(2011, 6, 17), 20));
            ts.AppendValue(30);
            Assert.AreEqual(new DateTime(2012, 6, 17), ts.Items[2].Time);

        }
    
        [TestMethod()]
        public void ConvertUnit()
        {
            TimestampSeries timestampSeries = new TimestampSeries();
            timestampSeries.Unit = new HydroNumerics.Core.Unit("cm pr second", 0.01, 0.0);
            timestampSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 0, 0), 7));
            timestampSeries.Items.Add(new TimestampValue(new DateTime(2010, 1, 2, 0, 0, 0), 9));
            HydroNumerics.Core.Unit newUnit = new HydroNumerics.Core.Unit("mm pr sec", 0.001, 0.0);
            timestampSeries.ConvertUnit(newUnit);
            Assert.AreEqual(70, timestampSeries.Items[0].Value);
            Assert.AreEqual(90, timestampSeries.Items[1].Value);
            Assert.IsTrue(timestampSeries.Unit.Equals(newUnit));
        }

        [TestMethod]
        public void RemoveAfter()
        {
            TimestampSeries ts = new TimestampSeries();
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 1, 0, 0, 0), 1));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 2, 0, 0, 0), 2));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 3, 0, 0, 0), 3));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 4, 0, 0, 0), 4));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 5, 0, 0, 0), 5));
            ts.Items.Add(new TimestampValue(new DateTime(2010, 1, 6, 0, 0, 0), 6));
            ts.RemoveAfter(new DateTime(2010, 1, 3, 0, 0, 0));
            Assert.AreEqual(3, ts.Items.Count);
            ts.RemoveAfter(new DateTime(2010, 1, 8, 0, 0, 0));
            Assert.AreEqual(3, ts.Items.Count);
            ts.RemoveAfter(new DateTime(2009, 1, 1, 0, 0, 0));
            Assert.AreEqual(0, ts.Items.Count);
            ts.RemoveAfter(new DateTime(2010, 3, 1, 0, 0, 0));
            Assert.AreEqual(0, ts.Items.Count);
        }

        [TestMethod]
        public void Save()
        {
            TimestampSeries ts = new TimestampSeries("TSName", new DateTime(2010, 1, 1), 10, 1, TimestepUnit.Days, 5.5);
            ts.Save("ts.xlm");
        }

        void timeSeries_DataChanged(object sender, string info)
        {
            throw new NotImplementedException();
        }



        void timeSeries_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            propertyChanged = true;
        }


   
    }
}
