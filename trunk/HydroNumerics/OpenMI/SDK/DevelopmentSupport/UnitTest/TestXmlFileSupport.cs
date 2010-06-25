using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.OpenMI.Sdk.DevelopmentSupport;

namespace HydroNumerics.OpenMI.Sdk.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestClass] 
	public class TestXmlFileSupport
	{
       public TestXmlFileSupport()
        {
            File.Delete("Network.xml");
            File.Delete("Network1.xml");
            File.Delete("Network2.xml");
            File.Delete("Network3.xml");

            MetaInfo.SetAttribute(typeof(Element), "ID", "XmlKey", true);
            MetaInfo.SetAttribute(typeof(Element), "ID", "XmlRefName", "RefID");
            MetaInfo.SetAttribute(typeof(Network), "Nodes", "XmlIndex", 1);
            MetaInfo.SetAttribute(typeof(Network), "XmlFile", true);

            ObjectSupport.LoadAssembly(this.GetType().Assembly);
        }
		

		[TestMethod] public void SaveAndOpen() 
		{
			Network network = Network.GetSampleNetwork();
			DateTime date = DateTime.Now;

			FileInfo file = new FileInfo("Network.xml");

			XmlFile.Write (network, file);

			Assert.IsTrue (File.Exists(file.FullName), "File created");
			Assert.IsTrue (XmlFile.GetRegisteredFile(network).FullName == file.FullName, "Registration correct");

			Network network1 = (Network) XmlFile.GetRead (file);
			Assert.AreSame (network, network1, "Same instance");

			Network network2 = new Network();
			XmlFile.Read (network2, file);

			Assert.AreEqual (false, network == network2, "New instance is created");
			Assert.AreEqual (network.Nodes.Count, network2.Nodes.Count, "Read file");
			Assert.AreEqual (((Node) network.Nodes[0]).Name, ((Node) network2.Nodes[0]).Name, "Node name");
			Assert.AreSame  (((Branch) network2.Branches[0]).BeginNode, (Node) network2.Nodes[0], "Branch refers to node in node list");
			Assert.AreSame  (((Branch) network2.Branches[0]).EndNode, ((Branch) network2.Branches[1]).BeginNode, "Branches share same node object");
			Assert.AreEqual (network.LastModificationTime.ToString(), network2.LastModificationTime.ToString(), "Date Time");
		}

		[TestMethod] public void ReferencedFiles()
		{
			Network network1 = Network.GetSampleNetwork();
			Network network2 = Network.GetSampleNetwork();
			
			Network network3 = new Network();
			network3.Nodes.Add (network1);
			network3.Nodes.Add (network2);

			FileInfo file = new FileInfo("Network3.xml");
			XmlFile.Write (network3, file);

			Assert.IsTrue (File.Exists(file.FullName), "File written");
			Assert.IsTrue (File.Exists(new FileInfo("Network1.xml").FullName), "Referenced file written");

			Assert.AreEqual (new FileInfo("Network1.xml").FullName, XmlFile.GetRegisteredFile(network1).FullName, "File registered");
		}

		[TestMethod] public void SameID()
		{
			Network network1 = Network.GetSampleNetwork();
			Network network2 = Network.GetSampleNetwork();

			((Node) network1.Nodes[0]).Location.X = 44;
			((Node) network2.Nodes[0]).Location.X = 55;
			
			Network network3 = new Network();
			network3.Nodes.Add (network1.Nodes[0]);
			network3.Nodes.Add (network2.Nodes[0]);

			XmlFile.Write (network1, new FileInfo("Network1.xml"));
			XmlFile.Write (network2, new FileInfo("Network2.xml"));
			XmlFile.Write (network3, new FileInfo("Network3.xml"));

			Assert.IsTrue (File.Exists(new FileInfo("Network3.xml").FullName), "File written");
			Assert.IsTrue (File.Exists(new FileInfo("Network1.xml").FullName), "Referenced file written");

			Network network = new Network();
			XmlFile.Read (network, new FileInfo("Network3.xml"));

			Assert.AreEqual (2, network.Nodes.Count, "Number of nodes read");
			Assert.AreEqual (((Node)network1.Nodes[0]).ID, ((Node)network.Nodes[0]).ID, "Node is the same");
		}
	}
}
