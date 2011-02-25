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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;


namespace HydroNumerics.Time.Core
{
    public static class TimeSeriesGroupFactory
    {
       
        public static TimeSeriesGroup Create(string FileName)
        {
            FileStream fileStream = new FileStream(FileName, FileMode.Open);
            return Create(fileStream);
        }

      /// <summary>
      /// Loads all timeseries found in the File. Will only work if they are stored without preserving references
      /// </summary>
      /// <param name="FileName"></param>
      /// <returns></returns>
        public static TimeSeriesGroup CreateAll(string FileName)
        {
          XDocument doc = XDocument.Load(FileName);
          List<Type> KnownTypes = new List<Type>();
          KnownTypes.Add(typeof(TimespanSeries));
          KnownTypes.Add(typeof(TimestampSeries));

          dc = new DataContractSerializer(typeof(BaseTimeSeries), KnownTypes, int.MaxValue, false, true, null);
          tsg = new TimeSeriesGroup();

          doc.ElementsAfterSelf("BaseTimeSeries");

          foreach (var el in doc.Elements())
          {
            Traverse(el);
          }

          return tsg;
        }

        private static TimeSeriesGroup tsg;
        private static DataContractSerializer dc;

      /// <summary>
      /// Recursive method that traverses an Xelement and deserializes all BaseTimeSeries
      /// </summary>
      /// <param name="xe"></param>
        private static void Traverse(XElement xe)
        {
          foreach (var x in xe.Elements())
          {
            if (x.Name.LocalName == "BaseTimeSeries")
            {
              var bs = (BaseTimeSeries)dc.ReadObject(x.CreateReader());
              tsg.Items.Add(bs);
            }
            else
              Traverse(x);
          }

        }


        public static TimeSeriesGroup Create(FileStream fileStream)
        {
          DataContractSerializer dc = new DataContractSerializer(typeof(TimeSeriesGroup), null, int.MaxValue, false, true, null);

          //  XmlSerializer serializer = new XmlSerializer(typeof(TimeSeriesGroup));
          

          TimeSeriesGroup timeSeriesGroup = (TimeSeriesGroup)dc.ReadObject(fileStream);
//            TimeSeriesGroup timeSeriesGroup = (TimeSeriesGroup)serializer.Deserialize(fileStream);
            fileStream.Close();
            return timeSeriesGroup;
        }
    }
}
