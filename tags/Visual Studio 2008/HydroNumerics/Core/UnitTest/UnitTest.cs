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
        private bool propertyChanged = false;
        private string changedPropertyName = " ";

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
            try
            {
                unit.ConversionFactorToSI = 0;
            }
            catch (System.Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(System.Exception));
            }
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

            try
            {
                unitA = new Unit("something", 0, 17.4, "somedescription", new Dimension(1, 2, 3, 4, 5, 6, 7, 8));
            }
            catch (System.Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(System.Exception));
            }
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

            try
            {
                unitA = new Unit("something", 0, 17.4, "somedescription");
            }
            catch (System.Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(System.Exception));
            }
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

            try
            {
                unit = new Unit("something", 0, 17.4);
            }
            catch (System.Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(System.Exception));
            }
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

        [TestMethod()]
        public void FromSiToThisUnit()
        {
            Unit thisUnit = new Unit("ThisUnit", 10, 5);
            Assert.AreEqual(1.2, thisUnit.FromSiToThisUnit(17.0));
        }

        [TestMethod]
        public void ToSiUnit()
        {
            Unit thisUnit = new Unit("ThisUnit", 10, 5);
            Assert.AreEqual(175, thisUnit.ToSiUnit(17));
        }

        [TestMethod]
        public void FromUnitToThisUnit()
        {
            Unit thisUnit = new Unit("ThisUnit", 10, 5);
            Unit fromUnit = new Unit("FromUnit", 3, 8);
            Assert.AreEqual(5.4, thisUnit.FromUnitToThisUnit(17.0, fromUnit));
        }

        [TestMethod]
        public void FromThisUnitToUnit()
        {
            Unit thisUnit = new Unit("ThisUnit", 10, 6);
            Unit toUnit = new Unit("toUnit", 4, 8);
            Assert.AreEqual(42.0, thisUnit.FromThisUnitToUnit(17.0, toUnit));
        }

        [TestMethod()]
        public void PropertyChangedEvent()
        {
            Unit unit = new Unit("Test unit", 3.4, 9.3);
            unit.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);

            propertyChanged = false; changedPropertyName = "";
            unit.ConversionFactorToSI = 22.2;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("ConversionFactorToSI", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            unit.OffSetToSI = 2.2;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("OffSetToSI", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            unit.ID = "new unit name";
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("ID", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            unit.Description = "new description";
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("Description", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            unit.Dimension = new Dimension(1, 1, 1, 1, 1, 1, 1, 1);
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("Dimension", changedPropertyName);

            propertyChanged = false; changedPropertyName = "";
            unit.Dimension.AmountOfSubstance = 6;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("AmountOfSubstance", changedPropertyName);

            unit = new Unit();
            unit.PropertyChanged+=new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);
            propertyChanged = false; changedPropertyName = "";
            unit.Dimension.AmountOfSubstance = 6;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("AmountOfSubstance", changedPropertyName);

            unit = new Unit(new Unit());
            unit.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);
            propertyChanged = false; changedPropertyName = "";
            unit.Dimension.AmountOfSubstance = 6;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("AmountOfSubstance", changedPropertyName);

            unit = new Unit("id", 3.4, 3.5);
            unit.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);
            propertyChanged = false; changedPropertyName = "";
            unit.Dimension.AmountOfSubstance = 6;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("AmountOfSubstance", changedPropertyName);

            unit = new Unit("id", 4.5, 3.2, "description");
            unit.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);
            propertyChanged = false; changedPropertyName = "";
            unit.Dimension.AmountOfSubstance = 6;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("AmountOfSubstance", changedPropertyName);

            unit = new Unit("id", 4.3, 2.1, "description", new Dimension());
            unit.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(unit_PropertyChanged);
            propertyChanged = false; changedPropertyName = "";
            unit.Dimension.AmountOfSubstance = 6;
            Assert.IsTrue(propertyChanged);
            Assert.AreEqual("AmountOfSubstance", changedPropertyName);

        }

        void unit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            propertyChanged = true;
            changedPropertyName = e.PropertyName;
        }
     
    }
}
