
using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.OpenMI.Sdk.DevelopmentSupport;

namespace HydroNumerics.OpenMI.Sdk.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// Summary description for TestCollectionSupport.
	/// </summary>
	[TestClass]
	public class TestCollectionSupport
	{
		private ArrayList list = new ArrayList();
		private Location loc1 = new Location (4, 5);
		private Location loc2 = new Location (4, 5);
		private Location loc3 = new Location (6, 8);

		public TestCollectionSupport()
		{
            list.Add(loc1);
            list.Add(loc3);
		}

		
		[TestMethod] public void Collection() 
		{
			Assert.AreEqual (loc1.Equals(loc2), true);
			Assert.AreEqual (loc1.Equals(loc3), false);

			Assert.AreEqual (list.Contains (loc1), true);
			Assert.AreEqual (list.Contains (loc2), true);

			Assert.AreEqual (CollectionSupport.ContainsObject (list, loc1), true);
			Assert.AreEqual (CollectionSupport.ContainsObject (list, loc2), false);
		}
	}
}
