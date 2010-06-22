using HydroNumerics.OpenMI.Sdk.Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMI.Standard;

namespace HydroNumerics.OpenMI.Sdk.Wrapper.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for SmartLinkTest and is intended
    ///to contain all SmartLinkTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DumDumTest
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


        internal virtual SmartLink_Accessor CreateSmartLink_Accessor()
        {
            // TODO: Instantiate an appropriate concrete class.
            SmartLink_Accessor target = null;
            return target;
        }

        /// <summary>
        ///A test for CompareDimensions
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HydroNumerics.OpenMI.Sdk.Wrapper.dll")]
        public void CompareDimensionsTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            SmartLink_Accessor target = new SmartLink_Accessor(param0); // TODO: Initialize to an appropriate value
            IDimension dimension1 = null; // TODO: Initialize to an appropriate value
            IDimension dimension2 = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.CompareDimensions(dimension1, dimension2);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
