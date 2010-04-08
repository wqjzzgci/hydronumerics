using HydroNumerics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for UnitTest and is intended
    ///to contain all UnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UnitTest
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
        ///A test for ID
        ///</summary>
        [TestMethod()]
        public void IDTest()
        {
            Unit unit = new Unit();
            unit.ID = "someUnitID";
            Assert.AreEqual("someUnitID", unit.ID);
        }

        /// <summary>
        ///A test for Description
        ///</summary>
        [TestMethod()]
        public void DescriptionTest()
        {
            Unit unit = new Unit();
            unit.Description = "someDescription";
            Assert.AreEqual("someDescription", unit.Description);
        }

        /// <summary>
        ///A test for ConversionFactorToSI
        ///</summary>
        [TestMethod()]
        public void ConversionFactorToSITest()
        {
            Unit unit = new Unit();
            unit.ConversionFactorToSI = 17;
            Assert.AreEqual(17, unit.ConversionFactorToSI);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            Unit unitA;
            Unit unitB;
            unitA = new Unit("something", 3.4, 17.4, "somedescription", new Dimension(1, 2, 3, 4, 5, 6, 7, 8));
            unitB = new Unit("something", 3.4, 17.4, "somedescription", new Dimension(1, 2, 3, 4, 5, 6, 7, 8));
            Assert.IsTrue(unitA.Equals(unitB));

            unitB = new Unit("somethingElse", 3.4, 17.4, "somedescription", new Dimension(1, 2, 3, 4, 5, 6, 7, 8));
            Assert.IsFalse(unitA.Equals(unitB));

            unitB = new Unit("something", 100, 17.4, "somedescription", new Dimension(1, 2, 3, 4, 5, 6, 7, 8));
            Assert.IsFalse(unitA.Equals(unitB));

            unitB = new Unit("something", 3.4, 100, "somedescription", new Dimension(1, 2, 3, 4, 5, 6, 7, 8));
            Assert.IsFalse(unitA.Equals(unitB));

            unitB = new Unit("something", 3.4, 17.4, "someOtherDescription", new Dimension(1, 2, 3, 4, 5, 6, 7, 8));
            Assert.IsFalse(unitA.Equals(unitB));

            unitB = new Unit("something", 3.4, 17.4, "somedescription", new Dimension(0, 2, 3, 4, 5, 6, 7, 8));
            Assert.IsFalse(unitA.Equals(unitB));
        }

        /// <summary>
        ///A test for Unit Constructor
        ///</summary>
        [TestMethod()]
        public void UnitConstructorTest4()
        {
            Unit unitA = new Unit("something", 3.4, 17.4, "somedescription",new Dimension(1,2,3,4,5,6,7,8));
            Unit unitB = new Unit(unitA);
            Assert.AreEqual("somedescription", unitB.Description);
            Assert.AreEqual(3.4, unitB.ConversionFactorToSI);
            Assert.AreEqual(17.4, unitB.OffSetToSI);
            Assert.IsTrue(unitB.Dimension.Equals(new Dimension(1, 2, 3, 4, 5, 6, 7, 8)));
            Assert.AreEqual("something", unitB.ID);
        }

        /// <summary>
        ///A test for Unit Constructor
        ///</summary>
        [TestMethod()]
        public void UnitConstructorTest3()
        {
            Unit unitA = new Unit("something", 3.4, 17.4, "somedescription");
            unitA.Dimension = new Dimension(1, 2, 3, 4, 5, 6, 7, 8);
            Unit unitB = new Unit(unitA);
            Assert.AreEqual("somedescription", unitB.Description);
            Assert.AreEqual(3.4, unitB.ConversionFactorToSI);
            Assert.AreEqual(17.4, unitB.OffSetToSI);
            Assert.IsTrue(unitB.Dimension.Equals(new Dimension(1, 2, 3, 4, 5, 6, 7, 8)));
            Assert.AreEqual("something", unitB.ID);
        }

        /// <summary>
        ///A test for Unit Constructor
        ///</summary>
        [TestMethod()]
        public void UnitConstructorTest2()
        {
            Unit unit = new Unit("something", 3.4, 17.4, "somedescription");
            Assert.AreEqual("somedescription", unit.Description);
            Assert.AreEqual(3.4, unit.ConversionFactorToSI);
            Assert.AreEqual(17.4, unit.OffSetToSI);
            Assert.IsTrue(unit.Dimension.Equals(new Dimension(0, 0, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual("something", unit.ID);
        }

        /// <summary>
        ///A test for Unit Constructor
        ///</summary>
        [TestMethod()]
        public void UnitConstructorTest1()
        {
            Unit unit = new Unit("something", 3.4, 17.4);
            Assert.AreEqual("", unit.Description);
            Assert.AreEqual(3.4, unit.ConversionFactorToSI);
            Assert.AreEqual(17.4, unit.OffSetToSI);
            Assert.IsTrue(unit.Dimension.Equals(new Dimension(0, 0, 0, 0, 0, 0, 0, 0)));
            Assert.AreEqual("something", unit.ID);
        }

        /// <summary>
        ///A test for Unit Constructor
        ///</summary>
        [TestMethod()]
        public void UnitConstructorTest()
        {
            Unit unit = new Unit();
            Assert.AreEqual("", unit.Description);
            Assert.AreEqual(1.0, unit.ConversionFactorToSI);
            Assert.AreEqual(0, unit.OffSetToSI);
            Assert.IsTrue(unit.Dimension.Equals(new Dimension(0,0,0,0,0,0,0,0)));
            Assert.AreEqual("", unit.ID);
        }

        /// <summary>
        ///A test for OffSetToSI
        ///</summary>
        [TestMethod()]
        public void OffSetToSITest()
        {
            Unit unit = new Unit();
            unit.OffSetToSI = 232;
            Assert.AreEqual(232, unit.OffSetToSI);
        }

     
    }
}
