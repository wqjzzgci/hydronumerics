using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using HydroNumerics.MikeSheTools.WellViewer;
using HydroNumerics.Time.Core;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class JupiterViewModel:BaseViewModel
  {
    public IPlantCollection Plants { get; private set; }
    public IWellCollection Wells { get; private set; }

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
    
    private Func<IWell, string> _wellSorter = new Func<IWell, string>(var => var.ID);

    private Func<Plant, string> _plantSorter = new Func<Plant, string>(var => var.Name);
    private Func<Plant, bool> _currentPlantFilter = new Func<Plant, bool>(var => true);

    
    public JupiterViewModel()
    {
      
      OnlyRo = true;
    }

 

    public IEnumerable<IWell> SortedAndFilteredWells
    {
      get
      {
        List<IWell> sels = new List<IWell>();
        if (Wells != null)
        {
          return Wells.Where(var => var.Intakes.Any(var2 => var2.HeadObservations.Items.Where(_onlyRoFilter).Where(_periodFilter).Count() >= NumberOfObs)).OrderBy(_wellSorter);
        }
        return null;
      }
    }

    public IEnumerable<Plant> SortedAndFilteredPlants
    {
      get
      {
        if (Plants!=null)
          return Plants.Where(_currentPlantFilter).OrderBy(_plantSorter);
        else
          return null;
      }
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

    /// <summary>
    /// Opens a Jupiter database and reads requested data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void ReadJupiter()
    {
      openFileDialog2.Filter = "Known file types (*.mdb)|*.mdb";
      this.openFileDialog2.ShowReadOnly = true;
      this.openFileDialog2.Title = "Select an Access file with data in JupiterXL format";

      if (openFileDialog2.ShowDialog().Value)
      {
        Reader R = new Reader(openFileDialog2.FileName);
        if (Wells == null) // if wells have been read from shape or other source
        {
          AddLineToLog("Reading wells...");
          Wells = R.ReadWellsInSteps();
          AddLineToLog(Wells.Count + " wells read.");
        }
        if (Plants == null) //If plants have been read from shape
        {
          AddLineToLog("Reading plants...");
          Plants = R.ReadPlants(Wells);
          AddLineToLog(Plants.Count + " plants read.");
        }

        AddLineToLog("Reading extraction data...");
        int  c = R.FillInExtractionWithCount(Plants);
        AddLineToLog(c + " extraction entries read.");

        AddLineToLog("Reading Lithology...");
        R.ReadLithology(Wells);

        R.Dispose();

        AddLineToLog("Reading observation data...");
        JupiterXLFastReader jxf = new JupiterXLFastReader(openFileDialog2.FileName);
        c = jxf.ReadWaterLevels(Wells);
        AddLineToLog(c + " observation entries read.");
        SortObservations();
        NotifyPropertyChanged("SortedAndFilteredWells");
        NotifyPropertyChanged("SortedAndFilteredPlants");
      }
    }

    private void SortObservations()
    {
      foreach (IWell w in Wells)
        foreach (IIntake I in w.Intakes)
          I.HeadObservations.Sort();
    }

  }
}
