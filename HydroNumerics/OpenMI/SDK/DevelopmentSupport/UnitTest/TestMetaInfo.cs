
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.OpenMI.Sdk.DevelopmentSupport;

namespace HydroNumerics.OpenMI.Sdk.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// Summary description for TestMetaInfo.
	/// </summary>
	[TestClass]
	public class TestMetaInfo
	{
		public TestMetaInfo()
		{
            MetaInfo.SetAttribute(typeof(Element), "subject1", "e1");
            MetaInfo.SetAttribute(typeof(Element), "property1", "subject1", "ep1");
            MetaInfo.SetAttribute(typeof(Element), "property2", "subject1", "ep2");
            MetaInfo.SetAttribute(typeof(Element), "property3", "subject1", "ep3");

            MetaInfo.SetAttribute(typeof(Node), "subject2", "n1");
            MetaInfo.SetAttribute(typeof(Node), "property1", "subject2", "np1");
            MetaInfo.SetAttribute(typeof(Node), "property2", "subject1", null);
            MetaInfo.SetAttribute(typeof(Node), "property3", "subject1", "np3");
		}

		[TestMethod] public void Attribute() 
		{
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "subject1"), "e1");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "property1", "subject1"), "ep1");
			Assert.AreEqual (MetaInfo.GetAttributeDefault (typeof (Element), "property1", "subject1", "def"), "ep1");
			Assert.AreEqual (MetaInfo.GetAttributeDefault (typeof (Element), "property1", "subject2", "def"), "def");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "property1", "subject2"), null);
		}

		[TestMethod] public void AttributeInheritance() 
		{
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "subject1"), "e1");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "subject2"), "n1");

			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "property1", "subject1"), "ep1");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "property1", "subject2"), "np1");

			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "property3", "subject1"), "ep3");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "property3", "subject1"), "np3");

			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Element), "property2", "subject1"), "ep2");
			Assert.AreEqual (MetaInfo.GetAttribute (typeof (Node), "property2", "subject1"), null, "null overridden");
		}
	}
}
