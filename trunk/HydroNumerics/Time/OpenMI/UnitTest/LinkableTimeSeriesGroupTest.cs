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

        /// <summary>
        /// Testing when the LinkableTimeSeriesGroup is providing values
        /// </summary>
        [TestMethod()]
        public void GetValues_AsProvider()
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
        public void GetValues_AsAcceptor()
        {
            filename = "TimeSeriesGroupAcceptor.xts";

            TimespanSeries timespanSeries = new TimespanSeries("Flow", new System.DateTime(2010, 1, 1), 10, 2, TimestepUnit.Days, 10.2);
            timespanSeries.Unit = new HydroNumerics.Core.Unit("Liters pr. sec", 0.001, 0.0, "Liters pr second");
            timespanSeries.Unit.Dimension.Length = 3;
            timespanSeries.Unit.Dimension.Time = -1;
            timespanSeries.Description = "Measured Flow";
            TimestampSeries timestampSeries = new TimestampSeries("Water Level", new System.DateTime(2010, 1, 1), 6, 2, TimestepUnit.Days, 12.2);
            timestampSeries.Unit = new HydroNumerics.Core.Unit("cm", 0.01, 0.0, "centimeters");
            timestampSeries.Unit.Dimension.Length = 1;
            timestampSeries.Description = "Measured Head";

            TimeSeriesGroup tsg = new TimeSeriesGroup();
            tsg.Name = "Acceptor";
            tsg.Items.Add(timespanSeries);
            tsg.Items.Add(timestampSeries);
            tsg.Save(filename);
            
            Argument argument = new Argument("FileName", filename, true, "someDescription");
            Argument[] acceptorArguments = new Argument[1] { argument };

            LinkableTimeSeriesGroup acceptorTs = new LinkableTimeSeriesGroup();
            acceptorTs.Initialize(acceptorArguments);
            acceptorTs.WriteOmiFile(filename);

            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);


            Link ts2tsLink1 = new Link();
            ts2tsLink1.SourceComponent = linkableTimeSeriesGroup;
            ts2tsLink1.TargetComponent = acceptorTs;
            ts2tsLink1.SourceQuantity = linkableTimeSeriesGroup.GetOutputExchangeItem(0).Quantity;
            ts2tsLink1.SourceElementSet = linkableTimeSeriesGroup.GetOutputExchangeItem(0).ElementSet;
            ts2tsLink1.TargetQuantity = acceptorTs.GetInputExchangeItem(0).Quantity;
            ts2tsLink1.TargetElementSet = acceptorTs.GetInputExchangeItem(0).ElementSet;
            ts2tsLink1.ID = "ts2ts1";
            linkableTimeSeriesGroup.AddLink(ts2tsLink1);
            acceptorTs.AddLink(ts2tsLink1);

            Link ts2tsLink2 = new Link();
            ts2tsLink2.SourceComponent = linkableTimeSeriesGroup;
            ts2tsLink2.TargetComponent = acceptorTs;
            ts2tsLink2.SourceQuantity = linkableTimeSeriesGroup.GetOutputExchangeItem(1).Quantity;
            ts2tsLink2.SourceElementSet = linkableTimeSeriesGroup.GetOutputExchangeItem(1).ElementSet;
            ts2tsLink2.TargetQuantity = acceptorTs.GetInputExchangeItem(1).Quantity;
            ts2tsLink2.TargetElementSet = acceptorTs.GetInputExchangeItem(1).ElementSet;
            ts2tsLink2.ID = "ts2ts2";
            linkableTimeSeriesGroup.AddLink(ts2tsLink2);
            acceptorTs.AddLink(ts2tsLink2);


            //setting up the work arround type of trigger
            InputExchangeItem targetExchangeItem = new InputExchangeItem();
            Quantity targetQuantity = new Quantity();
            targetQuantity.ID = "Water Level";
            targetQuantity.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("meter", 1, 0, "meter");
            ElementSet targetElementSet = new ElementSet("inputLocation", "Location", ElementType.IDBased, new SpatialReference(""));

            Link triggerLink = new Link();
            triggerLink.SourceComponent = acceptorTs;
            triggerLink.TargetComponent = null;
            triggerLink.SourceQuantity = acceptorTs.GetOutputExchangeItem(0).Quantity;
            triggerLink.SourceElementSet = acceptorTs.GetOutputExchangeItem(0).ElementSet;
            triggerLink.TargetQuantity = targetQuantity;
            triggerLink.TargetElementSet = targetElementSet;
            triggerLink.ID = "TriggerLink";

            acceptorTs.AddLink(triggerLink);

            TimespanSeries tss1 = (TimespanSeries)acceptorTs.TimeSeriesGroup.Items[0];
            TimestampSeries tss2 = (TimestampSeries)acceptorTs.TimeSeriesGroup.Items[1];
            Assert.AreEqual(10.2, tss1.Items[0].Value);
            Assert.AreEqual(12.2, tss2.Items[0].Value);

            acceptorTs.GetValues(new TimeStamp(new System.DateTime(2010, 1, 3)), triggerLink.ID);

            Assert.AreEqual(4.3, tss1.Items[0].Value);
            Assert.AreEqual(6.3, tss2.Items[0].Value);

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
        public void HasDiscreteTimes()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.IsTrue(linkableTimeSeriesGroup.HasDiscreteTimes(linkableTimeSeriesGroup.GetOutputExchangeItem(0).Quantity,linkableTimeSeriesGroup.GetOutputExchangeItem(0).ElementSet));
        }

        [TestMethod()]
        public void GetDiscreteTimesCount()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual(10, linkableTimeSeriesGroup.GetDiscreteTimesCount(linkableTimeSeriesGroup.GetOutputExchangeItem(0).Quantity,linkableTimeSeriesGroup.GetOutputExchangeItem(0).ElementSet));
            Assert.AreEqual(6, linkableTimeSeriesGroup.GetDiscreteTimesCount(linkableTimeSeriesGroup.GetOutputExchangeItem(1).Quantity, linkableTimeSeriesGroup.GetOutputExchangeItem(1).ElementSet));
        }

        [TestMethod()]
        public void GetDiscreteTime()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);

            //testing timespan series (exchange item 1), first record
            ITimeSpan time = (ITimeSpan) linkableTimeSeriesGroup.GetDiscreteTime(linkableTimeSeriesGroup.GetOutputExchangeItem(0).Quantity, linkableTimeSeriesGroup.GetOutputExchangeItem(0).ElementSet,0);
            System.DateTime startTime = new TimeStamp(new TimeSpan(time).Start).ToDateTime();
            System.DateTime endTime = new TimeStamp(new TimeSpan(time).End).ToDateTime();
            Assert.AreEqual(new System.DateTime(2010, 1, 1), startTime);
            Assert.AreEqual(new System.DateTime(2010, 1, 3), endTime);

            //testing timespan series (exchange item 1), last record
            time = (ITimeSpan)linkableTimeSeriesGroup.GetDiscreteTime(linkableTimeSeriesGroup.GetOutputExchangeItem(0).Quantity, linkableTimeSeriesGroup.GetOutputExchangeItem(0).ElementSet, 9);
            startTime = new TimeStamp(new TimeSpan(time).Start).ToDateTime();
            endTime = new TimeStamp(new TimeSpan(time).End).ToDateTime();
            Assert.AreEqual(new System.DateTime(2010, 1, 19), startTime);
            Assert.AreEqual(new System.DateTime(2010, 1, 21), endTime);

            //testing timestamp series (exchange item 1), first record
            ITimeStamp timestamp = (ITimeStamp)linkableTimeSeriesGroup.GetDiscreteTime(linkableTimeSeriesGroup.GetOutputExchangeItem(1).Quantity, linkableTimeSeriesGroup.GetOutputExchangeItem(1).ElementSet, 0);
            Assert.AreEqual(new System.DateTime(2010, 1, 1), new TimeStamp(timestamp).ToDateTime());

            //testing timestamp series (exchange item 1), last record
            timestamp = (ITimeStamp)linkableTimeSeriesGroup.GetDiscreteTime(linkableTimeSeriesGroup.GetOutputExchangeItem(1).Quantity, linkableTimeSeriesGroup.GetOutputExchangeItem(1).ElementSet, 5);
            Assert.AreEqual(new System.DateTime(2010, 1, 11), new TimeStamp(timestamp).ToDateTime());
        }

        [TestMethod()]
        public void ExpectedExceptions()
        {
            string str = "";
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            try
            {
                string modelID = linkableTimeSeriesGroup.ModelID;
            }
            catch (System.Exception exception)
            {
                str = string.Copy(exception.Message);
            }
            
            Assert.AreEqual("property \"ModelID\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked", str);

            try
            {
                ITimeSpan timeHorizon = linkableTimeSeriesGroup.TimeHorizon;
            }
            catch (System.Exception exception)
            {
                str = string.Copy(exception.Message);
            }

            Assert.AreEqual("property \"TimeHorizon\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked", str);

            try
            {
                ITimeStamp earlistNeededTime = linkableTimeSeriesGroup.EarliestInputTime;
            }
            catch (System.Exception exception)
            {
                str = string.Copy(exception.Message);
            }

            Assert.AreEqual("property \"EarliestInputTime\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked", str);

            try
            {
                IValueSet valueSet = linkableTimeSeriesGroup.GetValues(new TimeStamp(new System.DateTime(2010, 1, 1)), "LinkID");
            }
            catch (System.Exception exception)
            {
                str = string.Copy(exception.Message);
            }

            Assert.AreEqual("Method \"GetValues\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked", str);

            try
            {
                int  discreteTimeCount = linkableTimeSeriesGroup.GetDiscreteTimesCount(new Quantity(), new ElementSet ());
            }
            catch (System.Exception exception)
            {
                str = string.Copy(exception.Message);
            }

            Assert.AreEqual("Method \"GetDiscreteTimesCount\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked", str);

            try
            {
                ITime discreteTime = linkableTimeSeriesGroup.GetDiscreteTime(new Quantity(), new ElementSet(), 2);
            }
            catch (System.Exception exception)
            {
                str = string.Copy(exception.Message);
            }

            Assert.AreEqual("Method \"GetDiscreteTime\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked", str);
        }

        [TestMethod()]
        public void GetPublishedEventTypeCount()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual(5, linkableTimeSeriesGroup.GetPublishedEventTypeCount());
        }

        [TestMethod()]
        public void GetPublishedEventType()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.Initialize(arguments);
            Assert.AreEqual(EventType.SourceAfterGetValuesCall, linkableTimeSeriesGroup.GetPublishedEventType(0));
            Assert.AreEqual(EventType.SourceBeforeGetValuesReturn, linkableTimeSeriesGroup.GetPublishedEventType(1));
            Assert.AreEqual(EventType.Informative, linkableTimeSeriesGroup.GetPublishedEventType(2));
            Assert.AreEqual(EventType.TargetBeforeGetValuesCall, linkableTimeSeriesGroup.GetPublishedEventType(3));
            Assert.AreEqual(EventType.TargetAfterGetValuesReturn, linkableTimeSeriesGroup.GetPublishedEventType(4));
        }

        [TestMethod()]
        public void SaveOmiFile()
        {
            LinkableTimeSeriesGroup linkableTimeSeriesGroup = new LinkableTimeSeriesGroup();
            linkableTimeSeriesGroup.WriteOmiFile(filename);
        }
    

    }
}
