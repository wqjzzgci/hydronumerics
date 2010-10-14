using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.Core;
using System;

namespace HydroNumerics.Time.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TimespanValueTest and is intended
    ///to contain all TimespanValueTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimespanValueTest
    {


        private TestContext testContextInstance;
        private bool propertyChanged;
        private string changedProperty;

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
        ///A test for NotifyPropertyChanged
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HydroNumerics.Time.Core.dll")]
        public void NotifyPropertyChangedTest()
        {
            TimespanValue tsv = new TimespanValue(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2), 5.6);
            tsv.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(tsv_PropertyChanged);

            propertyChanged = false;
            changedProperty = "";
            tsv.StartTime = new DateTime(2009, 12, 31);
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("StartTime", changedProperty);

            changedProperty = "";
            propertyChanged = false;
            tsv.EndTime = new DateTime(2010, 12, 31);
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("EndTime", changedProperty);

            changedProperty = "";
            propertyChanged = false;
            tsv.Value = 99;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("Value", changedProperty);
          
        }

        void tsv_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            propertyChanged = true;
            changedProperty = e.PropertyName;
        }

        [TestMethod()]
        public void TimespanValue3()
        {
            TimespanValue tsv = new TimespanValue(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2), 5.6);
            Assert.AreEqual(tsv.Value, 5.6);
            Assert.AreEqual(tsv.StartTime, new DateTime(2010, 1, 1));
            Assert.AreEqual(tsv.EndTime, new DateTime(2010, 1, 2));
        }

        [TestMethod()]
        public void TimespanValue2()
        {
            TimespanValue tsv = new TimespanValue();
        }

        [TestMethod()]
        public void TimespanValue1()  //TimespanValue(TimespanValue obj)
        {
            TimespanValue tsv1 = new TimespanValue();
            tsv1.StartTime = new DateTime(2010, 1, 1);
            tsv1.EndTime = new DateTime(2010, 1, 2);
            tsv1.Value = 4.5;
            TimespanValue tsv2 = new TimespanValue(tsv1);
            Assert.AreEqual(tsv1.Value, tsv2.Value);
            Assert.AreEqual(tsv1.StartTime, tsv2.StartTime);
            Assert.AreEqual(tsv1.EndTime, tsv2.EndTime);
        }

        [TestMethod()]
        public void TimespanValue() //TimespanValue(TimeSpan timespan, double value)
        {
            TimespanValue tsv = new TimespanValue(new Timespan(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2)), 3.3);
            Assert.AreEqual(tsv.Value, 3.3);
            Assert.AreEqual(tsv.StartTime, new DateTime(2010, 1, 1));
            Assert.AreEqual(tsv.EndTime, new DateTime(2010, 1, 2));
        }

        [TestMethod()]
        public void EndTimeTest()
        {
            TimespanValue tsv = new TimespanValue(new Timespan(new DateTime(2010, 2, 1), new DateTime(2010, 2, 2)), 3.3);
            tsv.EndTime = new DateTime(2010, 2, 5);
            Assert.AreEqual(new DateTime(2010, 2, 5), tsv.EndTime);
        }

        [TestMethod()]
        public void StartTimeTest()
        {
            TimespanValue tsv = new TimespanValue(new Timespan(new DateTime(2010, 2, 1), new DateTime(2010, 2, 2)), 3.3);
            tsv.StartTime = new DateTime(2010, 1, 5);
            Assert.AreEqual(new DateTime(2010, 1, 5), tsv.StartTime);
        }

        [TestMethod()]
        public void TimeSpanTest()
        {
            TimespanValue tsv = new TimespanValue(new Timespan(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2)), 3.3);
            tsv.TimeSpan = new Timespan(new DateTime(2010, 2, 1), new DateTime(2010, 2, 2));
            Assert.AreEqual(new DateTime(2010, 2, 1), tsv.TimeSpan.Start);
            Assert.AreEqual(new DateTime(2010, 2, 2), tsv.TimeSpan.End);

        }


        [TestMethod()]
        public void ValueTest()
        {
            TimespanValue tsv = new TimespanValue(new Timespan(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2)), 3.3);
            tsv.Value = 6.7;
            Assert.AreEqual(6.7, tsv.Value);
        }

        [TestMethod]
        public void Equals()
        {
            TimespanValue tsv1 = new TimespanValue(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2), 5.5);
            TimespanValue tsv2 = new TimespanValue(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2), 5.5);
            Assert.AreEqual(tsv1, tsv2);
            tsv2.EndTime = new DateTime(2010, 1, 3);
            Assert.AreNotEqual(tsv1, tsv2);
            tsv2 = new TimespanValue(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2), 5.5);
            Assert.AreEqual(tsv1, tsv2);
            tsv2.StartTime = new DateTime(2009, 12, 31);
            Assert.AreNotEqual(tsv1, tsv2);
            tsv2 = new TimespanValue(new DateTime(2010, 1, 1), new DateTime(2010, 1, 2), 5.5);
            Assert.AreEqual(tsv1, tsv2);
            tsv2.Value = 44;
            Assert.AreNotEqual(tsv1, tsv2);
        }
    }
}
