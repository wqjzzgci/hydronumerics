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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.DataSources;

using HydroNumerics.JupiterTools;
using HydroNumerics.Time.Core;
using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.MikeSheTools.WellViewer
{
  /// <summary>
  /// Interaction logic for PlantView.xaml
  /// </summary>
  public partial class PlantView : UserControl
  {

    private List<LineGraph> _extGraphs = new List<LineGraph>();
    private List<LineGraph> _wells = new List<LineGraph>();

    public PlantView()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(PlantView_DataContextChanged);
      ZoomToTimeScale();

    }

    void PlantView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      foreach (var g in _extGraphs)
        ExtGraph.Children.Remove(g);
      _extGraphs.Clear();

      foreach (var g in _wells)
        ExtGraph.Children.Remove(g);
      _wells.Clear();


      Plant P = e.NewValue as Plant;
      if (P != null)
      {
        EnumerableDataSource<Time.Core.TimestampValue> ds = new EnumerableDataSource<TimestampValue>(P.Extractions.AsTimeStamps);
        ds.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
        ds.SetYMapping(var => var.Value);
        _extGraphs.Add(ExtGraph.AddLineGraph(ds, new Pen(Brushes.Black, 3), new PenDescription("Groundwater")));

        if (P.SurfaceWaterExtrations.Items.Count > 0)
        {
          EnumerableDataSource<Time.Core.TimestampValue> ds2 = new EnumerableDataSource<TimestampValue>(P.Extractions.AsTimeStamps);
          ds2.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
          ds2.SetYMapping(var => var.Value);
          _extGraphs.Add(ExtGraph.AddLineGraph(ds, new Pen(Brushes.Red, 3), new PenDescription("Surface water")));
        }
      }
      ZoomToTimeScale();
    }


    public void ZoomToTimeScale()
    {
      ExtGraph.FitToView();
      DataRect visible = new DataRect(dateAxis.ConvertToDouble(SelectionStartTime), ExtGraph.Visible.Y, dateAxis.ConvertToDouble(SelectionEndTime) - dateAxis.ConvertToDouble(SelectionStartTime), ExtGraph.Visible.Height);
      ExtGraph.Visible = visible;
    }

    public static DependencyProperty SelectionStartTimeProperty = DependencyProperty.Register("SelectionStartTime", typeof(DateTime), typeof(PlantView), new PropertyMetadata(null));
    public DateTime SelectionStartTime
    {
      get { return (DateTime) GetValue(SelectionStartTimeProperty); }
      set 
      {
        SetValue(SelectionStartTimeProperty, value);
        ZoomToTimeScale();
      }
    }


    public static DependencyProperty SelectionEndTimeProperty = DependencyProperty.Register("SelectionEndTime", typeof(DateTime), typeof(PlantView), new PropertyMetadata(null));
    public DateTime SelectionEndTime
    {
      get { return (DateTime)GetValue(SelectionEndTimeProperty); }
      set
      {
        SetValue(SelectionEndTimeProperty, value);
        ZoomToTimeScale();
      }
    }

    private void ShowWells_Checked(object sender, RoutedEventArgs e)
    {
      Plant P = DataContext as Plant;
      if (P != null)
      {
        foreach (var I in P.PumpingIntakes)
        {
          EnumerableDataSource<Time.Core.TimestampValue> ds = new EnumerableDataSource<TimestampValue>(I.Intake.Extractions.AsTimeStamps);
          ds.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
          ds.SetYMapping(var => var.Value);
          _wells.Add(ExtGraph.AddLineGraph(ds, I.Intake.ToString()));
        }
      }

    }

    private void ShowWells_Unchecked(object sender, RoutedEventArgs e)
    {
      foreach (var g in _wells)
        ExtGraph.Children.Remove(g);
      _wells.Clear();
    }
  }
}
