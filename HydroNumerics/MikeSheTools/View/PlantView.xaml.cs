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

namespace HydroNumerics.MikeSheTools.View
{
  /// <summary>
  /// Interaction logic for PlantView.xaml
  /// </summary>
  public partial class PlantView : UserControl
  {

    private List<LineGraph> _extGraphs = new List<LineGraph>();

    public PlantView()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(PlantView_DataContextChanged);
    }

    void PlantView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      foreach (var g in _extGraphs)
        ExtGraph.Children.Remove(g);

      _extGraphs.Clear();

      Plant P = e.NewValue as Plant;
      EnumerableDataSource<Time.Core.TimespanValue> ds = new EnumerableDataSource<TimespanValue>(P.Extractions.Items);
        ds.SetXMapping(var => dateAxis.ConvertToDouble(var.StartTime));
        ds.SetYMapping(var => var.Value);
        _extGraphs.Add(ExtGraph.AddLineGraph(ds, new Pen(Brushes.Black, 3), new PenDescription("Groundwater")));
       
      if (P.SurfaceWaterExtrations.Items.Count>0)
      {
        EnumerableDataSource<Time.Core.TimespanValue> ds2 = new EnumerableDataSource<TimespanValue>(P.Extractions.Items);
        ds2.SetXMapping(var => dateAxis.ConvertToDouble(var.StartTime));
        ds2.SetYMapping(var => var.Value);
        _extGraphs.Add(ExtGraph.AddLineGraph(ds, new Pen(Brushes.Red, 3), new PenDescription("Surface water")));
      }
    }
  }
}
