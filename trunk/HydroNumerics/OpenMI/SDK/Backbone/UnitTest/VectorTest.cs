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
	public class VectorTest
	{
		Vector vector;
        [TestInitialize()]
		public void Init()
		{
			vector = new Vector(1.0,2.0,3.0);
		}

		[TestMethod()]
		public void Constructor()
		{
			Vector vector2 = new Vector(vector);

			Assert.AreEqual(vector,vector2);
		}

		[TestMethod()]
		public void Components()
		{
			Assert.AreEqual(1.0,vector.XComponent);
			Assert.AreEqual(2.0,vector.YComponent);
			Assert.AreEqual(3.0,vector.ZComponent);

			vector.XComponent = 4.0;
			vector.YComponent = 5.0;
			vector.ZComponent = 6.0;

			Assert.AreEqual(4.0,vector.XComponent);
			Assert.AreEqual(5.0,vector.YComponent);
			Assert.AreEqual(6.0,vector.ZComponent);
		}

		[TestMethod()]
		public void Equals()
		{
			Vector vector1 = new Vector(1.0,2.0,3.0);
			Assert.IsTrue(vector.Equals(vector1));

			Assert.IsFalse(vector.Equals(null));
			Assert.IsFalse(vector.Equals("string"));
		}

		[TestMethod()]
		public void EqualsX()
		{
			Vector vector1 = new Vector(1.1,2.0,3.0);
			Assert.IsFalse(vector.Equals(vector1));
		}

		[TestMethod()]
		public void EqualsY()
		{
			Vector vector1 = new Vector(1.0,2.1,3.0);
			Assert.IsFalse(vector.Equals(vector1));
		}

		[TestMethod()]
		public void EqualsZ()
		{
			Vector vector1 = new Vector(1.0,2.0,3.1);
			Assert.IsFalse(vector.Equals(vector1));
		}

	}
}
