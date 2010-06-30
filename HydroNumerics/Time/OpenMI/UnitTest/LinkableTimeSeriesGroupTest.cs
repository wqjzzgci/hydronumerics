using HydroNumerics.Time.OpenMI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.Time.Core;
using HydroNumerics.OpenMI.Sdk.Backbone;
namespace HydroNumerics.Time.OpenMI.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for LinkableTimeSeriesGroupTest and is intended
    ///to contain all LinkableTimeSeriesGroupTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LinkableTimeSeriesGroupTest
    {
        Argument[] arguments;
        string filename;

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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            filename = "TimeSeriesGroup.xts";
            TimeSeriesGroup tsg = new TimeSeriesGroup();
            TimespanSeries timespanSeries = new TimespanSeries("timespanseriesname", new System.DateTime(2010, 1, 1), 10, 2, TimestepUnit.Days, 4.3);
            TimestampSeries timestampSeries = new TimestampSeries("timestampseriesname", new System.DateTime(2010, 1, 1), 10, 2, TimestepUnit.Days, 4.3);
            tsg.Items.Add(timespanSeries);
            tsg.Items.Add(timestampSeries);
            tsg.Save(filename);

            Argument argument = new Argument("FileName", filename, true, "someDescription");
            arguments = new Argument[1]{argument};
      

        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ComponentID
        ///</summary>
        [TestMethod()]
        public void ComponentID()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual("HydroNumerics.Time.TimeSeriesGroup", linkableTimeSeriesGroup.ComponentID);
        }

        [TestMethod()]
        public void SaveOmiFile()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.WriteOmiFile(filename);
        }
    

    }
}
