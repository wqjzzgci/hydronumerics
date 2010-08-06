using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.IO;

using MathNet.Numerics.LinearAlgebra;

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
      try
      {
        MikeSheGridInfo _grid = null;
        Results _res = null;
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

        sw.WriteLine("OBS_ID\tX\tY\tDepth\tLAYER\tOBS_VALUE\tDATO\tSIM_VALUE_INTP\tSIM_VALUE_CELL\tME\tME^2\t#DRY_CELLS\t#BOUNDARY_CELLS\tCOLUMN\tROW\tCOMMENT");
        swell.WriteLine("OBS_ID\tX\tY\tDepth\tLAYER\tME\tME^2");


        //Loops the wells that are within the model area
        foreach (MikeSheWell W in SelectedWells)
        {
          double MEWell=0;
          double ME2Well=0;
          

          //Get layer or depth
          if (W.Layer == -3)
            W.Layer = _grid.GetLayerFromDepth(W.Column, W.Row, W.Depth);
          else
            W.Depth = _grid.SurfaceTopography.Data[W.Row, W.Column] - (_grid.LowerLevelOfComputationalLayers.Data[W.Row, W.Column, W.Layer] + 0.5 * _grid.ThicknessOfComputationalLayers.Data[W.Row, W.Column, W.Layer]);

          //Calculate results
          foreach (Intake I in W.Intakes)
          {
            foreach (TimestampValue TSE in I.HeadObservations.Items)
            {
              StringBuilder ObsString = new StringBuilder();
              string Comment="";
              double? MECell=null;
              double? RMSCell=null;
              double? SimulatedValueCell=null;
              int DryCells=0;
              int BoundaryCells=0;
              double? InterpolatedValue=null;

              if (W.Layer < 0)
                Comment = "Depth is above the surface or below bottom of the model domain";
              else
              {
                Matrix M = _res.PhreaticHead.TimeData(TSE.Time)[W.Layer];
                SimulatedValueCell = M[W.Row, W.Column];

                //Interpolates in the matrix
                InterpolatedValue = _grid.Interpolate(W.X, W.Y, W.Layer, M, out DryCells, out BoundaryCells);


                MECell = TSE.Value - InterpolatedValue;
                RMSCell = Math.Pow(MECell.Value, 2);
                MEWell += MECell.Value;
                ME2Well += RMSCell.Value;

                if (SimulatedValueCell == _res.DeleteValue)
                {
                  ObsTotal[W.Layer]++;
                  Comment = "Cell is dry";
                }
                else
                {
                  ME[W.Layer] += MECell.Value;
                  RMSE[W.Layer] += RMSCell.Value;
                  ObsUsed[W.Layer]++;
                  ObsTotal[W.Layer]++;
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
              ObsString.Append(InterpolatedValue + "\t");
              ObsString.Append(SimulatedValueCell + "\t");
              ObsString.Append(MECell + "\t");
              ObsString.Append(RMSCell + "\t");
              ObsString.Append(DryCells + "\t");
              ObsString.Append(BoundaryCells + "\t");
              ObsString.Append(W.Column + "\t");
              ObsString.Append(W.Row + "\t");
              ObsString.Append(Comment);
              sw.WriteLine(ObsString.ToString());
            }
          }
          //Write for each well
          StringBuilder WellString = new StringBuilder();
          WellString.Append(W.ID + "\t");
          WellString.Append(W.X + "\t");
          WellString.Append(W.Y + "\t");
          WellString.Append(W.Depth + "\t");
          if (W.Layer >= 0)
          {
            WellString.Append((_grid.NumberOfLayers - W.Layer) + "\t");
            WellString.Append(MEWell / W.Intakes.First().HeadObservations.Items.Count + "\t");
            WellString.Append(ME2Well / W.Intakes.First().HeadObservations.Items.Count + "\t");
          }
          else
            WellString.Append((W.Layer) + "\t" + "\t" + "\t");
          swell.WriteLine(WellString.ToString());
        }
        sw.Flush();
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

        //Dispose MikeShe
        _grid.Dispose();
        _res.Dispose();
      }

      catch (Exception e)
      {
        MessageBox.Show("Der er opstået en fejl af typen: " + e.Message);
      } 
		}
	}
}
