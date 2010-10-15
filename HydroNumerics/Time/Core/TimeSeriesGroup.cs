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
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HydroNumerics.Time.Core
{
  [DataContract]
  public class TimeSeriesGroup : System.ComponentModel.INotifyPropertyChanged
  {

    private int current = 0; //The index of the timeseries currently being edited or viewed

    [XmlIgnore]
    public int Current
    {
      get { return current; }
      set
      {
        if (value < 0)
        {
          current = 0;
        }
        else if (value >= items.Count)
        {
          current = items.Count - 1;
        }
        else
        {
          current = value;
        }
        NotifyPropertyChanged("Current");
      }
    }
    [DataMember]
    private System.ComponentModel.BindingList<BaseTimeSeries> items;

    public System.ComponentModel.BindingList<BaseTimeSeries> Items
    {
      get { return items; }
      set
      {
        items = value;
        NotifyPropertyChanged("TimeSeriesList");
      }
    }

    [DataMember]
    private string name;
    public string Name 
    {
        get
        {
            return name;
        }
        set
        {
           name = value;
        }
    }

    DateTime startTime;
    /// <summary>
    /// The earliest time in all timeseries in the timeseriesgroup
    /// </summary>
    public DateTime StartTime 
    {
        get
        {
            startTime = DateTime.MaxValue;
            foreach (BaseTimeSeries ts in this.Items)
            {
              
                if (ts.StartTime < startTime)
                {
                    startTime = ts.StartTime;
                }
            }
            return startTime;
        }
    }

    DateTime endTime;
    /// <summary>
    /// The latest time in all timeseries in the timeseriesgroup
    /// </summary>
    public DateTime EndTime
    {
        get
        {
            endTime = DateTime.MinValue;
            foreach (BaseTimeSeries ts in this.Items)
            {
                if (ts.EndTime > endTime)
                {
                    endTime = ts.EndTime;
                }
            }
           return endTime;
        }
    }

    HydroNumerics.Core.Timespan overlap;
    /// <summary>
    /// The overlapping period of all timeseries in the timeseriesgroup.
    /// </summary>
    public HydroNumerics.Core.Timespan Overlap
    {
        get 
        {
            overlap = new HydroNumerics.Core.Timespan();
            overlap.Start = DateTime.MinValue;
            overlap.End = DateTime.MaxValue;
            foreach (BaseTimeSeries ts in this.items)
            {
                if (ts.StartTime > overlap.Start)
                {
                    overlap.Start = ts.StartTime;
                }
                if (ts.EndTime < overlap.End)
                {
                    overlap.End = ts.EndTime;
                }
            }
            if (overlap.Start > overlap.End) // no overlap
            {
                overlap = null;
            }

            return overlap;
        }
    }

    public TimeSeriesGroup()
    {
      this.items = new System.ComponentModel.BindingList<BaseTimeSeries>();
      current = 0;
      this.items.ListChanged += new System.ComponentModel.ListChangedEventHandler(timeSeriesList_ListChanged);
      name = "no name define";
    }

    void timeSeriesList_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
        NotifyPropertyChanged("TimeSeriesList");
    }
   

    #region INotifyPropertyChanged Members


    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }

    #endregion

    /// <summary>
    /// This method deletes all entries after the time
    /// </summary>
    /// <param name="Time"></param>
    public void ResetToTime(DateTime Time)
    {
      foreach (BaseTimeSeries T in Items)
      {
        T.RemoveAfter(Time);
      }
    }


    public void Save(FileStream fileStream)
    {
      DataContractSerializer dc = new DataContractSerializer(typeof(TimeSeriesGroup), null, int.MaxValue, false, true, null);

//      System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TimeSeriesGroup));
      FileStream stream = fileStream;
      dc.WriteObject(fileStream, this);
//      serializer.Serialize(stream, this);
      fileStream.Close();
    }

    public void Save(string filename)
    {
      FileStream stream = new FileStream(filename, FileMode.Create);
      Save(stream);
    }


  }
}
