using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Maps.MapControl.WPF;

using HydroNumerics.MikeSheTools.ViewModel;
using HydroNumerics.MikeSheTools.Mike11;
using HydroNumerics.Geometry;

namespace HydroNumerics.MikeSheTools.Mike11View
{
  /// <summary>
  /// Interaction logic for M11View.xaml
  /// </summary>
  public partial class M11View : Window
  {
    M11ViewModel m11 = new M11ViewModel();

    public M11View()
    {
      InitializeComponent();
      BranchList.SelectionChanged += new SelectionChangedEventHandler(BranchList_SelectionChanged);
    }

    /// <summary>
    /// Updates the selected cross sections every time new branches are selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void BranchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

      foreach (var b in e.RemovedItems)
        foreach (var CSC in ((M11Branch)b).CrossSections)
          m11.SelectedCrossSections.Remove((CSC));
      foreach (var b in e.AddedItems)
      {
        foreach (var CSC in ((M11Branch)b).CrossSections)
          m11.SelectedCrossSections.Add(CSC);


      }
    }

    /// <summary>
    /// Opens a sim 11 file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "sim 11 files | *.sim11";

      if (openFileDialog.ShowDialog().Value)
      {
        m11.Sim11FileName = openFileDialog.FileName;
        DataContext = m11;
        m11.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(m11_PropertyChanged);
      }     
    }

    void m11_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "CurrentBranch")
        UpdateView();
    }

    /// <summary>
    /// Saves to shape
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
      if (dlg.ShowDialog() == true)
      {
        m11.WriteToShape(dlg.FileName);
      }
    }

    /// <summary>
    /// Adjusts the heights on the selected cross sections
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AdjustDatums_Click(object sender, RoutedEventArgs e)
    {
      foreach (var CSC in CscList.SelectedItems)
      {
        CrossSection cs = CSC as CrossSection;
        if (cs.DEMHeight.HasValue)
        {
          cs.MaxHeightMrk1and3 = cs.DEMHeight.Value;
          m11.HasChanges = true;
        }
      }
      CscList.Items.Refresh();
    }


    

    /// <summary>
    /// Opens the DEM source dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
      DEMSourceDialog dms = new DEMSourceDialog();
      dms.DataContext = m11.DEMConfig;
      dms.ShowDialog();
    }


    List<IPlotterElement> graphs = new List<IPlotterElement>();
    List<MapPolyline> ngraphs = new List<MapPolyline>();

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      m11.CurrentBranch = e.NewValue as M11BranchViewModel;
    }



    private void UpdateView()
    {
      foreach (var r in graphs)
        ObsGraph.Children.Remove(r);
      graphs.Clear();

      foreach (var r in ngraphs)
        MapItems.Items.Remove(r);
      ngraphs.Clear();

      if (m11 == null)
        return;

      var v = ObsGraph.AddLineGraph(m11.CurrentBranch.Profile, new Pen(Brushes.Green, 3), new CircleElementPointMarker
     {
       Size = 10,
       Brush = Brushes.Red,
       Fill = Brushes.Orange
     }
             , null);

      var bl = ObsGraph.AddLineGraph(m11.CurrentBranch.BottomProfileOffset, Colors.Blue, 3);

      
      graphs.Add(v.LineGraph);
      graphs.Add(v.MarkerGraph);
      graphs.Add(bl);

      m11.CurrentBranch.ChainageOffset = m11.CurrentBranch.Branch.ChainageEnd;



      MapPolyline mp = new MapPolyline() { Stroke = new SolidColorBrush(Colors.Blue), StrokeThickness = 4, Locations = new LocationCollection() };
      foreach (var p in m11.CurrentBranch.Branch.Line.Points)
        mp.Locations.Add(new Location(((XYPoint)p).Latitude, ((XYPoint)p).Longitude));

      MyMap.Center = new Location(m11.CurrentBranch.Branch.Line.Points.Average(p => ((XYPoint)p).Latitude), m11.CurrentBranch.Branch.Line.Points.Average(p => ((XYPoint)p).Longitude));

      mp.ToolTip = m11.CurrentBranch.Branch.Name;
      MapItems.Items.Add(mp);
      ngraphs.Add(mp);

      foreach (var xsec in m11.CurrentBranch.Branch.CrossSections)
      {
        MapPolyline mp2 = new MapPolyline() { Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 1, Locations = new LocationCollection() };
        foreach (var p in xsec.Line.Points)
          mp2.Locations.Add(new Location(((XYPoint)p).Latitude, ((XYPoint)p).Longitude));
        MapItems.Items.Add(mp2);
        ngraphs.Add(mp2);
      }

      RecursiveAdd(m11.CurrentBranch);
    }


    private void RecursiveAdd(M11BranchViewModel b)
    {
      foreach (var c in b.UpstreamBranches)
      {
        MapPolyline mp = new MapPolyline() { Stroke = new SolidColorBrush(Colors.Gray), StrokeThickness = 3, Locations = new LocationCollection() };
        foreach (var p in c.Branch.Line.Points)
          mp.Locations.Add(new Location(((XYPoint)p).Latitude, ((XYPoint)p).Longitude));
        
          mp.ToolTip = c.Branch.Name;

        MapItems.Items.Add(mp);
        ngraphs.Add(mp);

        c.ChainageOffset = b.ChainageOffset - b.Branch.ChainageEnd + c.Branch.DownStreamConnection.StartChainage;
        var g = ObsGraph.AddLineGraph(c.ProfileOffset, new Pen(Brushes.Gray, 2), new CircleElementPointMarker
        {
          Size = 5,
          Brush = Brushes.Red,
          Fill = Brushes.Orange
        }
       , null);

        var bl = ObsGraph.AddLineGraph(c.BottomProfileOffset, Colors.Black, 2);

        graphs.Add(g.LineGraph);
        graphs.Add(g.MarkerGraph);
        graphs.Add(bl);
        RecursiveAdd(c);

      }
    }

    Dictionary<CrossSection, IPlotterElement> XsecsInPlotter = new Dictionary<CrossSection, IPlotterElement>();

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

      foreach (var v in e.RemovedItems)
      {
        XsecsPlot.Children.Remove(XsecsInPlotter[(CrossSection)v]);
        XsecsInPlotter.Remove((CrossSection)v);
      }

      foreach (var v in e.AddedItems)
      {
        CrossSection xsec = v as CrossSection;
        List<Tuple<double, double>>  points = xsec.GetXZPoints();


        var xData = new EnumerableDataSource<Tuple<double, double>>(points);
        xData.SetXMapping(x => x.Item1);
        var yData = new EnumerableDataSource<Tuple<double, double>>(points);
        yData.SetYMapping(y => y.Item2);
        var datasource = xData.Join(yData);

        XsecsInPlotter.Add(xsec, XsecsPlot.AddLineGraph(datasource, Colors.Black, 2, "Chainage = " + xsec.Chainage));

      }


    }
  }
}
