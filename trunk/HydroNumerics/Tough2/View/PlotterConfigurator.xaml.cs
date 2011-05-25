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

using Microsoft.Research.DynamicDataDisplay.DataSources.MultiDimensional;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Charts;


namespace HydroNumerics.Tough2.View
{
  /// <summary>
  /// Interaction logic for PlotterConfigurator.xaml
  /// </summary>
  public partial class PlotterConfigurator : Window
  {
    HorizontalAxisTitle x;
    VerticalAxisTitle y;
    public PlotterConfigurator()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(PlotterConfigurator_DataContextChanged);
      
      Closing += new System.ComponentModel.CancelEventHandler(PlotterConfigurator_Closing);
    }

    void PlotterConfigurator_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (DataContext != null)
      {
        x = ((ChartPlotter)DataContext).Children.OfType<HorizontalAxisTitle>().FirstOrDefault();
        y = ((ChartPlotter)DataContext).Children.OfType<VerticalAxisTitle>().FirstOrDefault();

        tx.Text = x.Content.ToString();
        ty.Text = y.Content.ToString();
      }
    }

    void PlotterConfigurator_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      x.Content = tx.Text;
      y.Content = ty.Text; 
    }


  }
}
