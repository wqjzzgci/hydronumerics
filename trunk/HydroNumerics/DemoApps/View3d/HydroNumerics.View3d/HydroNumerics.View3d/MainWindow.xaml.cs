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

using HydroNumerics.MikeSheTools.ViewModel;
using HydroNumerics.JupiterTools;

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


    private List<TruncatedConeVisual3D> list = new List<TruncatedConeVisual3D>();

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

          double x = Math.Abs(P.plant.X - v.X);
          double y = Math.Abs(P.plant.Y - v.Y);

          if (Math.Abs(x) < 15000)
          {
            if (v.Depth.HasValue)
            {
              TruncatedConeVisual3D tcv = new TruncatedConeVisual3D();
              tcv.TopRadius = 0.5;
              tcv.BaseRadius = 0.5;
              tcv.Origin = new System.Windows.Media.Media3D.Point3D(x, y, v.Terrain - v.Depth.Value);
              tcv.Height = v.Depth.Value;

              var m = new SolidColorBrush(Colors.Gray);
              tcv.Fill = m;
              view.Children.Add(tcv);
              list.Add(tcv);
            }

            foreach (var l in v.LithSamples)
            {
              if (l.Top != -999 & l.Bottom != -999)
              {
                TruncatedConeVisual3D tcv = new TruncatedConeVisual3D();
                tcv.TopRadius = 1;
                tcv.BaseRadius = 1;
                tcv.Origin = new System.Windows.Media.Media3D.Point3D(x, y, v.Terrain - l.Bottom);
                tcv.Height = l.Bottom - l.Top;
                if (l.RockType.ToLower().Contains("s"))
                {
                  var m = new SolidColorBrush(Colors.Blue);
                  m.Opacity = 0.3;
                  tcv.Fill = m;
                }
                else
                {
                  var m = new SolidColorBrush(Colors.Red);
                  m.Opacity = 0.3;
                  tcv.Fill = m;
                }
                view.Children.Add(tcv);
                list.Add(tcv);
              }
            }
          }
        }
      }
      view.ZoomToFit();
    }
  }
}
