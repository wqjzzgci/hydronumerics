using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;

using System.Windows.Input;
using FolderPickerLib;

using HydroNumerics.Time.Core;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.Geometry.Shapes;


namespace HydroNumerics.MikeSheTools.ViewModel
{  
  public class JupiterViewModel:BaseViewModel
  {
    
    public JupiterViewModel()
    {
      CanReadJupiter = true;
      CanReadMikeShe = true;
      OnlyRo = true;
      CVM = new ChangesViewModel();
    }


    /// <summary>
    /// Gets the changesviewmodel
    /// </summary>
    public ChangesViewModel CVM {get; private set;}

    public MikeSheViewModel Mshe { get; private set; }

    private string DataBaseFileName;

    #region Wells

    #region Filters and sorters

    private Func<TimestampValue, bool> _onlyRoFilter
    {
      get
      {
        if (_onlyRo)
          return new Func<TimestampValue, bool>(var => var.Description == "Ro");
        else
          return new Func<TimestampValue, bool>(var => true);
      }
    }

    private Func<TimestampValue, bool> _periodFilter
    {
      get
      {
        return new Func<TimestampValue, bool>(var3 => var3.Time >= SelectionStartTime & var3.Time <= SelectionEndTime);
      }
    }

    private Func<WellViewModel, string> _wellSorter = new Func<WellViewModel, string>(var => var.DisplayName);


    #endregion

    #region Collections
    private IWellCollection wells;
    private ObservableCollection<WellViewModel> allWells;
    /// <summary>
    /// Gets all the wells
    /// </summary>
    public ObservableCollection<WellViewModel> AllWells
    {
      get
      {
        if (allWells == null & wells!=null)
        {
          allWells = new ObservableCollection<WellViewModel>(wells.Select(var => new WellViewModel(var, CVM)));
        }
        return allWells;
      }
    }


    /// <summary>
    /// Gets the wells filtered by the filtes and sorted by the sorter.
    /// </summary>
    public IEnumerable<WellViewModel> SortedAndFilteredWells{get;private set;}

    private void BuildWellList()
    {
      if (AllWells != null)
      {
        if (MinNumberOfObservations)
          SortedAndFilteredWells = AllWells.Where(var => var.Intakes.Any(var2 => var2.HeadObservations.Items.Where(_onlyRoFilter).Where(_periodFilter).Count() >= NumberOfObs)).OrderBy(_wellSorter);
        else
          SortedAndFilteredWells = AllWells.Where(var => var.Intakes.Any(var2 => var2.HeadObservations.Items.Where(_onlyRoFilter).Where(_periodFilter).Count() <= NumberOfObs)).OrderBy(_wellSorter);

        NumberOfFixableWells = SortedAndFilteredWells.Count(var => var.HasFixableErrors);
        NumberOfFixedWells = SortedAndFilteredWells.Count(var => var.WasFixed);

        NotifyPropertyChanged("SortedAndFilteredWells");
        NotifyPropertyChanged("NumberOfFixableWells");
        NotifyPropertyChanged("NumberOfFixedWells");
      }
    }

    public int NumberOfFixableWells { get; private set; }

    public int NumberOfFixedWells { get; private set; }


    #endregion

    #region Counters
//    public int NumberOfWellsThatCanBeFixed

#endregion


    #endregion

    #region Plants

    private Func<PlantViewModel, string> _plantSorter = new Func<PlantViewModel, string>(var => var.DisplayName);
    private Func<Plant, bool> _currentPlantFilter = new Func<Plant, bool>(var => true);


    #region Collections

    private IPlantCollection Plants;

    private ObservableCollection<PlantViewModel> allPlants;
    /// <summary>
    /// Gets all the wells
    /// </summary>
    public ObservableCollection<PlantViewModel> AllPlants
    {
      get
      {
        if (allPlants == null & Plants != null)
        {
          allPlants = new ObservableCollection<PlantViewModel>(Plants.Select(var => new PlantViewModel(var, this)));
        }
        return allPlants;
      }
    }

