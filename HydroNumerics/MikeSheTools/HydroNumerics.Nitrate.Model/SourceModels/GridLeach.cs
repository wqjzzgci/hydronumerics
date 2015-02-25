using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core.Time;

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
      TimeData = new FixedTimeSeries();
    }
    

    public int GridID { get; private set; }
    public int DMIGridID { get; set; }
    public int LandUseID { get; set; }
    public int SoilID { get; set; }


    public FixedTimeSeries TimeData { get; private set; }

    /// <summary>
    /// Adds another year of data
    /// </summary>
    /// <param name="start"></param>
    /// <param name="data"></param>
    public void AddYear(DateTime start, float[] data)
    {
        TimeData.AddRange(start, TimeSpan.FromDays(1), data.Take(data.Count() - 2).ToList());

      //What to do with the sum and the negative data
      float sum = data[data.Count() - 2];
      float NegativeValues = data[data.Count() - 1];
    }

  }
}
