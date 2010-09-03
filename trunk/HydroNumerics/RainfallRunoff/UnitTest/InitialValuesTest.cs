using System;
using HydroNumerics.RainfallRunoff;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.RainfallRunoff.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for InitialValuesTest and is intended
    ///to contain all InitialValuesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class InitialValuesTest
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
        ///A test for BaseFlow
        ///</summary>
        [TestMethod()]
        public void BaseFlowTest()
        {
            bool exceptionWasThrown = false;
            InitialValues initialValues = new InitialValues();
            try
            {
                double baseFlow = initialValues.BaseFlow;
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
                initialValues.BaseFlow = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            initialValues.BaseFlow = 10;
            Assert.AreEqual(10, initialValues.BaseFlow);

        }

        /// <summary>
        ///A test for Interflow
        ///</summary>
        [TestMethod()]
        public void InterflowTest()
        {
            bool exceptionWasThrown = false;
            InitialValues initialValues = new InitialValues();
            try
            {
                double Interflow = initialValues.InterFlow;
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
                initialValues.InterFlow = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            initialValues.InterFlow = 10;
            Assert.AreEqual(10, initialValues.InterFlow);
        }

        /// <summary>
        ///A test for OverlandFlow
        ///</summary>
        [TestMethod()]
        public void OverlandFlowTest()
        {
            bool exceptionWasThrown = false;
            InitialValues initialValues = new InitialValues();
            try
            {
                double OverlandFlow = initialValues.OverlandFlow;
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
                initialValues.OverlandFlow = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            initialValues.OverlandFlow = 10;
            Assert.AreEqual(10, initialValues.OverlandFlow);
        }

        /// <summary>
        ///A test for RootZoneStorage
        ///</summary>
        [TestMethod()]
        public void RootZoneStorageTest()
        {
            bool exceptionWasThrown = false;
            InitialValues initialValues = new InitialValues();
            try
            {
                double RootZoneStorage = initialValues.RootZoneStorage;
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
                initialValues.RootZoneStorage = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            initialValues.RootZoneStorage = 10;
            Assert.AreEqual(10, initialValues.RootZoneStorage);
        }

        /// <summary>
        ///A test for SurfaceStorage
        ///</summary>
        [TestMethod()]
        public void SurfaceStorageTest()
        {
            bool exceptionWasThrown = false;
            InitialValues initialValues = new InitialValues();
            try
            {
                double SurfaceStorage = initialValues.SurfaceStorage;
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
                initialValues.SurfaceStorage = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            initialValues.SurfaceStorage = 10;
            Assert.AreEqual(10, initialValues.SurfaceStorage);
        }

        /// <summary>
        ///A test for SnowStorage
        ///</summary>
        [TestMethod()]
        public void SnowStorageTest()
        {
            bool exceptionWasThrown = false;
            InitialValues initialValues = new InitialValues();
            try
            {
                double SnowStorage = initialValues.SnowStorage;
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
                initialValues.SnowStorage = -10;
            }
            catch (Exception ex)
            {
                //-- Expected exception out of rage assignment 
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);

            initialValues.SnowStorage = 10;
            Assert.AreEqual(10, initialValues.SnowStorage);
        }
    }
}
