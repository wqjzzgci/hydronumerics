using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  /// <summary>
  /// This class holds the leaching from daisy i one GridBlock
  /// </summary>
  public class GridLeach
  {

    public GridLeach(int GridId)
    {
      this.GridID = GridId;
      Data = new List<DaisyTimeSeries>();
    }
    

    public int GridID { get; private set; }
    public int DMIGridID { get; set; }
    public int LandUseID { get; set; }
    public int SoilID { get; set; }

    public List<DaisyTimeSeries> Data { get; private set; }

    /// <summary>
    /// Adds another year of data
    /// </summary>
    /// <param name="start"></param>
    /// <param name="data"></param>
    public void AddYear(DateTime start, float[] data)
    {
      DaisyTimeSeries yd = new DaisyTimeSeries(start, data);

      bool Combined = false;

      for (int i=0;i<Data.Count;i++)
      {
        if (Data[i].TryCombine(yd))
        {
          yd = Data[i];
          Combined = true;
        }
      }
      if(!Combined)
        Data.Add(yd);
    }


    /// <summary>
    /// Gets the value at a point in time
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    public List<float> GetValues(DateTime First, DateTime Last)
    {
      List<float> toreturn = new List<float>();

      int start = (First.Year - Start.Year * 12 + First.Month - Start.Month);
      int last = (First.Year - Start.Year * 12 + First.Month - Start.Month);

      for (int i = start; i <= last; i++)
      {
        int tempi = Math.Min(Math.Max(i, 0), Data.First().DataSeries.Count()-1); //Because of this it will use the first year in the Daisy results for all previous years
        toreturn.Add(Data.First().DataSeries[tempi]);
      }
      return toreturn;
    }


    /// <summary>
    /// Gets the start of the timeseries
    /// </summary>
    public DateTime Start
    {
      get
      {
        return Data.First().Start;
      }
    }

    /// <summary>
    /// Gets the end of the timeseries
    /// </summary>
    public DateTime End
    {
      get
      {
        return Data.Last().End;
      }
    }
  }
}
