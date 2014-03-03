using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  /// <summary>
  /// This class holds all the data read from a file of daisy leaching
  /// </summary>
  public class DistributedLeaching
  {
    public SoilCodesGrid DaisyCodes;
    private Dictionary<int, GridLeach> Grids {get; set;}

    public DistributedLeaching()
    {
      Grids = new Dictionary<int, GridLeach>();
    }

    public void LoadSoilCodesGrid(string ShapeFileName)
    {
      DaisyCodes = new SoilCodesGrid();
      DaisyCodes.BuildGrid(ShapeFileName);
    }


    public double LoadAndSum(string FileName)
    {
      double sum =0;
      //This can be made faster. Read a lot of lines and process in parallel. Go to monthly directly
      using (StreamReader sr = new StreamReader(FileName))
      {
        //        Headers = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

        while (!sr.EndOfStream)
        {
          var data = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
          sum += double.Parse(data[data.Count() - 2]);
        }
      }
        return sum;

    }

    /// <summary>
    /// Loads a file
    /// </summary>
    /// <param name="FileName"></param>
    public void LoadFile(string FileName)
    {
      string[] Headers;

      GridLeach CurrentGrid=null;
      //This can be made faster. Read a lot of lines and process in parallel. Go to monthly directly
      using (StreamReader sr = new StreamReader(FileName))
      {
//        Headers = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

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

          //If the end date is written
//          DateTime Start = new DateTime(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3])).AddDays(-int.Parse(data[7]) + 1);
          //If the start date is written
          DateTime Start = new DateTime(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
          CurrentGrid.AddYear(Start, data.Skip(9).Select(v => float.Parse(v)).ToArray());
        }
      }
    }

    public float[] GetValues(int gridid, DateTime Start, DateTime End)
    {
      float area = (float)DaisyCodes.GetArea(gridid) / 10000f; //Square meters to ha
      return Grids[gridid].TimeData.GetValues(Start, End).Select(v => v * area).ToArray();

    }


    public float[] GetValues(double x, double y, DateTime Start, DateTime End)
    {
      int gridid = DaisyCodes.GetID(x, y);
      return GetValues(gridid, Start, End);
    }

  }
}
