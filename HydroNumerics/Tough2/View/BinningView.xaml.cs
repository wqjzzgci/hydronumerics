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

using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;

using HydroNumerics.Tough2.ViewModel;

namespace HydroNumerics.Tough2.View
{
  /// <summary>
  /// Interaction logic for BinningView.xaml
  /// </summary>
  public partial class BinningView : UserControl
  {
    public BinningView()
    {
      InitializeComponent();
    }

    List<object> graphs = new List<object>();

    int index = 0;

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Model M = DataContext as Model;

      int i =index;

        EnumerableDataSource<Element> x = new EnumerableDataSource<Element>(M.Elements.Where(var=>var.Z.HasValue));
        x.SetXMapping(var => (var.PrintData.Values[i]["PAIR"] + var.PrintData.Values[i]["PSATW"]) / var.PrintData.Values[i]["P"]);
        x.SetYMapping(var => var.Z.Value);
        var lg = Fig1.AddLineGraph(x, 3, "t = " + ((int)Math.Pow(10, i)) + "s");
        graphs.Add(lg);
          Fig1.UpdateLayout();


        EnumerableDataSource<Element> x2 = new EnumerableDataSource<Element>(M.Elements.Where(var => var.Z.HasValue));
        x2.SetXMapping(var => -var.Z.Value);
        x2.SetYMapping(var => var.PrintData.Values[i]["P"]-101300);
        var lg2 = Fig3.AddLineGraph(x2, 3, "t = " + ((int)Math.Pow(10, i)) + "s");
        graphs.Add(lg2);
        Fig3.UpdateLayout();

        index++;
      
    }

  }
}