    /// <summary>
    /// Returns the plants sorted and filtered based on the selected dates and minimum extraction
    /// </summary>
    public IEnumerable<PlantViewModel> SortedAndFilteredPlants
    {
      get
      {
        if (AllPlants == null)
          return null;
        else
        {
          double extra;
          
          List<PlantViewModel> ToReturn = new List<PlantViewModel>();
          foreach (PlantViewModel p in AllPlants)
          {
            var ext = p.plant.Extractions.Items.Where(var2 => var2.StartTime >= SelectionStartTime & var2.EndTime <= SelectionEndTime);
            if (ext.Count() == 0)
              extra = 0;
            else
              extra = ext.Average(var => var.Value);
            if (extra >= MinYearlyExtraction)
              ToReturn.Add(p);
          }
          return ToReturn.OrderBy(_plantSorter);
        }
      }
    }

    #endregion
    #endregion

    #region Selection properties

    private bool _onlyRo=false;
    public bool OnlyRo
    {
      get
      {
        return _onlyRo;
      }
      set
      {
        if (_onlyRo != value)
        {
          _onlyRo = value;
          NotifyPropertyChanged("OnlyRo");
          BuildWellList();
        }
      }
    }


    private DateTime _selectionStartTime =  new DateTime(2000,1,1);
    public DateTime SelectionStartTime
    {
      get
      {
        return _selectionStartTime;
      }
      set
      {
        if (_selectionStartTime != value)
        {
          _selectionStartTime = value;
          NotifyPropertyChanged("SelectionStartTime");
          BuildWellList();
          NotifyPropertyChanged("SortedAndFilteredPlants");
        }
      }
    }

    private DateTime _selectionEndTime= DateTime.Now;
    public DateTime SelectionEndTime
    {
      get
      {
        return _selectionEndTime;
      }
      set
      {
        if (_selectionEndTime != value)
        {
          _selectionEndTime = value;
          NotifyPropertyChanged("SelectionEndTime");
          BuildWellList();
          NotifyPropertyChanged("SortedAndFilteredPlants");
        }
      }
    }

    private int _numberOfObs=0;

    public int NumberOfObs
    {
      get { return _numberOfObs; }
      set
      {
        if (_numberOfObs != value)
        {
          _numberOfObs = value;
          NotifyPropertyChanged("NumberOfObs");
          BuildWellList();
        }
      }
    }

    private double _minYearLyExtraction = 0;
    public double MinYearlyExtraction
    {
      get { return _minYearLyExtraction; }
      set
      {
        if (_minYearLyExtraction != value)
        {
          _minYearLyExtraction = value;
          NotifyPropertyChanged("MinYearlyExtraction");
          NotifyPropertyChanged("SortedAndFilteredPlants");
        }
      }
    }

    private bool minNumberOfObservations = true;
    public bool MinNumberOfObservations
    {
      get
      {
        return minNumberOfObservations;
      }
      set
      {
        if (minNumberOfObservations != value)
        {
          minNumberOfObservations = value;
          NotifyPropertyChanged("MinNumberOfObservations");
          BuildWellList();
        }
      }
    }



    #endregion

    private StringBuilder log = new StringBuilder();

    private void AddLineToLog(string ToAdd)
    {
      log.AppendLine(ToAdd);
      NotifyPropertyChanged("Log");
    }

    public string Log
    {
      get
      {
        return log.ToString();
      }
    }


