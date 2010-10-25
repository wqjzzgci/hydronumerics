using System;
using OpenMI.Standard;
using HydroNumerics.HydroNet.Core;
using HydroNumerics.HydroNet.OpenMI;
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
        ///This test will write the OMI file for HydroNet.LinkableComponent, then read it again, subtract information and
        ///Instantiate the HydroNet.LinkableComponent class with argument from the file.
        ///</summary>
        [TestMethod()]
        public void WriteOmiFileTest()
        {
            string filename = "HydroNetFile";
            CreateHydroNetFile(filename + ".xml");
            LinkableComponent linkableComponent = new LinkableComponent();
            linkableComponent.WriteOmiFile(filename + ".xml", 100.0);

            HydroNumerics.OpenMI.Sdk.Backbone.OmiFileParser omiFileParser = new HydroNumerics.OpenMI.Sdk.Backbone.OmiFileParser();
            omiFileParser.ReadOmiFile(filename + ".omi");

            IArgument[] arguments = omiFileParser.GetArgumentsAsIArgumentArray();
                        
            LinkableComponent linkableComponentNew = new LinkableComponent();
            linkableComponent.Initialize(arguments);
            Assert.AreEqual("HydroNet test model", linkableComponent.ModelID);
        }

        [TestMethod()]
        public void LinkedToTestComponent()
        {
            // Write the OMI file and read the arguments
            string filename = "HydroNetFile";
            CreateHydroNetFile(filename + ".xml");
            LinkableComponent linkableComponent = new LinkableComponent();
            linkableComponent.WriteOmiFile(filename + ".xml", 100.0);

            HydroNumerics.OpenMI.Sdk.Backbone.OmiFileParser omiFileParser = new HydroNumerics.OpenMI.Sdk.Backbone.OmiFileParser();
            omiFileParser.ReadOmiFile(filename + ".omi");

            IArgument[] arguments = omiFileParser.GetArgumentsAsIArgumentArray();

            // Create the HydroNet linkableComponent
            LinkableComponent hydroNetLC = new LinkableComponent();
            hydroNetLC.Initialize(arguments);
            Assert.AreEqual("HydroNet test model", hydroNetLC.ModelID);

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

            // Run
            trigger.Run(testLC.TimeHorizon.End);

            trigger.Finish();
            hydroNetLC.Finish();
            testLC.Finish();

            trigger.Dispose();
            hydroNetLC.Dispose();
            testLC.Dispose();
        }

        private void CreateHydroNetFile(string filename)
        {
            

            Model model = CreateHydroNetModel();
            model.Save(filename);
        }

        private Model CreateHydroNetModel()
        {
            // Upper Lake configuration
            Lake upperLake = new Lake("Upper Lake", 1000);

            //Simple inflow boundary
            SinkSourceBoundary inflow = new SinkSourceBoundary(2);
            inflow.Name = "Inflow to Upper lake";
            inflow.ContactGeometry = new HydroNumerics.Geometry.XYPoint(350, 625);
            upperLake.Sources.Add(inflow);

            //Ground water boundary
            HydroNumerics.Geometry.XYPolygon contactPolygon = new HydroNumerics.Geometry.XYPolygon();
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(350, 625));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(447, 451));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(715, 433));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(863, 671));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(787, 823));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(447, 809));
            GroundWaterBoundary groundWaterBoundary = new GroundWaterBoundary();
            groundWaterBoundary.Connection = upperLake;
            groundWaterBoundary.ContactGeometry = contactPolygon;
            groundWaterBoundary.Distance = 2.3;
            groundWaterBoundary.HydraulicConductivity = 1e-4;
            groundWaterBoundary.GroundwaterHead = 3.4;
            groundWaterBoundary.Name = "Groundwater boundary under Upper Lake";
            upperLake.GroundwaterBoundaries.Add(groundWaterBoundary);

            //Stream between the lakes
            Stream stream = new Stream("stream", 2000, 2, 1.1);

            //Lower Lake configuration
            Lake lowerLake = new Lake("Lower Lake", 20);

            //Connecting the waterbodies.
            upperLake.AddDownStreamWaterBody(stream);
            stream.AddDownStreamWaterBody(lowerLake);

            //Creating the model
            Model model = new Model();
            model._waterBodies.Add(upperLake);
            model._waterBodies.Add(stream);
            model._waterBodies.Add(lowerLake);

            DateTime startTime = new DateTime(2010, 1, 1);
            model.SetState("MyState", startTime, new WaterPacket(1000));
            upperLake.SetState("MyState", startTime, new WaterPacket(2));
            model.Name = "HydroNet test model";
            model.Initialize();

            return model;
        }

        [TestMethod()]
        public void WriteTestLinkableComponentOMIFile()
        {
            TestLinkableComponent testLinkableComponent = new TestLinkableComponent();
            testLinkableComponent.WriteOmiFile();
            //testLinkableComponent.Initialize(new HydroNumerics.OpenMI.Sdk.Backbone.Argument[0] { });
            //IOutputExchangeItem outputExchangeItem = testLinkableComponent.GetOutputExchangeItem(0);
            //IInputExchangeItem inputExchangteItem = testLinkableComponent.GetInputExchangeItem(0);
        }
    }
}
