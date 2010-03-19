#region Copyright
/*
* Copyright (c) 2010, Jan Gregersen (HydroInform) & Jacob Gudbjerg
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the names of Jan Gregersen (HydroInform) & Jacob Gudbjerg nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "Jan Gregersen (HydroInform) & Jacob Gudbjerg" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "Jan Gregersen (HydroInform) & Jacob Gudbjerg" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System.IO;
using HydroNumerics.Time.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace HydroNumerics.Time.Core.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for TimeSeriesGroupTest and is intended
    ///to contain all TimeSeriesGroupTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimeSeriesGroupTest
    {


        private TestContext testContextInstance;
        private bool eventWasRaised = false;

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
        ///A test for DataChangedEvent
        ///</summary>
        [TestMethod()]
        [DeploymentItem("HydroNumerics.Time.Core.dll")]
        public void DataChangedEvent()
        {
            TimeSeriesGroup timeSeriesGroup = new TimeSeriesGroup();
            timeSeriesGroup.DataChanged += new TimeSeriesGroup.DataChangedEventHandler(timeSeriesGroup_DataChanged);
            eventWasRaised = false;
            timeSeriesGroup.TimeSeriesList.Add(new TimeSeries());
            Assert.IsTrue(eventWasRaised); eventWasRaised = false;
            timeSeriesGroup.TimeSeriesList[0].AddData(4.3);
           
            Assert.IsTrue(eventWasRaised); eventWasRaised = false;
            timeSeriesGroup.TimeSeriesList[0].TimeValuesList[0].Value = 2.1;
            
        }

        void timeSeriesGroup_DataChanged(object sender, string info)
        {
            eventWasRaised = true;
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            TimeSeriesGroup timeSeriesGroup = new TimeSeriesGroup();
            TimeSeries timeSeries1 = new TimeSeries();
            timeSeries1.ID = "Observed flow";
            timeSeries1.Description = "Flow rate at station 14";
            timeSeries1.Quantity.ID = "Flow";
            //timeSeries1.Quantity.Unit.ConversionFactorToSI = 0.001;
            //timeSeries1.Quantity.Unit.OffSetToSI = 0.0;
            //timeSeries1.Quantity.Unit.ID = "l/sec";
            //timeSeries1.Quantity.Unit.Description = "Liters pr. second";
            //timeSeries1.Quantity.Dimension.GetPower(global::OpenMI.Standard.DimensionBase.Length, 3);
            

            timeSeries1.AddData(2.3);
            timeSeries1.AddData(3.3);
            timeSeries1.AddData(4.3);
            timeSeries1.AddData(5.3);
            TimeSeries timeSeries2 = new TimeSeries();
            timeSeries2.ID = "ID2";
            timeSeries2.AddData(1.7);
            timeSeries2.AddData(2.7);
            timeSeries2.AddData(4.7);
            timeSeries2.AddData(6.7);
            timeSeries2.AddData(7.7);
            timeSeriesGroup.TimeSeriesList.Add(timeSeries1);
            timeSeriesGroup.TimeSeriesList.Add(timeSeries2);
            timeSeriesGroup.Save("test.xts");

            TimeSeriesGroup timeSeriesGroup2 = TimeSeriesGroupFactory.Create("test.xts");

            Assert.AreEqual("ID1", timeSeriesGroup2.TimeSeriesList[0].ID);
            Assert.AreEqual(2.3, timeSeriesGroup2.TimeSeriesList[0].TimeValuesList[0].Value);

            Assert.AreEqual("ID2", timeSeriesGroup2.TimeSeriesList[1].ID);
            Assert.AreEqual(1.7, timeSeriesGroup2.TimeSeriesList[1].TimeValuesList[0].Value);

        }
    }
}
