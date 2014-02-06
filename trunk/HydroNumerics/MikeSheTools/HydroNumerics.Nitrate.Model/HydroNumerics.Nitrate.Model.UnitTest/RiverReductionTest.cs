using HydroNumerics.Nitrate.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for RiverReductionTest and is intended
    ///to contain all RiverReductionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RiverReductionTest
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
        ///A test for GetStreamDepth
        ///</summary>
        [TestMethod()]
        //[DeploymentItem("HydroNumerics.Nitrate.Model.exe")]
        public void GetStreamDepthTest()
        {
            string xmlfilename = @"C:\Users\Gregersen\Documents\MyDocs\source\HydroNumerics\HydroNumerics\MikeSheTools\HydroNumerics.Nitrate.Model\config.xml ";
            XElement configuration = XDocument.Load(xmlfilename).Element("Configuration");
            XElement xmlRiverReduction = configuration.Elements("InternalReductionModels").Elements("ReductionModel").First(var => var.Attribute("Type").Value == "RiverReduction");
            RiverReduction riverReduction = new RiverReduction(xmlRiverReduction);
            double depth = riverReduction.GetStreamDepth(RiverReduction.StreamWidth.Narrow, RiverReduction.Season.Summer);
            Assert.AreEqual(0.17, depth);




            //PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            //RiverReduction_Accessor target = new RiverReduction_Accessor(param0); // TODO: Initialize to an appropriate value
            //RiverReduction_Accessor.StreamWidth streamWidth = ; // TODO: Initialize to an appropriate value
            //RiverReduction_Accessor.Season season = null; // TODO: Initialize to an appropriate value
            //XElement Configuration = null; // TODO: Initialize to an appropriate value
            //double expected = 0F; // TODO: Initialize to an appropriate value
            //double actual;
            //actual = target.GetStreamDepth(streamWidth, season, Configuration);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
