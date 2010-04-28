using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HydroNumerics.Time.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TimespanSeriesTest and is intended
    ///to contain all TimespanSeriesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimespanSeriesTest
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
        ///A test for ExtractValue
        ///</summary>
        [TestMethod()]
        public void ExtractValue01()  //ExtractValue(DateTime time)
        {
            TimespanSeries timeSeries = new TimespanSeries();
 
            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0));
            }
            catch (Exception ex)
            {
                //-- Expected exception when GetValues is invoked on an empty timeseries. --
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }
            
            timeSeries.RelaxationFactor = 0.0;
            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0), 3.0));
            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 3, 0, 0, 0), 6.0));
            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 3, 0, 0, 0), new DateTime(2010, 1, 4, 0, 0, 0), 4.0));

            // v
            //     |------------|------------|------------|
            //            3            6            4           
            Assert.AreEqual(1.5, timeSeries.GetValue(new DateTime(2009, 12, 31, 12, 0, 0)));

            // v
            // |------------|------------|------------|
            Assert.AreEqual(3, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0)));

            //       v
            // |------------|------------|------------|
            Assert.AreEqual(3, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0)));

            //              v
            // |------------|------------|------------|
            Assert.AreEqual(6, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0)));

            //                    v
            // |------------|------------|------------|
            Assert.AreEqual(6, timeSeries.GetValue(new DateTime(2010, 1, 2, 12, 0, 0)));

            //                           v
            // |------------|------------|------------|
            Assert.AreEqual(4, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0)));

            //                                 v
            // |------------|------------|------------|
            Assert.AreEqual(4, timeSeries.GetValue(new DateTime(2010, 1, 3, 12, 0, 0)));

            //                                        v
            // |------------|------------|------------|
            Assert.AreEqual(4, timeSeries.GetValue(new DateTime(2010, 1, 4, 0, 0, 0)));

            //                                             v
            // |------------|------------|------------|
            Assert.AreEqual(3, timeSeries.GetValue(new DateTime(2010, 1, 4, 12, 0, 0)));

            timeSeries.RelaxationFactor = 1.0;

            // v
            //     |------------|------------|------------|
            //            3            6            4           
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2009, 12, 31, 12, 0, 0)));

            //                    v
            // |------------|------------|------------|
            Assert.AreEqual(6, timeSeries.GetValue(new DateTime(2010, 1, 2, 12, 0, 0)));

            //                                             v
            // |------------|------------|------------|
            Assert.AreEqual(4, timeSeries.GetValue(new DateTime(2010, 1, 4, 12, 0, 0)));

            timeSeries.TimespanValues.RemoveAt(timeSeries.TimespanValues.Count - 1);
            timeSeries.TimespanValues.RemoveAt(0);
            timeSeries.RelaxationFactor = 1.0;

            //                               v
            //              |------------|
            //                     6
            Assert.AreEqual(6, timeSeries.GetValue(new DateTime(2010, 1, 3, 12, 0, 0)));

            //         v
            //              |------------|
            //                     6
            Assert.AreEqual(6, timeSeries.GetValue(new DateTime(2010, 1, 1, 12, 0, 0)));

            //                     v
            //              |------------|
            //                     6
            Assert.AreEqual(6, timeSeries.GetValue(new DateTime(2010, 1, 2, 12, 0, 0)));
        }


        [TestMethod()]
        public void ExtractValue02()
        {
            //-- Expected exception when GetValues is invoked on an empty timeseries. --
            TimespanSeries timeSeries = new TimespanSeries();

            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0));
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }

            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0), 3.0));

            //Testing when only one record in timeseries
            Assert.AreEqual(3.0, timeSeries.GetValue(new DateTime(2010, 11, 1, 0, 0, 0), new DateTime(2010, 12, 1, 0, 0, 0)));

            try
            {
                timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 1, 0, 0, 0));

            }
            catch (Exception ex)
            {
                //Testing invalid argument for timespan
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }

        
            timeSeries = new TimespanSeries();
            timeSeries.RelaxationFactor = 0.0;
            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 3, 0, 0, 0),new DateTime(2010, 1, 4, 0, 0, 0), 2));
            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 4, 0, 0, 0), new DateTime(2010, 1, 6, 0, 0, 0), 3));
            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 6, 0, 0, 0), new DateTime(2010, 1, 7, 0, 0, 0), 4));
            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 7, 0, 0, 0), new DateTime(2010, 1, 8, 0, 0, 0), 3));
            timeSeries.TimespanValues.Add(new TimespanValue(new DateTime(2010, 1, 8, 0, 0, 0), new DateTime(2010, 1, 10, 0, 0, 0), 5));

            //---------------------------------------------------------------------------------------
            //                            v-----v
            //                  |------|------------|------|------|------------|
            //                     2          3        4       3         5
            //    t--->         3      4            6      7      8            10
            //------------------------------------------------------------------------------------------
            Assert.AreEqual(3, timeSeries.GetValue(new DateTime(2010, 1, 4, 12, 0, 0), new DateTime(2010, 1, 5, 12, 0, 0)));

            //                         v------------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(3, timeSeries.GetValue(new DateTime(2010, 1, 4, 0, 0, 0), new DateTime(2010, 1, 5, 0, 0, 0)));

            //                  v------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(2, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0), new DateTime(2010, 1, 4, 0, 0, 0)));

            //                                                        v-----v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(5, timeSeries.GetValue(new DateTime(2010, 1, 8, 12, 0, 0), new DateTime(2010, 1, 9, 12, 0, 0)));

            //                                                    v------------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(5, timeSeries.GetValue(new DateTime(2010, 1, 8, 0, 0, 0), new DateTime(2010, 1, 10, 0, 0, 0)));

            //                  v----------------------------------------------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(25.0 / 7.0, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0), new DateTime(2010, 1, 10, 0, 0, 0)), 0.00000000001);

            //                               v----------------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(3.4, timeSeries.GetValue(new DateTime(2010, 1, 5, 0, 0, 0), new DateTime(2010, 1, 7, 12, 0, 0)));

            //           v------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(1.0, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 3, 0, 0, 0)), 0.00000000001);

            //    v------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(0.0, timeSeries.GetValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0)), 0.00000000001);

            //                  v------------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(2.5, timeSeries.GetValue(new DateTime(2010, 1, 3, 0, 0, 0), new DateTime(2010, 1, 5, 0, 0, 0)), 0.00000000001);

            //            v------------------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(2, timeSeries.GetValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 5, 0, 0, 0)), 0.00000000001);

            //                                                                 v-----v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(6.0, timeSeries.GetValue(new DateTime(2010, 1, 10, 0, 0, 0), new DateTime(2010, 1, 11, 0, 0, 0)), 0.00000000001);

            //                                                                        v-----v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(7.0, timeSeries.GetValue(new DateTime(2010, 1, 11, 0, 0, 0), new DateTime(2010, 1, 12, 0, 0, 0)), 0.00000000001);

            //                                                           v----------v
            //                  |------|------------|------|------|------------|
            Assert.AreEqual(6.5, timeSeries.GetValue(new DateTime(2010, 1, 9, 0, 0, 0), new DateTime(2010, 1, 11, 0, 0, 0)), 0.00000000001);
          

        }
    }
}
