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
using System.Windows.Controls.DataVisualization.Charting;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.DataSources;

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

    }

    private List<LineGraph> _obsGraphs = new List<LineGraph>();


    void WellView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {

      foreach (var g in _obsGraphs)
        ObsGraph.Children.Remove(g);

      WellViewModel wm = (WellViewModel)e.NewValue;
      foreach (TimestampSeries ts in wm.Observations)
      {

        EnumerableDataSource<Time.Core.TimestampValue> ds = new EnumerableDataSource<TimestampValue>(ts.Items);
        ds.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
        ds.SetYMapping(var => var.Value);
        var g = ObsGraph.AddLineGraph(ds, new Pen(Brushes.Black, 3), new PenDescription(ts.Name));
        _obsGraphs.Add(g);
      }

    }


    private void X_TextChanged(object sender, TextChangedEventArgs e)
    {
      
      int k = 2;

    }

    private void Y_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void X_TargetUpdated(object sender, DataTransferEventArgs e)
    {
      int k = 2;

    }
  }
}
