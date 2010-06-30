using HydroNumerics.Time.OpenMI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.Time.Core;
using HydroNumerics.OpenMI.Sdk.Backbone;
using OpenMI.Standard;
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
            
            TimespanSeries timespanSeries = new TimespanSeries("Flow", new System.DateTime(2010, 1, 1), 10, 2, TimestepUnit.Days, 4.3);
            timespanSeries.Unit = new HydroNumerics.Core.Unit("Liters pr. sec", 0.001, 0.0, "Liters pr second");
            timespanSeries.Unit.Dimension.Length = 3;
            timespanSeries.Unit.Dimension.Time = -1;
            timespanSeries.Description = "Measured Flow";
            TimestampSeries timestampSeries = new TimestampSeries("Water Level", new System.DateTime(2010, 1, 1), 6, 2, TimestepUnit.Days, 6.3);
            timestampSeries.Unit = new HydroNumerics.Core.Unit("cm", 0.01, 0.0, "centimeters");
            timestampSeries.Unit.Dimension.Length = 1;
            timestampSeries.Description = "Measured Head";

            TimeSeriesGroup tsg = new TimeSeriesGroup();
            tsg.Name = "MyTsGroup";
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

        [TestMethod()]
        public void Initialize()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
        }
      
        [TestMethod()]
        public void ComponentID()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual("HydroNumerics.Time.TimeSeriesGroup", linkableTimeSeriesGroup.ComponentID);
        }

        [TestMethod()]
        public void ComponentDescription()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual("HydroNumerics timeseries group", linkableTimeSeriesGroup.ComponentDescription);
        }

        [TestMethod()]
        public void ModelID()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual("MyTsGroup", linkableTimeSeriesGroup.ModelID);
        }

        [TestMethod()]
        public void ModelDescrition()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual("   ", linkableTimeSeriesGroup.ModelDescription);
        }

        [TestMethod()]
        public void TimeHorizon()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            System.DateTime start = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(linkableTimeSeriesGroup.TimeHorizon.Start).ToDateTime();
            System.DateTime end = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(linkableTimeSeriesGroup.TimeHorizon.End).ToDateTime();
            Assert.AreEqual(new System.DateTime(2010, 1, 1), start);
            Assert.AreEqual(new System.DateTime(2010, 1, 11), end);
        }

        [TestMethod()]
        public void OutputExchangeItemsCount()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual(2, linkableTimeSeriesGroup.OutputExchangeItemCount);
        }

        [TestMethod()]
        public void GetOutputExchangeItem()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            IOutputExchangeItem output01 = linkableTimeSeriesGroup.GetOutputExchangeItem(0);
            Assert.AreEqual(0, output01.DataOperationCount);
            Assert.AreEqual(ElementType.IDBased,output01.ElementSet.ElementType);
            Assert.AreEqual(1, output01.ElementSet.ElementCount);
            Assert.AreEqual("IDBased", output01.ElementSet.Description);
            Assert.AreEqual("Flow", output01.Quantity.ID);
            Assert.AreEqual("Measured Flow", output01.Quantity.Description);
            Assert.AreEqual("Liters pr. sec", output01.Quantity.Unit.ID);
            Assert.AreEqual(0.001, output01.Quantity.Unit.ConversionFactorToSI);
            Assert.AreEqual(0.0, output01.Quantity.Unit.OffSetToSI);
            Assert.AreEqual("Liters pr second", output01.Quantity.Unit.Description);
            Assert.AreEqual(3, output01.Quantity.Dimension.GetPower(DimensionBase.Length));
            Assert.AreEqual(-1, output01.Quantity.Dimension.GetPower(DimensionBase.Time));

            Assert.AreEqual("Flow", ((TsQuantity)output01.Quantity).BaseTimeSeries.Name);
        }

        [TestMethod()]
        public void GetValues()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);

            InputExchangeItem targetExchangeItem = new InputExchangeItem();
            Quantity targetQuantity = new Quantity();
            targetQuantity.ID = "Water Level";
            targetQuantity.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("meter", 1, 0, "meter");
            ElementSet targetElementSet = new ElementSet("inputLocation", "Location", ElementType.IDBased, new SpatialReference(""));
            targetElementSet.AddElement(new Element("E1"));

            Link link = new Link();
            link.SourceComponent = linkableTimeSeriesGroup;
            link.SourceQuantity = linkableTimeSeriesGroup.GetOutputExchangeItem(1).Quantity;
            link.SourceElementSet = linkableTimeSeriesGroup.GetOutputExchangeItem(1).ElementSet;
            link.TargetComponent = null;
            link.TargetQuantity = targetQuantity;
            link.TargetElementSet = targetElementSet;
            link.ID = "Link001";

            linkableTimeSeriesGroup.AddLink(link);
            linkableTimeSeriesGroup.Prepare();

            IValueSet valueSet = linkableTimeSeriesGroup.GetValues(new TimeStamp(new System.DateTime(2010, 1, 5)), "Link001");
            Assert.AreEqual(0.063, ((IScalarSet)valueSet).GetScalar(0)); 

            linkableTimeSeriesGroup.Finish();
            linkableTimeSeriesGroup.Dispose();
        }

        [TestMethod()]
        public void EarlietInputTime()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            System.DateTime earliestInputTime = new TimeStamp(linkableTimeSeriesGroup.EarliestInputTime).ToDateTime();
            Assert.AreEqual(new System.DateTime(2010, 1, 1), earliestInputTime);
        }

        [TestMethod()]
        public void SaveOmiFile()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.WriteOmiFile(filename);
        }
    

    }
}
