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
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMI.Standard;
using HydroNumerics.OpenMI.Sdk.Backbone;

namespace HydroNumerics.OpenMI.Sdk.Backbone.UnitTest
{
    [TestClass()]
	public class ScalarSetTest
	{
		ScalarSet scalarSet;

        [TestInitialize()]
		public void Init()
		{
			double[] values = {1.0,2.0,3.0};
			scalarSet = new ScalarSet(values);
		}

		[TestMethod()]
		public void Constructor()
		{
			ScalarSet scalarSet2 = new ScalarSet(scalarSet);
			Assert.AreEqual(scalarSet,scalarSet2);
		}

		[TestMethod()]
		public void GetScalar()
		{
			Assert.AreEqual(1.0,scalarSet.GetScalar(0));
			Assert.AreEqual(2.0,scalarSet.GetScalar(1));
			Assert.AreEqual(3.0,scalarSet.GetScalar(2));
		}

		[TestMethod()]
		public void Data()
		{
			Assert.AreEqual(1.0,scalarSet.data[0]);
			Assert.AreEqual(2.0,scalarSet.data[1]);
			Assert.AreEqual(3.0,scalarSet.data[2]);
		}

		[TestMethod()]
		public void Count()
		{
			Assert.AreEqual(3,scalarSet.Count);
		}

		[TestMethod()]
		public void Equals()
		{
			double[] values={1.0,2.0,3.0};
			ScalarSet scalarSet2 = new ScalarSet(values);
			Assert.IsTrue(scalarSet.Equals(scalarSet2));
			scalarSet2.data[1] = 2.5;
			Assert.IsFalse(scalarSet.Equals(scalarSet2));
		}
	}
}