    #region Import methods
    /// <summary>
    /// Opens a Jupiter database and reads requested data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void ReadJupiter(string FileName)
    {
      DataBaseFileName = FileName;

        Reader R = new Reader(FileName);
        if (wells == null) // if wells have been read from shape or other source
        {
          AddLineToLog("Reading wells...");
          wells = R.ReadWellsInSteps();
          AddLineToLog(wells.Count + " wells read.");
        }
        if (Plants == null) //If plants have been read from shape
        {
          AddLineToLog("Reading plants...");
          Plants = R.ReadPlants(wells);
          AddLineToLog(Plants.Count + " plants read.");
        }

        AddLineToLog("Reading extraction data...");
        int  c = R.FillInExtractionWithCount(Plants);
        AddLineToLog(c + " extraction entries read.");

        AddLineToLog("Reading Lithology...");
        R.ReadLithology(wells);

        R.Dispose();

        AddLineToLog("Reading observation data...");
        JupiterXLFastReader jxf = new JupiterXLFastReader(FileName);
        c = jxf.ReadWaterLevels(wells);
        AddLineToLog(c + " observation entries read.");
        SortObservations();
        BuildWellList();
      NotifyPropertyChanged("SortedAndFilteredPlants");

        CanReadJupiter = false;
        
        //Set properties on the change view model
        CVM.SetDataBaseConnection(jxf);
        CVM.Wells = wells;
        CVM.Plants = Plants; 
    }


    #endregion


    #region Commands

    #region LoadJupiter
    RelayCommand loadDatabase;

    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand LoadDatabaseCommand
    {
      get
      {
        if (loadDatabase == null)
        {
          loadDatabase = new RelayCommand(param => this.LoadDataBase(), param => this.CanReadJupiter);
        }
        return loadDatabase;
      }
    }

    private bool CanReadJupiter { get; set; }

    private void LoadDataBase()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.mdb)|*.mdb";
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Title = "Select an Access 2000 file with data in JupiterXL format";

