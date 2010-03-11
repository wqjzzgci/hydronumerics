using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace HydroNumerics.Time.Core
{
    public static class TimeSeriesGroupFactory
    {
       
        public static TimeSeriesGroup Create(string FileName)
        {
            FileStream fileStream = new FileStream(FileName, FileMode.Open);
            return Create(fileStream);
        }

        public static TimeSeriesGroup Create(FileStream fileStream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TimeSeriesGroup));
            TimeSeriesGroup timeSeriesGroup = (TimeSeriesGroup)serializer.Deserialize(fileStream);
            fileStream.Close();
            return timeSeriesGroup;
        }
    }
}
