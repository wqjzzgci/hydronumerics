using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.HydroNet.OpenMI;
using HydroNumerics.HydroNet.Core;
using HydroNumerics.Time.Core;
using OpenMI.Standard;

namespace HydroNumerics.HydroNet.OpenMI.UnitTest
{
    /// <summary>
    /// Summary description for MikeSheExample
    /// </summary>
    [TestClass]
    public class MikeSheExample
    {
        public MikeSheExample()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;
        private string filename = "HydroNetLakeModel";

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CreateHydroNetInputfile()
        {
            // Upper Lake configuration
            Lake lake = new Lake(1000);
            lake.Name = "The Lake";

            SinkSourceBoundary inflow = new SinkSourceBoundary(2);

            
            HydroNumerics.Geometry.XYPolygon contactPolygon = new HydroNumerics.Geometry.XYPolygon();
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(350,625));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(447,451));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(715,433));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(863, 671));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(787, 823));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(447, 809));
            //GroundWaterBoundary groundWaterBoundary = new GroundWaterBoundary(lake, 1e-4, 0, 2.0, 3.2); //TODO: ...
            GroundWaterBoundary groundWaterBoundary = new GroundWaterBoundary();
            groundWaterBoundary.Connection = lake;
            groundWaterBoundary.ContactGeometry = contactPolygon;
            groundWaterBoundary.Distance = 2.3;
            groundWaterBoundary.HydraulicConductivity = 1e-4;
            groundWaterBoundary.GroundwaterHead = 3.4;
            groundWaterBoundary.Name = "Groundwater boundary under Lake";
            groundWaterBoundary.Name = "MyGWBoundary";
 
            lake.Sources.Add(inflow);
            lake.GroundwaterBoundaries.Add(groundWaterBoundary);

            //Creating the model
            Model model = new Model();
            model._waterBodies.Add(lake);
   
            DateTime startTime = new DateTime(2010, 1, 1);
            model.SetState("MyState", startTime, new WaterPacket(1000));
            lake.SetState("MyState", startTime, new WaterPacket(2));
            model.Name = "HydroNet test model";
            model.Initialize();
           
            model.Save(filename+".xml");
            LinkableComponent linkableHydroNet = new LinkableComponent();
            linkableHydroNet.WriteOmiFile(filename+".xml", 100);
        }

        //[TestMethod]
        //public void GetOutputExchangeItems()
        //{
        //    CreateHydroNetInputfile();

        //}


        /// <summary>
        /// The purpose of this method is to create the OMI file, which can be used in the OpenMI configuration editor.
        /// The test also loads the OMI file, extracts the argument, and initializes the model, just to see that things works.
        /// </summary>
        [TestMethod]
        public void CreateHydroNetLinkableComponent()
        {
            CreateHydroNetInputfile();
            HydroNumerics.OpenMI.Sdk.Backbone.OmiFileParser omiFileParser = new HydroNumerics.OpenMI.Sdk.Backbone.OmiFileParser();
            omiFileParser.ReadOmiFile(filename + ".omi");

            IArgument[] arguments = omiFileParser.GetArgumentsAsIArgumentArray();

            LinkableComponent linkableHydroNet = new LinkableComponent();
            linkableHydroNet.Initialize(arguments);
            Assert.AreEqual("HydroNet test model", linkableHydroNet.ModelID);

            // check output exchange items, - moved to a list so debugging becoms easier
            List<IOutputExchangeItem> outputExchangeItems = new List<IOutputExchangeItem>();
            for (int i = 0; i < linkableHydroNet.OutputExchangeItemCount; i++)
            {
                outputExchangeItems.Add(linkableHydroNet.GetOutputExchangeItem(i));
            }

            // check input exchange items, - moved to a list so debugging becoms easier
            List<IInputExchangeItem> inputExchangeItems = new List<IInputExchangeItem>();
            for (int i = 0; i < linkableHydroNet.InputExchangeItemCount; i++)
            {
                inputExchangeItems.Add(linkableHydroNet.GetInputExchangeItem(i));
            }

            int kurt = 3;

        }
    }
}
