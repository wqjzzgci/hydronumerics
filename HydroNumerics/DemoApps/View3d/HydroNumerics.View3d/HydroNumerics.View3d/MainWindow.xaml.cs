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

    private JupiterViewModel JVM;
    public MainWindow()
    {
      InitializeComponent();
      JVM = new JupiterViewModel();

      JVM.MinYearlyExtraction = 1000;

      DataContext = JVM;
     // view.ZoomToFitWhenLoaded = true;
    }


    private List<Visual3D> list = new List<Visual3D>();

    private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      PlantViewModel P = e.AddedItems[0] as PlantViewModel;

      if (P != null)
      {
        foreach (var t in list)
          view.Children.Remove(t);
        list.Clear();

        foreach (JupiterWell v in P.plant.PumpingWells)
        {
          foreach( var vi in v.Representation3D(P.plant))
          {
            view.Children.Add(vi);
            list.Add(vi);
          }
        }

        double? height = 40;
        HydroNumerics.Geometry.Net.KMSData.TryGetHeight(P.plant, 32, out height);

        var plant = XYPolygon.GetSquare(100, P.plant).Representation3D(P.plant, height.Value);
        view.Children.Add(plant);
        list.Add(plant);
       

      }
      view.ZoomToFit();
    }
  }
}
