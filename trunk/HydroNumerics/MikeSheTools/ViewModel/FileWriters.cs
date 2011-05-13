using System;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra;

using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.JupiterTools;
using HydroNumerics.Wells;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.Time.Core;

using DHI.TimeSeries;
using DHI.Generic.MikeZero.DFS;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.ViewModel
{

    /// <summary>
    /// A class with static methods to read in well-information from various sources and print out various input files.
    /// </summary>
    public class FileWriters
    {
      /// <summary>
      /// Function that returns true if a time series entry is between the two dates
      /// </summary>
      public static Func<TimespanValue, DateTime, DateTime, bool> InBetween2 = (TSE, Start, End) => TSE.StartTime >= Start & TSE.StartTime < End;


      /// <summary>
      /// Select the wells that are inside the model area. Does not look at the 
      /// z - coordinate
      /// </summary>
      /// <param name="MikeShe"></param>
      public static IEnumerable<MikeSheWell> SelectByMikeSheModelArea(MikeSheGridInfo Grid, IEnumerable<MikeSheWell> Wells)
      {
        int Column;
        int Row;
        foreach (MikeSheWell W in Wells)
        {
          //Gets the index and sets the column and row
          if (Grid.TryGetIndex(W.X, W.Y, out Column, out Row))
          {
            W.Column = Column;
            W.Row = Row;
            yield return W;
          }
        }
      }





      #region Population Methods

      /// <summary>
      /// Reads in the wells defined in detailed timeseries input section
      /// </summary>
      /// <param name="Mshe"></param>
      public static IEnumerable<IWell> ReadInDetailedTimeSeries(Model Mshe)
      {
        MikeSheWell CurrentWell;
        IIntake CurrentIntake;
        TSObject _tso = null;

        foreach (var dt in Mshe.Input.MIKESHE_FLOWMODEL.StoringOfResults.DetailedTimeseriesOutput.Item_1s)
        {
          CurrentWell = new MikeSheWell(dt.Name);
          CurrentWell.X = dt.X;
          CurrentWell.Y = dt.Y;
          CurrentWell.UsedForExtraction = false;
          CurrentIntake = CurrentWell.AddNewIntake(1);
          Screen sc = new Screen(CurrentIntake);
          sc.DepthToTop = dt.Z;
          sc.DepthToBottom = dt.Z;

          CurrentWell.Row = Mshe.GridInfo.GetRowIndex(CurrentWell.X);
          CurrentWell.Column = Mshe.GridInfo.GetColumnIndex(CurrentWell.Y);

          CurrentWell.Terrain = Mshe.GridInfo.SurfaceTopography.Data[CurrentWell.Row, CurrentWell.Column];

          //Read in observations if they are included
          if (dt.InclObserved == 1)
          {
            if (_tso == null || _tso.Connection.FilePath != dt.TIME_SERIES_FILE.FILE_NAME)
            {
              _tso = new TSObjectClass();
              _tso.Connection.FilePath = dt.TIME_SERIES_FILE.FILE_NAME;
              _tso.Connection.Open();
            }

            //Loop the observations and add
            for (int i = 1; i <= _tso.Time.NrTimeSteps; i++)
            {
              CurrentIntake.HeadObservations.Items.Add(new TimestampValue((DateTime)_tso.Time.GetTimeForTimeStepNr(i), (float)_tso.Item(dt.TIME_SERIES_FILE.ITEM_NUMBERS).GetDataForTimeStepNr(i)));
            }
          }
          yield return CurrentWell;
        }
      }



      #endregion

      #region Private methods
      /// <summary>
      /// Returns a point in the screen in meters below surface. To be used for detailed time series output and layerstatistics.
      /// </summary>
      /// <param name="Intake"></param>
      /// <returns></returns>
      private static double PointInScreen(IIntake Intake)
      {
        double top = Intake.Screens.Min(var => var.DepthToTop.Value);
        double bottom = Intake.Screens.Max(var => var.DepthToBottom.Value);

        if (top == -999)
          return bottom - 1;
        else if (bottom == -999)
          return top + 1;
        else
          return (top + bottom) / 2;
      }

      #endregion


      #region Output methods
      /// <summary>
      /// Writes a textfile that can be used for importing detailed timeseries output
      /// Depth is calculated as the midpoint of the lowest screen
      /// </summary>
      /// <param name="TxtFileName"></param>
      public static void WriteToMikeSheModel(string OutputPath, IEnumerable<IIntake> SelectedIntakes, DateTime Start, DateTime End)
      {

        StreamWriter Sw2 = new StreamWriter(Path.Combine(OutputPath, "WellsWithMissingInfo.txt"), false, Encoding.Default);

        using (StreamWriter SW = new StreamWriter(Path.Combine(OutputPath, "DetailedTimeSeriesImport.txt"), false, Encoding.Default))
        {
          foreach (IIntake I in SelectedIntakes)
          {
            //If there is no screen information we cannot use it. 
            if (I.Screens.Count == 0)
              Sw2.WriteLine("Well: " + I.well.ID + "\tIntake: " + I.IDNumber + "\tError: Missing info about screen depth");
            else
            {
              int NoOfObs = I.HeadObservations.ItemsInPeriod(Start, End).Count();
              //          if (W.Dfs0Written)
              SW.WriteLine(I.ToString() + "\t101\t1\t" + I.well.X + "\t" + I.well.Y + "\t" + PointInScreen(I) + "\t1\t" + I.ToString() + "\t1 \t" + NoOfObs);
              //When is this necessary
              //        else  
              //        SW.WriteLine(W.ID + "\t101\t1\t" + W.X + "\t" + W.Y + "\t" + W.Depth + "\t0\t \t ");
            }
          }
        }
        Sw2.Dispose();
      }


      /// <summary>
      /// Write two text-files that can be used by LayerStatistics. First one uses all observations in selected intakes and the second one use the mean
      /// </summary>
      /// <param name="FileName"></param>
      /// <param name="SelectedWells"></param>
      /// <param name="Start"></param>
      /// <param name="End"></param>
      /// <param name="AllObs"></param>
      public static void WriteToLSInput(string OutputDirectory, IEnumerable<IIntake> SelectedIntakes, DateTime Start, DateTime End)
      {
        StreamWriter SWAll = new StreamWriter(Path.Combine(OutputDirectory, "LsInput_All.txt"), false, Encoding.Default);
        StreamWriter SWMean = new StreamWriter(Path.Combine(OutputDirectory, "LsInput_Mean.txt"), false, Encoding.Default);

        SWAll.WriteLine("NOVANAID\tXUTM\tYUTM\tDEPTH\tPEJL\tDATO\tBERELAG");
        SWMean.WriteLine("NOVANAID\tXUTM\tYUTM\tDEPTH\tMEANPEJ\tMAXDATO\tBERELAG");

        foreach (IIntake I in SelectedIntakes.Where(var => var.Screens.Count > 0))
        {
          var SelectedObs = I.HeadObservations.ItemsInPeriod(Start, End);

          StringBuilder S = new StringBuilder();

          S.Append(I.ToString() + "\t" + I.well.X + "\t" + I.well.Y + "\t" + PointInScreen(I) + "\t");

          foreach (var TSE in SelectedObs)
          {
            StringBuilder ObsString = new StringBuilder(S.ToString());
            ObsString.Append(TSE.Value + "\t" + TSE.Time.ToShortDateString());
            if (I.Layer != null)
              ObsString.Append("\t" + I.Layer.ToString());
            SWAll.WriteLine(ObsString.ToString());
          }

          if (SelectedObs.Count() > 0)
          {
            S.Append(SelectedObs.Average(num => num.Value).ToString() + "\t");
            S.Append(SelectedObs.Max(num => num.Time).ToShortDateString());
            if (I.Layer != null)
              S.Append("\t" + I.Layer.ToString());
            SWMean.WriteLine(S.ToString());
          }
        }
      }
      

      public static void WriteDetailedTimeSeriesDfs0(string OutputPath, IEnumerable<IIntake> Intakes, DateTime Start, DateTime End)
      {
        foreach (IIntake Intake in Intakes)
        {
          //Select the observations
          var SelectedObs = Intake.HeadObservations.ItemsInPeriod(Start, End);
          if (SelectedObs.Count() > 0)
          {
            using (DFS0 dfs = new DFS0(Path.Combine(OutputPath, Intake.ToString() + ".dfs0"), 1))
            {
              dfs.FirstItem.ValueType = DataValueType.Instantaneous;
              dfs.FirstItem.EumItem = eumItem.eumIElevation;
              dfs.FirstItem.EumUnit = eumUnit.eumUmeter;
              dfs.FirstItem.Name = Intake.ToString();

              DateTime _previousTimeStep = DateTime.MinValue;

              //Select the observations
              int i = 0;

              foreach (var Obs in SelectedObs)
              {
                //Only add the first measurement of the day
                if (Obs.Time != _previousTimeStep)
                {
                  dfs.SetTime(i, Obs.Time);
                  dfs.SetData(i, 1, Obs.Value);
                }
                i++;
              }
            }
          }
        }
      }

      /// <summary>
      /// Writes dfs0 files with head observations for the SelectedIntakes
      /// Only includes data within the period bounded by Start and End
      /// </summary>
      /// <param name="OutputPath"></param>
      public static void WriteToDfs0(string OutputPath, IEnumerable<IIntake> Intakes, DateTime Start, DateTime End)
      {
        foreach (IIntake Intake in Intakes)
        {
          //Create the TSObject
          TSObject _tso = new TSObjectClass();
          TSItem _item = new TSItemClass();
          _item.DataType = ItemDataType.Type_Float;
          _item.ValueType = ItemValueType.Instantaneous;
          _item.EumType = 171;
          _item.EumUnit = 1;
          _item.Name = Intake.ToString();
          _tso.Add(_item);

          DateTime _previousTimeStep = DateTime.MinValue;

          //Select the observations
          var SelectedObs = Intake.HeadObservations.ItemsInPeriod(Start, End);
          int i = 0;

          foreach (var Obs in SelectedObs)
          {
            //Only add the first measurement of the day
            if (Obs.Time != _previousTimeStep)
            {
              _tso.Time.AddTimeSteps(1);
              _tso.Time.SetTimeForTimeStepNr(i + 1, Obs.Time);
              _item.SetDataForTimeStepNr(i + 1, (float)Obs.Value);
            }
            i++;
          }

          //Now write the DFS0.
          if (_tso.Time.NrTimeSteps != 0)
          {
            _tso.Connection.FilePath = Path.Combine(OutputPath, Intake.ToString() + ".dfs0");
            _tso.Connection.Save();
          }
        }
      }

      /// <summary>
      /// Writes a dfs0 with extraction data for each active intake in every plant. 
      /// Also writes the textfile that can be imported by the well editor.
      /// </summary>
      /// <param name="OutputPath"></param>
      /// <param name="Plants"></param>
      /// <param name="Start"></param>
      /// <param name="End"></param>
      public static void WriteExtractionDFS0(string OutputPath, IEnumerable<PlantViewModel> Plants, DateTime Start, DateTime End)
      {

        //Create the text file to the well editor.
        StreamWriter Sw = new StreamWriter(Path.Combine(OutputPath, "WellEditorImport.txt"), false, Encoding.Default);
        StreamWriter Sw2 = new StreamWriter(Path.Combine(OutputPath, "WellsWithMissingInfo.txt"), false, Encoding.Default);

        //Create the TSObject
        TSObject _tso = new TSObjectClass();
        string dfs0FileName = Path.Combine(OutputPath, "Extraction.dfs0");
        _tso.Connection.FilePath = dfs0FileName;
        TSItem _item;

        int eumtype = 330;
        int eumunit = 3;


        TSObject _tsoStat = new TSObjectClass();
        _tsoStat.Connection.FilePath = Path.Combine(OutputPath, "ExtractionStat.dfs0");
        Dictionary<int, double> Sum = new Dictionary<int, double>();
        Dictionary<int, double> SumSurfaceWater = new Dictionary<int, double>();
        Dictionary<int, double> SumNotUsed = new Dictionary<int, double>();

        int Pcount = 0;

        int NumberOfYears = End.Year - Start.Year + 1;

        //Dummy year because of mean step accumulated
        _tso.Time.AddTimeSteps(1);
        _tso.Time.SetTimeForTimeStepNr(1, new DateTime(Start.Year, 1, 1, 0, 0, 0));

        for (int i = 0; i < NumberOfYears; i++)
        {
          _tso.Time.AddTimeSteps(1);
          _tso.Time.SetTimeForTimeStepNr(i + 2, new DateTime(Start.Year + i, 12, 31, 12, 0, 0));

          _tsoStat.Time.AddTimeSteps(1);
          _tsoStat.Time.SetTimeForTimeStepNr(i + 1, new DateTime(Start.Year + i, 12, 31, 12, 0, 0));
          Sum.Add(i, 0);
          SumSurfaceWater.Add(i, 0);
          SumNotUsed.Add(i, 0);
        }

        int itemCount = 1;

        double[] fractions = new double[NumberOfYears];

          //loop the plants
        foreach (PlantViewModel P in Plants)
        {
          double val;
          //Create statistics on surface water for all plants
          for (int i = 0; i < NumberOfYears; i++)
          {
            if (P.plant.SurfaceWaterExtrations.TryGetValue(Start.AddYears(i), out val))
              SumSurfaceWater[i] += val;
          }

          //Create statistics for plants without intakes
          if (P.PumpingIntakes.Count == 0)
          {
            //Create statistics on water not assigned
            for (int i = 0; i < NumberOfYears; i++)
            {
              if (P.plant.Extractions.TryGetValue(Start.AddYears(i), out val))
                SumNotUsed[i] += val;
            }
          }
          else
          {
            //Create statistics
            for (int i = 0; i < NumberOfYears; i++)
            {
              if (P.plant.Extractions.TryGetValue(Start.AddYears(i), out val))
                Sum[i] += val;
            }
            Pcount++;


            //Calculate the fractions based on how many intakes are active for a particular year.
            for (int i = 0; i < NumberOfYears; i++)
            {
              fractions[i] = 1.0 / P.PumpingIntakes.Count(var => var.Intake.well.UsedForExtraction & var.Start.Year <= Start.Year + i & var.End.Year >= Start.Year + i);
            }


            //Now loop the intakes
            foreach (PumpingIntake PI in P.PumpingIntakes)
            {
              IIntake I = PI.Intake;
              //Is it an extraction well?
              if (I.well.UsedForExtraction)
              {
                //If there is no screen information we cannot use it. 
                if (I.Screens.Count == 0)
                  Sw2.WriteLine("Well: " + I.well.ID + "\tIntake: " + I.IDNumber + "\tError: Missing info about screen depth");
                else
                {
                  //Build novanaid
                  string NovanaID = P.IDNumber.ToString() + "_" + I.well.ID.Replace(" ", "") + "_" + I.IDNumber;
                  //Build and add new item
                  _item = new TSItemClass();
                  _item.DataType = ItemDataType.Type_Float;
                  _item.ValueType = ItemValueType.Mean_Step_Accumulated;
                  _item.EumType = eumtype;
                  _item.EumUnit = eumunit;
                  _item.Name = NovanaID;
                  _tso.Add(_item);

                  //Loop the years
                  for (int i = 0; i < NumberOfYears; i++)
                  {
                    //Extractions are not necessarily sorted and the time series may have missing data
                    var k = P.plant.Extractions.Items.FirstOrDefault(var => var.StartTime.Year == Start.Year + i);

                    //First year should be printed twice
                    if (i == 0)
                    {
                      if (k != null & PI.Start.Year <= Start.Year + i & PI.End.Year >= Start.Year + i)
                        _item.SetDataForTimeStepNr(1, (float)(k.Value * fractions[i]));
                      else
                        _item.SetDataForTimeStepNr(1, 0F); //Prints 0 if no data available
                    }
                    //If data and the intake is active
                    if (k != null & PI.Start.Year <= Start.Year + i & PI.End.Year >= Start.Year + i)
                      _item.SetDataForTimeStepNr(i + 2, (float)(k.Value * fractions[i]));
                    else
                      _item.SetDataForTimeStepNr(i + 2, 0F); //Prints 0 if no data available
                  }

                  //Now add line to text file.
                  StringBuilder Line = new StringBuilder();
                  Line.Append(NovanaID + "\t");
                  Line.Append(I.well.X + "\t");
                  Line.Append(I.well.Y + "\t");
                  Line.Append(I.well.Terrain + "\t");
                  Line.Append("0\t");
                  Line.Append(P.IDNumber + "\t");
                  Line.Append(I.Screens.Max(var => var.TopAsKote) + "\t");
                  Line.Append(I.Screens.Min(var => var.BottomAsKote) + "\t");
                  Line.Append(1 + "\t");
                  Line.Append(dfs0FileName + "\t");
                  Line.Append(itemCount);
                  Sw.WriteLine(Line.ToString());

                  itemCount++;
                }
              }
            }
          }
        }
        TSItem SumItem = new TSItemClass();
        SumItem.DataType = ItemDataType.Type_Float;
        SumItem.ValueType = ItemValueType.Mean_Step_Accumulated;
        SumItem.EumType = eumtype;
        SumItem.EumUnit = eumunit;
        SumItem.Name = "Sum";
        _tsoStat.Add(SumItem);

        TSItem MeanItem = new TSItemClass();
        MeanItem.DataType = ItemDataType.Type_Float;
        MeanItem.ValueType = ItemValueType.Mean_Step_Accumulated;
        MeanItem.EumType = eumtype;
        MeanItem.EumUnit = eumunit;
        MeanItem.Name = "Mean";
        _tsoStat.Add(MeanItem);

        TSItem SumNotUsedItem = new TSItemClass();
        SumNotUsedItem.DataType = ItemDataType.Type_Float;
        SumNotUsedItem.ValueType = ItemValueType.Mean_Step_Accumulated;
        SumNotUsedItem.EumType = eumtype;
        SumNotUsedItem.EumUnit = eumunit;
        SumNotUsedItem.Name = "SumNotUsed";
        _tsoStat.Add(SumNotUsedItem);

        TSItem SumSurfaceWaterItem = new TSItemClass();
        SumSurfaceWaterItem.DataType = ItemDataType.Type_Float;
        SumSurfaceWaterItem.ValueType = ItemValueType.Mean_Step_Accumulated;
        SumSurfaceWaterItem.EumType = eumtype;
        SumSurfaceWaterItem.EumUnit = eumunit;
        SumSurfaceWaterItem.Name = "SumSurfaceWater";
        _tsoStat.Add(SumSurfaceWaterItem);

        for (int i = 0; i < NumberOfYears; i++)
        {
          SumItem.SetDataForTimeStepNr(i + 1, (float)Sum[i]);
          MeanItem.SetDataForTimeStepNr(i + 1, (float)Sum[i] / Pcount);
          SumNotUsedItem.SetDataForTimeStepNr(i + 1, (float)SumNotUsed[i]);
          SumSurfaceWaterItem.SetDataForTimeStepNr(i + 1, (float)SumSurfaceWater[i]);
        }

        _tsoStat.Connection.Save();
        _tso.Connection.Save();
        Sw.Dispose();
        Sw2.Dispose();
      }



      /// <summary>
      /// Write a specialized output with all observation in one long .dat file
      /// </summary>
      /// <param name="FileName"></param>
      /// <param name="SelectedWells"></param>
      /// <param name="Start"></param>
      /// <param name="End"></param>
      public static void WriteToDatFile(string FileName, IEnumerable<IIntake> SelectedIntakes, DateTime Start, DateTime End)
      {
        StringBuilder S = new StringBuilder();

        using (StreamWriter SW = new StreamWriter(FileName, false, Encoding.Default))
        {
          foreach (Intake I in SelectedIntakes)
          {
            var SelectedObs = I.HeadObservations.ItemsInPeriod(Start, End);
            foreach (var TSE in SelectedObs)
            {
              S.Append(I.ToString() + "    " + TSE.Time.ToString("dd/MM/yyyy hh:mm:ss") + " " + TSE.Value.ToString() + "\n");
            }
          }

          SW.Write(S.ToString());
        }
      }

      /// <summary>
      /// Writes a point shape with entries for each intake in the list. Uses the dataRow as attributes.
      /// </summary>
      /// <param name="FileName"></param>
      /// <param name="Intakes"></param>
      /// <param name="Start"></param>
      /// <param name="End"></param>
      public static void WriteShapeFromDataRow(string FileName, IEnumerable<JupiterIntake> Intakes)
      {
        ShapeWriter PSW = new ShapeWriter(FileName);
        foreach (JupiterIntake JI in Intakes)
        {
          PSW.WritePointShape(JI.well.X, JI.well.Y);
          PSW.Data.WriteData(JI.Data);
        }
        PSW.Dispose();

      }

      ///// <summary>
      ///// Writes the wells to a point shape
      ///// Calculates statistics on the observations within the period from start to end
      ///// </summary>
      ///// <param name="FileName"></param>
      ///// <param name="Wells"></param>
      ///// <param name="Start"></param>
      ///// <param name="End"></param>
      //public static void WriteSimpleShape(string FileName, IEnumerable<IIntake> Intakes, DateTime Start, DateTime End)
      //{
      //  ShapeWriter PSW = new ShapeWriter(FileName);
      //  OutputTables.PejlingerOutputDataTable PDT = new OutputTables.PejlingerOutputDataTable();

      //  foreach (Intake I in Intakes)
      //  {
      //    var SelectedObs = I.HeadObservations.ItemsInPeriod(Start, End);

      //    PSW.WritePointShape(I.well.X, I.well.Y);

      //    OutputTables.PejlingerOutputRow PR = PDT.NewPejlingerOutputRow();

      //    PR.NOVANAID = I.ToString();
      //    PR.LOCATION = I.well.Description;
      //    PR.XUTM = I.well.X;
      //    PR.YUTM = I.well.Y;
      //    PR.JUPKOTE = I.well.Terrain;

      //    if (I.Screens.Count > 0)
      //    {
      //      PR.INTAKETOP = I.Screens.Min(var => var.DepthToTop);
      //      PR.INTAKEBOT = I.Screens.Max(var => var.DepthToBottom);
      //    }

      //    PR.NUMBEROFOB = SelectedObs.Count();
      //    if (SelectedObs.Count() > 0)
      //    {
      //      PR.STARTDATO = SelectedObs.Min(x => x.Time);
      //      PR.ENDDATO = SelectedObs.Max(x => x.Time);
      //      PR.MAXOBS = SelectedObs.Max(num => num.Value);
      //      PR.MINOBS = SelectedObs.Min(num => num.Value);
      //      PR.MEANOBS = SelectedObs.Average(num => num.Value);
      //    }
      //    PDT.Rows.Add(PR);
      //  }


      //  PSW.Data.WriteDate(PDT);
      //  PSW.Dispose();
      //}
      #endregion
    }
  }
