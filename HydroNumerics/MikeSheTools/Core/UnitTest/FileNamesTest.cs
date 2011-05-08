using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.MikeSheTools.Core;



namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
  [TestClass]
  public class FileNamesTest
  {
    private FileNames _filenames;
        
    [TestInitialize()]
    public void Load()
    {
      _filenames = new FileNames(@"..\..\..\TestData\TestModelDemo.she");
    }


    [TestMethod]
    public void WellFile()
    {
      Assert.AreEqual(Path.GetFullPath(@"..\..\..\TestData\DemoWells.WEL"), _filenames.WelFileName);
    }

  }
}
