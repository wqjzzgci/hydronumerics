using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.Core.UnitTest
{
  [TestClass]
  public class FtpTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      FtpDownloader ftp = new FtpDownloader(@"ftp://jacobgudbjerg.dk", "", "");

      Assert.IsTrue(ftp.TryPutFile("TestFile2.txt", "Dette er en test\n"));



    }
  }
}
