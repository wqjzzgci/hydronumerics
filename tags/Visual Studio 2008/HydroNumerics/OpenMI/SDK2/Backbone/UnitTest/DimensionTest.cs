using HydroNumerics.OpenMI.Sdk.Backbone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMI.Standard2;

namespace HydroNumerics.OpenMI.Sdk2.Backbone.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for DimensionTest and is intended
    ///to contain all DimensionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DimensionTest
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

        [TestMethod()]
        public void Power()
        {
            Dimension dimension;
            dimension = new Dimension();
            dimension.SetPower(DimensionBase.Length, 3);
            dimension.SetPower(DimensionBase.Time, -1);
            Assert.AreEqual(3, dimension.GetPower(DimensionBase.Length));
            Assert.AreEqual(-1, dimension.GetPower(DimensionBase.Time));
        }

        [TestMethod()]
        public void Equals()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.Length, 3);
            dimension.SetPower(DimensionBase.Time, -1);

            Dimension dimension2 = new Dimension();
            dimension2.SetPower(DimensionBase.Length, 3);
            dimension2.SetPower(DimensionBase.Time, -1);

            Assert.IsTrue(Dimension.DescribesSameAs(dimension, dimension2));
            dimension.SetPower(DimensionBase.Length, 2);
            Assert.IsFalse(Dimension.DescribesSameAs(dimension, dimension2));

            Assert.IsFalse(dimension.Equals("string"));
        }
      
    }
}
