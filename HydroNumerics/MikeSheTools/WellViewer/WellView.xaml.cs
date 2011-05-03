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

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.Time.Core;
using HydroNumerics.MikeSheTools.ViewModel;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.WellViewer
{
  /// <summary>
  /// Interaction logic for WellView.xaml
  /// </summary>
  public partial class WellView : UserControl
  {

    private List<LineGraph> _obsGraphs = new List<LineGraph>();
    private List<LineGraph> _extGraphs = new List<LineGraph>();
    private ObservableDataSource<TimestampValue> SelectedPoint = new ObservableDataSource<TimestampValue>();

    public WellView()
    {
      
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(WellView_DataContextChanged);
      ObsSeriesSelector.ValueChanged += new RoutedPropertyChangedEventHandler<double>(ObsSeriesSelector_ValueChanged);

      SelectedPoint.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
      SelectedPoint.SetYMapping(var => var.Value);

      ObsGraph.AddLineGraph(SelectedPoint, null, new Microsoft.Research.DynamicDataDisplay.PointMarkers.CirclePointMarker(), null);
    }

    void ObsSeriesSelector_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      double k = e.NewValue; 
    }

    private static void DatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      ((WellView)sender).ZoomToTimeScale();
    }



    void WellView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue is WellViewModel)
      {
        SelectedPoint.Collection.Clear();

        foreach (var g in _obsGraphs)
          ObsGraph.Children.Remove(g);
        _obsGraphs.Clear();

        foreach (var g in _extGraphs)
          PumpingGraph.Children.Remove(g);
        _extGraphs.Clear();

        WellViewModel wm = (WellViewModel)e.NewValue;
        foreach (var ts in wm.Observations)
        {
          EnumerableDataSource<Time.Core.TimestampValue> ds = new EnumerableDataSource<TimestampValue>(ts.Second);
          ds.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
          ds.SetYMapping(var => var.Value);
          var g = ObsGraph.AddLineGraph(ds, new Pen(Brushes.Black, 3), new PenDescription(ts.First));
          _obsGraphs.Add(g); 
        }

        foreach (var ts in wm.Extractions)
        {
          EnumerableDataSource<Time.Core.TimestampValue> ds = new EnumerableDataSource<TimestampValue>(ts.Second);
          ds.SetXMapping(var => dateAxisExt.ConvertToDouble(var.Time));
          ds.SetYMapping(var => var.Value);
          var g = PumpingGraph.AddLineGraph(ds, new Pen(Brushes.Black, 3), new PenDescription(ts.First));
          _extGraphs.Add(g);
        }
      }
      ZoomToTimeScale();
    }


    public void ZoomToTimeScale()
    {
      if (SelectionEndTime != DateTime.MinValue)
      {
        ObsGraph.FitToView();
        DataRect visible = new DataRect(dateAxis.ConvertToDouble(SelectionStartTime), ObsGraph.Visible.Y, dateAxis.ConvertToDouble(SelectionEndTime) - dateAxis.ConvertToDouble(SelectionStartTime), ObsGraph.Visible.Height);
        ObsGraph.Visible = visible;

        PumpingGraph.FitToView();
        DataRect visible2 = new DataRect(dateAxis.ConvertToDouble(SelectionStartTime), PumpingGraph.Visible.Y, dateAxis.ConvertToDouble(SelectionEndTime) - dateAxis.ConvertToDouble(SelectionStartTime), PumpingGraph.Visible.Height);
        PumpingGraph.Visible = visible2;
      }
    }

    public static DependencyProperty SelectionStartTimeProperty = DependencyProperty.Register("SelectionStartTime", typeof(DateTime), typeof(WellView), new PropertyMetadata(new PropertyChangedCallback(DatePropertyChanged)));
    public DateTime SelectionStartTime
    {
      get { return (DateTime)GetValue(SelectionStartTimeProperty); }
      set
      {
        SetValue(SelectionStartTimeProperty, value);
        ZoomToTimeScale();
      }
    }

    public static DependencyProperty SelectionEndTimeProperty = DependencyProperty.Register("SelectionEndTime", typeof(DateTime), typeof(WellView), new PropertyMetadata(new PropertyChangedCallback(DatePropertyChanged)));
    public DateTime SelectionEndTime
    {
      get { return (DateTime)GetValue(SelectionEndTimeProperty); }
      set
      {
        SetValue(SelectionEndTimeProperty, value);
        ZoomToTimeScale();
      }
    }



    private void ObsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      foreach (var ToRemove in e.RemovedItems)
        SelectedPoint.Collection.Remove((TimestampValue)ToRemove);
      foreach (var ToAdd in e.AddedItems)
        SelectedPoint.Collection.Add((TimestampValue)ToAdd);

    }    
  }
}
