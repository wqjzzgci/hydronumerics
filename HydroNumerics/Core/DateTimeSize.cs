using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroNumerics.Core
{
  public class DateTimeSize:BaseViewModel
  {

    public DateTimeSize(DateTime Start, DateTime End)
    {
      this.Start = Start;
      this.End = End;
    }

    private DateTime _Start;
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

  }
}
