using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  /// <summary>
  /// This class holds all the data read from a file of daisy leaching
  /// </summary>
  public class DistributedLeaching
  {
    public SoilCodesGrid DaisyCodes;
    private Dictionary<int, GridLeach> Grids = new Dictionary<int, GridLeach>();
    object Lock = new object();

    private Dictionary<int, float[]> CatchLeach = new Dictionary<int, float[]>();
    private DateTime Start;


    public DistributedLeaching()
    {
    }

    public void LoadSoilCodesGrid(string ShapeFileName)
    {
      DaisyCodes = new SoilCodesGrid();
      DaisyCodes.BuildGrid(ShapeFileName);
    }

    /// <summary>
    /// Loads a file and returns the sum. For testing only
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public double LoadAndSum(string FileName)
    {
      double sum = 0;
      double neg = 0;
      using (StreamReader sr = new StreamReader(FileName))
      {
        //        Headers = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        while (!sr.EndOfStream)
        {
          var data = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
          sum += double.Parse(data[data.Count() - 2]);
          neg += double.Parse(data[data.Count() - 1]);
        }
      }
      return sum;
    }


    public void DebugPrint(string outputpath, string FileName, string SoilGridCodesFileName)
    {

      List<GeoRefData> Allpoints;
      ProjNet.CoordinateSystems.ICoordinateSystem proj;
      using (ShapeReader shr = new ShapeReader(SoilGridCodesFileName))
      {
        proj = shr.Projection;
        Allpoints = shr.GeoData.ToList();
      }
      Dictionary<int, GeoRefData> data= new Dictionary<int,GeoRefData>();
      foreach(var p in Allpoints)
      {
        data.Add((int)p.Data["GRIDID"], p);
      }

      string name = Path.GetFileNameWithoutExtension(FileName);
      name = "Y_"+ name.Substring(name.Length - 4);

      var dt = Allpoints.First().Data.Table;
      dt.Columns.Add(name, typeof(double));

      List<GeoRefData> NewPoints = new List<GeoRefData>();

      using (ShapeWriter sw = new ShapeWriter(Path.Combine(outputpath, "Leach_"+name+"_debug.shp")) { Projection = proj })
      {
        using (StreamReader sr = new StreamReader(FileName))
        {
          //        Headers = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
          while (!sr.EndOfStream)
          {
            var de = sr.ReadLine().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            double leach = double.Parse(de[de.Count() - 2]);
            int gridid = int.Parse(de[0]);
            var p = data[gridid];
            p.Data[name] = leach;
            NewPoints.Add(p);
          }
        }
        foreach (var v in NewPoints)
          sw.Write(v);

      }
    }

    /// <summary>
    /// In this method it is calculated how much leaches out of catchment
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <param name="Catchments"></param>
    public void BuildLeachData(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      this.Start = Start;
//      NewMessage("Build leach time series for: " + Catchments.Count());
      var dist = DaisyCodes.GetGridIdsWithInCatchment(Catchments);

      int numberofmonths = (End.Year - Start.Year) * 12 + End.Month - Start.Month;

      Parallel.ForEach(dist, c =>
      {
        List<float> values = new List<float>();
        for (int i = 0; i < numberofmonths; i++)
          values.Add(0);

        foreach (var p in c.Value)
        {
          var newlist = GetValues(p, Start, End);
          if (newlist != null)
          {
            for (int i = 0; i < numberofmonths; i++)
              values[i] += newlist[i];
          }
        }
        lock (Lock)
          CatchLeach.Add(c.Key, values.ToArray());
      });
    }



    public void LoadFileParallel(string FileName)
    {
      using (StreamReader sr = new StreamReader(FileName))
      {
        List<string> lines = new List<string>();
        bool finished = false;

        while (!finished)
        {
          lines.Clear();
          for (int i = 0; i < 10000; i++)
          {
            if (sr.EndOfStream)
            {
              finished = true;
              break;
            }
            lines.Add(sr.ReadLine());
          }

          Parallel.ForEach(lines, l =>
          {
            var data = l.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            int gridid = int.Parse(data[0]);

            GridLeach CurrentGrid = null;
            if (!Grids.TryGetValue(gridid, out CurrentGrid)) //Check if we have read the grid before. No need for the grids to be ordered
            {
              CurrentGrid = new GridLeach(gridid);
              CurrentGrid.SoilID = int.Parse(data[4]);
              CurrentGrid.DMIGridID = int.Parse(data[5]);
              CurrentGrid.LandUseID = int.Parse(data[6]);
              lock (Lock)
                Grids.Add(gridid, CurrentGrid);
            }

            //If the end date is written
            //          DateTime Start = new DateTime(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3])).AddDays(-int.Parse(data[7]) + 1);
            //If the start date is written
            DateTime Start = new DateTime(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
            CurrentGrid.AddYear(Start, data.Skip(9).Select(v => float.Parse(v)).ToArray());
          });
        }
      }
    }

    public void ClearMemory()
    {
      Grids.Clear();
    }

    /// <summary>
    /// Returns the leaching from the catchment in kg/s at the current time
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>
    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      if (CatchLeach.ContainsKey(c.ID))
        return CatchLeach[c.ID][(CurrentTime.Year - Start.Year) * 12 + CurrentTime.Month - Start.Month];
      else
        return 0;
    }


    public double GetSum(DateTime Start, DateTime End)
    {
      double sum = 0;
      foreach(var kvp in Grids)
      {
        sum += GetValues(kvp.Key, Start, End).Sum();
      }

      return sum*86400.0*30.5;
    }

    /// <summary>
    /// Gets the leaching in a gird
    /// </summary>
    /// <param name="gridid"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    private float[] GetValues(int gridid, DateTime Start, DateTime End)
    {
      //Get the area. Not all grids have the same area
      float area = (float)DaisyCodes.GetArea(gridid) / 10000f; //Square meters to ha

      if (Grids.ContainsKey(gridid))
        return Grids[gridid].TimeData.GetValues(Start, End).Select(v => v * area).ToArray();
      else
        return null;
    }


    /// <summary>
    /// Gets the leaching in a point
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <returns></returns>
    public float[] GetValues(double x, double y, DateTime Start, DateTime End)
    {
      int gridid = DaisyCodes.GetID(x, y);
      if (gridid == 0)
        return null;
      return GetValues(gridid, Start, End);
    }

  }
}