      if (openFileDialog2.ShowDialog().Value)
      {
        ReadJupiter(openFileDialog2.FileName);
      }
    }

    #endregion

    #region SaveDetailedTimeSeries

    RelayCommand saveDetailedTimeSeriesCommand;

    /// <summary>
    /// Gets the command that saves the detailed time series files
    /// </summary>
    public ICommand SaveDetailedTimeSeriesCommand
    {
      get
      {
        if (saveDetailedTimeSeriesCommand == null)
        {
          saveDetailedTimeSeriesCommand = new RelayCommand(param => this.SaveDetailedTimeSeries(), param => this.CanSaveDetailedTimeSeries);
        }
        return saveDetailedTimeSeriesCommand;
      }
    }

    
    private bool CanSaveDetailedTimeSeries
    {
      get
      {
        return SortedAndFilteredWells != null && SortedAndFilteredWells.Count() > 0;
      }
    }

    private void SaveDetailedTimeSeries()
    {
      var dlg = new FolderPickerDialog();
      if (dlg.ShowDialog() == true)
      {

        var intakes = SortedAndFilteredWells.Where(var=>!var.MissingData).SelectMany(var => var.Intakes);
        MsheInputFileWriters.WriteDetailedTimeSeriesText(dlg.SelectedPath, intakes, SelectionStartTime, SelectionEndTime);
        MsheInputFileWriters.WriteDetailedTimeSeriesDfs0(dlg.SelectedPath, intakes, _periodFilter, _onlyRoFilter);
        MsheInputFileWriters.WriteToDatFile(System.IO.Path.Combine(dlg.SelectedPath, "Timeseries.dat"), intakes, _periodFilter, _onlyRoFilter);
      }
    }

    #endregion

    #region SaveNovanaObservations

    RelayCommand saveNovanaObservations;

    /// <summary>
    /// Gets the command that saves the detailed time series files
    /// </summary>
    public ICommand SaveNovanaObservationsCommand
    {
      get
      {
        if (saveNovanaObservations == null)
        {
          saveNovanaObservations = new RelayCommand(param => SaveNovanaObservations(), param => CanSaveNovanaObservations);
        }
        return saveNovanaObservations;
      }
    }


    private bool CanSaveNovanaObservations
    {
      get
      {
        return SortedAndFilteredWells != null && SortedAndFilteredWells.Count() > 0;
      }
    }

    private void SaveNovanaObservations()
    {
      Microsoft.Win32.SaveFileDialog openFileDialog2 = new Microsoft.Win32.SaveFileDialog();
      openFileDialog2.Filter = "Known file types (*.shp)|*.sh";
      openFileDialog2.Title = "Save observations to a shape file";


      if (openFileDialog2.ShowDialog().Value)
      {
        Reader R = new Reader(DataBaseFileName);

        var Jints = SortedAndFilteredWells.SelectMany(var => var.Intakes.Cast<JupiterIntake>());
        R.AddDataForNovanaPejl(Jints);
        WriteShapeFromDataRow(openFileDialog2.FileName, Jints);  
      }
    }

          /// <summary>
      /// Writes a point shape with entries for each intake in the list. Uses the dataRow as attributes.
      /// </summary>
      /// <param name="FileName"></param>
      /// <param name="Intakes"></param>
      /// <param name="Start"></param>
      /// <param name="End"></param>
      private void WriteShapeFromDataRow(string FileName, IEnumerable<JupiterIntake> Intakes)
      {
        ShapeWriter PSW = new ShapeWriter(FileName);
        foreach (JupiterIntake JI in Intakes)
        {
          PSW.WritePointShape(JI.well.X, JI.well.Y);
          PSW.Data.WriteData(JI.Data);
        }
        PSW.Dispose();
      }



    #endregion

    #region SaveNovanaExtractions

    RelayCommand saveNovanaExtractionsCommand;

    /// <summary>
    /// Gets the command that saves the detailed time series files
    /// </summary>
    public ICommand SaveNovanaExtractionsCommand
    {
      get
      {
        if (saveNovanaExtractionsCommand == null)
        {
          saveNovanaExtractionsCommand = new RelayCommand(param => SaveNovanaExtractions(), param => CanSaveNovanaExtractions);
        }
        return saveNovanaExtractionsCommand;
      }
    }


    private bool CanSaveNovanaExtractions
    {
      get
      {
        return SortedAndFilteredPlants != null && SortedAndFilteredPlants.Count() > 0;
      }
    }

    private void SaveNovanaExtractions()
    {
      Microsoft.Win32.SaveFileDialog openFileDialog2 = new Microsoft.Win32.SaveFileDialog();
      openFileDialog2.Filter = "Known file types (*.shp)|*.sh";
      openFileDialog2.Title = "Save selected extractions to a shape file";


      if (openFileDialog2.ShowDialog().Value)
      {
        Reader R = new Reader(DataBaseFileName);

        var Jints = R.AddDataForNovanaExtraction(SortedAndFilteredPlants.Select(var => var.plant), SelectionStartTime, SelectionEndTime);

        WriteShapeFromDataRow(openFileDialog2.FileName, Jints);
      }
    }

    #endregion

    #region SaveExtractions
    RelayCommand saveExtractionsCommand;

    /// <summary>
    /// Gets the command that saves the extration files
    /// </summary>
    public ICommand SaveExtractionsCommand
    {
      get
      {
        if (saveExtractionsCommand == null)
        {
          saveExtractionsCommand = new RelayCommand(param => this.SaveExtractions(), param => this.CanSaveExtractions);
        }
        return saveExtractionsCommand;
      }
    }


    private bool CanSaveExtractions
    {
      get
      {
        return SortedAndFilteredPlants != null && SortedAndFilteredPlants.Count() > 0;
      }
    }

    private void SaveExtractions()
    {
      var dlg = new FolderPickerDialog();
      if (dlg.ShowDialog() == true)
      {
        MsheInputFileWriters.WriteExtractionDFS0(dlg.SelectedPath, SortedAndFilteredPlants, SelectionStartTime, SelectionEndTime);
      }
    }

    #endregion

    #region SaveLayerStatistics

    RelayCommand saveLayerStatisticsFilesCommand;

    /// <summary>
    /// Gets the command that saves the LayerStatistics files
    /// </summary>
    public ICommand SaveLayerStatisticsFilesCommand
    {
      get
      {
        if (saveLayerStatisticsFilesCommand == null)
        {
          saveLayerStatisticsFilesCommand = new RelayCommand(param => this.SaveLayerStatisticsFiles(), param => this.CanSaveDetailedTimeSeries);
        }
        return saveLayerStatisticsFilesCommand;
      }
    }


    private void SaveLayerStatisticsFiles()
    {
      var dlg = new FolderPickerDialog();
      if (dlg.ShowDialog() == true)
      {
        var intakes = SortedAndFilteredWells.SelectMany(var => var.Intakes);
        MsheInputFileWriters.WriteToLSInput(dlg.SelectedPath, intakes, _periodFilter, _onlyRoFilter);
      }
    }

    #endregion

    #region LoadMikeShe
    RelayCommand loadMikeSheCommand;

    /// <summary>
    /// Gets the command that loads the Mike she
    /// </summary>
    public ICommand LoadMikeSheCommand
    {
      get
      {
        if (loadMikeSheCommand == null)
        {
          loadMikeSheCommand = new RelayCommand(param => this.LoadMikeShe(), param => this.CanReadMikeShe);
        }
        return loadMikeSheCommand;
      }
    }

    private bool CanReadMikeShe { get; set; }

    private void LoadMikeShe()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.she)|*.she";
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Title = "Select a Mike She input file";

      if (openFileDialog2.ShowDialog().Value)
      {
        LoadMikeSheMethod(openFileDialog2.FileName);
      }
    }

    public void LoadMikeSheMethod(string filename)
    {
      Model mShe = new Model(filename);
      CanReadMikeShe = false;
      if(Plants!=null & wells!=null)
        SelectByMikeShe(mShe);
      Mshe = new MikeSheViewModel(mShe);
      Mshe.wells = AllWells;
      Mshe.RefreshChalk();
      Mshe.RefreshBelowTerrain();
      NotifyPropertyChanged("Mshe");
    }

    #endregion

    #region DeselectWells
    RelayCommand deselectWellsWithShapeCommand;

    /// <summary>
    /// Gets the command that saves the detailed time series files
    /// </summary>
    public ICommand DeselectWellsWithShapeCommand
    {
      get
      {
        if (deselectWellsWithShapeCommand == null)
        {
          deselectWellsWithShapeCommand = new RelayCommand(param => this.DeselectWellsWithShape(), param => this.CanDeselectWellsWithShape);
        }
        return deselectWellsWithShapeCommand;
      }
    }


    private bool CanDeselectWellsWithShape
    {
      get
      {
        return SortedAndFilteredWells != null && SortedAndFilteredWells.Count() > 0;
      }
    }

    private void DeselectWellsWithShape()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.shp)|*.shp";
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Title = "Select a shape file with wells";

      if (openFileDialog2.ShowDialog().Value)
      {
        using (ShapeReader sr = new ShapeReader(openFileDialog2.FileName))
        {
          for (int i=0;i<sr.Data.NoOfEntries;i++)
          {
            string wellid = sr.Data.ReadString(i, "BOREHOLENO").Trim();
            var w = allWells.SingleOrDefault(var=>var.DisplayName == wellid);
            if (w != null)
              allWells.Remove(w);
          }
        }
        BuildWellList();
      }
    }

    #endregion

    #region DeselectPlants

    RelayCommand deselectPlantsWithShapeCommand;

    /// <summary>
    /// Gets the command that saves the detailed time series files
    /// </summary>
    public ICommand DeselectPlantsWithShapeCommand
    {
      get
      {
        if (deselectPlantsWithShapeCommand == null)
        {
          deselectPlantsWithShapeCommand = new RelayCommand(param => this.DeselectPlantsWithShape(), param => this.CanDeselectPlantsWithShape);
        }
        return deselectPlantsWithShapeCommand;
      }
    }


    private bool CanDeselectPlantsWithShape
    {
      get
      {
        return SortedAndFilteredPlants != null && SortedAndFilteredPlants.Count() > 0;
      }
    }

    private void DeselectPlantsWithShape()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.shp)|*.shp";
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Title = "Select a shape file with plants";

      if (openFileDialog2.ShowDialog().Value)
      {
        using (ShapeReader sr = new ShapeReader(openFileDialog2.FileName))
        {
          for (int i = 0; i < sr.Data.NoOfEntries; i++)
          {
            int plantid = sr.Data.ReadInt(i, "PLANTID");
            var p = allPlants.SingleOrDefault(var => var.IDNumber == plantid);
            if (p != null)
              allPlants.Remove(p);
          }
        }
        NotifyPropertyChanged("SortedAndFilteredPlants");
      }
    }

    #endregion


    #region FixErrors
    RelayCommand fixErrorsCommand;

    /// <summary>
    /// Gets the command that saves the detailed time series files
    /// </summary>
    public ICommand FixErrorsCommand
    {
      get
      {
        if (fixErrorsCommand == null)
        {
          fixErrorsCommand = new RelayCommand(param => FixErrors(), param => CanFixErrors);
        }
        return fixErrorsCommand;
      }
    }


    private bool CanFixErrors
    {
      get
      {
        return SortedAndFilteredWells != null && NumberOfFixableWells > 0;
      }
    }

    private void FixErrors()
    {
      foreach (var v in SortedAndFilteredWells)
        v.Fix();
      BuildWellList();
      NotifyPropertyChanged("SortedAndFilteredPlants");
    }

    #endregion

    #endregion

    /// <summary>
    /// Removes wells and plants outside model area. If a well is assigned to a plant with another well in the model area it is kept in the collection
    /// Do not look at depths.
    /// </summary>
    /// <param name="mShe"></param>
    private void SelectByMikeShe(Model mShe)
    {
      Dictionary<string, WellViewModel> WellsToSave = new Dictionary<string,WellViewModel>();
      ObservableCollection<WellViewModel> WellsToKeep = new ObservableCollection<WellViewModel>();
      ObservableCollection<PlantViewModel> PLantsToKeep = new ObservableCollection<PlantViewModel>();

      foreach (var p in AllPlants)
      {
        if (p.Wells.Any(var => mShe.GridInfo.IsInModelArea(var.X, var.Y)))
        {
          PLantsToKeep.Add(p);
          foreach (var w in p.Wells)
            if (!WellsToSave.ContainsKey(w.DisplayName))
              WellsToSave.Add(w.DisplayName, w);
        }
        else
          Plants.Remove(p.IDNumber);
      }

      allPlants = PLantsToKeep;

      foreach (var w in AllWells)
      {
        if (w.LinkToMikeShe(mShe) || WellsToSave.ContainsKey(w.DisplayName))
          WellsToKeep.Add(w);
        else
          wells.Remove(w.DisplayName);
      }
      allWells = WellsToKeep;
      BuildWellList();
      NotifyPropertyChanged("SortedAndFilteredPlants");
      
    }

    private void SortObservations()
    {
      BackgroundWorker bw = new BackgroundWorker();
      bw.DoWork += new DoWorkEventHandler(bw_DoWork);
      bw.RunWorkerAsync();
    }

    void bw_DoWork(object sender, DoWorkEventArgs e)
    {

      foreach (IWell w in wells)
        foreach (IIntake I in w.Intakes)
          I.HeadObservations.Sort();

      foreach (Plant P in Plants)
      {
        P.DistributeExtraction(false);
        P.SurfaceWaterExtrations.Sort();
      }
    }
  }
}
