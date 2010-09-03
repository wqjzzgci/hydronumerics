using System;
using HydroNumerics.RainfallRunoff;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.RainfallRunoff.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ParametersTest and is intended
    ///to contain all ParametersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ParametersTest
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
        ///A test for SurfaceStorageCapacity
        ///</summary>
        [TestMethod()]
        public void SurfaceStorageCapacityTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double surfaceStorageCapacity = parameters.SurfaceStorageCapacity;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.SurfaceStorageCapacity = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.SurfaceStorageCapacity = 10;
            Assert.AreEqual(10, parameters.SurfaceStorageCapacity);
        }

        /// <summary>
        ///A test for SnowmeltCoefficient
        ///</summary>
        [TestMethod()]
        public void SnowmeltCoefficientTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double SnowmeltCoefficient = parameters.SnowmeltCoefficient;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.SnowmeltCoefficient = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.SnowmeltCoefficient = 10;
            Assert.AreEqual(10, parameters.SnowmeltCoefficient);
        }

        /// <summary>
        ///A test for RootZoneStorageCapacity
        ///</summary>
        [TestMethod()]
        public void RootZoneStorageCapacityTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double RootZoneStorageCapacity = parameters.RootZoneStorageCapacity;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.RootZoneStorageCapacity = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.RootZoneStorageCapacity = 10;
            Assert.AreEqual(10, parameters.RootZoneStorageCapacity);
        }

        /// <summary>
        ///A test for OverlandFlowTreshold
        ///</summary>
        [TestMethod()]
        public void OverlandFlowTresholdTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double OverlandFlowTresholdTest = parameters.OverlandFlowTreshold;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.OverlandFlowTreshold = 10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;


            try
            {
                parameters.OverlandFlowTreshold = -0.5;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.OverlandFlowTreshold = 0.5;
            Assert.AreEqual(0.5, parameters.OverlandFlowTreshold);
        }

        /// <summary>
        ///A test for OverlandFlowTimeConstant
        ///</summary>
        [TestMethod()]
        public void OverlandFlowTimeConstantTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double OverlandFlowTimeConstant = parameters.OverlandFlowTimeConstant;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.OverlandFlowTimeConstant = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.OverlandFlowTimeConstant = 10;
            Assert.AreEqual(10, parameters.OverlandFlowTimeConstant);
        }

        /// <summary>
        ///A test for OverlandFlowCoefficient
        ///</summary>
        [TestMethod()]
        public void OverlandFlowCoefficientTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double OverlandFlowCoefficient = parameters.OverlandFlowCoefficient;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.OverlandFlowCoefficient = 10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.OverlandFlowCoefficient = -0.5;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.OverlandFlowCoefficient = 0.5;
            Assert.AreEqual(0.5, parameters.OverlandFlowCoefficient);
        }

        /// <summary>
        ///A test for InterflowTreshold
        ///</summary>
        [TestMethod()]
        public void InterflowTresholdTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double InterflowTreshold = parameters.InterflowTreshold;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.InterflowTreshold = 10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.InterflowTreshold = -0.5;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.InterflowTreshold = 0.5;
            Assert.AreEqual(0.5, parameters.InterflowTreshold);
        }

        /// <summary>
        ///A test for InterflowTimeConstant
        ///</summary>
        [TestMethod()]
        public void InterflowTimeConstantTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double InterflowTimeConstant = parameters.InterflowTimeConstant;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.InterflowTimeConstant = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.InterflowTimeConstant = 10;
            Assert.AreEqual(10, parameters.InterflowTimeConstant);
        }

        /// <summary>
        ///A test for InterflowCoefficient
        ///</summary>
        [TestMethod()]
        public void InterflowCoefficientTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double InterflowCoefficient = parameters.InterflowCoefficient;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.InterflowCoefficient = 10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.InterflowCoefficient = -0.5;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.InterflowCoefficient = 0.5;
            Assert.AreEqual(0.5, parameters.InterflowCoefficient);
        }

        ///// <summary>
        /////A test for BaseFlowTreshold
        /////</summary>
        //[TestMethod()]
        //public void BaseFlowTresholdTest()
        //{
        //    bool exceptionWasThrown = false;
        //    Parameters parameters = new Parameters();
        //    try
        //    {
        //        double BaseFlowTreshold = parameters.BaseFlowTreshold;
        //    }
        //    catch (Exception ex)
        //    {
        //        //-- Expected exception for unassigned property
        //        Assert.IsTrue(ex.GetType() == typeof(Exception));
        //        exceptionWasThrown = true;
        //    }
        //    Assert.IsTrue(exceptionWasThrown);
        //    exceptionWasThrown = false;

        //    try
        //    {
        //        parameters.BaseFlowTreshold = -10;
        //    }
        //    catch (Exception ex)
        //    {
        //        //-- Expected exception out of rage assignment 
        //        Assert.IsTrue(ex.GetType() == typeof(Exception));
        //        exceptionWasThrown = true;
        //    }
        //    Assert.IsTrue(exceptionWasThrown);

        //    parameters.BaseFlowTreshold = 10;
        //    Assert.AreEqual(10, parameters.BaseFlowTreshold);
        //}

        /// <summary>
        ///A test for BaseflowTimeConstant
        ///</summary>
        [TestMethod()]
        public void BaseflowTimeConstantTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double BaseflowTimeConstant = parameters.BaseflowTimeConstant;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.BaseflowTimeConstant = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.BaseflowTimeConstant = 10;
            Assert.AreEqual(10, parameters.BaseflowTimeConstant);
        }

        /// <summary>
        ///A test for CatchmentArea
        ///</summary>
        [TestMethod()]
        public void CatchmentAreaTest()
        {
            bool exceptionWasThrown = false;
            Parameters parameters = new Parameters();
            try
            {
                double CatchmentArea = parameters.CatchmentArea;
            }
            catch (Exception ex)
            {
                //-- Expected exception for unassigned property
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
            exceptionWasThrown = false;

            try
            {
                parameters.CatchmentArea = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            parameters.CatchmentArea = 10;
            Assert.AreEqual(10, parameters.CatchmentArea);
        }
    }
}
