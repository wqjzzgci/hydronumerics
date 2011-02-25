using HydroNumerics.OpenMI.Sdk.Backbone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.OpenMI.Sdk2.Backbone.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ExchangeItemTest and is intended
    ///to contain all ExchangeItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExchangeItemTest
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
        public void ElementSet()
        {
            Output output = new Output("ES.Q", new Quantity("Q"), new ElementSet("ES"));
            ElementSet elementSet = new ElementSet("ES");
            Assert.IsTrue(elementSet.Equals(output.SpatialDefinition));
        }

        [TestMethod()]
        public void Quantity()
        {
            Output output = new Output("ES.Q", new Quantity("Q"), new ElementSet("ES"));
            Assert.IsTrue(output.ValueDefinition.Equals(new Quantity("Q")));
        }

        [TestMethod()]
        public void Equals()
        {
            Output output = new Output("ES.Q", new Quantity("Q"), new ElementSet("ES"));
            
            ElementSet elementSet = new ElementSet("ES");
            Output exchangeItem2 = new Output("ES.Q", new Quantity("Q"), elementSet);

            Assert.IsTrue(exchangeItem2.Equals(output));

            exchangeItem2.ValueDefinition = new Quantity("Q1");
            Assert.IsFalse(exchangeItem2.Equals(output));
            exchangeItem2.ValueDefinition = new Quantity("Q");
            elementSet.Caption = "ES2";

            Assert.IsFalse(exchangeItem2.Equals(output));
            Assert.IsFalse(exchangeItem2.Equals(null));
            Assert.IsFalse(exchangeItem2.Equals("string"));
        }
    }
}
