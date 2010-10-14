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
	public class ElementSetTest
	{
		ElementSet elementSet;
		Element element1,element2;

        [TestInitialize()]
		public void Init() 
		{
			elementSet = new ElementSet("ElementSet","ElementSetID",
				ElementType.XYPolygon,new SpatialReference("ReferenceID"));
			element1 = new Element("element1");
			element1.AddVertex(new Vertex(1.0,2.0,3.0));
			element1.AddVertex(new Vertex(2.0,3.0,4.0));
			element1.AddVertex(new Vertex(4.0,5.0,6.0));
			int[] index = {1,2,3,4,5};
			element1.AddFace(index);
			elementSet.AddElement(element1);
			element2 = new Element("element2");
			element2.AddVertex(new Vertex(6.0,7.0,8.0));
			element2.AddVertex(new Vertex(9.0,10.0,11.0));
			element2.AddVertex(new Vertex(12.0,13.0,14.0));
			int[] index2 = {6,7,8,9};
			element2.AddFace(index2);
			elementSet.AddElement(element2);	
		}


		[TestMethod()]
		public void Constructor()
		{
			Assert.AreEqual("ElementSet",elementSet.Description);
			Assert.AreEqual("ElementSetID",elementSet.ID);
			Assert.AreEqual(ElementType.XYPolygon,elementSet.ElementType);
			Assert.AreEqual(new SpatialReference("ReferenceID"),elementSet.SpatialReference);

			ElementSet elementSet2 = new ElementSet(elementSet);
			Assert.AreEqual(elementSet,elementSet2);
		}

		[TestMethod()]
		public void Description()
		{	
			elementSet.Description = "Description";
			Assert.AreEqual("Description",elementSet.Description);
		}

		[TestMethod()]
		public void ID()
		{
			elementSet.ID = "ID1";
			Assert.AreEqual("ID1",elementSet.ID);
		}

		[TestMethod()]
		public void Elementtype()
		{
			elementSet.ElementType = ElementType.XYLine;
			Assert.AreEqual(ElementType.XYLine,elementSet.ElementType);
		}

		[TestMethod()]
		public void SpatialReference()
		{
			elementSet.SpatialReference = new SpatialReference("ID");
			Assert.AreEqual(new SpatialReference("ID"),elementSet.SpatialReference);
		}

		private bool CompareIntArrays(int[] ar1,int[] ar2)
		{
			if (ar1.Length!=ar2.Length)
				return false;
			for (int i=0;i<ar1.Length;i++)
				if (ar1[i]!=ar2[i])
					return false;
			return true;
		}

		[TestMethod()]
		public void CompareIntArrayTest()
		{
			int[] ar1 = {1,2,3,4};
			int[] ar2 = {1,2,3,4};
			int[] ar3 = {1,2};
			int[] ar4 = {1,2,3,5};

			Assert.IsTrue(CompareIntArrays(ar1,ar2));
			Assert.IsFalse(CompareIntArrays(ar1,ar3));
			Assert.IsFalse(CompareIntArrays(ar1,ar4));
		}

		[TestMethod()]
		public void Faces()
		{
			Assert.AreEqual(1,elementSet.GetFaceCount(0));
			Assert.AreEqual(1,elementSet.GetFaceCount(1));
			int[] index = {1,2,3,4,5};
			int[] index2 = {6,7,8,9};
			Assert.IsTrue(CompareIntArrays(index,elementSet.GetFaceVertexIndices(0,0)));
			Assert.IsTrue(CompareIntArrays(index2,elementSet.GetFaceVertexIndices(1,0)));

		}

		[TestMethod()]
		public void AddElement()
		{
			Assert.AreEqual(2,elementSet.ElementCount);
			Assert.AreEqual(element1,elementSet.Elements[0]);
			Assert.AreEqual(element2,elementSet.Elements[1]);
		}

		[TestMethod()]
		public void GetElement() 
		{
			Assert.AreEqual(element1,elementSet.GetElement(0));
			Assert.AreEqual(element2,elementSet.GetElement(1));
		}

		[TestMethod()]
		public void Element()
		{
			Assert.AreEqual(element1,elementSet.Elements[0]);
			Assert.AreEqual(element2,elementSet.Elements[1]);
		}

		[TestMethod()]
		public void GetElementID()
		{
			Assert.AreEqual("element1",elementSet.GetElementID(0));
			Assert.AreEqual("element2",elementSet.GetElementID(1));
		}
		[TestMethod()]
		public void GetXCoordinate()
		{
			Assert.AreEqual(1.0,elementSet.GetXCoordinate(0,0));
			Assert.AreEqual(2.0,elementSet.GetXCoordinate(0,1));
			Assert.AreEqual(4.0,elementSet.GetXCoordinate(0,2));
			Assert.AreEqual(6.0,elementSet.GetXCoordinate(1,0));
			Assert.AreEqual(9.0,elementSet.GetXCoordinate(1,1));
			Assert.AreEqual(12.0,elementSet.GetXCoordinate(1,2));
		}
		[TestMethod()]
		public void GetYCoordinate()
		{
			Assert.AreEqual(2.0,elementSet.GetYCoordinate(0,0));
			Assert.AreEqual(3.0,elementSet.GetYCoordinate(0,1));
			Assert.AreEqual(5.0,elementSet.GetYCoordinate(0,2));
			Assert.AreEqual(7.0,elementSet.GetYCoordinate(1,0));
			Assert.AreEqual(10.0,elementSet.GetYCoordinate(1,1));
			Assert.AreEqual(13.0,elementSet.GetYCoordinate(1,2));
		}
		[TestMethod()]
		public void GetZCoordinate()
		{
			Assert.AreEqual(3.0,elementSet.GetZCoordinate(0,0));
			Assert.AreEqual(4.0,elementSet.GetZCoordinate(0,1));
			Assert.AreEqual(6.0,elementSet.GetZCoordinate(0,2));
			Assert.AreEqual(8.0,elementSet.GetZCoordinate(1,0));
			Assert.AreEqual(11.0,elementSet.GetZCoordinate(1,1));
			Assert.AreEqual(14.0,elementSet.GetZCoordinate(1,2));
		}

		[TestMethod()]
		public void ElementCount()
		{
			Assert.AreEqual(2,elementSet.ElementCount);
		}
		[TestMethod()]
		public void VertexCount()
		{
			Assert.AreEqual(3,elementSet.GetVertexCount(0));
			Assert.AreEqual(3,elementSet.GetVertexCount(1));
		}

		[TestMethod()]
		public void GetElementIndex()
		{
			Assert.AreEqual(0,elementSet.GetElementIndex("element1"));
			Assert.AreEqual(1,elementSet.GetElementIndex("element2"));
		}

		[TestMethod()]
		public void GetElementIndexException()
		{
            try
            {
                elementSet.GetElementIndex("element3");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }
		}

		[TestMethod()]
		public void Equals()
		{
			ElementSet elementSet1 = new ElementSet("ElementSet","ElementSetID",
				ElementType.XYPolygon,new SpatialReference("ReferenceID"));
			element1 = new Element("element1");
			element1.AddVertex(new Vertex(1.0,2.0,3.0));
			element1.AddVertex(new Vertex(2.0,3.0,4.0));
			element1.AddVertex(new Vertex(4.0,5.0,6.0));
			elementSet1.AddElement(element1);
			element2 = new Element("element2");
			element2.AddVertex(new Vertex(6.0,7.0,8.0));
			element2.AddVertex(new Vertex(9.0,10.0,11.0));
			element2.AddVertex(new Vertex(12.0,13.0,14.0));
			elementSet1.AddElement(element2);

			Assert.IsTrue(elementSet.Equals(elementSet1));

			Assert.IsFalse(elementSet.Equals(null));
			Assert.IsFalse(elementSet.Equals("string"));

		}

		[TestMethod()]
		public void EqualsDescription()
		{
			ElementSet elementSet1 = new ElementSet("Element","ElementSetID",
				ElementType.XYPolygon,new SpatialReference("ReferenceID"));
			element1 = new Element("element1");
			element1.AddVertex(new Vertex(1.0,2.0,3.0));
			element1.AddVertex(new Vertex(2.0,3.0,4.0));
			element1.AddVertex(new Vertex(4.0,5.0,6.0));
			elementSet1.AddElement(element1);
			element2 = new Element("element2");
			element2.AddVertex(new Vertex(6.0,7.0,8.0));
			element2.AddVertex(new Vertex(9.0,10.0,11.0));
			element2.AddVertex(new Vertex(12.0,13.0,14.0));
			elementSet1.AddElement(element2);

			Assert.IsFalse(elementSet.Equals(elementSet1));
		}

		[TestMethod()]
		public void EqualsID()
		{
			ElementSet elementSet1 = new ElementSet("ElementSet","ElementID",
				ElementType.XYPolygon,new SpatialReference("ReferenceID"));
			element1 = new Element("element1");
			element1.AddVertex(new Vertex(1.0,2.0,3.0));
			element1.AddVertex(new Vertex(2.0,3.0,4.0));
			element1.AddVertex(new Vertex(4.0,5.0,6.0));
			elementSet1.AddElement(element1);
			element2 = new Element("element2");
			element2.AddVertex(new Vertex(6.0,7.0,8.0));
			element2.AddVertex(new Vertex(9.0,10.0,11.0));
			element2.AddVertex(new Vertex(12.0,13.0,14.0));
			elementSet1.AddElement(element2);


			Assert.IsFalse(elementSet.Equals(elementSet1));
		}

		[TestMethod()]
		public void EqualsElementType()
		{
			ElementSet elementSet1 = new ElementSet("ElementSet","ElementSetID",
				ElementType.XYLine,new SpatialReference("ReferenceID"));
			element1 = new Element("element1");
			element1.AddVertex(new Vertex(1.0,2.0,3.0));
			element1.AddVertex(new Vertex(2.0,3.0,4.0));
			element1.AddVertex(new Vertex(4.0,5.0,6.0));
			elementSet1.AddElement(element1);
			element2 = new Element("element2");
			element2.AddVertex(new Vertex(6.0,7.0,8.0));
			element2.AddVertex(new Vertex(9.0,10.0,11.0));
			element2.AddVertex(new Vertex(12.0,13.0,14.0));
			elementSet1.AddElement(element2);


			Assert.IsFalse(elementSet.Equals(elementSet1));
		}

		[TestMethod()]
		public void EqualsSpatialReference()
		{
			ElementSet elementSet1 = new ElementSet("ElementSet","ElementSetID",
				ElementType.XYPolygon,new SpatialReference("Reference"));
			element1 = new Element("element1");
			element1.AddVertex(new Vertex(1.0,2.0,3.0));
			element1.AddVertex(new Vertex(2.0,3.0,4.0));
			element1.AddVertex(new Vertex(4.0,5.0,6.0));
			elementSet1.AddElement(element1);
			element2 = new Element("element2");
			element2.AddVertex(new Vertex(6.0,7.0,8.0));
			element2.AddVertex(new Vertex(9.0,10.0,11.0));
			element2.AddVertex(new Vertex(12.0,13.0,14.0));
			elementSet1.AddElement(element2);


			Assert.IsFalse(elementSet.Equals(elementSet1));
		}

		[TestMethod()]
		public void EqualsVertices()
		{
			ElementSet elementSet1 = new ElementSet("ElementSet","ElementSetID",
				ElementType.XYPolygon,new SpatialReference("ReferenceID"));
			element1 = new Element("element1");
			element1.AddVertex(new Vertex(1.1,2.0,3.0));
			element1.AddVertex(new Vertex(2.0,3.0,4.0));
			element1.AddVertex(new Vertex(4.0,5.0,6.0));
			elementSet1.AddElement(element1);
			element2 = new Element("element2");
			element2.AddVertex(new Vertex(6.0,7.0,8.0));
			element2.AddVertex(new Vertex(9.0,10.0,11.0));
			element2.AddVertex(new Vertex(12.0,13.0,14.0));
			elementSet1.AddElement(element2);


			Assert.IsFalse(elementSet.Equals(elementSet1));
		}

	}
}
