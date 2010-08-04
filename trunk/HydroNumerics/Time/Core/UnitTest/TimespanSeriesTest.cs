using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
        private bool propertyChanged = false;
        private string changedPropertyName = "";

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


        [TestMethod]
        public void ConstructorTest01()  //Default contructor
        {
            TimespanSeries ts = new TimespanSeries();
            Assert.AreEqual(0, ts.Items.Count);
            Assert.AreEqual(new Unit("Default Unit", 1.0, 0.0, "Default Unit", new Dimension(0, 0, 0, 0, 0, 0, 0, 0)), ts.Unit);
        }

        [TestMethod]
        public void ConstructorTest02()
        {
            TimespanSeries ts = new TimespanSeries("TestSeries", new DateTime(2010, 1, 1), 10, 2, TimestepUnit.Days, 7.5);
            Assert.AreEqual(new Unit("Default Unit", 1.0, 0.0, "Default Unit", new Dimension(0, 0, 0, 0, 0, 0, 0, 0)), ts.Unit);
            Assert.AreEqual(10, ts.Items.Count);
            Assert.AreEqual(new DateTime(2010,01,03), ts.Items[1].StartTime);
            Assert.AreEqual(new DateTime(2010, 01, 19), ts.Items[9].StartTime);
            Assert.AreEqual(new DateTime(2010, 01, 21), ts.Items[9].EndTime);
            Assert.AreEqual(7.5, ts.Items[9].Value);

            ts = new TimespanSeries("TestSeries", new DateTime(2010, 9, 15), 10, 2, TimestepUnit.Months, 7.5);
            Assert.AreEqual(new DateTime(2011, 03, 15), ts.Items[3].StartTime);
            

        }

        [TestMethod()]
        public void GetValue01()  //GetValue(DateTime time)
        {
            TimespanSeries timeSeries = new TimespanSeries();
            timeSeries.AllowExtrapolation = true;
 
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
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0), 3.0));
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 3, 0, 0, 0), 6.0));
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 3, 0, 0, 0), new DateTime(2010, 1, 4, 0, 0, 0), 4.0));

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

            timeSeries.Items.RemoveAt(timeSeries.Items.Count - 1);
            timeSeries.Items.RemoveAt(0);
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
        public void GetValue02() //GetValue(DateTime fromTime, DateTime toTime)
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

            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0), 3.0));

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
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 3, 0, 0, 0),new DateTime(2010, 1, 4, 0, 0, 0), 2));
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 4, 0, 0, 0), new DateTime(2010, 1, 6, 0, 0, 0), 3));
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 6, 0, 0, 0), new DateTime(2010, 1, 7, 0, 0, 0), 4));
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 7, 0, 0, 0), new DateTime(2010, 1, 8, 0, 0, 0), 3));
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 8, 0, 0, 0), new DateTime(2010, 1, 10, 0, 0, 0), 5));

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

        [TestMethod()]
        public void ConvertUnit()
        {
            TimespanSeries timeSeries = new TimespanSeries();
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 3, 0, 0, 0), new DateTime(2010, 1, 4, 0, 0, 0), 2));
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 4, 0, 0, 0), new DateTime(2010, 1, 6, 0, 0, 0), 4));
            timeSeries.Items.Add(new TimespanValue(new DateTime(2010, 1, 6, 0, 0, 0), new DateTime(2010, 1, 7, 0, 0, 0), 8));
            timeSeries.Unit = new HydroNumerics.Core.Unit("Decimenters", 0.1, 0.0);
            HydroNumerics.Core.Unit newUnit = new HydroNumerics.Core.Unit("centimeters", 0.01, 0.0);
            timeSeries.ConvertUnit(newUnit);
            Assert.AreEqual("centimeters", timeSeries.Unit.ID);
            Assert.AreEqual(0.01, timeSeries.Unit.ConversionFactorToSI);
            Assert.AreEqual(20, timeSeries.Items[0].Value);
            Assert.AreEqual(40, timeSeries.Items[1].Value);
            Assert.AreEqual(80, timeSeries.Items[2].Value);
        }

        [TestMethod()]
        public void PropertyChangedEvent()
        {
            TimespanSeries ts = new TimespanSeries();
            ts.Items.Add(new TimespanValue(new Timespan(new DateTime(2010,1,1), new DateTime(2010,1,2)),7.5));
            ts.PropertyChanged +=new System.ComponentModel.PropertyChangedEventHandler(ts_PropertyChanged);

            propertyChanged = false; changedPropertyName = "";
            ts.Items[0].Value = 5;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("Items", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            ts.Items[0].StartTime = new DateTime(2010, 1, 2);
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("Items", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            ts.Items[0].EndTime = new DateTime(2010, 1, 4);
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("Items", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            ts.Name = "anotherName";
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("Name", changedPropertyName);
            
            propertyChanged = false; changedPropertyName = "";
            ts.Unit.ConversionFactorToSI = 3.2;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("ConversionFactorToSI", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            ts.Unit = new Unit("someUnit", 3.2, 9.3);
            Assert.AreEqual("Unit", changedPropertyName);

            propertyChanged = false; changedPropertyName = ""; //test when a new unit is assigned
            ts.Unit.ConversionFactorToSI = 5.2;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("ConversionFactorToSI", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            ts.RelaxationFactor = 0.5;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("RelaxationFactor", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            ts.Unit.Dimension.AmountOfSubstance = 5;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("AmountOfSubstance", changedPropertyName);

        }

        void ts_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            propertyChanged = true;
            changedPropertyName = e.PropertyName;
        }

        [TestMethod()]
        public void Example()
        {
            //The code below demonstrates different ways of using the TimespanSeries class

            // -- Constructing the class (default constructor)--
            TimespanSeries ts = new TimespanSeries();

            // -- Constructing the class (overloaded  constructor) --
            //The timespanSeries below is given the name "My Timeseries, starts at January 1st, 2010 at 00:00:00, has
            //10 time timesteps with length 2 days and value 3
            ts = new TimespanSeries("My Timeseries", new DateTime(2010, 1, 1, 0, 0, 0), 10,  2, TimestepUnit.Days, 3);

            // -- Getting and setting values and time from the timeseries --
            // Direct access:
            double secondValue = ts.Items[1].Value; //The value in the unit as defined by the timeseries
            double secondValueInSI = ts.Unit.ToSiUnit(ts.Items[1].Value); // The value in SI unit
          
            DateTime startTimeforSecondTimestep = ts.Items[1].StartTime;
            DateTime endTimeforSecondTimestep = ts.Items[1].EndTime;
            
            ts.Items[1].Value = 4.5;  // when 4.5 is in the same unit at used by the timeseries
            ts.Items[1].Value = ts.Unit.FromSiToThisUnit(4.5); // when 4.5 is in SI units
            ts.Items[1].Value = ts.Unit.FromUnitToThisUnit(4.5, new Unit("cm/sec", 0.01, 0)); // when 4.5 is in cm pr second

            ts.Items[1].StartTime = new DateTime(2010, 1, 1, 0, 0, 0); //changing the starttime for the second timestep
            //TODO: see if it is possible to check that end time is later than start time.

            // -- Getting values for a given time --
            double x = ts.GetValue(new DateTime(2010, 1, 1, 12, 0, 0));  
            double xINSi = ts.GetSiValue(new DateTime(2010, 1, 1, 12, 0, 0));
            double xInMyUnit = ts.GetValue(new DateTime(2010, 1, 1, 12, 0, 0), new Unit("CM/SEC", 0.01, 0));
           
            // -- Getting values for a give timespan (period) --
            double y = ts.GetValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0));
            double yInSi = ts.GetSiValue(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0));
            double yInMyUnit = ts.GetValue(new DateTime(2010, 1, 1, 12, 0, 0), new DateTime(2010,1,2,0,0,0), new Unit("CM/SEC", 0.01, 0));

            // -- Adding values --
            ts.AddValue(new DateTime(2010, 1, 1, 12, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0), 4.5);
            ts.AddSiValue(new DateTime(2010, 1, 1, 12, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0), 4.5);
            
            // -- Append value --
            ts.AppendValue(4.5);  //Appending using the same timestep length as the last time step

            // -- Removing values --
            ts.RemoveAfter(new DateTime(2010, 1, 1, 12, 0, 0));
        }

        [TestMethod]
        public void Save()
        {
            string filename = "HydroNumerics.Time.Core.UnitTest.TimespanSeriesTest.Save.xts";
            TimespanSeries ts = new TimespanSeries("TSName", new DateTime(2010, 1, 1), 10, 1, TimestepUnit.Days, 5.5);
            ts.Save(filename);
        }

        [TestMethod]
        public void Load()
        {
            string filename = "HydroNumerics.Time.Core.UnitTest.TimespanSeriesTest.Load.xts";
            TimespanSeries ts1 = new TimespanSeries("TSName", new DateTime(2010, 1, 1), 10, 1, TimestepUnit.Days, 5.5);
            ts1.Save(filename);

            TimespanSeries ts2 = new TimespanSeries();
            ts2.Load(filename);
            Assert.AreEqual(ts1.Id, ts2.Id);
            Assert.AreEqual(ts1.Description, ts2.Description);
            Assert.AreEqual(ts1.Name, ts2.Name);
            Assert.AreEqual(ts1.Unit, ts2.Unit);
            Assert.AreEqual(ts1.Items.Count, ts2.Items.Count);
            for (int i = 0; i < ts1.Items.Count; i++)
            {
                Assert.AreEqual(ts1.Items[i].StartTime, ts2.Items[i].StartTime);
                Assert.AreEqual(ts1.Items[i].EndTime, ts2.Items[i].EndTime);
                Assert.AreEqual(ts1.Items[i].Value, ts2.Items[i].Value);
            }
        }

        [TestMethod]
        public void Equals()
        {
            TimespanSeries ts1 = new TimespanSeries("tsname", new DateTime(2010, 1, 1), 3, 2, TimestepUnit.Days, 4.5);
            TimespanSeries ts2 = new TimespanSeries("tsname", new DateTime(2010, 1, 1), 3, 2, TimestepUnit.Days, 4.5);
                      
            Assert.AreEqual(ts1, ts2);
            ts2.Name = "something else";
            Assert.AreNotEqual(ts1, ts2);

            ts2 = new TimespanSeries("tsname", new DateTime(2010, 1, 1), 3, 2, TimestepUnit.Days, 4.5);
            Assert.AreEqual(ts1, ts2);
            ts2.Id = 99;
            Assert.AreNotEqual(ts1, ts2);

            ts2 = new TimespanSeries("tsname", new DateTime(2010, 1, 1), 3, 2, TimestepUnit.Days, 4.5);
            Assert.AreEqual(ts1, ts2);
            ts2.Description = "another description";
            Assert.AreNotEqual(ts1, ts2);

            ts2 = new TimespanSeries("tsname", new DateTime(2010, 1, 1), 3, 2, TimestepUnit.Days, 4.5);
            Assert.AreEqual(ts1, ts2);
            ts2.RelaxationFactor = 0.2323;
            Assert.AreNotEqual(ts1, ts2);

            ts2 = new TimespanSeries("tsname", new DateTime(2010, 1, 1), 3, 2, TimestepUnit.Days, 4.5);
            Assert.AreEqual(ts1, ts2);
            ts2.Unit.Dimension.ElectricCurrent = 6;
            Assert.AreNotEqual(ts1, ts2);

            ts2 = new TimespanSeries("tsname", new DateTime(2010, 1, 1), 3, 2, TimestepUnit.Days, 4.5);
            Assert.AreEqual(ts1, ts2);
            ts2.Items.Add(new TimespanValue(new DateTime(2010,1,1), new DateTime(2010,1,2), 66));
            Assert.AreNotEqual(ts1, ts2);

            ts2 = new TimespanSeries("tsname", new DateTime(2010, 1, 1), 3, 2, TimestepUnit.Days, 4.5);
            Assert.AreEqual(ts1, ts2);
            ts2.Items[2].Value = 33;
            Assert.AreNotEqual(ts1, ts2);
        }


        //[TestMethod]  //This test is not relevant to the time series and will be removed 
        //public void ToBeRemoved()
        //{
        //    double g = 0.1;  // b/a
        //    double x;        //Vg/Vx
        //    double beta = Math.PI/8.0; // højde i forhold til vind

        //    for (g = 0.2; g >= 0; g -= 0.001)
        //    {
        //        x = Math.Cos(Math.Atan(g) + beta) / Math.Cos(Math.Atan(g));
        //        Console.WriteLine(g.ToString() + " " + x.ToString());
        //    }

        //}
    
    }
}
