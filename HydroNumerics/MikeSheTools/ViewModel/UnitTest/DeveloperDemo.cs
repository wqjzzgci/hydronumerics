using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.MikeSheTools.ViewModel.UnitTest
{
  [TestClass]
  public class DeveloperDemo
  {
    [TestMethod]
    public void TestMethod1()
    {
      JupiterViewModel JVM = new JupiterViewModel();

      //Read the database. This call is asynchrone and will return before finished reading
      JVM.ReadJupiter(@"..\..\..\..\JupiterTools\TestData\AlbertslundPcJupiter.mdb");

      //Wait until the database has finished reading. The time depends on the size of the database
      Thread.Sleep(TimeSpan.FromSeconds(10));

      //Only include plants that have extraction in the period
      JVM.MinYearlyExtraction = 1;
      JVM.SelectionStartTime = new DateTime(2005, 1, 1);
      JVM.SelectionEndTime = new DateTime(2005, 12, 31);

      //Fix the errors that can be automatically fixed.
      JVM.FixErrors();

      //Example of change a property on well
      JVM.SortedAndFilteredPlants.First().Wells.First().IsUsedForExtraction = true;

      //Now write the output files
      MsheInputFileWriters.WriteGMSExtraction(@"..\..\..\..\JupiterTools\TestData", JVM.SortedAndFilteredPlants, JVM.SelectionStartTime, JVM.SelectionEndTime);
    }
  }
}
