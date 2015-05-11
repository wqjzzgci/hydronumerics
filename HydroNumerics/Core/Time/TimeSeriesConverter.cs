using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HydroNumerics.Core.Time
{
  [DataContract(Name = "ConverterTypes")]
  public enum ConverterTypes
  {
    [EnumMember]
    Offset = 1,
    [EnumMember]
    LinearConverter
  }


  [DataContract]
  [KnownType(typeof(OffsetConverter))]
  [KnownType(typeof(LinearConverter))]
  public class TimeSeriesConverter:BaseViewModel
  {

    public static TimeSeriesConverter GetConverter(ConverterTypes ct)
    {
      switch (ct)
      {
        case ConverterTypes.Offset:
          return new OffsetConverter() { TypeOfConverter = ct };
        case ConverterTypes.LinearConverter:
          return new LinearConverter() { TypeOfConverter = ct };
        default:
          break;
      }
      return null;
    }

    public TimeSeriesConverter()
    {
      Initialize();
    }

    /// <summary>
    /// Use this method to setup functions that cannot be serialized
    /// </summary>
    protected virtual void Initialize()
    { }

    /// <summary>
    /// This method is called when the object is built on the client
    /// </summary>
    /// <param name="context"></param>
    [OnDeserialized]
    public void Initialize(StreamingContext context)
    {
      Initialize();
    }


    private ConverterTypes _TypeOfConverter;
    [DataMember]
    public ConverterTypes TypeOfConverter
    {
      get { return _TypeOfConverter; }
      set
      {
        if (_TypeOfConverter != value)
        {
          _TypeOfConverter = value;
          RaisePropertyChanged("TypeOfConverter");
        }
      }
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

    private Func<double,double> _ConvertFunction;
    public Func<double,double> ConvertFunction
    {
      get { return _ConvertFunction; }
      set
      {
        if (_ConvertFunction != value)
        {
          _ConvertFunction = value;
          RaisePropertyChanged("ConvertFunction");
        }
      }
    }

    private Func<double, double> _ConvertBackFunction;
    public Func<double, double> ConvertBackFunction
    {
      get { return _ConvertBackFunction; }
      set
      {
        if (_ConvertBackFunction != value)
        {
          _ConvertBackFunction = value;
          RaisePropertyChanged("ConvertBackFunction");
        }
      }
    }
    
    /// <summary>
    /// Converts the values using the apropriate converter. Start time is included. Endtime is not
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public IEnumerable<TimeStampValue> Convert(IEnumerable<TimeStampValue> values)
    {
      if (values == null)
        yield return null;
     
      foreach (var val in values)
        yield return Convert(val);
    }

    /// <summary>
    /// Converts the value using the apropriate converter. Start time is included. Endtime is not
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual TimeStampValue Convert(TimeStampValue val)
    {
      if (val.Time >= Start & val.Time < End)
         return new TimeStampValue(val.Time, ConvertFunction(val.Value));
      else
         return val;
    }

    /// <summary>
    /// Converts the values back using the apropriate convertback function.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IEnumerable<TimeStampValue> ConvertBack(IEnumerable<TimeStampValue> values)
    {
      if (values == null)
        yield return null;

      foreach (var val in values)
      {
        if (val.Time >= Start & val.Time <= End)
          yield return new TimeStampValue(val.Time, ConvertBackFunction(val.Value));
        else
          yield return val;
      }
    }

    private double? _Par1;
    [DataMember]
    public double? Par1
    {
      get { return _Par1; }
      set
      {
        if (_Par1 != value)
        {
          _Par1 = value;
          RaisePropertyChanged("Par1");
        }
      }
    }

    private double? _Par2;
    [DataMember]
    public double? Par2
    {
      get { return _Par2; }
      set
      {
        if (_Par2 != value)
        {
          _Par2 = value;
          RaisePropertyChanged("Par2");
        }
      }
    }

    private double? _Par3;
    [DataMember]
    public double? Par3
    {
      get { return _Par3; }
      set
      {
        if (_Par3 != value)
        {
          _Par3 = value;
          RaisePropertyChanged("Par3");
        }
      }
    }
  }
}
