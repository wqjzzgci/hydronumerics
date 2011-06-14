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

using HydroNumerics.Tough2.ViewModel;

using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;


namespace HydroNumerics.Tough2.View
{
  /// <summary>
  /// Interaction logic for TsTimeSeries.xaml
  /// </summary>
  public partial class TsTimeSeries : UserControl
  {
    public TsTimeSeries()
    {
      InitializeComponent();
    }

    List<LineGraph> _graphs = new List<LineGraph>();

    private void checkBox1_Checked(object sender, RoutedEventArgs e)
    {
      if (((CheckBox)sender).IsChecked.Value)
      {
        foreach (var v in listBox1.SelectedItems)
        {
          EnumerableDataSource<TSBrtEntry> x = new EnumerableDataSource<TSBrtEntry>(((KeyValuePair<Element,IEnumerable<TSBrtEntry>>) v).Value);

          x.SetXMapping(var => var.Time.TotalHours);
          x.SetYMapping(var => var.Pressure);
          var lg = Graf1.AddLineGraph(x, 3, "Pressure");
          _graphs.Add(lg);
        }
      }
    }

    private void checkBox2_Checked(object sender, RoutedEventArgs e)
    {
      if (((CheckBox)sender).IsChecked.Value)
      {
        foreach (var v in listBox1.SelectedItems)
        {
          EnumerableDataSource<TSBrtEntry> x = new EnumerableDataSource<TSBrtEntry>(((KeyValuePair<Element, IEnumerable<TSBrtEntry>>)v).Value);

          x.SetXMapping(var => var.Time.TotalHours);
          x.SetYMapping(var => var.WaterSaturation);
          var lg = Graf1.AddLineGraph(x, 3, "Water Saturation");
          _graphs.Add(lg);
        }
      }

    }

    private void checkBox3_Checked(object sender, RoutedEventArgs e)
    {
      if (((CheckBox)sender).IsChecked.Value)
      {
        foreach (var v in listBox1.SelectedItems)
        {
          EnumerableDataSource<TSBrtEntry> x = new EnumerableDataSource<TSBrtEntry>(((KeyValuePair<Element, IEnumerable<TSBrtEntry>>)v).Value);

          x.SetXMapping(var => var.Time.TotalHours);
          x.SetYMapping(var => var.VOCGasPhase);
          var lg = Graf1.AddLineGraph(x, 3, "VOC in gas phase");
          _graphs.Add(lg);
        }
      }

    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      foreach(var v in _graphs)
        Graf1.Children.Remove(v);
    }
  }
}
