using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.OpenMI.Sdk.DevelopmentSupport;


namespace HydroNumerics.OpenMI.Sdk.DevelopmentSupport.UnitTest
{
	/// <summary>
	/// TemporalTester tests (gregorian) DateTime to
	/// Modified Julian Date(/Time) and vice versa.
	/// </summary>
	[TestClass]
	public class TestCalendarConverter
	{
		public TestCalendarConverter()
		{
		}

		[TestMethod] public void TestDates()
		{
			Evaluate(new DateTime(1985,1,1,1,0,0,0));
			Evaluate(new DateTime(1980,11,29,23,59,59,999));
			Evaluate(new DateTime(1980,11,30,00,00,00,000));

			DateTime inDateTime_1 = new DateTime(1980,11,30,23,59,59,999);
			DateTime inDateTime_2 = inDateTime_1.AddSeconds(1);

			Evaluate(inDateTime_1);
			Evaluate(inDateTime_2);
			Evaluate(new DateTime(1981,1,28,23,59,59,999));
		}

		[TestMethod] public void TestYears()
		{
			int nItems = 2000;
			DateTime gregDate = new DateTime(1111,12,15,1,0,0,0);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddYears(1);
				Evaluate(gregDate);
			}
		}

		[TestMethod] public void TestMonths()
		{
			int nItems = 200;
			DateTime gregDate = new DateTime(1998,11,30,23,59,59,999);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddMonths(1);
				Evaluate(gregDate);
			}
		}

		[TestMethod] public void TestDays()
		{
			int nItems = 1000;
			DateTime gregDate = new DateTime(1999,11,30,23,59,59,999);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddDays(1);
				Evaluate(gregDate);
			}
		}

		[TestMethod] public void TestHours()
		{
			int nItems = 500;
			DateTime gregDate = new DateTime(1999,12,25,23,59,59,999);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddHours(1);
				Evaluate (gregDate);
			}
		}

		[TestMethod] public void TestMinutes()
		{
			int nItems = 500;
			DateTime gregDate = new DateTime(1999,12,31,21,00,59,999);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddMinutes(1);
				Evaluate(gregDate);
			}
		}

		[TestMethod] public void TestSeconds()
		{
			int nItems = 1000;
			DateTime gregDate = new DateTime(1999,12,31,23,55,00,499);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddSeconds(1);
				Evaluate(gregDate);
			}
		}

		[TestMethod] public void TestMilliSeconds()
		{
			int nItems = 5000;
			DateTime gregDate = new DateTime(1999,12,31,23,59,58,350);

			for ( int i = 0 ; i < nItems ; i++ )
			{
				gregDate = gregDate.AddMilliseconds(1);
				Evaluate(gregDate);
			}
		}

		[TestMethod] public void SomeDates() 
		{
			double zero = 0;
			DateTime zeroDate = CalendarConverter.ModifiedJulian2Gregorian (zero);

			Assert.AreEqual (1858, zeroDate.Year, "Year of Modified Julian Date Zero");
			Assert.AreEqual (11, zeroDate.Month, "Month of Modified Julian Date Zero");
			Assert.AreEqual (17, zeroDate.Day, "Day of Modified Julian Date Zero");

			double jan1_1985 = 46066.25;
			DateTime jan1_1985Date = CalendarConverter.ModifiedJulian2Gregorian (jan1_1985);

			Assert.AreEqual (1985, jan1_1985Date.Year, "Year of jan 1 1985");
			Assert.AreEqual (1, jan1_1985Date.Month, "Month of jan 1 1985");
			Assert.AreEqual (1, jan1_1985Date.Day, "Day of jan 1 1985");
			Assert.AreEqual (6, jan1_1985Date.Hour, "Hour of jan 1 1985");
		}

		[TestMethod] public void TrickyDates() 
		{
			double julianDate = 46096.999999998196;
			DateTime gregorian = CalendarConverter.ModifiedJulian2Gregorian (julianDate);

			Assert.AreEqual (1985, gregorian.Year, "Year expected");
			Assert.AreEqual (2, gregorian.Month, "Month expected");
			Assert.AreEqual (1, gregorian.Day, "Day expected");
		}

		private void Evaluate(DateTime inGregDate)
		{
			double modJulDate = CalendarConverter.Gregorian2ModifiedJulian(inGregDate);
			long mjdInt = (long) modJulDate;

			DateTime outGregDate = CalendarConverter.ModifiedJulian2Gregorian(modJulDate);

			Assert.AreEqual (inGregDate.ToString(), outGregDate.ToString(), modJulDate.ToString());
		}
	}
}
