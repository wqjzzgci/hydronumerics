using System;
using HydroNumerics.OpenMI.Sdk.Backbone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.OpenMI.Sdk.Backbone.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for RegularGridTest and is intended
    ///to contain all RegularGridTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RegularGridTest
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
        public void GetXCoordinate()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);

            Assert.AreEqual(100.0, regularGrid.GetXCoordinate(0, 0));
            Assert.AreEqual(110.0, regularGrid.GetXCoordinate(0, 1));
            Assert.AreEqual(110.0, regularGrid.GetXCoordinate(0, 2));
            Assert.AreEqual(100.0, regularGrid.GetXCoordinate(0, 3));

            Assert.AreEqual(120.0, regularGrid.GetXCoordinate(11, 0));
            Assert.AreEqual(130.0, regularGrid.GetXCoordinate(11, 1));
            Assert.AreEqual(130.0, regularGrid.GetXCoordinate(11, 2));
            Assert.AreEqual(120.0, regularGrid.GetXCoordinate(11, 3));


        }

        [TestMethod()]
        public void GetYCoordinate()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual(230.0, regularGrid.GetYCoordinate(0, 0));
            Assert.AreEqual(230.0, regularGrid.GetYCoordinate(0, 1));
            Assert.AreEqual(240.0, regularGrid.GetYCoordinate(0, 2));
            Assert.AreEqual(240.0, regularGrid.GetYCoordinate(0, 3));

            Assert.AreEqual(200.0, regularGrid.GetYCoordinate(11, 0));
            Assert.AreEqual(200.0, regularGrid.GetYCoordinate(11, 1));
            Assert.AreEqual(210.0, regularGrid.GetYCoordinate(11, 2));
            Assert.AreEqual(210.0, regularGrid.GetYCoordinate(11, 3));

        }

        [TestMethod()]
        public void GetZCoordinate()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            bool exceptionWasThrown = false;
            try
            {
                regularGrid.GetZCoordinate(5, 0);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
                exceptionWasThrown = true;

            }
            Assert.IsTrue(exceptionWasThrown);
        }


        [TestMethod()]
        public void GetElementID()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual("105_235", regularGrid.GetElementID(0));
            Assert.AreEqual("125_205", regularGrid.GetElementID(11));

        }

        [TestMethod()]
        public void Description()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual("Regular Grid", regularGrid.Description);
        }

        [TestMethod()]
        public void ID()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual("RegularGrid", regularGrid.ID);
        }

        [TestMethod()]
        public void GetFaceVertexIndices()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual(null, regularGrid.GetFaceVertexIndices(0, 0));
        }

        [TestMethod()]
        public void GetFaceCount()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual(0, regularGrid.GetFaceCount(0));
            Assert.AreEqual(0, regularGrid.GetFaceCount(11));
        }

        [TestMethod()]
        public void ElementType()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual(global::OpenMI.Standard.ElementType.XYPolygon, regularGrid.ElementType);
        }

        [TestMethod()]
        public void Version()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual(0, regularGrid.Version);
        }

        [TestMethod()]
        public void ElementCount()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);

            Assert.AreEqual(12, regularGrid.ElementCount);
        }

        [TestMethod()]
        public void GetVertexCount()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual(4, regularGrid.GetVertexCount(0));
            Assert.AreEqual(4, regularGrid.GetVertexCount(11));
        }

        [TestMethod()]
        public void SpatialReference()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual(new SpatialReference("no reference"), regularGrid.SpatialReference);
        }

        [TestMethod()]
        public void GetElementIndex()
        {
            RegularGrid regularGrid = new RegularGrid(100, 200, 10, 3, 4, 0.0);
            Assert.AreEqual(0, regularGrid.GetElementIndex("105_235"));
            Assert.AreEqual(11, regularGrid.GetElementIndex("125_205"));
        }

        [TestMethod()]
        public void CreateSmallElementSets()
        {
            RegularGrid regularGrid = new RegularGrid(10, 10, 20, 2, 2, 0);
            regularGrid = new RegularGrid(10, 10, 20, 1, 2, 0);
            regularGrid = new RegularGrid(10, 10, 20, 2, 1, 0);
            regularGrid = new RegularGrid(10, 10, 20, 1, 1, 0);
        }
      
    }
}
