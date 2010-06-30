using HydroNumerics.OpenMI.Sdk.Backbone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.OpenMI.Sdk.Backbone.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for OmiFileParserTest and is intended
    ///to contain all OmiFileParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OmiFileParserTest
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
        ///A test for ReadOmiFile
        ///</summary>
        [TestMethod()]
        public void ReadOmiFile()
        {
            WriteOmiFile();
            OmiFileParser omiFileParserReader = new OmiFileParser();
            omiFileParserReader.ReadOmiFile("test.omi");
            Assert.AreEqual("MyNamespace.MyModel.LinkableComponent", omiFileParserReader.LinkableComponentClassName);
            Assert.AreEqual(@"..\..\bin\debug\MyAssemble.dll", omiFileParserReader.AssemblyName);
            Assert.AreEqual("MyOmiFile.omi", omiFileParserReader.Arguments["Filename"]);
            Assert.AreEqual("3600", omiFileParserReader.Arguments["TimestepLength"]);
        }

        /// <summary>
        ///A test for WriteOmiFile
        ///</summary>
        [TestMethod()]
        public void WriteOmiFile()
        {
            OmiFileParser omiFileParserWriter = new OmiFileParser();
            omiFileParserWriter.LinkableComponentClassName = "MyNamespace.MyModel.LinkableComponent";
            omiFileParserWriter.AssemblyName = @"..\..\bin\debug\MyAssemble.dll";
            omiFileParserWriter.Arguments.Add("Filename", "MyOmiFile.omi");
            omiFileParserWriter.Arguments.Add("TimestepLength", "3600");
            omiFileParserWriter.WriteOmiFile("test.omi");
        }

        [TestMethod()]
        public void AssemblyName()
        {
            OmiFileParser omiFileParser = new OmiFileParser();
            omiFileParser.AssemblyName = "something";
            Assert.AreEqual("something", omiFileParser.AssemblyName);
        }

        [TestMethod()]
        public void LinkableComponentClassName()
        {
            OmiFileParser omiFileParser = new OmiFileParser();
            omiFileParser.LinkableComponentClassName = "something";
            Assert.AreEqual("something", omiFileParser.LinkableComponentClassName);
        }

        [TestMethod()]
        public void Arguments()
        {
            OmiFileParser omiFileParser = new OmiFileParser();
            omiFileParser.Arguments.Add("MyKey", "MyValue");
            Assert.AreEqual("MyValue", omiFileParser.Arguments["MyKey"]);
        }
    }
}
