using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using HydroNumerics.MikeSheTools.WellViewer;
using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class JupiterViewModel:BaseViewModel
  {
    public IPlantCollection Plants { get; private set; }
    public IWellCollection Wells { get; private set; }

    private Func<IWell,bool> _currentWellFilter = new Func<IWell,bool>(var=>true);
    private Func<IWell, string> _wellSorter = new Func<IWell, string>(var => var.ID);

    public IEnumerable<IWell> SortedAndFilteredWells
    {
      get
      {
        return Wells.Where(_currentWellFilter).OrderBy(_wellSorter);
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
        if (Wells==null) // if wells have been read from shape or other source
          Wells = R.ReadWellsInSteps();
        if (Plants==null) //If plants have been read from shape
          Plants = R.ReadPlants(Wells);

        R.FillInExtraction(Plants);
        R.Dispose();

        JupiterXLFastReader jxf = new JupiterXLFastReader(openFileDialog2.FileName);
        jxf.ReadWaterLevels(Wells);
        SortObservations();
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
