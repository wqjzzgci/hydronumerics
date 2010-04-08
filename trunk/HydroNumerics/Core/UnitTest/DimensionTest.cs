using HydroNumerics.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.Core.UnitTest
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
        public void SetPowerTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.AmountOfSubstance, 5);
            dimension.SetPower(DimensionBase.Currency, 6);
            dimension.SetPower(DimensionBase.ElectricCurrent, 7);
            dimension.SetPower(DimensionBase.Length, 8);
            dimension.SetPower(DimensionBase.LuminousIntensity, 9);
            dimension.SetPower(DimensionBase.Mass, 10);
            dimension.SetPower(DimensionBase.Temperature, 11);
            dimension.SetPower(DimensionBase.Time, 12);
            Assert.AreEqual(5,dimension.GetPower(DimensionBase.AmountOfSubstance));
            Assert.AreEqual(6, dimension.GetPower(DimensionBase.Currency));
            Assert.AreEqual(7, dimension.GetPower(DimensionBase.ElectricCurrent));
            Assert.AreEqual(8, dimension.GetPower(DimensionBase.Length));
            Assert.AreEqual(9, dimension.GetPower(DimensionBase.LuminousIntensity));
            Assert.AreEqual(10, dimension.GetPower(DimensionBase.Mass));
            Assert.AreEqual(11, dimension.GetPower(DimensionBase.Temperature));
            Assert.AreEqual(12, dimension.GetPower(DimensionBase.Time));
        }

        [TestMethod()]
        public void GetPowerTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.AmountOfSubstance, 5);
            dimension.SetPower(DimensionBase.Currency, 6);
            dimension.SetPower(DimensionBase.ElectricCurrent, 7);
            dimension.SetPower(DimensionBase.Length, 8);
            dimension.SetPower(DimensionBase.LuminousIntensity, 9);
            dimension.SetPower(DimensionBase.Mass, 10);
            dimension.SetPower(DimensionBase.Temperature, 11);
            dimension.SetPower(DimensionBase.Time, 12);
            Assert.AreEqual(5, dimension.GetPower(DimensionBase.AmountOfSubstance));
            Assert.AreEqual(6, dimension.GetPower(DimensionBase.Currency));
            Assert.AreEqual(7, dimension.GetPower(DimensionBase.ElectricCurrent));
            Assert.AreEqual(8, dimension.GetPower(DimensionBase.Length));
            Assert.AreEqual(9, dimension.GetPower(DimensionBase.LuminousIntensity));
            Assert.AreEqual(10, dimension.GetPower(DimensionBase.Mass));
            Assert.AreEqual(11, dimension.GetPower(DimensionBase.Temperature));
            Assert.AreEqual(12, dimension.GetPower(DimensionBase.Time));
        }


        [TestMethod()]
        public void EqualsTest()
        {
            Dimension dimensionA;
            Dimension dimensionB;
            dimensionA = new Dimension(1, 2, 3, 4, 5, 6, 7, 8);
            dimensionB = new Dimension(1, 2, 3, 4, 5, 6, 7, 8);
            Assert.IsTrue(dimensionA.Equals(dimensionB));
            dimensionB = new Dimension(0, 2, 3, 4, 5, 6, 7, 8);
            Assert.IsFalse(dimensionA.Equals(dimensionB));
            dimensionB = new Dimension(1, 0, 3, 4, 5, 6, 7, 8);
            Assert.IsFalse(dimensionA.Equals(dimensionB));
            dimensionB = new Dimension(1, 2, 0, 4, 5, 6, 7, 8);
            Assert.IsFalse(dimensionA.Equals(dimensionB));
            dimensionB = new Dimension(1, 2, 3, 0, 5, 6, 7, 8);
            Assert.IsFalse(dimensionA.Equals(dimensionB));
            dimensionB = new Dimension(1, 2, 3, 4, 0, 6, 7, 8);
            Assert.IsFalse(dimensionA.Equals(dimensionB));
            dimensionB = new Dimension(1, 2, 3, 4, 5, 0, 7, 8);
            Assert.IsFalse(dimensionA.Equals(dimensionB));
            dimensionB = new Dimension(1, 2, 3, 4, 5, 6, 0, 8);
            Assert.IsFalse(dimensionA.Equals(dimensionB));
            dimensionB = new Dimension(1, 2, 3, 4, 5, 6, 7, 0);
            Assert.IsFalse(dimensionA.Equals(dimensionB));

        }

        [TestMethod()]
        public void DimensionConstructorTest()
        {
            Dimension dimension = new Dimension();
            Assert.AreEqual(0, dimension.Length);
            Assert.AreEqual(0, dimension.AmountOfSubstance);
            Assert.AreEqual(0, dimension.Currency);
            Assert.AreEqual(0, dimension.ElectricCurrent);
            Assert.AreEqual(0, dimension.LuminousIntensity);
            Assert.AreEqual(0, dimension.Mass);
            Assert.AreEqual(0, dimension.Temperature);
            Assert.AreEqual(0, dimension.Time);
        }

        [TestMethod()]
        public void DimensionConstructorTest1()
        {
            Dimension dimension = new Dimension(1, 6, 8, 4, 7, 2, 5, 3);
            Assert.AreEqual(1, dimension.Length);
            Assert.AreEqual(2, dimension.AmountOfSubstance);
            Assert.AreEqual(3, dimension.Currency);
            Assert.AreEqual(4, dimension.ElectricCurrent);
            Assert.AreEqual(5, dimension.LuminousIntensity);
            Assert.AreEqual(6, dimension.Mass);
            Assert.AreEqual(7, dimension.Temperature);
            Assert.AreEqual(8, dimension.Time);
        }

        [TestMethod()]
        public void DimensionConstructorTest2()
        {
            Dimension dimensionA = new Dimension(1, 2, 3, 4, 5, 6, 7, 8);
            Dimension dimensionB = new Dimension(dimensionA);
            Assert.IsTrue(dimensionA.Equals(dimensionB));
        }

        [TestMethod()]
        public void TimeTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.Time, -4);
            Assert.AreEqual(-4, dimension.Time);
        }

        [TestMethod()]
        public void TemperatureTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.Temperature, -4);
            Assert.AreEqual(-4, dimension.Temperature);
        }

        [TestMethod()]
        public void MassTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.Mass, -4);
            Assert.AreEqual(-4, dimension.Mass);
        }


        [TestMethod()]
        public void LuminousIntensityTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.LuminousIntensity, -4);
            Assert.AreEqual(-4, dimension.LuminousIntensity);
        }

        [TestMethod()]
        public void LengthTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.Length, -4);
            Assert.AreEqual(-4, dimension.Length);
        }

        [TestMethod()]
        public void ElectricCurrentTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.ElectricCurrent, -4);
            Assert.AreEqual(-4, dimension.ElectricCurrent);
        }

        [TestMethod()]
        public void CurrencyTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.Currency, -4);
            Assert.AreEqual(-4, dimension.Currency);

        }

        [TestMethod()]
        public void AmountOfSubstanceTest()
        {
            Dimension dimension = new Dimension();
            dimension.SetPower(DimensionBase.AmountOfSubstance, -4);
            Assert.AreEqual(-4, dimension.AmountOfSubstance);
        }
    
    }
}
