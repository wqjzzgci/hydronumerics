using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.OpenMI.Sdk.Buffer;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace HydroNumerics.OpenMI.Sdk.Buffer.UnitTest
{
    [TestClass()]
	public class SupportTest
	{
		[TestMethod()]
		public void IsBefore()
		{
			TimeStamp t_4 = new TimeStamp(4);
			TimeStamp t_7 = new TimeStamp(7);
            Assert.AreEqual(true, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_4, t_7));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_7, t_4));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_4, t_4));

            HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan t_3_5 = new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(3), new TimeStamp(5));
            HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan t_4_6 = new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(4), new TimeStamp(6));
            HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan t_5_7 = new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(5), new TimeStamp(7));
            HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan t_6_8 = new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new TimeStamp(6), new TimeStamp(8));

            Assert.AreEqual(true, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_3_5, t_6_8));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_6_8, t_3_5));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_3_5, t_4_6));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_3_5, t_5_7));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_3_5, t_3_5));

            Assert.AreEqual(true, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_4, t_5_7));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_4, t_3_5));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_7, t_3_5));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_4, t_4_6));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_7, t_5_7));

            Assert.AreEqual(true, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_3_5, t_7));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_3_5, t_4));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_6_8, t_4));
            Assert.AreEqual(false, global::HydroNumerics.OpenMI.Sdk.Buffer.Support.IsBefore(t_5_7, t_7));

		}
	}
}
