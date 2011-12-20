using HydroNumerics.Tough2.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

namespace HydroNumerics.Tough2.ViewModel.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for MeshTest and is intended
    ///to contain all MeshTest Unit Tests
    ///</summary>
  [TestClass()]
  public class MeshTest
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
    ///A test for Save
    ///</summary>
    [TestMethod()]
    public void SaveTest()
    {
      Mesh target = new Mesh(@"..\..\..\DotNetT2VOC.UnitTest\TestData\mesh"); // TODO: Initialize to an appropriate value
      
      target.SaveAs(@"..\..\..\DotNetT2VOC.UnitTest\TestData\mesh_new");
    }


    [TestMethod]
    public void AdjustMesh2()
    {
      Mesh m = new Mesh(@"C:\Flemming\Model\ToughReact\Radial_CoarseModel\mesh");

      List<Connection> NewConnections = new List<Connection>();
      int i = 1;
      foreach (var v in m.Connections)
      {
        if (v.First.Name.StartsWith("A1") & v.Second.Name.StartsWith("A2") & i<25)
        {
          Connection c  = new Connection(v);
          c.Second = m.Elements.Single(el =>el.Name=="AIR11");
          NewConnections.Add(c);
          i++;
        }
      }
      m.Connections.AddRange(NewConnections);
      m.SaveAs(@"C:\Flemming\Model\ToughReact\Radial_CoarseModel\mesh");


    }


    [TestMethod]
    public void AdjustMesh()
    {
      Mesh m = new Mesh(@"C:\Jacob\Projects\Flemming\Model\2DFracture\mesh");

      Element atm = new Element("ATM11",3,0);
      m.Elements.Add(atm);

      foreach (var v in m.Elements)
      {
        if (v.Z < -3)
          v.Material = 2;
        else
          v.Material = 1;
        if (v.X < 1.5000E-02)
          v.Material = 2;

        if (v.Z>-0.3)
        {
          Connection c = new Connection(m.Connections[1]);
          c.First = atm;
          c.Second = v;
          m.Connections.Add(c);
        }
      }
      atm.Material = 3;

      m.Save();

    }
  }
}
