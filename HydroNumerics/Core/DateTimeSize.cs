using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HydroNumerics.Core
{
  [DataContract]
  public class DateTimeSize : GalaSoft.MvvmLight.ObservableObject,IComparable<DateTimeSize>
  {

    public DateTimeSize()
    { }

    public DateTimeSize(DateTime Start, DateTime End)
    {
      this.Start = Start;
      this.End = End;
    }

    private DateTime _Start;
    [DataMember]
    public DateTime Start
    {
      get { return _Start; }
      set
      {
        if (_Start != value)
        {
          _Start = value;
          RaisePropertyChanged("Start");
        }
      }
    }

    private DateTime _End;
    [DataMember]
    public DateTime End
    {
      get { return _End; }
      set
      {
        if (_End != value)
        {
          _End = value;
          RaisePropertyChanged("End");
        }
      }
    }

    public TimeSpan Size { get { return End.Subtract(Start); } }

    /// <summary>
    /// Returns true if the two DateTimeSizes overlaps;
    /// </summary>
    /// <param name="Other"></param>
    /// <returns></returns>
    public bool OverLaps(DateTimeSize Other)
    {
      if (this.Start < Other.End & this.End > Other.Start)
        return true;

      return false;
    }

    /// <summary>
    /// Returns true if the Datetimesize includes the time. Both Start and End are included.
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public bool Includes(DateTime Time)
    {
      return Start <= Time & End >= Time;
    }

    public override string ToString()
    {
      return Start.ToString() + "  " + End.ToString();
    }

    public override bool Equals(object obj)
    {
      DateTimeSize other = obj as DateTimeSize;
      if (other == null)
        return false;

      return Start.Equals(other.Start) & End.Equals(other.End);
    }

    public override int GetHashCode()
    {
      return Start.GetHashCode() * 397 * End.GetHashCode();
    }


    public int CompareTo(DateTimeSize other)
    {
      int compare = Start.CompareTo(other.Start);
      if (compare == 0) //Same starttime, use endtime
        compare = End.CompareTo(other.End);
      return compare;
    }
  }
}
