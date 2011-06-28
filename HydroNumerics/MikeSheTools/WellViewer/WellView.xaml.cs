using System;
using System.Diagnostics;
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
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

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

    private List<IPlotterElement> _obsGraphs = new List<IPlotterElement>();
    private List<LineGraph> _extGraphs = new List<LineGraph>();
    private ObservableDataSource<TimestampValue> SelectedPoint = new ObservableDataSource<TimestampValue>();

    private Pen[] pens;

    public WellView()
    {
      
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(WellView_DataContextChanged);

      SelectedPoint.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
      SelectedPoint.SetYMapping(var => var.Value);

      ObsGraph.MouseLeftButtonDown += new MouseButtonEventHandler(g_MouseLeftButtonDown);

      ObsGraph.AddLineGraph(SelectedPoint, null, new CircleElementPointMarker {
                      Size = 10,
                      Brush = Brushes.Red,
                      Fill = Brushes.Orange
                    }
                    , null);

      pens = new Pen[10];

      pens[0] = new Pen(Brushes.Black, 3);
      pens[1] = new Pen(Brushes.Red, 3);
      pens[2] = new Pen(Brushes.Blue, 3);
      pens[3] = new Pen(Brushes.Green, 3);
      pens[4] = new Pen(Brushes.Yellow, 3);
      pens[5] = new Pen(Brushes.Brown, 3);
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
        
        int Pencount = 0;
        foreach (var ts in wm.HeadObservations)
        {
          ts.Value.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
          ts.Value.SetYMapping(var => var.Value);
          var g = ObsGraph.AddLineGraph(ts.Value,pens[Pencount], new PenDescription(ts.Key.ToString()));
          _obsGraphs.Add(g);
          Pencount++;
        }

        Pencount = 0;
        foreach (var ts in wm.Extractions)
        {
          EnumerableDataSource<Time.Core.TimestampValue> ds = new EnumerableDataSource<TimestampValue>(ts.Second);
          ds.SetXMapping(var => dateAxisExt.ConvertToDouble(var.Time));
          ds.SetYMapping(var => var.Value);
          var g = PumpingGraph.AddLineGraph(ds, pens[Pencount], new PenDescription(ts.First));
          _extGraphs.Add(g);
        }
      }
      ZoomToTimeScale();
    }

    void g_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
   

      var p = e.GetPosition((IInputElement)sender);
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

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
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
