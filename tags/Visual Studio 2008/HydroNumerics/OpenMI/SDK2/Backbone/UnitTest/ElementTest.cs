using HydroNumerics.OpenMI.Sdk.Backbone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.OpenMI.Sdk2.Backbone.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ElementTest and is intended
    ///to contain all ElementTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ElementTest
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
            Element element = new Element("ElementID");
            element.AddVertex(new Coordinate(3.0, 4.0, 5.0));
            element.AddVertex(new Coordinate(4.0, 5.0, 6.0));
            element.AddVertex(new Coordinate(5.0, 6.0, 7.0));
            Assert.AreEqual("ElementID", element.Id);

            Element element2 = new Element(element);

            Assert.AreEqual(element, element2);
        }

        [TestMethod()]
        public void ID()
        {
            Element element = new Element();
            element.Id = "ElementID";
            Assert.AreEqual("ElementID", element.Id);
        }

        [TestMethod()]
        public void Vertices()
        {
            Element element = new Element();

            element.AddVertex(new Coordinate(3.0, 4.0, 5.0));
            element.AddVertex(new Coordinate(4.0, 5.0, 6.0));
            element.AddVertex(new Coordinate(5.0, 6.0, 7.0));

            Assert.AreEqual(3, element.VertexCount);
            Assert.AreEqual(new Coordinate(3.0, 4.0, 5.0), element.Vertices[0]);
            Assert.AreEqual(new Coordinate(4.0, 5.0, 6.0), element.Vertices[1]);
            Assert.AreEqual(new Coordinate(5.0, 6.0, 7.0), element.Vertices[2]);
        }

        [TestMethod()]
        public void Faces()
        {
            int[] index = { 1, 2, 3, 4, 5 };
            Element element = new Element();
            element.AddFace(index);
            Assert.AreEqual(1, element.FaceCount);
            Assert.AreEqual(index, element.GetFaceVertexIndices(0));
        }

        [TestMethod()]
        public void Equals()
        {
            Element element1 = new Element("ElementID");

            element1.AddVertex(new Coordinate(3.0, 4.0, 5.0));
            element1.AddVertex(new Coordinate(4.0, 5.0, 6.0));
            element1.AddVertex(new Coordinate(5.0, 6.0, 7.0));

            Element element2 = new Element("ElementID");

            element2.AddVertex(new Coordinate(3.0, 4.0, 5.0));
            element2.AddVertex(new Coordinate(4.0, 5.0, 6.0));

            Assert.IsFalse(element1.Equals(element2));

            element2.AddVertex(new Coordinate(5.0, 6.0, 7.0));

            Assert.IsTrue(element1.Equals(element2));

            element1.Id = "ElementID1";

            Assert.IsFalse(element1.Equals(element2));

            Assert.IsFalse(element1.Equals(null));
            Assert.IsFalse(element1.Equals("string"));


        }
    }
}
