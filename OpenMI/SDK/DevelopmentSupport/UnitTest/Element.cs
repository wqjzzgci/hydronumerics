
using System;

namespace HydroNumerics.OpenMI.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// Summary description for Element.
	/// </summary>
	public class Element
	{
		private string _name;
		private string _id;

		public Element()
		{
		}

		public string ID 
		{
			get {return _id;}
			set {_id = value;}
		}

		public string Name 
		{
			get {return _name;}
			set {_name = value;}
		}
	}
}
