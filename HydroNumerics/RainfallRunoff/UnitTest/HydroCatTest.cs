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
    public class HydroCatTest
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
            HydroCat hydroCat = new HydroCat();

            double[] precipitation = new double[365] { 0, 0, 2.3, 0.2, 0, 0.1, 0.1, 0, 0.3, 0.4, 0.1, 0.1, 1, 0, 0.5, 5.3, 1.9, 0.1, 0, 0.4, 1.1, 1.2, 2.6, 0, 1.5, 3.4, 6, 0.7, 4, 0.1, 0.6, 2, 0.4, 0.1, 0, 0, 0, 4.1, 0.2, 4, 0.3, 0.6, 3.1, 14.1, 3.9, 0.1, 3.7, 2.4, 4.3, 0, 0, 0, 0, 2.7, 1, 0.1, 0.3, 0.2, 0, 0.2, 8.7, 2, 1.8, 0.9, 0.1, 0.2, 0.1, 0.1, 0, 0, 0, 0, 0, 0, 0, 0.1, 0.1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.6, 1.8, 0, 1.1, 4.7, 2.4, 0, 0.1, 8.2, 4.6, 1, 0.1, 0, 0, 7.8, 2, 0.3, 0, 0.1, 0.1, 0, 0, 0.3, 0.1, 2.2, 2, 0, 0, 0, 0, 0, 0, 0, 0.1, 6.7, 0, 0, 0, 5.4, 2.2, 1, 10.8, 3.6, 3.1, 10.1, 0, 0.3, 2, 0, 0, 0, 0, 0, 0, 1.3, 0, 0, 7.6, 3.6, 0.1, 0, 0, 0, 0, 0.1, 4.5, 0, 12.8, 0, 4.4, 0, 0, 0, 0, 4.9, 0, 0.8, 0, 2.1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7.1, 0.2, 0, 0, 0, 0, 0, 0, 0, 0, 4.4, 1.5, 4.6, 1.4, 0.8, 0, 0, 0, 0, 5.5, 4.6, 1.3, 0.1, 2.8, 2.6, 0, 0.1, 3.5, 1, 8.7, 0.5, 0, 0.4, 0, 0, 0, 0, 3.8, 4.9, 4.6, 2.5, 7.9, 0.1, 4.7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.2, 14.4, 2, 6.2, 0.5, 0.3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3.2, 8.1, 1.3, 20.2, 13.5, 1.1, 1.3, 0, 0, 2.1, 13.7, 3.9, 0.5, 0.1, 0, 0, 0.1, 0, 0, 0.2, 4.8, 7.2, 0.4, 0, 4.6, 0.2, 0, 0, 0.5, 4.8, 9.8, 0.6, 0, 0, 0, 0, 3.2, 0, 0, 0, 0, 0.4, 0, 0, 0, 0, 0, 0.5, 8.5, 0.5, 1.9, 1.2, 0, 3.2, 1.7, 12.3, 1.9, 0, 4.3, 3.6, 1.1, 6.7, 5.4, 0, 0, 0.9, 10, 7.8, 0.1, 1.3, 0.5, 0.1, 0.2, 0.5, 0, 7.2, 0, 0, 10.2, 9.7, 0.2, 0.1, 0.9, 3.5, 1.2, 4.9, 4.6, 3.5, 1.1, 5.8, 0.4, 0, 0, 0.6, 0.1, 1.2, 0.8, 9.4, 1.5, 0.1, 0.1, 0.2, 0.9, 4.8 };
            double[] potentialEvaporation = new double[365] { 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.18, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 0.65, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.23, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 2.10, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 3.90, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 4.19, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 3.23, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 1.63, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.77, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.27, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16, 0.16 };
            double[] temperature = new double[365] { -2.2, 0.1, 2.9, 2.6, 2.7, 6.1, 2.8, 1.4, -1.7, -1.7, -0.5, -2.4, -0.7, 1.2, 1.4, 1.6, 0.8, 0.5, 0.2, -0.3, 0.0, 1.8, 1.3, 0.9, 1.5, 3.7, 4.7, 2.6, 2.0, 3.8, 1.7, 2.8, 2.5, 3.8, 5.9, 6.5, 5.4, 6.0, 4.2, 3.3, 1.6, 1.0, 2.7, 3.0, 0.0, -0.8, 0.7, 0.3, 0.8, 0.6, 3.4, 7.3, 5.4, 2.5, -0.2, -1.2, -0.9, -0.6, 1.2, 4.2, 6.0, 3.4, 3.7, 4.8, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 10.0, 2.8, -0.9, 1.1, 1.7, 1.6, 4.5, 5.2, 1.8, 3.5, 7.6, 4.6, 2.5, -3.9, -5.4, -5.2, -5.7, -7.5, -8.3, -10.2, -2.5, 5.4, 3.2, 2.8, 2.3, -0.3, -3.2, -0.5, 4.4, 2.9, 2.7, 2.4, -0.7, -2.2, 1.4, -1.0, -2.1, -0.8, 0.5, 0.9, 0.8, 0.9, 0.9, 0.9, 3.9, 5.5, 4.0, 5.9, 3.5 };

            double[] runoff = new double[365];

            for (int i = 0; i < precipitation.Length; i++)
            {
                hydroCat.Step(precipitation[i], potentialEvaporation[i], temperature[i]);
                runoff[i] = hydroCat.Runoff;
            }


            //hydroCat.Step(0, 0.16, -2.2);
            //hydroCat.Step(0, 0.16, 0.1);
            //hydroCat.Step(2.3, 0.16, 2.9);
            //hydroCat.Step(0.2, 0.16, -2.2);
            //hydroCat.Step(0, 0.16, -2.2);

            

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

            HydroCat nam = new HydroCat(); 

            nam.PrecipitationTs = precipitationTs;
            nam.PotentialEvaporationTs = potentialEvaporationTs;
            nam.TemperatureTs = temperatureTs;

            //nam.InitialSnowStorage = 0;
            //nam.InitialSurfaceStorage = 0;
            //nam.InitialRootZoneStorage = 220;
            //nam.InitialOverlandFlow = 0;
            //nam.InitialInterflow = 0;
            //nam.InitialBaseFlow = 0.6;

            //nam.CatchmentArea = 160; //TODO: wrong unit..
            //nam.SnowmeltCoefficient = 2;
            //nam.SurfaceStorageCapacity = 18;
            //nam.RootZoneStorageCapacity = 250;
            //nam.OverlandFlowTimeConstant = 0.61;
            //nam.InterflowCoefficient = 1.0 / 870;
            //nam.OverlandFlowTreshold = 0.38;
            //nam.InterflowTreshold = 0.08;
            //nam.BaseFlowTreshold = 0.25;
            //nam.BaseflowTimeConstant = 2800;
            
 
            nam.SimulationStartTime = new System.DateTime(2010, 1, 1);
            nam.SimulationEndTime = new System.DateTime(2010, 1, 10);
            

            nam.Initialize();

            nam.PerformTimeStep();
            nam.PerformTimeStep();
            nam.PerformTimeStep();
              
        }

        /// <summary>
        ///A test for RunSimulation
        ///</summary>
        [TestMethod()]
        public void RunSimulationTest()
        {
            HydroCat hydroCat = new HydroCat();
            TimeSeriesGroup climaDataTsG = TimeSeriesGroupFactory.Create("somefilename.xts");
            hydroCat.PrecipitationTs = climaDataTsG.Items[0];
            hydroCat.PotentialEvaporationTs = climaDataTsG.Items[1];
            hydroCat.TemperatureTs = climaDataTsG.Items[2];
            hydroCat.RunSimulation();
        }
    }
}
