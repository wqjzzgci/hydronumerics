using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HydroNumerics.Core.Time
{
  [DataContract]
  public class TimeSeriesConverter:BaseViewModel
  {

    [DataMember]
    private List<Tuple<DateTime, Func<double, double>>> ConvertFunctions = new List<Tuple<DateTime, Func<double, double>>>();

    [DataMember]
    private List<Tuple<DateTime, Func<double, double>>> ConvertBackFunctions = new List<Tuple<DateTime, Func<double, double>>>();

    /// <summary>
    /// Adds a convertfunction and a convert back function
    /// </summary>
    /// <param name="FunctionStart"></param>
    /// <param name="Convertfunction"></param>
    /// <param name="ConvertBackfunction"></param>
    public void AddConvertFunction(DateTime FunctionStart, Func<double, double> Convertfunction, Func<double, double> ConvertBackfunction)
    {
      ConvertFunctions.Add(new Tuple<DateTime, Func<double, double>>(FunctionStart, Convertfunction));
      ConvertFunctions.Sort((e1, e2) => e1.Item1.CompareTo(e2.Item1));
      ConvertBackFunctions.Add(new Tuple<DateTime, Func<double, double>>(FunctionStart, ConvertBackfunction));
      ConvertBackFunctions.Sort((e1, e2) => e1.Item1.CompareTo(e2.Item1));
    }


    /// <summary>
    /// Converts the values using the apropriate converter
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public IEnumerable<TimeStampValue> Convert(IEnumerable<TimeStampValue> values)
    {
      if (values == null)
        yield return null;
     
      foreach (var val in values)
      {
        var cor = ConvertFunctions.LastOrDefault(h => h.Item1 < val.Time);
        if (cor != null)
          yield return new TimeStampValue(val.Time, cor.Item2(val.Value));
        else
          yield return val;
      }
    }

    /// <summary>
    /// Converts the values back using the apropriate convertback function.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public IEnumerable<TimeStampValue> ConvertBack(IEnumerable<TimeStampValue> values)
    {
      if (values == null)
        yield return null;

      foreach (var val in values)
      {
        var cor = ConvertBackFunctions.LastOrDefault(h => h.Item1 < val.Time);
        if (cor != null)
          yield return new TimeStampValue(val.Time, cor.Item2(val.Value));
        else
          yield return val;
      }
    }

  }
}
