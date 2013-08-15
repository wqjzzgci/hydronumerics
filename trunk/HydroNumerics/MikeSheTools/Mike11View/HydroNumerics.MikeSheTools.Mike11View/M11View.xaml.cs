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
        foreach (var CSC in ((M11Branch)b).CrossSections)
          m11.SelectedCrossSections.Add(CSC);
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
      }     
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
          
        }
      }
      CscList.Items.Refresh();

    }

    /// <summary>
    /// Saves changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveChanges_Click(object sender, RoutedEventArgs e)
    {
      m11.SaveChanges();
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
      M11BranchViewModel b = e.NewValue as M11BranchViewModel;

      foreach (var r in graphs)
        ObsGraph.Children.Remove(r);
      graphs.Clear();

      var v = ObsGraph.AddLineGraph(b.Profile, new Pen(Brushes.Blue, 3), new CircleElementPointMarker
     {
       Size = 10,
       Brush = Brushes.Red,
       Fill = Brushes.Orange
     }
             , null);

      graphs.Add(v.LineGraph);
      graphs.Add(v.MarkerGraph);

      b.ChainageOffset = b.Branch.ChainageEnd;


      foreach (var r in ngraphs)
        MapItems.Items.Remove(r);
      ngraphs.Clear();

      MapPolyline mp = new MapPolyline() { Stroke = new SolidColorBrush(Colors.Blue), StrokeThickness = 3, Locations = new LocationCollection() };
      foreach (var p in b.Branch.Line.Points)
        mp.Locations.Add(new Location(((XYPoint)p).Latitude, ((XYPoint)p).Longitude));

      mp.ToolTip = b.Branch.Name;
      MapItems.Items.Add(mp);
      ngraphs.Add(mp);

      //foreach (var xsec in b.Branch.CrossSections)
      //{
      //  MapPolyline mp2 = new MapPolyline() { Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 1, Locations = new LocationCollection() };
      //  foreach (var p in xsec.Line.Points)
      //    mp2.Locations.Add(new Location(((XYPoint)p).Latitude, ((XYPoint)p).Longitude));
      //  MapItems.Items.Add(mp2);
      //  ngraphs.Add(mp2);
      //}




      RecursiveAdd(b);
    }


    private void RecursiveAdd(M11BranchViewModel b)
    {
      foreach (var c in b.UpstreamBranches)
      {
        MapPolyline mp = new MapPolyline() { Stroke = new SolidColorBrush(Colors.Gray), StrokeThickness = 2, Locations = new LocationCollection() };
        foreach (var p in c.Branch.Line.Points)
          mp.Locations.Add(new Location(((XYPoint)p).Latitude, ((XYPoint)p).Longitude));

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

        graphs.Add(g.LineGraph);
        graphs.Add(g.MarkerGraph);

        RecursiveAdd(c);

      }
    }
  }
}
