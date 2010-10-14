using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.Time.Core;
using HydroNumerics.Core;

namespace HydroNumerics.DemoApps.TimeSeriesFromDFS2
{
  class Program
  {
    /// <summary>
    /// Small program that extracts a time series from a time variant dfs2.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {

      TimespanSeries PrecipTS = new TimespanSeries();
      PrecipTS.Name = "Precipitation";
      PrecipTS.Description = "Precipitation extracted from dfs2";
      PrecipTS.Unit = UnitFactory.Instance.GetUnit(NamedUnits.millimeterperday);

      //Open the DFS2-file
      DFS2 precip = new DFS2(@"C:\Users\Jacob\Projekter\MikeSheWrapperForGEUS\novomr6\2-layer-filer\Standard_korrigeret_Prec_DK_10km_1990-2008.dfs2");
      
      //UTM-coordinates for Gjeller sø
      double XUTM = 456198;
      double YUTM = 6272321;

      //Get column and row index from UTM- coordinates
      int col = precip.GetColumnIndex(XUTM);
      int row = precip.GetRowIndex(YUTM);

      //Loop all the time steps
      for (int i = 0; i < precip.NumberOfTimeSteps; i++)
      {
        //Extract the value
        var val =  precip.GetData(i, 1)[row, col];
        //Insert into timeseries
        PrecipTS.AddValue(precip.TimeSteps[i].Subtract(TimeSpan.FromDays(1)), precip.TimeSteps[i], val);
      }
      precip.Dispose();

      //Now do the same for evaporation. DFS2-file may have another grid and timesteps
      DFS2 evap = new DFS2(@"C:\Users\Jacob\Projekter\MikeSheWrapperForGEUS\novomr6\2-layer-filer\Novana_DK_EPmak_40km_1990-1998_20km_1999-2008_ed.dfs2");
      TimespanSeries EvapTS = new TimespanSeries();
      EvapTS.Name = "Evaporation";
      EvapTS.Description = "Evaporation extracted from dfs2";
      EvapTS.Unit = UnitFactory.Instance.GetUnit(NamedUnits.millimeterperday);


      //Get column and row index from UTM- coordinates
      col = evap.GetColumnIndex(XUTM);
      row = evap.GetRowIndex(YUTM);

      for (int i = 0; i < evap.NumberOfTimeSteps; i++)
      {
        //Extract the value
        var val = evap.GetData(i, 1)[row, col];
        //Insert into timeseries
        EvapTS.AddValue(evap.TimeSteps[i].Subtract(TimeSpan.FromDays(1)), evap.TimeSteps[i], val);
      }
      

      //Put all time series into a group and save
      TimeSeriesGroup tsgroup = new TimeSeriesGroup();
      tsgroup.Items.Add(PrecipTS);
      tsgroup.Items.Add(EvapTS);
      tsgroup.Save(@"C:\Users\Jacob\Projekter\GWSW-Interaction\Gjeller Sø\climate.xts");
    }
  }
}
