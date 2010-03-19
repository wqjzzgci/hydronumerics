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
	public class ExchangeItemTest
	{
		ExchangeItem exchangeItem;
        [TestInitialize()]
		public void Init()
		{
			exchangeItem = new ExchangeItem();
			exchangeItem.Quantity = new Quantity("Q");
			ElementSet elementSet = new ElementSet();
			elementSet.ID = "ES";
			exchangeItem.ElementSet = elementSet;
		}

		[TestMethod()]
		public void ElementSet()
		{
			ElementSet elementSet = new ElementSet();
			elementSet.ID = "ES";
			Assert.IsTrue(elementSet.Equals(exchangeItem.ElementSet));
		}
		
		[TestMethod()]
		public void Quantity()
		{
			Assert.IsTrue(exchangeItem.Quantity.Equals(new Quantity("Q")));
		}

		[TestMethod()]
		public void Equals()
		{
			ExchangeItem exchangeItem = new ExchangeItem();
			exchangeItem.Quantity = new Quantity("Q");
			ElementSet elementSet = new ElementSet();
			elementSet.ID = "ES";
			exchangeItem.ElementSet = elementSet;

			Assert.IsTrue(exchangeItem.Equals(this.exchangeItem));

			exchangeItem.Quantity = new Quantity("Q1");
			Assert.IsFalse(exchangeItem.Equals(this.exchangeItem));
			exchangeItem.Quantity = new Quantity("Q");
			elementSet.ID = "ES2";

			Assert.IsFalse(exchangeItem.Equals(this.exchangeItem));
			Assert.IsFalse(exchangeItem.Equals(null));
			Assert.IsFalse(exchangeItem.Equals("string"));
		}
	}
}
