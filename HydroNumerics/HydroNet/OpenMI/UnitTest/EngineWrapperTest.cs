using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.HydroNet.OpenMI;

namespace HydroNumerics.HydroNet.OpenMI.UnitTest
{
    /// <summary>
    /// Summary description for EngineWrapperTest
    /// </summary>
    [TestClass]
    public class EngineWrapperTest
    {
        string testDataPath; 
        System.Collections.Hashtable arguments;

        public EngineWrapperTest()
        {
            testDataPath = @"..\..\..\TestData\";
            arguments = new System.Collections.Hashtable();
            string inputFilename = testDataPath + "setup.xml";
            arguments.Add("InputFilename", inputFilename);
            arguments.Add("TimestepLength", "3600");
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Initialize()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
        }

        [TestMethod]
        public void GetComponentID()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual("HydroNet", engineWrapper.GetComponentID());
        }

        [TestMethod]
        public void GetComponentDescription()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual("Conceptual model for trasport of water and solutes", engineWrapper.GetComponentDescription());
        }

        [TestMethod]
        public void GetModelID()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual("Vedsted-opsætning", engineWrapper.GetModelID());
        }

        [TestMethod]
        public void GetModelDescription()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            Assert.AreEqual("No modeldescription available", engineWrapper.GetModelDescription());
        }

        [TestMethod]
        public void Finish()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            engineWrapper.Finish();
        }

        [TestMethod]
        public void PerformTimeStep()
        {
            EngineWrapper engineWrapper = new EngineWrapper();
            engineWrapper.Initialize(arguments);
            engineWrapper.PerformTimeStep();
            engineWrapper.Finish();
            
        }
    }
}
