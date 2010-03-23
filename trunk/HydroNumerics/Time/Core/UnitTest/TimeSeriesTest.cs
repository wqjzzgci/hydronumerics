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
    public class TimeSeriesTest
    {


        private TestContext testContextInstance;

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


        /// <summary>
        ///A test for Description
        ///</summary>
        [TestMethod()]
        public void DescriptionTest()
        {
            TimeSeries target = new TimeSeries();
            string expected = "MyDescription";
            string actual;
            target.Description = expected;
            actual = target.Description;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Name
        ///</summary>
        [TestMethod()]
        public void NameTest()
        {
            TimeSeries target = new TimeSeries();
            string expected = "MyName";
            string actual;
            target.Name = expected;
            actual = target.Name;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IdTest()
        {
            TimeSeries target = new TimeSeries();
            int expected = 123;
            int actual;
            target.Id = expected;
            actual = target.Id;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetValue
        ///</summary>
        [TestMethod()]
        public void GetValueTest()
        {
            //-- Expected exception when GetValues is invoked on an empty timeseries. --
            TimeSeries timeSeries = new TimeSeries();
            timeSeries.Unit.ConversionFactorToSI = 10;
            timeSeries.Unit.OffSetToSI = 5;

            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }

            // ========================================================================================
            // Getting values for time staps from timestamp based time series (Timestamp to TimeStamp)
            // ========================================================================================
           
            //-- When only one record in time series --
            timeSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 1, 0, 0, 0), 3.0));
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2011, 1, 1, 0, 0, 0)));
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2009, 1, 1, 0, 0, 0)));

            //-- timeseries with two records ---
            timeSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 2, 0, 0, 0), 6.0));
            timeSeries.RelaxationFactor = 1.0;
            Assert.AreEqual(4.5 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0))); //Inbetween
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));  //Hit first time
            Assert.AreEqual(6.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));  // Hit last time
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0)));  // one day before 
            Assert.AreEqual(6.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));  // one day after 
            timeSeries.RelaxationFactor = 0.0;
            Assert.AreEqual(4.5 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0))); //Inbetween
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));  // Hit first time
            Assert.AreEqual(6.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));  // Hit last time
            Assert.AreEqual(0.0 * 10 + 5, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0)));  // one day before 
            Assert.AreEqual(9.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));  // one day after 
            timeSeries.RelaxationFactor = 0.5;
            Assert.AreEqual(4.5 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0))); //Inbetween
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));  // Hit first time
            Assert.AreEqual(6.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));  // Hit last time
            Assert.AreEqual(1.5 * 10 + 5, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0)));  // one day before 
            Assert.AreEqual(7.5 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));  // one day after
 
            // -- timeseries with 4 records ---
            timeSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 3, 0, 0, 0), 6.0));
            timeSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 4, 0, 0, 0), 4.0));
            timeSeries.RelaxationFactor = 0.0;
            Assert.AreEqual(4.5 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0))); //Inbetween
            Assert.AreEqual(6.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 12, 0, 0))); //Inbetween
            Assert.AreEqual(5.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 3, 12, 0, 0))); //Inbetween
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));  //Hit first time
            Assert.AreEqual(6.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));  // Hit Second
            Assert.AreEqual(6.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));  // Hit third time
            Assert.AreEqual(4.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 4, 0, 0, 0)));  // Hit last time
            Assert.AreEqual(0.0 * 10 + 5, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0)));  // one day before 
            Assert.AreEqual(2.0 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 5, 0, 0, 0)));  // one day after 

            // ========================================================================================
            // With unit in the argument list(Timestamp to TimeStamp)
            // ========================================================================================

            Assert.AreEqual(4.5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0),timeSeries.Unit)); //Inbetween

            
            // ========================================================================================
            // Getting values for timespans from timestamp based time series (Timestamp to TimeSpan)
            // ========================================================================================

            timeSeries.RelaxationFactor = 0.0;
            
            // v--------------------------------------V
            // |------------|------------|------------|
            // 3            6            6            4 
            Assert.AreEqual((15.5 / 3.0) * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 4, 0, 0, 0))); //interval same as the full timeseries

            //        v-----------v
            // |------------|------------|------------|
            Assert.AreEqual(5.625 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0), new DateTime(2010, 1, 2, 12, 0, 0))); 

            //        v-------------------------v
            // |------------|------------|------------|
            Assert.AreEqual((11.375 / 2.0) * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0), new DateTime(2010, 1, 3, 12, 0, 0))); 

            //     v----v
            // |------------|------------|------------|
            Assert.AreEqual(4.5 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 6, 0, 0), new DateTime(2010, 1, 1, 18, 0, 0)));

            // v--------------------------------------------------------------v
            // ------------|------------|------------|------------|------------
            Assert.AreEqual(4.0 * 10 + 5, timeSeries.GetValue(new DateTime(2009, 12, 31, 0, 0, 0), new DateTime(2010, 1, 5, 0, 0, 0)));  //Extrapolating outside timeseries

            // ========================================================================================
            // With unit in the argument list(Timestamp to TimeSpan)
            // ========================================================================================
            Assert.AreEqual(4.5, timeSeries.GetValue(new DateTime(2010, 1, 1, 6, 0, 0), new DateTime(2010, 1, 1, 18, 0, 0),timeSeries.Unit));
  
            // ========================================================================================
            // Getting values for timestamps from timespan based time series (Timestamp to TimeStamp)
            // ========================================================================================

            timeSeries.RelaxationFactor = 0.0;
            timeSeries.TimeSeriesType = TimeSeriesType.TimeSpanBased;
            timeSeries.TimeValues[2].Value = 4.0;
            timeSeries.TimeValues[3].Value = 100;

            // v
            //     |------------|------------|------------|
            //            3            6            4           
            Assert.AreEqual(1.5 * 10 + 5, timeSeries.GetValue(new DateTime(2009, 12, 31, 12, 0, 0))); 

            // v
            // |------------|------------|------------|
            Assert.AreEqual(3 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0))); 
            
            //       v
            // |------------|------------|------------|
            Assert.AreEqual(3 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0)));

            //              v
            // |------------|------------|------------|
            Assert.AreEqual(6 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));

            //                    v
            // |------------|------------|------------|
            Assert.AreEqual(6 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 12, 0, 0)));

            //                           v
            // |------------|------------|------------|
            Assert.AreEqual(4 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));

            //                                 v
            // |------------|------------|------------|
            Assert.AreEqual(4 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 3, 12, 0, 0)));

            //                                        v
            // |------------|------------|------------|
            Assert.AreEqual(4 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 4, 0, 0, 0)));

            //                                             v
            // |------------|------------|------------|
            Assert.AreEqual(3 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 4, 12, 0, 0)));

            timeSeries.RelaxationFactor = 1.0;

            // v
            //     |------------|------------|------------|
            //            3            6            4           
            Assert.AreEqual(3.0 * 10 + 5, timeSeries.GetValue(new DateTime(2009, 12, 31, 12, 0, 0)));

            //                    v
            // |------------|------------|------------|
            Assert.AreEqual(6 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 12, 0, 0)));

            //                                             v
            // |------------|------------|------------|
            Assert.AreEqual(4 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 4, 12, 0, 0)));

            timeSeries.TimeValues.RemoveAt(timeSeries.TimeValues.Count - 1);
            timeSeries.TimeValues.RemoveAt(0);
            timeSeries.RelaxationFactor = 1.0;

            //                               v
            //              |------------|
            //                     6
            Assert.AreEqual(6 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 3, 12, 0, 0)));

            //         v
            //              |------------|
            //                     6
            Assert.AreEqual(6 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0)));

            //                     v
            //              |------------|
            //                     6
            Assert.AreEqual(6 * 10 + 5, timeSeries.GetValue(new DateTime(2010, 1, 2, 12, 0, 0)));

          

            //-- Expected exception when GetValues is invoked for timespan based timeseries with only one timestamp --
            timeSeries.TimeValues.RemoveAt(0);
            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }


          

        
 
        }
    }
}
