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

        private void CreateHydroNetFile(string filename)
        {
            // Upper Lake configuration
            Lake upperLake = new Lake(1000);
            upperLake.Name = "Upper Lake";

            FlowBoundary inflow = new FlowBoundary(2);

            upperLake.SinkSources.Add(inflow);

            //Stream between the lakes
            Stream stream = new Stream(2000, 2, 1.1);

            //Lower Lake configuration
            Lake lowerLake = new Lake(20);
            lowerLake.Name = "Lower Lake";

            //Connecting the waterbodies.
            upperLake.DownStreamConnections.Add(stream);
            stream.DownStreamConnections.Add(lowerLake);

            //Creating the model
            Model model = new Model();
            model._waterBodies.Add(upperLake);
            model._waterBodies.Add(stream);
            model._waterBodies.Add(lowerLake);

            DateTime startTime = new DateTime(2010, 1, 1);
            model.SetState("MyState", startTime, new WaterPacket(1000));
            upperLake.SetState("MyState", startTime, new WaterPacket(2));
            model.Name = "HydroNet test model";

            model.Save(filename);
        }
    }
}
