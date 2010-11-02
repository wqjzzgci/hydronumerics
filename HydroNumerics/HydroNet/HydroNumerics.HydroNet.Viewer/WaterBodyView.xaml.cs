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

using System.Windows.Controls.DataVisualization.Charting;

using HydroNumerics.HydroNet.ViewModel;
using HydroNumerics.Time.Core;


namespace HydroNumerics.HydroNet.View
{
  /// <summary>
  /// Interaction logic for VedstedCalibration.xaml
  /// </summary>
  public partial class WaterBodyView : UserControl
  {
    public WaterBodyView()
    {
      InitializeComponent();
      GWBoundaries.SelectionChanged += new SelectionChangedEventHandler(Boundaries_SelectionChanged);
      SinksBoundary.SelectionChanged += new SelectionChangedEventHandler(Boundaries_SelectionChanged);
      SourcesBoundary.SelectionChanged += new SelectionChangedEventHandler(Boundaries_SelectionChanged);

      DataContextChanged += new DependencyPropertyChangedEventHandler(WaterBodyView_DataContextChanged);
    }

    void WaterBodyView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      WaterBodyViewModel wbm = DataContext as WaterBodyViewModel;

      if (wbm != null)
      {
        foreach (var s in wbm.Chemicals)
          ChemicalsChart.Series.Add(s);
        foreach (var s in wbm.ChemicalMeasurements)
          ChemicalsChart.Series.Add(s);
        foreach (var c in wbm.Compositions)
          CompositionChart.Series.Add(c);
      }
    }



    void Boundaries_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
      {
        SingleBndGrid.Children.Clear();
        if (((Microsoft.Windows.Controls.DataGrid)sender).Name.Equals("GWBoundaries"))
        {
          var gw = new GroundwaterBoundaryView();
          gw.DataContext = e.AddedItems[0];
          SingleBndGrid.Children.Add(gw);
          SourcesBoundary.SelectedIndex = -1;
          SinksBoundary.SelectedIndex = -1;
        }
        else if (((Microsoft.Windows.Controls.DataGrid)sender).Name.Equals("SinksBoundary"))
        {
          SourcesBoundary.SelectedIndex = -1;
          GWBoundaries.SelectedIndex = -1;
        }
        else
        {
          GWBoundaries.SelectedIndex = -1;
          SinksBoundary.SelectedIndex = -1;
        }
      }
    }
  }
}
