using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.HydroNet.OpenMI;
using HydroNumerics.HydroNet.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.OpenMI.UnitTest
{
    /// <summary>
    /// Summary description for EngineWrapperTest
    /// </summary>
    [TestClass]
    public class EngineWrapperTest
    {
        //string testDataPath; 
        System.Collections.Hashtable arguments;
        string inputFilename;

        public EngineWrapperTest()
        {
            //testDataPath = @"..\..\..\TestData\";
            arguments = new System.Collections.Hashtable();
            inputFilename = "HydroNetFile.xml";
            arguments.Add("InputFilename", inputFilename);
            arguments.Add("TimestepLength", "2"); //2 seconds
        }

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
        // You can use the following additional attributes as you write your tests:
        //
         //Use ClassInitialize to run code before running the first test in the class
         //[ClassInitialize()]
         //public static void MyClassInitialize(TestContext testContext) 
         //{
            
         //}
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
         //Use TestInitialize to run code before running each test 
         [TestInitialize()]
         public void MyTestInitialize() 
         {
             Model model = CreateHydroNetModel();
             model.Save(inputFilename);
         }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        [TestMethod]
        public void SaveModel()
        {
            Model model = CreateHydroNetModel();
            model.Save(inputFilename);
        }
        
        [TestMethod]
        public void Initialize()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
        }

        [TestMethod]
        public void GetComponentID()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual("HydroNet", engineWrapper.GetComponentID());
        }

        [TestMethod]
        public void GetComponentDescription()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual("Conceptual model for trasport of water and solutes", engineWrapper.GetComponentDescription());
        }

        [TestMethod]
        public void GetModelID()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual("HydroNet test model", engineWrapper.GetModelID());
        }

        [TestMethod]
        public void GetModelDescription()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual("No modeldescription available", engineWrapper.GetModelDescription());
        }

        [TestMethod]
        public void Finish()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            engineWrapper.Finish();
        }

        [TestMethod]
        public void PerformTimeStep()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual(2, engineWrapper.HydroNetModel._waterBodies[0].CurrentStoredWater.Volume);
            engineWrapper.PerformTimeStep();
            engineWrapper.Finish();
            
        }

        //[TestMethod]
        //public void DummyTest()
        //{
        //    Model model = CreateHydroNetModel();
        //    double volume = model._waterBodies[0].CurrentStoredWater.Volume;
        //    for (int i = 0; i < 10; i++)
        //    {
        //        model.MoveInTime(new TimeSpan(0, 0, 10));
        //    }
          
        //}

        private Model CreateHydroNetModel()
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

            return model;
        }

        //private Model createSomethingElse()
        //{
        //    Lake upperLake = new Lake(10);
        //    upperLake.WaterLevel = 2;

        //    Lake lowerLake = new Lake(20);

        //    Stream stream01 = new Stream(200, 2, 1);

        //    upperLake.DownStreamConnections.Add(stream01);
        //    stream01.DownStreamConnections.Add(lowerLake);

        //    FlowBoundary inflow = new FlowBoundary(.5);
        //    inflow.WaterSample = new WaterPacket(3, 8);
            
        //    upperLake.SinkSources.Add(inflow);

        //    GroundWaterBoundary gwb1 = new GroundWaterBoundary(upperLake, 1.0e-4, upperLake.Area / 2, .5, upperLake.WaterLevel - 1);
        //    upperLake.SinkSources.Add(gwb1);

        //    EvaporationRateBoundary evapboundary = new EvaporationRateBoundary(0.0001);
        //    upperLake.EvaporationBoundaries.Add(evapboundary);

        //    Model model = new Model();
        //    model._waterBodies.Add(upperLake);
        //    model._waterBodies.Add(stream01);
        //    model._waterBodies.Add(lowerLake);

        //    return model;

        //}
    }
}
