
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.OpenMI.UnitTest
{
    /// <summary>
    /// Summary description for Demo
    /// </summary>
    [TestClass]
    public class Demo
    {
        public Demo()
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
        public void TestMethod1()
        {


        }

        private Model CreateHydroNetModel()
        {
            // Upper Lake configuration
            Lake upperLake = new Lake("Upper Lake", 1000);
            upperLake.WaterLevel = 6.1;
            upperLake.Output.LogAllChemicals = true;

            Lake lowerLake = new Lake("lower Lake", 1000);
            lowerLake.WaterLevel = 6.0;
            lowerLake.Output.LogAllChemicals = true;

            Stream stream = new Stream("The stream", 2000, 2, 1);
            stream.WaterLevel = 6.0;
            stream.Output.LogAllChemicals = true;

            SinkSourceBoundary inflow = new SinkSourceBoundary(0);
            inflow.Name = "Inflow to lake";

            // -------- Groundwater boundary under upper lake ----------
            HydroNumerics.Geometry.XYPolygon contactPolygonUpperLake = new HydroNumerics.Geometry.XYPolygon();
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(535.836, 2269.625));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(675.768, 2187.713));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(771.331, 2177.474));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(887.372, 2184.300));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(935.154, 2255.973));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(945.392, 2385.666));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(931.741, 2505.119));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(877.133, 2546.075));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(812.287, 2638.225));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(696.246, 2675.768));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(638.225, 2627.986));
            contactPolygonUpperLake.Points.Add(new HydroNumerics.Geometry.XYPoint(573.379, 2587.031));

            GroundWaterBoundary groundWaterBoundaryUpperLake = new GroundWaterBoundary();
            groundWaterBoundaryUpperLake.Connection = upperLake;
            groundWaterBoundaryUpperLake.ContactGeometry = contactPolygonUpperLake;
            groundWaterBoundaryUpperLake.Distance = 2.3;
            groundWaterBoundaryUpperLake.HydraulicConductivity = 1e-9;
            groundWaterBoundaryUpperLake.GroundwaterHead = 5.0;
            groundWaterBoundaryUpperLake.Name = "Groundwater boundary under UpperLake";
            groundWaterBoundaryUpperLake.Name = "UpperGWBoundary";
            ((WaterPacket)groundWaterBoundaryUpperLake.WaterSample).AddChemical(ChemicalFactory.Instance.GetChemical(ChemicalNames.Radon), 2.3);
            ((WaterPacket)groundWaterBoundaryUpperLake.WaterSample).AddChemical(ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl), 2.3);

            // -------- Groundwater boundary under lower lake ----------
            HydroNumerics.Geometry.XYPolygon contactPolygonLowerLake = new HydroNumerics.Geometry.XYPolygon();
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1935.154, 1150.171));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1901.024, 1058.020));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1877.133, 965.870));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1894.198, 897.611));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1938.567, 808.874));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2023.891, 761.092));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2116.041, 740.614));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2232.082, 747.440));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2327.645, 808.874));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2389.078, 969.283));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2372.014, 1109.215));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2262.799, 1218.430));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(2105.802, 1235.495));
            contactPolygonLowerLake.Points.Add(new HydroNumerics.Geometry.XYPoint(1982.935, 1225.256));

            GroundWaterBoundary groundWaterBoundaryLowerLake = new GroundWaterBoundary();
            groundWaterBoundaryLowerLake.Connection = lowerLake;
            groundWaterBoundaryLowerLake.ContactGeometry = contactPolygonLowerLake;
            groundWaterBoundaryLowerLake.Distance = 2.3;
            groundWaterBoundaryLowerLake.HydraulicConductivity = 1e-9;
            groundWaterBoundaryLowerLake.GroundwaterHead = 5.0;
            groundWaterBoundaryLowerLake.Name = "Groundwater boundary under LowerLake";
            groundWaterBoundaryLowerLake.Name = "LowerGWBoundary";

            upperLake.Sources.Add(inflow);
            upperLake.GroundwaterBoundaries.Add(groundWaterBoundaryUpperLake);
            upperLake.DownStreamConnections.Add(stream);
                     
            lowerLake.GroundwaterBoundaries.Add(groundWaterBoundaryLowerLake);

            stream.DownStreamConnections.Add(lowerLake);

            //Creating the model
            Model model = new Model();
            model._waterBodies.Add(upperLake);
            model._waterBodies.Add(stream);
            model._waterBodies.Add(lowerLake);
            

            DateTime startTime = new DateTime(2000, 1, 1);
            model.SetState("MyState", startTime, new WaterPacket(1000));
            //upperLake.SetState("MyState", startTime, new WaterPacket(2));
            model.Name = "Lake model";
            model.Initialize();
            //model.Update(new DateTime(2001, 1, 1));
            return model;
        }

        [TestMethod]
        public void CreateHydroNetInputfile()
        {

            string filename = "DemoHydroNet";
            Model model = CreateHydroNetModel();
            model.Save(filename + ".xml");

            LinkableComponent linkableHydroNet = new LinkableComponent();
            linkableHydroNet.WriteOmiFile(filename + ".xml", 86400);

        }

        //[TestMethod()]
        //public void WriteTestLinkableComponentOMIFile()
        //{
        //    TestLinkableComponent testLinkableComponent = new TestLinkableComponent();
        //    testLinkableComponent.WriteOmiFile();
        //}
    }
}
