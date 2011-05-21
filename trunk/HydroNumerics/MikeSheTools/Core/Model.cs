using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.PFS.SheFile;
using HydroNumerics.MikeSheTools.PFS.WellFile;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.Core
{
  /// <summary>
  /// This class provides access to setup data, processed data and results.
  /// Access to processed data and results requires that the model is preprocessed and run, respectively. 
  /// </summary>
  public class Model:IDisposable
  {
    private ProcessedData _processed;
    private Results _results;
    private FileNames _files;
    private InputFile _input;
    private TimeInfo _time;
    private string _shefilename;


    public Model(string SheFileName)
    {
      _shefilename = SheFileName;
    }


    /// <summary>
    /// Gets the file names
    /// </summary>
    public FileNames Files
    {
      get { 
        if (_files == null)
          _files = new FileNames(Input);

        return _files; }
    }

    /// <summary>
    /// Gets the grid info object
    /// Returns null if the model has not been preprocessed.
    /// </summary>
    public MikeSheGridInfo GridInfo
    {
      get 
      {
        if (Processed !=null)
          return Processed.Grid;
        return null;
      }
    }

    /// <summary>
    /// Gets read and write access to the input in the .she-file.
    /// Remember to save changes.
    /// </summary>
    public InputFile Input
    {
      get {
        if (_input == null)
          _input = new InputFile(_shefilename);
        return _input;
      }
    }

    /// <summary>
    /// Gets read access to the results
    /// Returns null if there are no results
    /// </summary>
    public Results Results
    {
      get { 
        if (_results == null)
          if (File.Exists(Files.SZ3DFileName))
            _results = new Results(Files, GridInfo);

        return _results; }
    }

    /// <summary>
    /// Gets read access to the processed data
    /// Returns null if the model has not been preprocessed.
    /// </summary>
    public ProcessedData Processed
    {
      get
      {
        if (_processed == null)
          if (File.Exists(Files.PreProcessed2D)) 
            _processed = new ProcessedData(Files);

        return _processed;
      }
    }


    private List<MikeSheWell> extractionWells;
    public List<MikeSheWell> ExtractionWells
    {
      get
      {
        if (extractionWells == null)
        {
          WelFile WF = new WelFile(Files.WelFileName);
          extractionWells = new List<MikeSheWell>();
          foreach (var w in WF.WELLDATA.Wells)
          {
            MikeSheWell NewW = new MikeSheWell(w.ID, w.XCOR, w.YCOR, GridInfo);
            NewW.Terrain = w.LEVEL;
            NewW.AddNewIntake(1);
            double[] screenDepths = new double[GridInfo.NumberOfLayers];

            foreach (var filter in w.FILTERDATA.FILTERITEMS)
            {
              Screen sc = new Screen(NewW.Intakes.First());
              sc.BottomAsKote = filter.Bottom;
              sc.TopAsKote = filter.Top;

              for (int i = Math.Max(0,GridInfo.GetLayerFromDepth(NewW.Column, NewW.Row, sc.DepthToBottom.Value)); i < GridInfo.GetLayerFromDepth(NewW.Column, NewW.Row, sc.DepthToTop.Value); i++)
              {
                double d1 = Math.Max(sc.BottomAsKote.Value, GridInfo.LowerLevelOfComputationalLayers.Data[NewW.Row, NewW.Column,i]);
                double d2 = Math.Min(sc.TopAsKote.Value, GridInfo.UpperLevelOfComputationalLayers.Data[NewW.Row, NewW.Column, i]);
                screenDepths[i] += d2 - d1;
              }
            }

            int maxi=0;
            for (int i=0; i< screenDepths.Count();i++)
              if (screenDepths[i]>screenDepths[maxi])
                maxi = i;

            NewW.Layer = maxi;
            extractionWells.Add(NewW);
          }
        }
        return extractionWells;
      }
    }

    /// <summary>
    /// Gets a class holding info about time.
    /// </summary>
    public TimeInfo Time
    {
      get
      {
        if (_time == null)
          _time = new TimeInfo(Input);
        return _time;
      }
    }

    /// <summary>
    /// Runs the Mike She model
    /// </summary>
    public void Run()
    {
      if (_input!= null)
        Input.Save();
      Dispose();
      MSheLauncher.PreprocessAndRun(_shefilename, false);
    }
      
      

    #region IDisposable Members

    public void Dispose()
    {
      if (_processed != null)
      {
        _processed.Dispose();
        _processed = null;
      }
      if (_results != null)
      {
        _results.Dispose();
        _results = null;
      }
      _input = null;
      extractionWells = null;
    }

    #endregion
  }
}
