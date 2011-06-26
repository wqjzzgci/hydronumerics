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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HydroNumerics.MikeSheTools.ViewModel;
using HydroNumerics.JupiterTools;
using HydroNumerics.Geometry;

using HelixToolkit;

namespace HydroNumerics.View3d
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    public MainWindow()
    {
      InitializeComponent();
      RegionViewModel rvm = new RegionViewModel();
      DataContext = rvm;
    }


    private List<Visual3D> list = new List<Visual3D>();

    private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SiteViewModel P = e.AddedItems[0] as SiteViewModel;

      if (P != null)
      {
        foreach (var t in list)
          view.Children.Remove(t);
        list.Clear();

        foreach (var vi in P.Representation3D )
        {
          view.Children.Add(vi);
            list.Add(vi);          
        }
      }
      view.ZoomToFit();
    }
  }
}
