using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using System.IO;


namespace HydroNumerics.Core.Time
{
  [DataContract]
  public class TimeStampSeries:BaseTimeSeries<TimeStampValue>
  {

    #region Constructors

    public TimeStampSeries():base()
    {
      Items.CollectionChanged += Items_CollectionChanged;
    }

    void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      dateIndex = null;
    }

    public TimeStampSeries(IEnumerable<TimeStampValue> Values)
      : base(Values)
    {
      if (Items.Count > 1)
        TimeStepSize = TSTools.GetTimeStep(Items[0].Time, Items[1].Time);
      else
        TimeStepSize = TimeStepUnit.None;
      Items.CollectionChanged += Items_CollectionChanged;
    }

    #endregion

    public void GapFill(InterpolationMethods Method)
    {
      if (this.TimeStepSize == TimeStepUnit.None)
        throw new Exception("Cannot GapFill when the timestep unit is not set");

      List<int> Xvalues = new List<int>();
      List<double> Yvalues = new List<double>();
      Xvalues.Add(0);
      Yvalues.Add(Items.First().Value);
      int counter = 0;

      for (int i = 1; i < Items.Count; i++)
      {
        DateTime next =Items[i - 1].Time;
        while ( (next= TSTools.GetNextTime(next, this.TimeStepSize)) <= Items[i].Time)
        {
          counter++;
        }
        Yvalues.Add(Items[i].Value);
        Xvalues.Add(counter);

      }

      if (Method == InterpolationMethods.DeleteValue)
      {
        for (int i = 1; i < Yvalues.Count; i++)
        {
          for (int j = Xvalues[i - 1]+1; j < Xvalues[i]; j++)
            Items.Insert(j, new TimeStampValue(TSTools.GetNextTime(Items[j - 1].Time, this.TimeStepSize), DeleteValue));
        }
      }
      else
        throw new Exception("Not implemented yet");
    }


    /// <summary>
    /// Gets the value at a specified time
    /// Currently, does not interpolate
    /// </summary>
    /// <param name="Time"></param>
    /// <param name="interpolate"></param>
    /// <returns></returns>
    public double GetValue(DateTime Time, InterpolationMethods interpolate)
    {
      double value= DeleteValue;
      int index = GetIndexOfValue(Time);
        switch (interpolate)
        {
          case InterpolationMethods.Linear:
            break;
          case InterpolationMethods.CubicSpline:
            break;
          case InterpolationMethods.DeleteValue:
            if (index < Count & index>=0)
              if (Items[index].Time == Time) //Time has to be correct
                value = Items[index].Value;
            break;
          case InterpolationMethods.Nearest:
            break;
          default:
            break;
        }
      return value;
    }



    private int GetIndexOfValue(DateTime Time)
    {
      if (Time <= StartTime)
        return 0;
      if (Time >= EndTime)
        return Count - 1;

      int index =0;
      if (DateIndex.TryGetValue(Time.Date, out index))
      {
        while (index < Count - 2 && Items[index + 1].Time <= Time)
          index++;
      }
      return index;
    }

    /// <summary>
    /// Returns items within the time span. Both start and end are included
    /// </summary>
    /// <param name="TimeSpan"></param>
    /// <returns></returns>
    public IEnumerable<TimeStampValue> GetSubSeries(DateTimeSize TimeSpan)
    {
      int start = GetIndexOfValue(TimeSpan.Start);
      int end = GetIndexOfValue(TimeSpan.End);
      return Items.Skip(start).Take(end - start+1);
    }


    /// <summary>
    /// Converts the values using the converter
    /// </summary>
    /// <param name="tc"></param>
    public void Convert(TimeSeriesConverter tc)
    {
      var vals = tc.Convert(Items);
      Items.Clear();
      Items.AddRange(vals);
    }


    /// <summary>
    /// Gets the time of the first entry
    /// </summary>
    public DateTime StartTime
    {
      get
      {
        if (Items.Count == 0)
          return DateTime.MinValue;
        return Items.First().Time;
      }
    }

    /// <summary>
    /// Gets the time of the last entry
    /// </summary>
    public DateTime EndTime
    {
      get
      {
        if (Items.Count == 0)
          return DateTime.MinValue;
        return Items.Last().Time;
      }
    }


    private Dictionary<DateTime, int> dateIndex;

    private Dictionary<DateTime, int> DateIndex
    {
      get
      {
        if (dateIndex == null)
        {
          dateIndex = new Dictionary<DateTime, int>();
          for (int i = 0; i < Items.Count; i++)
          {
            if (!dateIndex.ContainsKey(Items[i].Time.Date))
              dateIndex.Add(Items[i].Time.Date, i);
          }
        }
        return dateIndex;
      }
    }

    /// <summary>
    /// Serialize this to xml
    /// </summary>
    /// <returns></returns>
    public string Serialize()
    {
      MemoryStream ms = new MemoryStream();
      DataContractSerializer ser = new DataContractSerializer(this.GetType());
      ser.WriteObject(ms, this);
      var bytearray =ms.ToArray();
      return Encoding.UTF8.GetString(bytearray,0, bytearray.Count());
    }

    public static TimeStampSeries DeSerialize(string xml)
    {
      MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
      DataContractSerializer ser = new DataContractSerializer(typeof(TimeStampSeries));
      return (TimeStampSeries) ser.ReadObject(ms);
    }
  }
}
