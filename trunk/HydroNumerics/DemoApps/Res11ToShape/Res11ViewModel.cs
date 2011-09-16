using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Data;

using HydroNumerics.Core.WPF;
using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace Res11ToShape
{
  public class Res11ViewModel:NotifyPropertyChangedBase
  {

    private Res11 res11file;

    public DateTime MinStartTime { get; private set; }
    public DateTime MaxEndTime { get; private set; }

    public string FileName { get; private set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }


    #region LoadRes11Command
    RelayCommand loadRes11;

    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand LoadRes11Command
    {
      get
      {
        if (loadRes11 == null)
        {
          loadRes11 = new RelayCommand(param => this.LoadRes11(), param => true);
        }
        return loadRes11;
      }
    }


    private void LoadRes11()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.res11)|*.res11";
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Title = "Select a .res11 file";

      if (openFileDialog2.ShowDialog().Value)
      {
        AsyncWithWait(() => LoadFile(openFileDialog2.FileName));
      }
    }

    private void LoadFile(string FileName)
    {
      if (res11file != null)
      {
        res11file.Dispose();
      }
      res11file = new Res11(FileName);
      StartTime = res11file.TimeSteps.First();
      EndTime = res11file.TimeSteps.Last();
      MinStartTime = StartTime;
      MaxEndTime = EndTime;

      this.FileName = res11file.AbsoluteFileName;

      NotifyPropertyChanged("FileName");
      NotifyPropertyChanged("StartTime");
      NotifyPropertyChanged("EndTime");
      NotifyPropertyChanged("MinStartTime");
      NotifyPropertyChanged("MaxEndTime");
    }

    #endregion
 

    #region SaveToShapeCommand
      RelayCommand savetoShp;

    /// <summary>
    /// Gets the command that loads the database
    /// </summary>
    public ICommand SavetoShpCommand
    {
      get
      {
        if (savetoShp == null)
        {
          savetoShp = new RelayCommand(param => SaveToShp(), param => res11file !=null);
        }
        return savetoShp;
      }
    }


    private void SaveToShp()
    {

            Microsoft.Win32.SaveFileDialog openFileDialog2 = new Microsoft.Win32.SaveFileDialog();
      openFileDialog2.Filter = "Known file types (*.shp)|*.sh";
      openFileDialog2.Title = "Save results to a shape file";


      if (openFileDialog2.ShowDialog().Value)
      {
        AsyncWithWait(() => SaveAndCalc(openFileDialog2.FileName));
      }
    }

    private void SaveAndCalc(string filename)
    {
      using (ShapeWriter sw = new ShapeWriter(filename))
      {
        DataTable dt = new DataTable();
        dt.Columns.Add("Branch", typeof(string));
        dt.Columns.Add("TopoID", typeof(string));
        dt.Columns.Add("Chainage", typeof(double));
        dt.Columns.Add("DataType", typeof(string));
        dt.Columns.Add("Min", typeof(double));
        dt.Columns.Add("Max", typeof(double));
        dt.Columns.Add("Average", typeof(double));
        dt.Columns.Add("MedianMin", typeof(double));


        foreach (var p in res11file.Points)
        {
          double min = double.MaxValue; ;
          double max = double.MinValue;
          double middel =0;
          List<double> yearlymins = new List<double>();
          int CurrentYear = 0;
          for (int i = res11file.GetTimeStep(StartTime); i < res11file.GetTimeStep(EndTime); i++)
          {
            if (CurrentYear != res11file.TimeSteps[i].Year)
            {
              CurrentYear = res11file.TimeSteps[i].Year;
              yearlymins.Add(Double.MaxValue);
            }
            double d = p.GetData(i);
            min = Math.Min(min, d);
            max = Math.Max(max, d);
            middel += d;
            yearlymins[yearlymins.Count - 1] = Math.Min(yearlymins.Last(), d);
          }

          middel /= (res11file.GetTimeStep(EndTime) - res11file.GetTimeStep(StartTime));
          var drow = dt.NewRow();
          drow[0] = p.BranchName;
          drow[1] = p.TopoID;
          drow[2] = p.Chainage;
          drow[3] = p.pointType.ToString();
          drow[4] = min;
          drow[5] = max;
          drow[6] = middel;
          drow[7] = yearlymins.Sum(var => var) / yearlymins.Count;

          GeoRefData grf = new GeoRefData();
          grf.Geometry = p;
          grf.Data = drow;

          sw.Write(grf);
            
        }
      }
    }

#endregion
  }
}
