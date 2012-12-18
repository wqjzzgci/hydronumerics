using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.IO;

using MathNet.Numerics.LinearAlgebra.Double;

using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.Wells;
using HydroNumerics.Time.Core;


namespace HydroNumerics.MikeSheTools.LayerStatistics
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
      DFS.DFS3.MaxEntriesInBuffer =10;
      bool stay = true;

      while (stay)
      {
        stay = false;
        foreach (var v in System.Diagnostics.Process.GetProcesses())
        {
          if (v.Id != System.Diagnostics.Process.GetCurrentProcess().Id)
          {
            if (v.ProcessName.ToLower().StartsWith("ls.exe"))
            {
              if (v.UserProcessorTime > TimeSpan.FromSeconds(0.2))
              {
                stay = true;
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
                break;
              }
            }
          }
        }
      }

      MikeSheGridInfo _grid = null;
      Results _res = null;
      try
      {
        string ObsFileName;

        //Input is a -she-file and an observation file
        if (args.Length == 2)
        {
          Model MS = new Model(args[0]);
          _grid = MS.GridInfo;
          _res = MS.Results;
          ObsFileName = args[1];
        }
        //Input is an .xml-file
        else if (args.Length == 1)
        {
          Configuration cf = Configuration.ConfigurationFactory(args[0]);

          _grid = new MikeSheGridInfo(cf.PreProcessedDFS3, cf.PreProcessedDFS2);

          if (cf.HeadItemText != null)
            _res = new Results(cf.ResultFile, _grid, cf.HeadItemText);
          else
            _res = new Results(cf.ResultFile, _grid);

          if (_res.Heads == null)
            throw new Exception("Heads could not be found. Check that item: \"" + _res.HeadElevationString + "\" exists in + " + cf.ResultFile);

          if (_grid.NumberOfLayers != _res.Heads.TimeData(0).LayerCount)
            throw new Exception("Number of layers in preprocessed files do not match number of layers in resultfile: " + cf.ResultFile);

          ObsFileName = cf.ObservationFile;
        }
        else
        {
          OpenFileDialog Ofd = new OpenFileDialog();

          Ofd.Filter = "Known file types (*.she)|*.she";
          Ofd.ShowReadOnly = true;
          Ofd.Title = "Select a MikeShe setup file";


          if (DialogResult.OK == Ofd.ShowDialog())
          {
            Model MS = new Model(Ofd.FileName);
            _grid = MS.GridInfo;
            _res = MS.Results;
            Ofd.Filter = "Known file types (*.txt)|*.txt";
            Ofd.ShowReadOnly = true;
            Ofd.Title = "Select a LayerStatistics setup file";

            if (DialogResult.OK == Ofd.ShowDialog())
              ObsFileName = Ofd.FileName;
            else
              return;
          }
          else
            return;
        }

        InputOutput IO = new InputOutput(_grid.NumberOfLayers);

        string _baseOutPutFileName;
        string path = Path.GetDirectoryName(ObsFileName);
        string FileName = Path.GetFileNameWithoutExtension(ObsFileName);
        _baseOutPutFileName = Path.Combine(path, FileName);


        //Read in the wells
        Dictionary<string, MikeSheWell> Wells = IO.ReadFromLSText(ObsFileName);

        int NLay = _grid.NumberOfLayers;
        double[] ME = new double[NLay];
        double[] RMSE = new double[NLay];
        int[] ObsUsed = new int[NLay];
        int[] ObsTotal = new int[NLay];

        //Initialiserer
        for (int i = 0; i < NLay; i++)
        {
          ME[i] = 0;
          RMSE[i] = 0;
          ObsUsed[i] = 0;
          ObsTotal[i] = 0;
        }

        //Only operate on wells within the mikeshe area
        var SelectedWells = _grid.SelectByMikeSheModelArea(Wells.Values);


        StreamWriter sw = new StreamWriter(_baseOutPutFileName + "_observations.txt");
        StreamWriter swell = new StreamWriter(_baseOutPutFileName + "_wells.txt");

        sw.WriteLine("OBS_ID\tX\tY\tDepth\tLAYER\tOBS_VALUE\tDATO\tSIM_VALUE_INTP\tSIM_VALUE_CELL\tME\tME^2\t#DRY_CELLS\t#BOUNDARY_CELLS\tCOLUMN\tROW\tCOMMENT\t#OBSInWell");
        swell.WriteLine("OBS_ID\tX\tY\tDepth\tLAYER\tME\tME^2");


        //Loops the wells that are within the model area and set the layer or depth
        foreach (MikeSheWell W in SelectedWells)
        {
          //Get layer or depth
          if (W.Layer == -3)
            W.Layer = _grid.GetLayerFromDepth(W.Column, W.Row, W.Depth.Value);
          else
            W.Depth = _grid.SurfaceTopography.Data[W.Row, W.Column] - (_grid.LowerLevelOfComputationalLayers.Data[W.Row, W.Column, W.Layer] + 0.5 * _grid.ThicknessOfComputationalLayers.Data[W.Row, W.Column, W.Layer]);
        }

        System.Diagnostics.Stopwatch swm = new System.Diagnostics.Stopwatch();

        swm.Start();


        foreach (Observation TSE in SelectedWells.Where(var => var.Layer >= 0).SelectMany(var2 => var2.Intakes).SelectMany(var3 => ((LsIntake)var3).Observations).OrderBy(var4 => var4.Time))
        {
          var M = _res.PhreaticHead.TimeData(TSE.Time)[TSE.Well.Layer];
          TSE.SimulatedValueCell= M[TSE.Well.Row, TSE.Well.Column];
          int DryCells = 0;
          int BoundaryCells = 0;

          //Interpolates in the matrix
          TSE.InterpolatedValue = _grid.Interpolate(TSE.Well.X, TSE.Well.Y, TSE.Well.Layer, M, out DryCells, out BoundaryCells);

          TSE.DryCells = DryCells;
          TSE.BoundaryCells = BoundaryCells;
        }
        swm.Stop();


        //Loops the wells that are within the model area
        foreach (MikeSheWell W in SelectedWells)
        {
          double MEWell = 0;
          double RMSWell = 0;
          int UsedObsInWells = 0;

          //Calculate results
          foreach (LsIntake I in W.Intakes)
          {
            foreach (Observation TSE in I.Observations.OrderBy(var=>var.Time))
            {
              StringBuilder ObsString = new StringBuilder();
              string Comment = "";
              double? MECell = null;
              double? RMSCell = null;

              if (W.Layer < 0)
                Comment = "Depth is above the surface or below bottom of the model domain";
              else
              {

                MECell = TSE.Value - TSE.InterpolatedValue;
                RMSCell = Math.Pow(MECell.Value, 2);

                if (TSE.SimulatedValueCell == _res.DeleteValue)
                {
                  Comment = "Cell is dry";
                }
                else
                {
                  UsedObsInWells++;
                  MEWell += MECell.Value;
                  RMSWell += RMSCell.Value;
                }
              }
              ObsString.Append(W.ID + "\t");
              ObsString.Append(W.X + "\t");
              ObsString.Append(W.Y + "\t");
              ObsString.Append(W.Depth + "\t");

              if (W.Layer >= 0)
                ObsString.Append((_grid.NumberOfLayers - W.Layer) + "\t");
              else
                ObsString.Append((W.Layer) + "\t");
              ObsString.Append(TSE.Value + "\t");
              ObsString.Append(TSE.Time.ToShortDateString() + "\t");
              ObsString.Append(TSE.InterpolatedValue + "\t");
              ObsString.Append(TSE.SimulatedValueCell + "\t");
              ObsString.Append(MECell + "\t");
              ObsString.Append(RMSCell + "\t");
              ObsString.Append(TSE.DryCells + "\t");
              ObsString.Append(TSE.BoundaryCells + "\t");
              ObsString.Append(W.Column + "\t");
              ObsString.Append(W.Row + "\t");
              ObsString.Append(Comment + "\t");
              ObsString.Append(I.Observations.Count);
              sw.WriteLine(ObsString.ToString());
            }
          }

          if (UsedObsInWells > 0)
          {
            MEWell /= UsedObsInWells;
            RMSWell /= UsedObsInWells;
            ME[W.Layer] += MEWell;
            RMSE[W.Layer] += RMSWell;
            ObsUsed[W.Layer]++;
          }

          if (W.Layer>=0)
            ObsTotal[W.Layer]++;

          //Write for each well
          StringBuilder WellString = new StringBuilder();
          WellString.Append(W.ID + "\t");
          WellString.Append(W.X + "\t");
          WellString.Append(W.Y + "\t");
          WellString.Append(W.Depth + "\t");
          if (W.Layer >= 0)
          {
            WellString.Append((_grid.NumberOfLayers - W.Layer) + "\t");
            WellString.Append(MEWell + "\t");
            WellString.Append(RMSWell+ "\t");
          }
          else
            WellString.Append((W.Layer) + "\t" + "\t" + "\t");
          swell.WriteLine(WellString.ToString());
        }
        sw.Dispose();

        swell.Dispose();



        //Divide with the number of observations.
        for (int i = 0; i < NLay; i++)
        {
          ME[i] = ME[i] / ObsUsed[i];
          RMSE[i] = Math.Pow(RMSE[i] / ObsUsed[i], 0.5);
        }

        //Write output
        IO.WriteLayers(ME, RMSE, ObsUsed, ObsTotal);

      }

      catch (Exception e)
      {
        MessageBox.Show("An error has occurred in LayerStatistics  " + e.Message);
      }

      finally
      {
        //Dispose MikeShe
        _grid.Dispose();
        _res.Dispose();
      }
		}
	}
}
