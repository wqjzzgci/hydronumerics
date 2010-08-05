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

            FlowBoundary inflow = new FlowBoundary(2);

            GroundWaterBoundary groundWaterBoundary = new GroundWaterBoundary();
            groundWaterBoundary.Connection = lake;
            HydroNumerics.Geometry.XYPolygon contactPolygon = new HydroNumerics.Geometry.XYPolygon();
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(10,10));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(20, 10));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(20, 20));
            contactPolygon.Points.Add(new HydroNumerics.Geometry.XYPoint(10, 20));
            groundWaterBoundary.ContactArea = contactPolygon;
            groundWaterBoundary.Distance = 2.0;
            groundWaterBoundary.Head = 3.4;
            groundWaterBoundary.HydraulicConductivity = 1e-4;
            groundWaterBoundary.Name = "MyGWBoundary";
 

            lake.SinkSources.Add(inflow);
            lake.SinkSources.Add(groundWaterBoundary);

            //Creating the model
            Model model = new Model();
            model._waterBodies.Add(lake);
   
            DateTime startTime = new DateTime(2010, 1, 1);
            model.SetState("MyState", startTime, new WaterPacket(1000));
            lake.SetState("MyState", startTime, new WaterPacket(2));
            model.Name = "HydroNet test model";
            model.Save("HydroNetLakeModel.xml");
        }
    }
}
