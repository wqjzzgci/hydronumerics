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

        private RiverReduction CreateRiverReductionObject()
        {
            const string xmlfilename = @"..\..\..\HydroNumerics.Nitrate.Model\config.xml";
            XElement configuration = XDocument.Load(xmlfilename).Element("Configuration");
            XElement xmlRiverReduction = configuration.Elements("InternalReductionModels").Elements("ReductionModel").First(var => var.Attribute("Type").Value == "RiverReduction");
            RiverReduction riverReduction = new RiverReduction(xmlRiverReduction);
            return riverReduction;
        }

        [TestMethod()]
        public void GetStreamDepthTest()
        {
     
            RiverReduction riverReduction = CreateRiverReductionObject();


            Assert.AreEqual(0.21, riverReduction.GetStreamDepth(RiverReduction.StreamWidth.Narrow, RiverReduction.Season.Winter));
            Assert.AreEqual(0.17, riverReduction.GetStreamDepth(RiverReduction.StreamWidth.Narrow, RiverReduction.Season.Summer));

            Assert.AreEqual(0.54, riverReduction.GetStreamDepth(RiverReduction.StreamWidth.Itermediate, RiverReduction.Season.Winter));
            Assert.AreEqual(0.44, riverReduction.GetStreamDepth(RiverReduction.StreamWidth.Itermediate, RiverReduction.Season.Summer));

            Assert.AreEqual(1.2, riverReduction.GetStreamDepth(RiverReduction.StreamWidth.Large, RiverReduction.Season.Winter));
            Assert.AreEqual(1.1, riverReduction.GetStreamDepth(RiverReduction.StreamWidth.Large, RiverReduction.Season.Summer));
        }


        [TestMethod()]
        public void GetGetVelocityTest()
        {
   
            RiverReduction riverReduction = CreateRiverReductionObject();
                       
            Assert.AreEqual(0.22, riverReduction.GetVelocity(RiverReduction.StreamWidth.Narrow, RiverReduction.Season.Winter));
            Assert.AreEqual(0.18, riverReduction.GetVelocity(RiverReduction.StreamWidth.Narrow, RiverReduction.Season.Summer));

            Assert.AreEqual(0.37, riverReduction.GetVelocity(RiverReduction.StreamWidth.Itermediate, RiverReduction.Season.Winter));
            Assert.AreEqual(0.30, riverReduction.GetVelocity(RiverReduction.StreamWidth.Itermediate, RiverReduction.Season.Summer));

            Assert.AreEqual(0.48, riverReduction.GetVelocity(RiverReduction.StreamWidth.Large, RiverReduction.Season.Winter));
            Assert.AreEqual(0.35, riverReduction.GetVelocity(RiverReduction.StreamWidth.Large, RiverReduction.Season.Summer));
        }


        [TestMethod()]
        public void GetSeasonTest()
        {

            RiverReduction riverReduction = CreateRiverReductionObject();

            Assert.AreEqual(RiverReduction.Season.Winter, riverReduction.GetSeason(new DateTime(2012, 1, 1)));
            Assert.AreEqual(RiverReduction.Season.Winter, riverReduction.GetSeason(new DateTime(2012, 2, 1)));
            Assert.AreEqual(RiverReduction.Season.Winter, riverReduction.GetSeason(new DateTime(2012, 3, 1)));
            Assert.AreEqual(RiverReduction.Season.Summer, riverReduction.GetSeason(new DateTime(2012, 4, 1)));
            Assert.AreEqual(RiverReduction.Season.Summer, riverReduction.GetSeason(new DateTime(2012, 5, 1)));
            Assert.AreEqual(RiverReduction.Season.Summer, riverReduction.GetSeason(new DateTime(2012, 6, 1)));
            Assert.AreEqual(RiverReduction.Season.Summer, riverReduction.GetSeason(new DateTime(2012, 7, 1)));
            Assert.AreEqual(RiverReduction.Season.Summer, riverReduction.GetSeason(new DateTime(2012, 8, 1)));
            Assert.AreEqual(RiverReduction.Season.Summer, riverReduction.GetSeason(new DateTime(2012, 9, 1)));
            Assert.AreEqual(RiverReduction.Season.Summer, riverReduction.GetSeason(new DateTime(2012, 10, 1)));
            Assert.AreEqual(RiverReduction.Season.Winter, riverReduction.GetSeason(new DateTime(2012, 11, 1)));
            Assert.AreEqual(RiverReduction.Season.Winter, riverReduction.GetSeason(new DateTime(2012, 12, 1)));

           
        }

        [TestMethod()]
        public void StreamLengthFactorTest()
        {
            RiverReduction riverReduction = CreateRiverReductionObject();
            Assert.AreEqual(0.25, riverReduction.StreamLengthFactor);
        }

        [TestMethod()]
        public void ReductionEquationFactorTest()
        {
            RiverReduction riverReduction = CreateRiverReductionObject();
            Assert.AreEqual(74.61, riverReduction.ReductionEquationFactor);
        }

        [TestMethod()]
        public void ReductionEquationPowerTest()
        {
            RiverReduction riverReduction = CreateRiverReductionObject();
            Assert.AreEqual(-0.344, riverReduction.ReductionEquationPower);
        }
    }
}
