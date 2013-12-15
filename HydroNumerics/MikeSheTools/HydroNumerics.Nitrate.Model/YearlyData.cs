using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  /// <summary>
  /// This class holds one year of daisy leaching data
  /// </summary>
  public class YearlyData
  {
    private float[] dataarray;
    private int? StartZeros;
    private int? EndZeros=-1;
    public DateTime Start { get; private set; }

    public float LeachSum { get; private set; }
    public float NegativeLeaching { get; private set; }

    public YearlyData(DateTime Start, float[] rawdata)
    {
      IsTimeStepMonth = false;
      this.Start = Start;
      List<float> datalist = new List<float>();
      int streak = 0;

      List<int> zeros = new List<int>();

      for (int i = 0; i < rawdata.Count()-2; i++) //If necessary use this reduce memory by not storing a row of zeros
      {
      //  if (rawdata[i] != 0)
          datalist.Add(rawdata[i]);
        //else
          //zeros.Add(i);
      }

      dataarray = datalist.ToArray();

      LeachSum = rawdata[rawdata.Count() - 2];
      NegativeLeaching = rawdata[rawdata.Count() - 1];
    }

    public bool IsTimeStepMonth { get; private set; }

    /// <summary>
    /// Reduce the daily data to monthly data
    /// </summary>
    public void ReduceToMonthlyTimeSteps()
    {
      List<float> temparray = new List<float>();
      int currentmonth= Start.Month;
      int daysincurrentmonth=0;
      double monthlysum = 0;
      for (int i = 0; i < dataarray.Count(); i++)
      {
        var next =Start.AddDays(i);
        if(next.Month != currentmonth)
        {
          temparray.Add((float) monthlysum/daysincurrentmonth);
          daysincurrentmonth=0;
          monthlysum=0;
          currentmonth =next.Month;
        }

        monthlysum += dataarray[i];
          daysincurrentmonth++;
      }
      temparray.Add((float)monthlysum / daysincurrentmonth);
      dataarray = temparray.ToArray();
      IsTimeStepMonth = true;
    }


    public DateTime End
    {
      get
      {
        if (StartZeros.HasValue)
          return Start.AddDays(dataarray.Count() + EndZeros.Value - StartZeros.Value);
        else
          return Start.AddDays(dataarray.Count());
      }
    }

    /// <summary>
    /// Gets the value for a given timestep
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public float GetValue(DateTime Time)
    {
      if (IsTimeStepMonth)
      {
        return dataarray[Time.Month - Start.Month];
      }
      else
      {
        int days = (int)Time.Subtract(Start).TotalDays;
        if (StartZeros.HasValue)
        {
          if (days < StartZeros.Value)
          {
            return dataarray[days];
          }
          else
          {
            if (days < EndZeros.Value)
              return 0;
            else
              return dataarray[days - (EndZeros.Value - StartZeros.Value)];
          }
        }
        else
          return dataarray[days];
      }
    }
  }
}
