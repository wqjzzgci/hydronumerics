using HydroNumerics.RainfallRunoff;
using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.RainfallRunoff.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for NamTest and is intended
    ///to contain all NamTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NamTest
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


        [TestMethod]
        public void Step()
        {
            Nam nam = new Nam();

            nam.InitialValues.SnowStorage = 0;
            nam.InitialValues.SnowStorage = 0;
            nam.InitialValues.RootZoneStorage = 220;
            nam.InitialValues.OverlandFlow = 0;
            nam.InitialValues.InterFlow = 0;
            nam.InitialValues.BaseFlow = 0.6;

            nam.Parameters.CatchmentArea = 50000;

            nam.Parameters.SnowmeltCoefficient = 0.2;
            nam.Parameters.OverlandFlowCoefficient = 0;
            nam.Parameters.OverlandFlowTreshold = 12;

            nam.Parameters.SurfaceStorageCapacity = 12;
            nam.Parameters.RootZoneStorageCapacity = 30;

            double x = nam.Parameters.InterflowTreshold; 

            
            

            nam.Parameters.OverlandFlowTimeConstant = 0.5;
            nam.Parameters.InterflowTimeConstant = 1.1;
            nam.Parameters.BaseflowTimeConstant = 3.2;

        }
        
        /// <summary>
        ///A test for PerformTimeStep
        ///</summary>
        [TestMethod()]
        public void PerformTimeStepTest()
        {

            TimespanSeries precipitationTs = new TimespanSeries();
            precipitationTs.Unit.Dimension.Length = 1;
            precipitationTs.Unit.Dimension.Time = 1;
            precipitationTs.Unit = new HydroNumerics.Core.Unit("mm pr. day", 1/(24.0*3600*1000),0);
            precipitationTs.Name = "Precipitation";
            precipitationTs.AddValue(new System.DateTime(2010, 1, 1), new System.DateTime(2010, 1, 2), 1.2);
            precipitationTs.AddValue(new System.DateTime(2010, 1, 2), new System.DateTime(2010, 1, 3), 3.2);
            precipitationTs.AddValue(new System.DateTime(2010, 1, 3), new System.DateTime(2010, 1, 4), 2.3);

            TimespanSeries potentialEvaporationTs = new TimespanSeries();
            potentialEvaporationTs.Unit.Dimension.Length = 1;
            potentialEvaporationTs.Unit.Dimension.Time = 1;
            potentialEvaporationTs.Unit = new HydroNumerics.Core.Unit("mm pr. day", 1.0 / (24.0 * 3600 * 1000), 0);
            potentialEvaporationTs.Name = "Potential Evaporation";
            potentialEvaporationTs.AddValue(new System.DateTime(2010, 1, 1), new System.DateTime(2010, 1, 2), 0.16);
            potentialEvaporationTs.AddValue(new System.DateTime(2010, 1, 2), new System.DateTime(2010, 1, 3), 0.5);
            potentialEvaporationTs.AddValue(new System.DateTime(2010, 1, 3), new System.DateTime(2010, 1, 4), 0.3);

            TimespanSeries temperatureTs = new TimespanSeries();
            temperatureTs.Unit.Dimension.Temperature = 1;
            temperatureTs.Unit = new HydroNumerics.Core.Unit("Centigrade", 1.0, -273.15);
            temperatureTs.AddValue(new System.DateTime(2010, 1, 1), new System.DateTime(2010, 1, 2), -2.2);
            temperatureTs.AddValue(new System.DateTime(2010, 1, 2), new System.DateTime(2010, 1, 3), 0.1);
            temperatureTs.AddValue(new System.DateTime(2010, 1, 3), new System.DateTime(2010, 1, 4), 2.9);

            Nam nam = new Nam(); 

            nam.PrecipitationTs = precipitationTs;
            nam.PotentialEvaporationTs = potentialEvaporationTs;
            nam.TemperatureTs = temperatureTs;

            //nam.InitialSnowStorage = 0;
            //nam.InitialSurfaceStorage = 0;
            //nam.InitialRootZoneStorage = 220;
            //nam.InitialOverlandFlow = 0;
            //nam.InitialInterflow = 0;
            //nam.InitialBaseFlow = 0.6;

            nam.CatchmentArea = 160; //TODO: wrong unit..
            nam.SnowmeltCoefficient = 2;
            nam.SurfaceStorageCapacity = 18;
            nam.RootZoneStorageCapacity = 250;
            nam.OverlandFlowTimeConstant = 0.61;
            nam.InterflowCoefficient = 1.0 / 870;
            nam.OverlandFlowTreshold = 0.38;
            nam.InterflowTreshold = 0.08;
            nam.BaseFlowTreshold = 0.25;
            nam.BaseflowTimeConstant = 2800;
            
            




            nam.SimulationStartTime = new System.DateTime(2010, 1, 1);
            nam.SimulationEndTime = new System.DateTime(2010, 1, 10);
            

            nam.Initialize();

            nam.PerformTimeStep();
            nam.PerformTimeStep();
            nam.PerformTimeStep();

              
        }
    }
}
