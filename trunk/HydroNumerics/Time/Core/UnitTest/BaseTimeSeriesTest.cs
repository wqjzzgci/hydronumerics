using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Time.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for BaseTimeSeriesTest and is intended
    ///to contain all BaseTimeSeriesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BaseTimeSeriesTest
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


        internal virtual BaseTimeSeries CreateBaseTimeSeries()
        {
            // TODO: Instantiate an appropriate concrete class.
            BaseTimeSeries target = null;
            return target;
        }

        /// <summary>
        ///A test for Description
        ///</summary>
        [TestMethod()]
        public void DescriptionTest()
        {
            TimestampSeries target = new TimestampSeries();
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
            TimestampSeries target = new TimestampSeries();
            string expected = "MyName";
            string actual;
            target.Name = expected;
            actual = target.Name;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void IdTest()
        {
            TimestampSeries target = new TimestampSeries();
            int expected = 123;
            int actual;
            target.Id = expected;
            actual = target.Id;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for RelaxationFactor
        ///</summary>
        [TestMethod()]
        public void RelaxationFactorTest()
        {
            TimestampSeries timeseries = new TimestampSeries();
            timeseries.RelaxationFactor = 0.5;
            Assert.AreEqual(0.5, timeseries.RelaxationFactor);
            //-- Expected exception when relaxation factor is assigned to a value outside the interval [0,1]

            try
            {
                timeseries.RelaxationFactor = -0.1;
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }
            try
            {
                timeseries.RelaxationFactor = 1.1;
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }
        }


        /// <summary>
        ///A test for ExtractValue
        ///</summary>
        [TestMethod()]
        public void ExtractValue01()  //ExtractValue(DateTime time, bool toSIUnit)
        {
            TimestampSeries timestampSeries = new TimestampSeries();
            timestampSeries.TimeValues.Add(new TimeValue(new DateTime(2010, 1, 1, 0, 0, 0), 3000.0));
            timestampSeries.Unit = new HydroNumerics.Core.Unit("Liters pr. second", 0.001, 0.0);
            Assert.AreEqual(3.0, timestampSeries.ExtractValue(new DateTime(2010, 1, 2, 0, 0, 0), true));
            Assert.AreEqual(3000.0, timestampSeries.ExtractValue(new DateTime(2010, 1, 2, 0, 0, 0), false));
        }

        [TestMethod()]
        public void ExtractValue02()  //ExtractValue(DateTime time, Unit toUnit)
        {
            TimestampSeries timestampSeries = new TimestampSeries();
            timestampSeries.TimeValues.Add(new TimeValue(new DateTime(2010, 1, 1, 0, 0, 0), 3000.0));
            timestampSeries.Unit = new HydroNumerics.Core.Unit("Liters pr. second", 0.001, 0.0);
            HydroNumerics.Core.Unit toUnit = new HydroNumerics.Core.Unit("Hectoliters pr sec", 0.1, 0.0);
            Assert.AreEqual(30.0, timestampSeries.ExtractValue(new DateTime(2010, 1, 2, 0, 0, 0), toUnit));
        }

        [TestMethod()]
        public void ExtractValue03()  //ExtractValue(DateTime fromTime, DateTime toTime, bool toSIUnit)
        {
            TimestampSeries timestampSeries = new TimestampSeries();
            timestampSeries.TimeValues.Add(new TimeValue(new DateTime(2010, 1, 1, 0, 0, 0), 3000.0));
            timestampSeries.Unit = new HydroNumerics.Core.Unit("Liters pr. second", 0.001, 0.0);
            Assert.AreEqual(3.0, timestampSeries.ExtractValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0), true));
            Assert.AreEqual(3000.0, timestampSeries.ExtractValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0), false));
        }

        [TestMethod()]
        public void ExtractValue04()  //ExtractValue(DateTime fromTime, DateTime toTime, Unit toUnit)
        {
            TimestampSeries timestampSeries = new TimestampSeries();
            timestampSeries.TimeValues.Add(new TimeValue(new DateTime(2010, 1, 1, 0, 0, 0), 3000.0));
            timestampSeries.Unit = new HydroNumerics.Core.Unit("Liters pr. second", 0.001, 0.0);
            HydroNumerics.Core.Unit toUnit = new HydroNumerics.Core.Unit("Hectoliters pr sec", 0.1, 0.0);
            Assert.AreEqual(30.0, timestampSeries.ExtractValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 3, 0, 0, 0), toUnit));
        }
    }
}
