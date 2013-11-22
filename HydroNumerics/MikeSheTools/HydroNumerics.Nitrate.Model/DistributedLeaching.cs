using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public class DistributedLeaching
  {

    public Dictionary<int, GridLeach> Grids {get;private set;}

    public DistributedLeaching()
    {
      Grids = new Dictionary<int, GridLeach>();
    }

    /// <summary>
    /// Loads a file
    /// </summary>
    /// <param name="FileName"></param>
    public void LoadFile(string FileName)
    {
      string[] Headers;

      GridLeach CurrentGrid=null;
      using (StreamReader sr = new StreamReader(FileName))
      {
        Headers = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        while (!sr.EndOfStream)
        {
          var data = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
          int gridid =int.Parse(data[0]);

          if (CurrentGrid == null || CurrentGrid.GridID!=gridid )
          {
            if (!Grids.TryGetValue(gridid, out CurrentGrid)) //Check if we have read the grid before. No need for the grids to be ordered
            {
              CurrentGrid = new GridLeach(gridid);
              CurrentGrid.SoilID = int.Parse(data[4]);
              CurrentGrid.DMIGridID = int.Parse(data[5]);
              CurrentGrid.LandUseID = int.Parse(data[6]);
              Grids.Add(gridid, CurrentGrid);
            }
          }
          CurrentGrid.AddYear(new DateTime(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3])).AddDays(-int.Parse(data[7])+1), data.Skip(9).Select(v => float.Parse(v)).ToArray());
        }
      }
    }
  }
}
