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
