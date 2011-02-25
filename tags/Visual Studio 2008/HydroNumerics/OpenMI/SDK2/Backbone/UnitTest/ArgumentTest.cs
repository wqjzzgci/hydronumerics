using HydroNumerics.OpenMI.Sdk.Backbone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMI.Standard2;
namespace HydroNumerics.OpenMI.Sdk2.Backbone.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ArgumentTest and is intended
    ///to contain all ArgumentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArgumentTest
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
        public void Constructor()
        {
            IArgument param = new ArgumentString("key", "value");
            Assert.AreEqual("key", param.Caption);
            Assert.AreEqual("value", param.Value);

            IArgument param2 = new ArgumentString(param);
            Assert.AreEqual(param, param2);
        }

        [TestMethod()]
        public void Key()
        {
            IArgument param = new ArgumentString("OperationKey");
            Assert.AreEqual("OperationKey", param.Caption);
        }

        [TestMethod()]
        public void Value()
        {
            IArgument param = new ArgumentString("id1", "OperationValue");
            Assert.AreEqual("OperationValue", param.Value);
        }

        [TestMethod()]
        public void ReadOnly()
        {
            ArgumentString param = new ArgumentString("Fred");
            param.IsReadOnly = true;
            Assert.AreEqual(true, param.IsReadOnly);
            param.IsReadOnly = false;
            Assert.AreEqual(false, param.IsReadOnly);
        }

        [TestMethod()]
        public void Description()
        {
            IArgument param = new ArgumentString("Fred");
            param.Description = "Description";
            Assert.AreEqual("Description", param.Description);
        }

        [TestMethod()]
        public void Equals()
        {
            IArgument param1 = new ArgumentString("key", "value");
            IArgument param2 = new ArgumentString("key", "value");

            Assert.IsTrue(param1.Equals(param2));
            param1.Caption = "key1";
            Assert.IsFalse(param1.Equals(param2));
            param1.Caption = "key";
            param1.Value = "value1";
            Assert.IsFalse(param1.Equals(param2));

            Assert.IsFalse(param1.Equals(null));
            Assert.IsFalse(param1.Equals("string"));
        }

    }
}
