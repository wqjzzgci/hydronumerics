using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.Time.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TimeSeriesGroupFactoryTest and is intended
    ///to contain all TimeSeriesGroupFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimeSeriesGroupFactoryTest
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
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void Create()
        {
            string filename = "HydroNumerics.Time.Core.UnitTest.TimeSeriesGroupFactoryTest.Create.xts";
            TimeSeriesGroup tsg = new TimeSeriesGroup();
            TimespanSeries timespanSeries = new TimespanSeries("timespanseriesname", new System.DateTime(2010, 1, 1), 10, 2, TimestepUnit.Days, 4.3);
            TimestampSeries timestampSeries = new TimestampSeries("timestampseriesname", new System.DateTime(2010, 1, 1), 10, 2, TimestepUnit.Days, 4.3);
            tsg.Items.Add(timespanSeries);
            tsg.Items.Add(timestampSeries);
            tsg.Save(filename);

            TimeSeriesGroup tsg2 = TimeSeriesGroupFactory.Create(filename);

            for (int i = 0; i < tsg.Items.Count; i++)
            {
                Assert.AreEqual(tsg.Items[i], tsg2.Items[i]);
            }

            
        }
    }
}
