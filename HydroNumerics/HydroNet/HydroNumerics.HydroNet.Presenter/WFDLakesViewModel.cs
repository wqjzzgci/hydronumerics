using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;


using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.Time.Core;


namespace HydroNumerics.HydroNet.ViewModel
{
  public class WFDLakesViewModel
  {
    public Dictionary<string, WFDLake> Lakes{get;set;}

    public DataTable DT { get; private set; }

    public WFDLakesViewModel()
    {
      PointShapeReader psp = new PointShapeReader(@"soervp1.shp");
      Lakes = new Dictionary<string, WFDLake>();

      DT = psp.Data.Read();

      psp.Data.SpoolBack();
      
      foreach (var l in psp.GeoData)
      {
        WFDLake wf = new WFDLake(); 

        wf.Polygon = (XYPolygon)l.Geometry;
        wf.Data = l.Data;
        string name =(string) l.Data[0];
        if (!Lakes.ContainsKey(name))
          Lakes.Add(name, wf);
  
      }
      psp.Dispose();

      Precipitation = new TimespanSeries();
      Precipitation.Name = "Precipitation";
      double[] values = new double[] { 108, 83, 73, 52, 61, 86, 99, 101, 75, 108, 85, 101 };
      AddMonthlyValues(Precipitation, 2007, values);

      Evaporation = new TimespanSeries();
      Evaporation.Name = "Evaporation";
      double[] values2 = new double[] { 4, 11, 34, 66, 110, 118, 122, 103, 61, 26, 7, 1 };
      AddMonthlyValues(Evaporation, 2007, values2);

    }



    public TimespanSeries Evaporation { get; set; }
    public TimespanSeries Precipitation { get; set; }

    private void AddMonthlyValues(TimespanSeries TS, int year, double[] values)
    {
      double conversion1 = 1.0 / 1000 / 86400 / 31;
      double conversion2 = 1.0 / 1000 / 86400 / 28;
      double conversion3 = 1.0 / 1000 / 86400 / 30;
      TS.Unit = new HydroNumerics.Core.Unit("mm/month", conversion3, 0);

      TS.AddValue(new DateTime(year, 1, 1), new DateTime(year, 2, 1), values[0]);
      TS.AddValue(new DateTime(year, 2, 1), new DateTime(year, 3, 1), values[1]);
      TS.AddValue(new DateTime(year, 3, 1), new DateTime(year, 4, 1), values[2]);
      TS.AddValue(new DateTime(year, 4, 1), new DateTime(year, 5, 1), values[3]);
      TS.AddValue(new DateTime(year, 5, 1), new DateTime(year, 6, 1), values[4]);
      TS.AddValue(new DateTime(year, 6, 1), new DateTime(year, 7, 1), values[5]);
      TS.AddValue(new DateTime(year, 7, 1), new DateTime(year, 8, 1), values[6]);
      TS.AddValue(new DateTime(year, 8, 1), new DateTime(year, 9, 1), values[7]);
      TS.AddValue(new DateTime(year, 9, 1), new DateTime(year, 10, 1), values[8]);
      TS.AddValue(new DateTime(year, 10, 1), new DateTime(year, 11, 1), values[9]);
      TS.AddValue(new DateTime(year, 11, 1), new DateTime(year, 12, 1), values[10]);
      TS.AddValue(new DateTime(year, 12, 1), new DateTime(year + 1, 1, 1), values[11]);

    }

        
}
}

