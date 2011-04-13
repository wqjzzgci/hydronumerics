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
using HydroNumerics.MikeSheTools.ViewModel;
using HydroNumerics.Time.Core;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.View
{
  /// <summary>
  /// Interaction logic for WellView.xaml
  /// </summary>
  public partial class WellView : UserControl
  {
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

    private List<LineGraph> _obsGraphs = new List<LineGraph>();
    private List<LineGraph> _extGraphs = new List<LineGraph>();

    ObservableDataSource<TimestampValue> SelectedPoint = new ObservableDataSource<TimestampValue>();


    private bool waitx = true;
    private bool waity = true;
    private bool waitterrain = true;

    void WellView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {

      if (e.NewValue is WellViewModel)
      {
        waitx = true;
        waity = true;
        waitterrain = true;

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

    }



    private void X_TargetUpdated(object sender, DataTransferEventArgs e)
    {
      if (waitx)
        waitx = false;
      else
      {
      }
    }

    private void ObsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      foreach (var ToRemove in e.RemovedItems)
        SelectedPoint.Collection.Remove((TimestampValue)ToRemove);
      foreach (var ToAdd in e.AddedItems)
        SelectedPoint.Collection.Add((TimestampValue)ToAdd);

    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      ScreenAdder sca = new ScreenAdder();
      sca.DataContext = DataContext;
      bool? result =sca.ShowDialog();
      if (result.HasValue)
        if (result.Value)
        {
          WellViewModel wm = DataContext as WellViewModel;
          if (wm != null)
          {
            wm.AddScreen((IIntake)sca.IntakeSelect.SelectedItem, double.Parse(sca.ScreenTop.Text), double.Parse(sca.ScreenBottom.Text), sca.CommentText.Text);
          }

        }

    }

    private void Y_TargetUpdated(object sender, DataTransferEventArgs e)
    {
      if (waity)
        waity = false;
      else
      {
      }

    }

    private void TextBox_TargetUpdated(object sender, DataTransferEventArgs e)
    {
      if (waitterrain)
        waitterrain = false;
      else
      {
      }

    }

    private void TOPButton_Click(object sender, RoutedEventArgs e)
    {

      string k="1";

    }
  }
}
