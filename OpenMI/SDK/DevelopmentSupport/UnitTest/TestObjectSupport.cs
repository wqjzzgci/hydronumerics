using System;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.OpenMI.Sdk.DevelopmentSupport;

namespace HydroNumerics.OpenMI.Sdk.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestClass] 
	public class TestObjectSupport
	{
		private Network network = null;
		private Node node1 = null;
		private Node node2 = null;
		private Location commonLocation = null;

        public TestObjectSupport()
        {
            node1 = new Node("Node1", new Location(35, 53), 30.4, 18000);
            node2 = new Node("Node2", new Location(23, 18), 26.4, 14000);
            network = new Network();
            network.Nodes.Add(node1);
            network.Nodes.Add(node2);
            commonLocation = new Location(12, 37);
        }
		

		
		[TestMethod] public void SimpleCopy() 
		{
			Location loc = new Location (4, 5);
			Location loc2 = new Location();
			ObjectSupport.Copy (loc, loc2);

			Assert.AreEqual(loc2.X, 4, "Copy simple object (1)");
			Assert.AreEqual(loc2.Y, 5, "Copy simple object (2)");

			Location loc3 = (Location) ObjectSupport.GetCopy (loc);
			Assert.AreEqual(loc3.X, 4, "GetCopy simple object (1)");
			Assert.AreEqual(loc3.Y, 5, "GetCopy simple object (2)");
		}

		[TestMethod] public void NestedCopy() 
		{
			MetaInfo.SetAttribute (typeof(Network), "Nodes", "ObjectCopy", false);

			Network copy = (Network) ObjectSupport.GetCopy (network);

			Assert.AreEqual("Node1", ((Node)copy.Nodes[0]).Name, "Name of Node1");
			Assert.AreEqual("Node2", ((Node)copy.Nodes[1]).Name, "Name of Node2");

			Assert.AreSame(node1, ((Node)copy.Nodes[0]), "No copy of node1");
			Assert.AreEqual(35, ((Node)copy.Nodes[0]).Location.X, "Location X");

			MetaInfo.SetAttribute (typeof(Network), "Nodes", "ObjectCopy", true);
			Network copy2 = (Network) ObjectSupport.GetCopy (network);
			Assert.AreEqual("Node1", ((Node)copy2.Nodes[0]).Name, "Name of node1 when node is copied");
			Assert.AreEqual(false, ((Node)copy2.Nodes[0]) == node1, "Copy of node1");
		}

		[TestMethod] public void NonHierarchicalCopy() 
		{
			MetaInfo.SetAttribute (typeof(Network), "Nodes", "ObjectCopy", true);
			MetaInfo.SetAttribute (typeof(Node), "Location", "ObjectCopy", true);

			node1.Location = commonLocation;
			node2.Location = commonLocation;
			
			Network copy = (Network) ObjectSupport.GetCopy (network);

			Assert.AreEqual(((Node)copy.Nodes[0]).Location == ((Node)copy.Nodes[1]).Location, true);
			Assert.AreEqual(((Node)copy.Nodes[0]).Location == commonLocation, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// 
		[TestMethod] public void FindType() 
		{
			ObjectSupport.LoadAssembly (Assembly.GetAssembly(this.GetType()).Location);
			Type type = ObjectSupport.GetType ("HydroNumerics.OpenMI.Sdk.DevelopmentSupport.UnitTest.Location");
			Assert.AreEqual (typeof (Location), type);
		}

		[TestMethod] public void Instantiation() 
		{
			object node = ObjectSupport.GetInstance(typeof(Node));
			Assert.AreEqual (node is Node, true);

			object str = ObjectSupport.GetInstance(typeof(string), "hello");
			Assert.AreEqual (str.Equals("hello"), true);

			object d = ObjectSupport.GetInstance(typeof(int), "23");
			Assert.AreEqual (d.Equals (23), true);

			object b = ObjectSupport.GetInstance(typeof(bool), "true");
			Assert.AreEqual (b, true);
		} 
     
		[TestMethod] public void InstantiationWithCulture() 
		{
			object d1 = ObjectSupport.GetInstance(typeof(double), "23.4", CultureInfo.CreateSpecificCulture(""));
			Assert.AreEqual (true, d1.Equals (23.4), "Neutral culture");

			object d2 = ObjectSupport.GetInstance(typeof(double), "23,4", CultureInfo.CreateSpecificCulture("nl"));
			Assert.AreEqual (true, d2.Equals (23.4), "Dutch culture");
		}
	}
}
