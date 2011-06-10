using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;


using HydroNumerics.Tough2.ViewModel;


namespace HydroNumerics.Tough2.View
{
	/// <summary>
	/// Interaction logic for ResultsGraph.xaml
	/// </summary>
	public partial class ResultsGraph : UserControl
	{
		public ResultsGraph()
		{
			this.InitializeComponent();
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(ResultsGraph_DataContextChanged);

		}

    Func<Element, bool> CoordinateFilter;

    void ResultsGraph_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      CoordinateFilter = new Func<Element,bool>(var=>var.Z.HasValue);
    }


    Dictionary<string, LineGraph> output1d = new Dictionary<string, LineGraph>();

    /// <summary>
    /// Add to the graph
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      TimesOutput ts = (TimesOutput)TimeStep1.SelectedItem;

      foreach (var i in e.RemovedItems)
      {
        if (output1d.ContainsKey(i.ToString()))
        {
          Graf1.Children.Remove(output1d[i.ToString()]);
          output1d.Remove(i.ToString());
        }
      }

      foreach (var i in e.AddedItems)
      {
        EnumerableDataSource<Element>  x = new EnumerableDataSource<Element>(((Model)DataContext).Elements.Where(CoordinateFilter));
        
        string key = i.ToString();
        x.SetXMapping(var => var.X.Value);
        x.SetYMapping(var=>var.PrintData[ts.TotalTime][key]);
        var lg = Graf1.AddLineGraph(x, 3, key);
        output1d.Add(key, lg);
      }
    }

    private void Graf1_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key.Equals(Key.C))
      {
        PlotterConfigurator pc = new PlotterConfigurator();
        pc.DataContext = Graf1;
        pc.ShowDialog();
      }

    }

	}
}