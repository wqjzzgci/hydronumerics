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
        ///A test for ID
        ///</summary>
        [TestMethod()]
        public void IDTest()
        {
            TimeSeries target = new TimeSeries();
            string expected = "MyID";
            string actual;
            target.ID = expected;
            actual = target.ID;
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
            Assert.AreEqual(3.0, timeSeries.GetValue(2011, 1, 1, 0, 0, 0));
            Assert.AreEqual(3.0, timeSeries.GetValue(2010, 1, 1, 0, 0, 0));
            Assert.AreEqual(3.0, timeSeries.GetValue(2009, 1, 1, 0, 0, 0));

            //-- timeseries with two records ---
            timeSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 2, 0, 0, 0), 6.0));
            timeSeries.RelaxationFactor = 1.0; 
            Assert.AreEqual(4.5, timeSeries.GetValue(2010, 1, 1, 12, 0, 0)); //Inbetween
            Assert.AreEqual(3.0, timeSeries.GetValue(2010, 1, 1, 0, 0, 0));  //Hit first time
            Assert.AreEqual(6.0, timeSeries.GetValue(2010, 1, 2, 0, 0, 0));  // Hit last time
            Assert.AreEqual(3.0, timeSeries.GetValue(2009, 12, 31, 0, 0, 0));  // one day before 
            Assert.AreEqual(6.0, timeSeries.GetValue(2010, 1, 3, 0, 0, 0));  // one day after 
            timeSeries.RelaxationFactor = 0.0;
            Assert.AreEqual(4.5, timeSeries.GetValue(2010, 1, 1, 12, 0, 0)); //Inbetween
            Assert.AreEqual(3.0, timeSeries.GetValue(2010, 1, 1, 0, 0, 0));  // Hit first time
            Assert.AreEqual(6.0, timeSeries.GetValue(2010, 1, 2, 0, 0, 0));  // Hit last time
            Assert.AreEqual(0.0, timeSeries.GetValue(2009, 12, 31, 0, 0, 0));  // one day before 
            Assert.AreEqual(9.0, timeSeries.GetValue(2010, 1, 3, 0, 0, 0));  // one day after 
            timeSeries.RelaxationFactor = 0.5;
            Assert.AreEqual(4.5, timeSeries.GetValue(2010, 1, 1, 12, 0, 0)); //Inbetween
            Assert.AreEqual(3.0, timeSeries.GetValue(2010, 1, 1, 0, 0, 0));  // Hit first time
            Assert.AreEqual(6.0, timeSeries.GetValue(2010, 1, 2, 0, 0, 0));  // Hit last time
            Assert.AreEqual(1.5, timeSeries.GetValue(2009, 12, 31, 0, 0, 0));  // one day before 
            Assert.AreEqual(7.5, timeSeries.GetValue(2010, 1, 3, 0, 0, 0));  // one day after
 
            // -- timeseries with 4 records ---
            timeSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 3, 0, 0, 0), 6.0));
            timeSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 4, 0, 0, 0), 4.0));
            timeSeries.RelaxationFactor = 0.0;
            Assert.AreEqual(4.5, timeSeries.GetValue(2010, 1, 1, 12, 0, 0)); //Inbetween
            Assert.AreEqual(6.0, timeSeries.GetValue(2010, 1, 2, 12, 0, 0)); //Inbetween
            Assert.AreEqual(5.0, timeSeries.GetValue(2010, 1, 3, 12, 0, 0)); //Inbetween
            Assert.AreEqual(3.0, timeSeries.GetValue(2010, 1, 1, 0, 0, 0));  //Hit first time
            Assert.AreEqual(6.0, timeSeries.GetValue(2010, 1, 2, 0, 0, 0));  // Hit Second
            Assert.AreEqual(6.0, timeSeries.GetValue(2010, 1, 3, 0, 0, 0));  // Hit third time
            Assert.AreEqual(4.0, timeSeries.GetValue(2010, 1, 4, 0, 0, 0));  // Hit last time
            Assert.AreEqual(0.0, timeSeries.GetValue(2009, 12, 31, 0, 0, 0));  // one day before 
            Assert.AreEqual(2.0, timeSeries.GetValue(2010, 1, 5, 0, 0, 0));  // one day after 

            // ========================================================================================
            // Getting values for timespans from timestamp based time series (Timestamp to TimeStamp)
            // ========================================================================================

            timeSeries.RelaxationFactor = 0.0;
            
            // v--------------------------------------V
            // |------------|------------|------------|
            Assert.AreEqual(15.5 / 3.0, timeSeries.GetValue(2010, 1, 1, 0, 0, 0, 2010, 1, 4, 0, 0, 0)); //interval same as the full timeseries

            //        v-----------v
            // |------------|------------|------------|
            Assert.AreEqual(5.625, timeSeries.GetValue(2010, 1, 1, 12, 0, 0, 2010, 1, 2, 12, 0, 0)); 

            //        v-------------------------v
            // |------------|------------|------------|
            Assert.AreEqual(11.375/2.0, timeSeries.GetValue(2010, 1, 1, 12, 0, 0, 2010, 1, 3, 12, 0, 0)); 

            //     v----v
            // |------------|------------|------------|
            Assert.AreEqual(4.5, timeSeries.GetValue(2010, 1, 1, 6, 0, 0, 2010, 1, 1, 18, 0, 0));

            // v--------------------------------------------------------------v
            // ------------|------------|------------|------------|------------
            Assert.AreEqual(4.0, timeSeries.GetValue(2009, 12,31, 0, 0, 0, 2010, 1, 5, 0, 0, 0));  //Extrapolating outside timeseries
 




        
 
        }
    }
}
