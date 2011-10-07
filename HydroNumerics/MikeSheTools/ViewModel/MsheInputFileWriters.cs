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


using DHI.Generic.MikeZero.DFS;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.ViewModel
{

  /// <summary>
  /// A class with static methods to read in well-information from various sources and print out various input files.
  /// </summary>
  public class MsheInputFileWriters
  {
    /// <summary>
    /// Function that returns true if a time series entry is between the two dates
    /// </summary>
    public static Func<TimespanValue, DateTime, DateTime, bool> InBetween2 = (TSE, Start, End) => TSE.StartTime >= Start & TSE.StartTime < End;


    #region Private methods
    /// <summary>
    /// Returns a point in the screen in meters below surface. To be used for detailed time series output and layerstatistics.
    /// </summary>
    /// <param name="Intake"></param>
    /// <returns></returns>
    private static double PointInScreen(IIntake Intake)
    {

      bool mis = Intake.well.HasMissingData();
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
    public static void WriteDetailedTimeSeriesText(string OutputPath, IEnumerable<IIntake> SelectedIntakes, DateTime Start, DateTime End)
    {

      StreamWriter Sw2 = new StreamWriter(Path.Combine(OutputPath, "WellsWithMissingInfo.txt"), false, Encoding.Default);

      using (StreamWriter SW = new StreamWriter(Path.Combine(OutputPath, "DetailedTimeSeriesImport.txt"), false, Encoding.Default))
      {
        foreach (IIntake I in SelectedIntakes)
        {
          //If there is no screen information we cannot use it. 
          if (I.Screens.Count == 0)
            Sw2.WriteLine("Well: " + I.well.ID + "\tIntake: " + I.IDNumber + "\tError: Missing info about screen depth");
          else if (I.well.X == 0 || I.well.Y == 0)
          {
            Sw2.WriteLine("Well: " + I.well.ID + "\tError: No x or y coordinate");
          }
          else
          {
            int NoOfObs = I.HeadObservations.ItemsInPeriod(Start, End).Count();
            SW.WriteLine(I.ToString() + "\t101\t1\t" + I.well.X + "\t" + I.well.Y + "\t" + PointInScreen(I) + "\t1\t" + I.ToString() + "\t1 \t" + NoOfObs);
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
    public static void WriteToLSInput(string OutputDirectory, IEnumerable<IIntake> SelectedIntakes, params Func<TimestampValue, bool>[] filters)
    {
      StreamWriter SWAll = new StreamWriter(Path.Combine(OutputDirectory, "LsInput_All.txt"), false, Encoding.Default);
      StreamWriter SWMean = new StreamWriter(Path.Combine(OutputDirectory, "LsInput_Mean.txt"), false, Encoding.Default);

      SWAll.WriteLine("NOVANAID\tXUTM\tYUTM\tDEPTH\tPEJL\tDATO\tBERELAG");
      SWMean.WriteLine("NOVANAID\tXUTM\tYUTM\tDEPTH\tMEANPEJ\tMAXDATO\tBERELAG");

      foreach (IIntake I in SelectedIntakes)
      {
        var SelectedObs = I.HeadObservations.Items.AsEnumerable<TimestampValue>();
        //Select the observations
        foreach (var v in filters)
          SelectedObs = SelectedObs.Where(v);

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
      SWAll.Dispose();
      SWMean.Dispose();
    }

    /// <summary>
    /// Writes the dfs0-files to used for detailed time series
    /// </summary>
    /// <param name="OutputPath"></param>
    /// <param name="Intakes"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    public static void WriteDetailedTimeSeriesDfs0(string OutputPath, IEnumerable<IIntake> Intakes, params Func<TimestampValue, bool>[] filters)
    {
      foreach (IIntake Intake in Intakes)
      {
        var SelectedObs = Intake.HeadObservations.Items.AsEnumerable<TimestampValue>();
        //Select the observations
        foreach (var v in filters)
          SelectedObs = SelectedObs.Where(v);

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
        StreamWriter Sw3 = new StreamWriter(Path.Combine(OutputPath, "PlantWithoutWells.txt"), false, Encoding.Default);


        var TheIntakes = Plants.Sum(var => var.ActivePumpingIntakes.Count());

        //Create the DFS0 Object
        string dfs0FileName = Path.Combine(OutputPath, "Extraction.dfs0");
        DFS0 _tso = new DFS0(dfs0FileName, TheIntakes);

        DFS0 _tsoStat = new DFS0(Path.Combine(OutputPath, "ExtractionStat.dfs0"), 4);
        Dictionary<int, double> Sum = new Dictionary<int, double>();
        Dictionary<int, double> SumSurfaceWater = new Dictionary<int, double>();
        Dictionary<int, double> SumNotUsed = new Dictionary<int, double>();

        int Pcount = 0;

        int NumberOfYears = End.Year - Start.Year + 1;

        //Dummy year because of mean step accumulated
        _tso.SetTime(0, new DateTime(Start.Year, 1, 1, 0, 0, 0));

        for (int i = 0; i < NumberOfYears; i++)
        {
          _tso.SetTime(i + 1, new DateTime(Start.Year + i, 12, 31, 12, 0, 0));
          _tsoStat.SetTime(i, new DateTime(Start.Year + i, 12, 31, 12, 0, 0));
          Sum.Add(i, 0);
          SumSurfaceWater.Add(i, 0);
          SumNotUsed.Add(i, 0);
        }

        double[] fractions = new double[NumberOfYears];
        int itemCount = 0;

        //loop the plants
        foreach (PlantViewModel P in Plants)
        {
          double val;
          //Add to summed extraction, surface water and not assigned
          for (int i = 0; i < NumberOfYears; i++)
          {
            if (P.plant.SurfaceWaterExtrations.TryGetValue(Start.AddYears(i), out val))
              SumSurfaceWater[i] += val;
            //Create statistics for plants without active intakes
            if (P.ActivePumpingIntakes.Count() == 0)
              if (P.plant.Extractions.TryGetValue(Start.AddYears(i), out val))
                SumNotUsed[i] += val;

            if (P.plant.Extractions.TryGetValue(Start.AddYears(i), out val))
              Sum[i] += val;
          }
          Pcount++;

          //Used for extraction but has missing data
          foreach (var NotUsedWell in P.PumpingIntakes.Where(var => var.Intake.well.UsedForExtraction & var.Intake.well.HasMissingData()))
          {
            StringBuilder Line = new StringBuilder();
            Line.Append(NotUsedWell.Intake.well.X + "\t");
            Line.Append(NotUsedWell.Intake.well.Y + "\t");
            Line.Append(NotUsedWell.Intake.well.Terrain + "\t");
            Line.Append("0\t");
            Line.Append(P.IDNumber + "\t");
            Sw2.WriteLine(Line);
          }

          //Only go in here if the plant has active intakes
          if (P.ActivePumpingIntakes.Count() > 0)
          {
            //Calculate the fractions based on how many intakes are active for a particular year.
            for (int i = 0; i < NumberOfYears; i++)
            {
              fractions[i] = 1.0 / P.ActivePumpingIntakes.Count(var => (var.StartNullable ?? DateTime.MinValue).Year <= Start.Year + i & (var.EndNullable ?? DateTime.MaxValue).Year >= Start.Year + i);
            }

            //Now loop the intakes
            foreach (var PI in P.ActivePumpingIntakes)
            {
              IIntake I = PI.Intake;
              //Build novanaid
              string NovanaID = P.IDNumber.ToString() + "_" + I.well.ID.Replace(" ", "") + "_" + I.IDNumber;

              _tso.Items[itemCount].ValueType = DataValueType.MeanStepBackward;
              _tso.Items[itemCount].EumItem = eumItem.eumIPumpingRate;
              _tso.Items[itemCount].EumUnit = eumUnit.eumUm3PerSec;
              _tso.Items[itemCount].Name = NovanaID;

              //Loop the years
              for (int i = 0; i < NumberOfYears; i++)
              {
                //Extractions are not necessarily sorted and the time series may have missing data
                var k = P.plant.Extractions.Items.FirstOrDefault(var => var.StartTime.Year == Start.Year + i);

                //If data and the intake is active
                if (k != null & (PI.StartNullable ?? DateTime.MinValue).Year <= Start.Year + i & (PI.EndNullable ?? DateTime.MaxValue).Year >= Start.Year + i)
                  _tso.SetData(i + 1, itemCount + 1, (k.Value * fractions[i]));
                else
                  _tso.SetData(i + 1, itemCount + 1, 0); //Prints 0 if no data available

                //First year should be printed twice
                if (i == 0)
                  _tso.SetData(i, itemCount + 1, _tso.GetData(i + 1, itemCount + 1));
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
              Line.Append(Path.GetFileNameWithoutExtension(dfs0FileName) + "\t");
              Line.Append(itemCount+1);
              Sw.WriteLine(Line.ToString());

              itemCount++;
            }
          }
          else //Plants with no wells
          {
            Sw3.WriteLine(P.DisplayName + "\t" + P.IDNumber);
          }
        }


        foreach(var Item in _tsoStat.Items)
        {
          Item.EumItem = eumItem.eumIPumpingRate;
          Item.EumUnit = eumUnit.eumUm3PerSec;
          Item.ValueType = DataValueType.MeanStepBackward;
        }
        
        _tsoStat.Items[0].Name="Sum";
        _tsoStat.Items[1].Name = "Mean";
        _tsoStat.Items[2].Name = "SumNotUsed";
        _tsoStat.Items[3].Name = "SumSurfaceWater";

        for (int i = 0; i < NumberOfYears; i++)
        {
          _tsoStat.SetData(i, 1, Sum[i]);
          _tsoStat.SetData(i, 2, Sum[i]/((double)Pcount));
          _tsoStat.SetData(i, 3, SumNotUsed[i]);
          _tsoStat.SetData(i, 4, SumSurfaceWater[i]);
        }

        _tsoStat.Dispose(); 
        _tso.Dispose();
        Sw.Dispose();
        Sw2.Dispose();
        Sw3.Dispose();
      }

        /// <summary>
    /// Writes a dfs0 with extraction data for each active intake in every plant using the Permits. 
    /// Also writes the textfile that can be imported by the well editor.
    /// </summary>
    /// <param name="OutputPath"></param>
    /// <param name="Plants"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    public static void WriteExtractionDFS0Permits(string OutputPath, IEnumerable<PlantViewModel> Plants, int DistributionYear, int StartYear, int Endyear)
    {

      //Create the text file to the well editor.
      StreamWriter Sw = new StreamWriter(Path.Combine(OutputPath, "WellEditorImportPermits.txt"), false, Encoding.Default);
      StreamWriter Sw2 = new StreamWriter(Path.Combine(OutputPath, "WellsWithMissingInfo.txt"), false, Encoding.Default);
      StreamWriter Sw3 = new StreamWriter(Path.Combine(OutputPath, "PlantWithoutWells.txt"), false, Encoding.Default);

      var TheIntakes = Plants.Sum(var => var.ActivePumpingIntakes.Count());

      //Create the DFS0 Object
      string dfs0FileName = Path.Combine(OutputPath, "ExtractionPermits.dfs0");
      DFS0 _tso = new DFS0(dfs0FileName, TheIntakes);

      int Pcount = 0;

      //Set time
      _tso.SetTime(0, new DateTime(StartYear, 1, 1, 0, 0, 0));
      _tso.SetTime(1, new DateTime(Endyear, 12, 31, 0, 0, 0));

      double fractions;
      int itemCount = 0;

      //loop the plants
      foreach (PlantViewModel P in Plants)
      {
        Pcount++;

        //Used for extraction but has missing data
        foreach (var NotUsedWell in P.PumpingIntakes.Where(var => var.Intake.well.UsedForExtraction & var.Intake.well.HasMissingData()))
        {
          StringBuilder Line = new StringBuilder();
          Line.Append(NotUsedWell.Intake.well.X + "\t");
          Line.Append(NotUsedWell.Intake.well.Y + "\t");
          Line.Append(NotUsedWell.Intake.well.Terrain + "\t");
          Line.Append("0\t");
          Line.Append(P.IDNumber + "\t");
          Sw2.WriteLine(Line);
        }

        //Only go in here if the plant has active intakes
        if (P.ActivePumpingIntakes.Count() > 0)
        {
          //Calculate the fractions based on how many intakes are active for a particular year.
          fractions = 1.0 / P.ActivePumpingIntakes.Count(var => (var.StartNullable ?? DateTime.MinValue).Year <= DistributionYear & (var.EndNullable ?? DateTime.MaxValue).Year >= DistributionYear);

          //Now loop the intakes
          foreach (var PI in P.ActivePumpingIntakes.Where(var => (var.StartNullable ?? DateTime.MinValue).Year <= DistributionYear & (var.EndNullable ?? DateTime.MaxValue).Year >= DistributionYear))
          {
            IIntake I = PI.Intake;
            //Build novanaid
            string NovanaID = P.IDNumber.ToString() + "_" + I.well.ID.Replace(" ", "") + "_" + I.IDNumber;

            _tso.Items[itemCount].ValueType = DataValueType.MeanStepBackward;
            _tso.Items[itemCount].EumItem = eumItem.eumIPumpingRate;
            _tso.Items[itemCount].EumUnit = eumUnit.eumUm3PerSec;
            _tso.Items[itemCount].Name = NovanaID;


            //If data and the intake is active
            _tso.SetData(0, itemCount + 1, (P.Permit * fractions));
            _tso.SetData(1, itemCount + 1, (P.Permit * fractions));


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
            Line.Append(Path.GetFileNameWithoutExtension(dfs0FileName) + "\t");
            Line.Append(itemCount+1);
            Sw.WriteLine(Line.ToString());

            itemCount++;
          }
        }
        else //Plants with no wells
        {
          Sw3.WriteLine(P.DisplayName + "\t" + P.IDNumber);
        }
      }
      _tso.Dispose();
      Sw.Dispose();
      Sw2.Dispose();
      Sw3.Dispose();
    }




    /// <summary>
    /// Write a specialized output with all observation in one long .dat file
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="SelectedWells"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    public static void WriteToDatFile(string FileName, IEnumerable<IIntake> SelectedIntakes, params Func<TimestampValue, bool>[] filters)
    {
      StringBuilder S = new StringBuilder();

      using (StreamWriter SW = new StreamWriter(FileName, false, Encoding.Default))
      {
        foreach (Intake I in SelectedIntakes)
        {
          var SelectedObs = I.HeadObservations.Items.AsEnumerable<TimestampValue>();
          //Select the observations
          foreach (var v in filters)
            SelectedObs = SelectedObs.Where(v);

          foreach (var TSE in SelectedObs)
          {
            S.Append(I.ToString() + "    " + TSE.Time.ToString("dd/MM/yyyy hh:mm:ss") + " " + TSE.Value.ToString() + "\n");
          }
        }

        SW.Write(S.ToString());
      }
    }

    #endregion


    #region GMS files

    public static void WriteGMSExtraction(string OutputPath, IEnumerable<PlantViewModel> Plants, DateTime Start, DateTime End)
    {

      int NumberOfYears = End.Year - Start.Year + 1;

      double[] fractions = new double[NumberOfYears];

      StreamWriter sw2 = new StreamWriter(Path.Combine(OutputPath, "GMSWellsImport.wpf"));

      using (StreamWriter sw = new StreamWriter(Path.Combine(OutputPath, "GMSWellsImport.wdf")))
      {
        //loop the plants
        foreach (PlantViewModel P in Plants)
        {

          //Only go in here if the plant has active intakes
          if (P.ActivePumpingIntakes.Count() > 0)
          {
            //Calculate the fractions based on how many intakes are active for a particular year.
            for (int i = 0; i < NumberOfYears; i++)
            {
              fractions[i] = 1.0 / P.ActivePumpingIntakes.Count(var => (var.StartNullable ?? DateTime.MinValue).Year <= Start.Year + i & (var.EndNullable ?? DateTime.MaxValue).Year >= Start.Year + i);
            }
          }


          foreach (var I in P.ActivePumpingIntakes)
          {
            string NovanaID = P.IDNumber.ToString() + "_" + I.Intake.well.ID.Replace(" ", "") + "_" + I.Intake.IDNumber;
            //Now add line to text file.
            StringBuilder Line = new StringBuilder();
            Line.Append(NovanaID + "\t");
            Line.Append(I.Intake.well.X + "\t");
            Line.Append(I.Intake.well.Y + "\t");
            Line.Append(I.Intake.well.Terrain + "\t");
            Line.Append(I.Intake.Screens.Max(var => var.DepthToTop) + "\t");
            Line.Append(I.Intake.Screens.Max(var => var.DepthToBottom) - I.Intake.Screens.Max(var => var.DepthToTop));
            sw.WriteLine(Line.ToString());


            //Loop the years
            for (int i = 0; i < NumberOfYears; i++)
            {
              //Extractions are not necessarily sorted and the time series may have missing data
              var k = P.plant.Extractions.Items.FirstOrDefault(var => var.StartTime.Year == Start.Year + i);

              //If data and the intake is active
              if (k != null & (I.StartNullable ?? DateTime.MinValue).Year <= Start.Year + i & (I.EndNullable ?? DateTime.MaxValue).Year >= Start.Year + i)
                sw2.WriteLine(NovanaID + "\t" + Start.AddYears(i).ToShortDateString() + " " + Start.ToShortTimeString() + " " + (k.Value * fractions[i]).ToString());
              else
                sw2.WriteLine(NovanaID + "\t" + Start.AddYears(i).ToShortDateString() + " " + Start.ToShortTimeString() + "  0");
            }

          }
        }
      }
      sw2.Dispose();
    }

    #endregion

    #region Shape output
    /// <summary>
    /// Fills the data row with entries common for Intake and Extractions.
    /// </summary>
    /// <param name="CurrentIntake"></param>
    private static void AddCommonDataForNovana(JupiterIntake CurrentIntake)
    {
      JupiterWell CurrentWell = CurrentIntake.well as JupiterWell;

      ShapeOutputTables.IntakeCommonRow CurrentRow = (ShapeOutputTables.IntakeCommonRow)CurrentIntake.Data;

      CurrentWell = (JupiterWell)CurrentIntake.well;

      CurrentRow.NOVANAID = CurrentWell.ID.Replace(" ", "") + "_" + CurrentIntake.IDNumber;

      CurrentRow.XUTM = CurrentWell.X;
      CurrentRow.YUTM = CurrentWell.Y;


      CurrentRow.JUPKOTE = CurrentWell.Terrain;
      CurrentRow.BOREHOLENO = CurrentWell.ID;
      CurrentRow.INTAKENO = CurrentIntake.IDNumber;
      CurrentRow.LOCATION = CurrentWell.Description;

      CurrentRow.ANTINT_B = CurrentWell.Intakes.Count();


      if (CurrentWell.EndDate.HasValue)
        CurrentRow.DRILENDATE = CurrentWell.EndDate.Value;

      if (CurrentWell.Depth.HasValue)
        CurrentRow.DRILLDEPTH = CurrentWell.Depth.Value;


      if (CurrentIntake.Depth.HasValue)
        CurrentRow.CASIBOT = CurrentIntake.Depth.Value;

      CurrentRow.PURPOSE = CurrentWell.Purpose;
      CurrentRow.USE = CurrentWell.Use;
      CurrentRow.INTAKETOP = -999;
      CurrentRow.INTAKEBOT = -999;

      if (CurrentIntake.Screens.Count != 0)
      {
        if (CurrentIntake.Screens.Where(var1 => var1.DepthToTop.HasValue).Count() != 0)
          CurrentRow.INTAKETOP = CurrentIntake.Screens.Where(var1 => var1.DepthToTop.HasValue).Min(var => var.DepthToTop.Value);

        if (CurrentIntake.Screens.Where(var1 => var1.DepthToBottom.HasValue).Count() != 0)
          CurrentRow.INTAKEBOT = CurrentIntake.Screens.Where(var1 => var1.DepthToBottom.HasValue).Max(var => var.DepthToBottom.Value);
      }

      CurrentRow.INTAKTOPK = -999;
      CurrentRow.INTAKBOTK = -999;

      if (CurrentRow.JUPKOTE != -999)
      {
        if (CurrentRow.INTAKETOP != -999)
          CurrentRow.INTAKTOPK = CurrentRow.JUPKOTE - CurrentRow.INTAKETOP;
        if (CurrentRow.INTAKEBOT != -999)
          CurrentRow.INTAKBOTK = CurrentRow.JUPKOTE - CurrentRow.INTAKEBOT;
      }

     

      CurrentRow.RESROCK = "-999";
      CurrentRow.RESROCK = CurrentIntake.ResRock;

      CurrentRow.SUMSAND = -999;
      CurrentRow.BOTROCK = "-999";


      if (CurrentWell.LithSamples.Count != 0 & CurrentIntake.Screens.Count != 0)
      {
        CurrentWell.LithSamples.Sort();
        CurrentRow.BOTROCK = CurrentWell.LithSamples[CurrentWell.LithSamples.Count - 1].RockSymbol;
        Dictionary<string, double> SoilLengths = new Dictionary<string, double>();

        double ScreenLength = 0;

        //Now build information about reservoir rock in front of screen
        //Loop all screens
        foreach (Screen SC in CurrentIntake.Screens)
        {
          //Do not use dummy values
          if (SC.DepthToBottom.HasValue & SC.DepthToTop.HasValue)
          {
            ScreenLength += SC.DepthToBottom.Value - SC.DepthToTop.Value;

            //Get the samples that are within the filter
            var sampleswithinFilter = CurrentWell.LithSamples.Where(var => var.Top < SC.DepthToBottom & var.Bottom > SC.DepthToTop);

            //Now calculate the percentages
            foreach (Lithology L in sampleswithinFilter)
            {
              double percent = (Math.Min(SC.DepthToBottom.Value, L.Bottom) - Math.Max(SC.DepthToTop.Value, L.Top));
              if (SoilLengths.ContainsKey(L.RockSymbol))
                SoilLengths[L.RockSymbol] += percent;
              else
                SoilLengths.Add(L.RockSymbol, percent);
            }
          }
        }

        if (SoilLengths.Count != 0)
        {
          double sumsand = 0;
          string[] magasiner = new string[] { "s", "k", "g" };
          //Build the resrock string
          StringBuilder resrock = new StringBuilder();
          foreach (KeyValuePair<string, double> KVP in SoilLengths)
          {
            double percent = KVP.Value / ScreenLength * 100;
            resrock.Append(KVP.Key + ": " + percent.ToString("###") + "% ");
            if (magasiner.Contains(KVP.Key.ToLower()))
              sumsand += percent;
            if (KVP.Key.Length >= 2 && magasiner.Contains(KVP.Key.Substring(1, 1).ToLower()))
              sumsand += percent;
          }
          CurrentRow.RESROCK = resrock.ToString();
          CurrentRow.SUMSAND = sumsand;
        }
      }
    }


    public static IEnumerable<JupiterIntake> AddDataForNovanaExtraction(IEnumerable<Plant> Plants, DateTime StartDate, DateTime EndDate)
    {
      ShapeOutputTables.IntakeCommonDataTable DT2 = new ShapeOutputTables.IntakeCommonDataTable();
      ShapeOutputTables.IndvindingerDataTable DT1 = new ShapeOutputTables.IndvindingerDataTable();
      ShapeOutputTables.IndvindingerRow CurrentRow;

      List<JupiterIntake> _intakes = new List<JupiterIntake>();

      //Loop the plants
      foreach (Plant P in Plants)
      {
        //Loop the pumping intakes
        foreach (var PI in P.PumpingIntakes)
        {

          JupiterIntake CurrentIntake = PI.Intake as JupiterIntake;
          CurrentIntake.Data = DT2.NewIntakeCommonRow();
          //Read generic data
          AddCommonDataForNovana(CurrentIntake);
          DT2.Rows.Add(CurrentIntake.Data);
          CurrentRow = DT1.NewIndvindingerRow();

          //Construct novana id
          string NovanaID = P.IDNumber + "_" + CurrentIntake.well.ID.Replace(" ", "") + "_" + CurrentIntake.IDNumber;

          if (P.PumpingIntakes.Count(var => var.Intake.ToString() == CurrentIntake.ToString()) > 1)
            NovanaID += "_" + P.PumpingIntakes.IndexOf(PI);

          CurrentRow.NOVANAID = NovanaID;
          CurrentIntake.Data["NOVANAID"] = NovanaID;

          CurrentRow.PLANTID = P.IDNumber;
          CurrentRow.PLANTNAME = P.Name;

          //Get additional data about the plant from the dataset
          CurrentRow.NYKOMNR = P.NewCommuneNumber;
          CurrentRow.KOMNR = P.OldCommuneNumber;
          CurrentRow.ANTUNDERA = P.SubPlants.Count;
          CurrentRow.ANLUTMX = P.X;
          CurrentRow.ANLUTMY = P.Y;
          CurrentRow.VIRKTYP = P.CompanyType;
          CurrentRow.ACTIVE = P.Active;

          if (P.SuperiorPlantNumber.HasValue)
            CurrentRow.OVERANL = P.SuperiorPlantNumber.Value; ;

          if (P.Extractions.Items.Count > 0)
          {
            var SelectecExtrations = P.Extractions.Items.Where(var => var.StartTime >= StartDate && var.StartTime <= EndDate);
            var ActualValue = SelectecExtrations.FirstOrDefault(var => var.StartTime.Year == EndDate.Year);

            if (SelectecExtrations.Count() > 0)
            {
              CurrentRow.MEANINDV = SelectecExtrations.Average(var => var.Value);
              if (ActualValue != null)
                CurrentRow.AKTUELIND = ActualValue.Value;
              else
                CurrentRow.AKTUELIND = 0;
            }
          }

          CurrentRow.PERMIT = P.Permit;
          CurrentRow.ANTINT_A = P.PumpingIntakes.Count;
          CurrentRow.ANTBOR_A = P.PumpingWells.Count;

          if (PI.StartNullable.HasValue)
          {
            CurrentRow.INTSTDATE = PI.StartNullable.Value;
            CurrentRow.FRAAAR = GetFraAar(PI.StartNullable.Value);
          }
          else
            CurrentRow.FRAAAR = 9999;

          if (PI.EndNullable.HasValue)
          {
            CurrentRow.INTENDDATE = PI.EndNullable.Value;
            CurrentRow.TILAAR = GetTilAar(PI.EndNullable.Value);
          }
          else
            CurrentRow.TILAAR = 9999;


          DT1.Rows.Add(CurrentRow);
          _intakes.Add(CurrentIntake);
        }
      }

      //Add a blank string to ensure length of column
      DT2.Rows[0]["COMMENT"] = "                                                   ";
      DT2.Merge(DT1);

      return _intakes;
    }

    private static int GetFraAar(DateTime Date)
    {
      if (Date.DayOfYear > 182)
        return Date.Year + 1;
      else
        return Date.Year;
    }

    private static int GetTilAar(DateTime Date)
    {
      if (Date.DayOfYear < 182)
        return Date.Year - 1;
      else
        return Date.Year;
    }


    public static void AddDataForNovanaPejl(IEnumerable<JupiterIntake> Intakes, DateTime start, DateTime end)
    {
      ShapeOutputTables.PejlingerDataTable DT1 = new ShapeOutputTables.PejlingerDataTable();
      ShapeOutputTables.PejlingerRow CurrentRow;

      ShapeOutputTables.IntakeCommonDataTable DT2 = new ShapeOutputTables.IntakeCommonDataTable();

      foreach (JupiterIntake CurrentIntake in Intakes)
      {
        CurrentIntake.Data = DT2.NewIntakeCommonRow();
        AddCommonDataForNovana(CurrentIntake);
        DT2.Rows.Add(CurrentIntake.Data);
        CurrentRow = DT1.NewPejlingerRow();
        CurrentRow.NOVANAID = CurrentIntake.Data["NOVANAID"].ToString();

        DT1.Rows.Add(CurrentRow);

        var selectedobs = CurrentIntake.HeadObservations.ItemsInPeriod(start, end);

        //Create statistics on water levels
        CurrentRow.ANTPEJ = selectedobs.Count();
        if (CurrentRow.ANTPEJ > 0)
        {
          CurrentRow.REFPOINT = CurrentIntake.RefPoint;
          CurrentRow.MINDATO = selectedobs.First().Time;
          CurrentRow.MAXDATO = selectedobs.Last().Time;
          CurrentRow.AKTAAR = CurrentRow.MAXDATO.Year - CurrentRow.MINDATO.Year + 1;
          CurrentRow.AKTDAGE = CurrentRow.MAXDATO.Subtract(CurrentRow.MINDATO).Days + 1;
          CurrentRow.PEJPRAAR = CurrentRow.ANTPEJ / CurrentRow.AKTAAR;
          CurrentRow.MAXPEJ = selectedobs.Max(num => num.Value);
          CurrentRow.MINPEJ = selectedobs.Min(num => num.Value);
          CurrentRow.MEANPEJ = selectedobs.Average(num => num.Value);
        }
      }
      //Add a blank string to ensure length of column
      DT2.Rows[0]["COMMENT"] = "                                                   ";

      DT2.Merge(DT1);
    }

    #endregion

  }
}
