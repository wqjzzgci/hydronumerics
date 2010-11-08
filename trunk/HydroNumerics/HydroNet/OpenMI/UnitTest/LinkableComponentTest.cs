using System;
using OpenMI.Standard;
using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.HydroNet.Core;
using HydroNumerics.HydroNet.OpenMI;
using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.HydroNet.OpenMI.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for LinkableComponentTest and is intended
    ///to contain all LinkableComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LinkableComponentTest
    {

        private TestContext testContextInstance;
        const string filename = "HydroNetLakeModel";

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
        public void LinkedToTestComponent()
        {

            CreateHydroNetInputfile();

            HydroNumerics.OpenMI.Sdk.Backbone.OmiFileParser omiFileParser = new HydroNumerics.OpenMI.Sdk.Backbone.OmiFileParser();
            omiFileParser.ReadOmiFile(filename + ".omi");

            IArgument[] arguments = omiFileParser.GetArgumentsAsIArgumentArray();

            // Create the HydroNet linkableComponent
            LinkableComponent hydroNetLC = new LinkableComponent();
            hydroNetLC.Initialize(arguments);
            Assert.AreEqual("Lake model", hydroNetLC.ModelID);

            // Create the TestLinkableComponent
            TestLinkableComponent testLC = new TestLinkableComponent();
            testLC.Initialize(new HydroNumerics.OpenMI.Sdk.Backbone.Argument[0] { });

            // Link (hydroNet to TestLC) (leakage --> infiltration)
            IOutputExchangeItem leakageOutputExchangeItem = hydroNetLC.GetOutputExchangeItem(0);
            HydroNumerics.OpenMI.Sdk.Backbone.Link hydroNetLC2testLC = new HydroNumerics.OpenMI.Sdk.Backbone.Link();
            hydroNetLC2testLC.ID = "HydroNet2TestLC";
            hydroNetLC2testLC.SourceComponent = hydroNetLC;
            hydroNetLC2testLC.TargetComponent = testLC;
            hydroNetLC2testLC.SourceElementSet = hydroNetLC.GetOutputExchangeItem(0).ElementSet;
            hydroNetLC2testLC.SourceQuantity = hydroNetLC.GetOutputExchangeItem(0).Quantity;
            Assert.AreEqual("Leakage", hydroNetLC2testLC.SourceQuantity.ID);
            hydroNetLC2testLC.TargetElementSet = testLC.GetOutputExchangeItem(0).ElementSet;
            hydroNetLC2testLC.TargetQuantity = testLC.GetInputExchangeItem(0).Quantity;
            hydroNetLC2testLC.AddDataOperation(hydroNetLC.GetOutputExchangeItem(0).GetDataOperation(3));

            // Link (TestLC to hydroNet) (groundwaterhead --> groundwaterhead)
            HydroNumerics.OpenMI.Sdk.Backbone.Link testLC2HydroNetLC = new HydroNumerics.OpenMI.Sdk.Backbone.Link();
            testLC2HydroNetLC.ID = "testLC2HydroNetLc";
            testLC2HydroNetLC.SourceComponent = testLC;
            testLC2HydroNetLC.TargetComponent = hydroNetLC;
            testLC2HydroNetLC.SourceQuantity = testLC.GetOutputExchangeItem(0).Quantity;
            testLC2HydroNetLC.SourceElementSet = testLC.GetOutputExchangeItem(0).ElementSet;
            testLC2HydroNetLC.TargetQuantity = hydroNetLC.GetInputExchangeItem(0).Quantity;
            Assert.AreEqual("Head", testLC2HydroNetLC.TargetQuantity.ID); //check to see if this is the right quantity
            testLC2HydroNetLC.TargetElementSet = hydroNetLC.GetInputExchangeItem(0).ElementSet;
            testLC2HydroNetLC.AddDataOperation(testLC.GetOutputExchangeItem(0).GetDataOperation(3));
            testLC.AddLink(testLC2HydroNetLC);
            hydroNetLC.AddLink(testLC2HydroNetLC);

            hydroNetLC.AddLink(hydroNetLC2testLC);
            testLC.AddLink(hydroNetLC2testLC);

            // triggerlink
            HydroNumerics.OpenMI.Sdk.DevelopmentSupport.Trigger trigger = new HydroNumerics.OpenMI.Sdk.DevelopmentSupport.Trigger();
            trigger.Initialize(new HydroNumerics.OpenMI.Sdk.Backbone.Argument[0] { });
            HydroNumerics.OpenMI.Sdk.Backbone.Link triggerLink = new HydroNumerics.OpenMI.Sdk.Backbone.Link();
            triggerLink.ID = "TriggerLink";
            triggerLink.SourceComponent = testLC;
            triggerLink.TargetComponent = trigger;
            triggerLink.SourceQuantity = testLC.GetOutputExchangeItem(0).Quantity;
            triggerLink.SourceElementSet = testLC.GetOutputExchangeItem(0).ElementSet;
            triggerLink.TargetQuantity = trigger.GetInputExchangeItem(0).Quantity;
            triggerLink.TargetElementSet = trigger.GetInputExchangeItem(0).ElementSet;

            testLC.AddLink(triggerLink);
            trigger.AddLink(triggerLink);

            // prepare
            trigger.Prepare();
            hydroNetLC.Prepare();
            testLC.Prepare();

            ScalarSet scalarSet =  (ScalarSet) hydroNetLC.GetValues(new TimeStamp(new DateTime(2001, 6, 1)), hydroNetLC2testLC.ID);
            double expected = -0.0000065836086956521805 * 2.0;
            Assert.AreEqual(0, scalarSet.data[0], 0.000000000001);
            Assert.AreEqual(0, scalarSet.data[1], 0.000000000001);
            Assert.AreEqual(expected, scalarSet.data[2], 0.000000000001);
            Assert.AreEqual(0, scalarSet.data[3], 0.000000000001);

            // Run
            //trigger.Run(testLC.TimeHorizon.End);

            //TimestampSeries infiltTSS = ((TimestampSeries) testLC.TestEngine.Infiltrations.Items[0]);

           
            trigger.Finish();
            hydroNetLC.Finish();
            testLC.Finish();

            trigger.Dispose();
            hydroNetLC.Dispose();
            testLC.Dispose();
        }

        /// <summary>
        /// This test method is only testing the HydroNet model. Så this test has nothing to do with OpenMI.
        /// </summary>
        [TestMethod]
        public void TestHydroNetModel()
        {
            Model model = CreateHydroNetModel();
            Lake lake = (Lake) model._waterBodies[0];
            GroundWaterBoundary groundWaterBoundary = (GroundWaterBoundary)model._waterBodies[0].GroundwaterBoundaries[0];
            double areaOfGroundwaterboundary = ((HydroNumerics.Geometry.XYPolygon)groundWaterBoundary.ContactGeometry).GetArea();

            double handcalckLeakage = -groundWaterBoundary.HydraulicConductivity * areaOfGroundwaterboundary * (lake.WaterLevel - groundWaterBoundary.GroundwaterHead) / groundWaterBoundary.Distance;
            double expected = -0.0000065836086956521805;
            Assert.AreEqual(expected, handcalckLeakage, 0.000000000001);
            Assert.AreEqual(expected, groundWaterBoundary.CurrentFlowRate, 0.000000000001);

            model.Update(new DateTime(2001, 6, 2));

            Assert.AreEqual(expected, handcalckLeakage, 0.000000000001);
            Assert.AreEqual(expected, groundWaterBoundary.CurrentFlowRate, 0.000000000001);

        }

        private Model CreateHydroNetModel()
        {
            // Upper Lake configuration
            Lake lake = new Lake("The Lake", 1000);
            lake.WaterLevel = 5.1;

            SinkSourceBoundary inflow = new SinkSourceBoundary(2);
            inflow.Name = "Inflow to lake";


            HydroNumerics.Geometry.XYPolygon contactPolygon = new HydroNumerics.Geometry.XYPolygon();
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(350, 625));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(447, 451));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(715, 433));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(863, 671));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(787, 823));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(447, 809));
            GroundWaterBoundary groundWaterBoundary = new GroundWaterBoundary();
            groundWaterBoundary.Connection = lake;
            groundWaterBoundary.ContactGeometry = contactPolygon;
            groundWaterBoundary.Distance = 2.3;
            groundWaterBoundary.HydraulicConductivity = 1e-9;
            groundWaterBoundary.GroundwaterHead = 5.0;
            groundWaterBoundary.Name = "Groundwater boundary under Lake";
            groundWaterBoundary.Name = "MyGWBoundary";

            lake.Sources.Add(inflow);
            lake.GroundwaterBoundaries.Add(groundWaterBoundary);

            //Creating the model
            Model model = new Model();
            model._waterBodies.Add(lake);

            DateTime startTime = new DateTime(2000, 1, 1);
            model.SetState("MyState", startTime, new WaterPacket(1000));
            lake.SetState("MyState", startTime, new WaterPacket(2));
            model.Name = "Lake model";
            model.Initialize();
            model.Update(new DateTime(2001, 1, 1));
            return model;
        }

        [TestMethod]
        public void CreateHydroNetInputfile()
        {

            Model model = CreateHydroNetModel();
            model.Save(filename + ".xml");

            LinkableComponent linkableHydroNet = new LinkableComponent();
            linkableHydroNet.WriteOmiFile(filename + ".xml", 86400);

        }

        [TestMethod()]
        public void WriteTestLinkableComponentOMIFile()
        {
            TestLinkableComponent testLinkableComponent = new TestLinkableComponent();
            testLinkableComponent.WriteOmiFile();
        }
    }
}
