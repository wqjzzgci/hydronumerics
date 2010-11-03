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

using HydroNumerics.HydroNet.Core;
using HydroNumerics.HydroNet.ViewModel;

namespace HydroNumerics.HydroNet.View
{
  /// <summary>
  /// Interaction logic for GroundwaterBoundaryView.xaml
  /// </summary>
  public partial class GroundwaterBoundaryView : UserControl
  {
    public GroundwaterBoundaryView()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(GroundwaterBoundaryView_DataContextChanged);
    }

    void GroundwaterBoundaryView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      GroundWaterBoundary g = DataContext as GroundWaterBoundary;
      if (g!=null)
      {
        WaterExpander.DataContext = new WaterViewModel(g.WaterSample);
      }
    }
  }
}
