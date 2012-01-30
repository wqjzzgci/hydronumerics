using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace SimpleSample
{
  public partial class MainPage : UserControl
  {
    public MainPage()
    {
      InitializeComponent();


      Graph.Loaded += new RoutedEventHandler(Graph_Loaded);

    }

    void Graph_Loaded(object sender, RoutedEventArgs e)
    {

      double[] x =new double[]{2,6}; 
      double[] y =new double[]{2,6};

      var xd = x.AsXDataSource();
      xd.SetXMapping(v => v);

      var yd = y.AsYDataSource();
      yd.SetYMapping(v => v);

      CompositeDataSource csd = xd.Join(yd);

      
      Graph.Children.Add(new LineGraph(csd,"line"));

      Graph.FitToView();
     

      
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Graph.Height += 50;
    }
  }
}
