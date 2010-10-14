using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra;
using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{
  [TestFixture]
  public class DFS3Test
  {
    DFS3 _dfs;
    DFS3 _dfsWrite;

    [SetUp]
    public void ConstructTest()
    {
      string file1 =@"..\..\TestData\omr4_jag_3DSZ.dfs3";
      string file2 =@"..\..\TestData\omr4_jag_3DSZ_copy.dfs3";
      File.Copy(file1, file2,true);
      _dfs = new DFS3(file1);
      _dfsWrite = new DFS3(file2);
    }
       

    [TearDown]
    public void Destruct()
    {
      _dfs.Dispose();
      _dfsWrite.Dispose();
    }


   

    [Test]
    public void StaticTest()
    {
      Assert.AreEqual(460, _dfs.NumberOfColumns);
      Assert.AreEqual(196, _dfs.NumberOfRows);
      Assert.AreEqual(18, _dfs.NumberOfLayers);
    }

    [Test]
    public void IndexTest()
    {
      //Left and right
      Assert.AreEqual(-1, _dfs.GetColumnIndex(0));
      Assert.AreEqual(-2, _dfs.GetColumnIndex(double.MaxValue));

      //Over and under
      Assert.AreEqual(-1, _dfs.GetRowIndex(0));
      Assert.AreEqual(-2, _dfs.GetRowIndex(double.MaxValue));

      Assert.AreEqual(0, _dfs.GetColumnIndex(410000));
      Assert.AreEqual(139, _dfs.GetColumnIndex(479336));

      Assert.AreEqual(49, _dfs.GetRowIndex(6128437));
    }



    [Test]
    public void GetTimeTest()
    {
      Assert.AreEqual(new DateTime(1990, 1, 2, 12, 0, 0), _dfs.TimeOfFirstTimestep);
      Assert.AreEqual(new TimeSpan(0, 0, 864000), _dfs.TimeStep);
    }



    [Test]
    public void GetDataTest()
    {
      Matrix3d M = _dfs.GetData(0, 1);
      Assert.AreEqual(6.733541, M[151, 86, 17], 1e-5);
      Assert.AreEqual(13.94974, _dfs.GetData(1, 1)[150, 86, 17], 1e-5);
      Assert.AreEqual(13.7237, _dfs.GetData(0, 1)[150, 86, 17], 1e-5);
    }

    [Test]
    public void SetDataTest()
    {
      Matrix3d M = _dfs.GetData(0, 1);
      M[90, 130, 1] = 100000;
      _dfsWrite.SetData(0, 1, M);

      //Check that buffer is updated
      Assert.AreEqual(_dfsWrite.GetData(0, 1)[90, 130, 1], 100000);

      //Note that here is a potential pitfall. Because of the reference and the buffering it will appear as if the data also 
      //changes in _dfs
      Assert.AreEqual(_dfs.GetData(0, 1)[90, 130, 1], 100000);

    }
  }
}
