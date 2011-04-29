using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;

using System.Windows.Input;


using HydroNumerics.Time.Core;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{


 
  
  public class JupiterViewModel:BaseViewModel
  {

    private bool CanReadJupiter{get; set;}
    RelayCommand loadDatabase;
    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand LoadDatabase
    {
      get
      {
        if (loadDatabase == null)
        {
          loadDatabase = new RelayCommand(param => this.ReadJupiter(), param => this.CanReadJupiter);
        }
        return loadDatabase;
      }
    }


    public void Notify()
    {
      NotifyPropertyChanged(String.Empty);
    }


    public IPlantCollection Plants { get; private set; }

    private Func<PlantViewModel, string> _plantSorter = new Func<PlantViewModel, string>(var => var.DisplayName);
    private Func<Plant, bool> _currentPlantFilter = new Func<Plant, bool>(var => true);



    private ChangesViewModel changesViewModel;
    public ChangesViewModel ChangesViewModel
    {
      get
      {
        return changesViewModel;
      }
      set
      {
        if (changesViewModel != value)
        {
          changesViewModel = value;
          NotifyPropertyChanged("ChangesViewModel");
        }
      }
    }

    public JupiterViewModel()
    {
      CanReadJupiter = true;
      OnlyRo = true;
    }


    #region Wells

    #region Filters and sorters

    public Func<TimestampValue, bool> _onlyRoFilter
    {
      get
      {
        if (_onlyRo)
          return new Func<TimestampValue, bool>(var => var.Description == "Ro");
        else
          return new Func<TimestampValue, bool>(var => true);
      }
    }

    public Func<TimestampValue, bool> _periodFilter
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
          allWells = new ObservableCollection<WellViewModel>(wells.Select(var => new WellViewModel(var, this)));
        }
        return allWells;
      }
    }

    /// <summary>
    /// Gets the wells filtered by the filtes and sorted by the sorter.
    /// </summary>
    public IEnumerable<WellViewModel> SortedAndFilteredWells
    {
      get
      {
        if (AllWells == null)
          return null;
        return AllWells.Where(var => var.Intakes.Any(var2 => var2.HeadObservations.Items.Where(_onlyRoFilter).Where(_periodFilter).Count() >= NumberOfObs)).OrderBy(_wellSorter);
      }
    }

    #endregion

    #endregion

    /// <summary>
    /// Returns the plants sorted and filtered based on the selected dates and minimum extraction
    /// </summary>
    public IEnumerable<PlantViewModel> SortedAndFilteredPlants
    {
      get
      {
        if (Plants != null)
        {
          //Denne her søgning må kunne laves mere elegant
          List<PlantViewModel> ToReturn = new List<PlantViewModel>();
          double extra;
          foreach (Plant p in Plants)
          {
            var ext = p.Extractions.Items.Where(var2 => var2.StartTime >= SelectionStartTime & var2.EndTime <= SelectionEndTime);
            if (ext.Count() == 0)
              extra = 0;
            else
              extra = ext.Average(var => var.Value);
            if (extra >= MinYearlyExtraction)
              ToReturn.Add(new PlantViewModel(p));
          }
          return ToReturn.OrderBy(_plantSorter);

        }
        return null;
      }
    }

    public Plant SelectedPlant
    {

      get;
      set;
    }




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
          NotifyPropertyChanged("SortedAndFilteredWells");
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
          NotifyPropertyChanged("SortedAndFilteredWells");
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
          NotifyPropertyChanged("SortedAndFilteredWells");
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
          NotifyPropertyChanged("SortedAndFilteredWells");
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

    

        //private ShapeReaderConfiguration ShpConfig = null;
    //private List<IIntake> Intakes;
    
    Microsoft.Win32.OpenFileDialog openFileDialog2= new Microsoft.Win32.OpenFileDialog();


    #region Import methods
    /// <summary>
    /// Opens a Jupiter database and reads requested data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ReadJupiter()
    {
      openFileDialog2.Filter = "Known file types (*.mdb)|*.mdb";
      this.openFileDialog2.ShowReadOnly = true;
      this.openFileDialog2.Title = "Select an Access file with data in JupiterXL format";

      if (openFileDialog2.ShowDialog().Value)
      {
        Reader R = new Reader(openFileDialog2.FileName);
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
        JupiterXLFastReader jxf = new JupiterXLFastReader(openFileDialog2.FileName);
        c = jxf.ReadWaterLevels(wells);
        AddLineToLog(c + " observation entries read.");
        SortObservations();
        NotifyPropertyChanged("SortedAndFilteredWells");
        NotifyPropertyChanged("SortedAndFilteredPlants");

        CanReadJupiter = false;

        ChangesViewModel = new ChangesViewModel(new ChangeController(openFileDialog2.FileName));
      }
    }

    public void ImportChanges()
    {
      openFileDialog2.Filter = "Known file types (*.xml)|*.xml";
      this.openFileDialog2.ShowReadOnly = true;
      this.openFileDialog2.Title = "Select an xml file with Jupiter changes";

      if (openFileDialog2.ShowDialog().Value)
      {
        XDocument xd = XDocument.Load(openFileDialog2.FileName);
        int changesread = 0;
        int changesused = 0;

        IWell well=null;

        foreach (var c in xd.Element("Changes").Elements("Change"))
        {
          changesread++;
          string wellid = c.Element("PrimaryKeys").Element("PrimaryKey").Element("Value").Value;

          if (wells.TryGetValue(wellid, out well))
          {
              string Column1 = c.Element("ChangedValues").Element("ChangedValue").Element("Column").Value;
            if (c.Element("Table").Value == "BOREHOLE")
            {

              if (Column1 == "UTMX")
              {
                well.X = double.Parse(c.Element("ChangedValues").Element("ChangedValue").Element("NewValue").Value);
              }
              else if(Column1=="UTMY")
              {
                well.Y = double.Parse(c.Element("ChangedValues").Element("ChangedValue").Element("NewValue").Value);
              }
              else if (Column1 == "ELEVATION")
              {
                well.Terrain = double.Parse(c.Element("ChangedValues").Element("ChangedValue").Element("NewValue").Value);
              }
            }
            else if (c.Element("Table").Value == "SCREEN")
            {
              int screenno = int.Parse(c.Element("PrimaryKeys").Elements("PrimaryKey").ToArray()[1].Element("Value").Value);
              if (c.Element("Action").Value == "InsertRow")
              {
                
                //if (well.Intakes.Max(var=>var.Screens.Max(var2=>var2.Number)<screenno))
                {
                  Screen sc = new Screen(well.Intakes.First());
                  foreach (var ssc in c.Element("ChangedValues").Elements("ChangedValue"))
                  {
                    if (ssc.Element("Column").Value == "TOP")
                      sc.DepthToTop = double.Parse(ssc.Element("NewValue").Value);
                    else if (ssc.Element("Column").Value == "BOTTOM")
                      sc.DepthToBottom = double.Parse(ssc.Element("NewValue").Value);
                  }
                }
              }
              else if (c.Element("Action").Value == "EditValue")
              {
                Screen sc = well.Intakes.First().Screens.First(var => var.Number == screenno);
                if (c.Element("ChangedValues").Element("ChangedValue").Element("Column").Value == "TOP")
                  sc.DepthToTop = double.Parse(c.Element("ChangedValues").Element("ChangedValue").Element("NewValue").Value);
                else if (c.Element("ChangedValues").Element("ChangedValue").Element("Column").Value == "BOTTOM")
                  sc.DepthToBottom = double.Parse(c.Element("ChangedValues").Element("ChangedValue").Element("NewValue").Value);
              }
            }
          }
        }

        NotifyPropertyChanged("SortedAndFilteredWells");
      }
    }


    #endregion

    public void SaveChanges(string UserName, string ProjectName)
    {
      Microsoft.Win32.SaveFileDialog savedialog = new Microsoft.Win32.SaveFileDialog();

      if (savedialog.ShowDialog().Value)
      {
        XDocument _changes = new XDocument();
        XElement cc = new XElement("Changes");

        //foreach (var c in Changes)
        //{
        //  c.User = UserName;
        //  c.Project = ProjectName;
        //  c.Date = DateTime.Now;

        //  XElement cx= c.ToXML();
        //  cc.Add(cx);
        //}

        _changes.Add(cc);
        _changes.Save(savedialog.FileName);
      }
    }

    private void SortObservations()
    {
      foreach (IWell w in wells)
        foreach (IIntake I in w.Intakes)
          I.HeadObservations.Sort();

      foreach (Plant P in Plants)
      {
        P.DistributeExtraction();
        P.SurfaceWaterExtrations.Sort();
      }
    }
  }
}
