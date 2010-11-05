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

namespace HydroNumerics.HydroNet.View
{
  /// <summary>
  /// Interaction logic for Chart.xaml
  /// </summary>
  public partial class Chart : UserControl
  {
    public Chart()
    {
      InitializeComponent();
    }

    public void AddSeries(Series S)
    {
      TheChart.Series.Add(S);
      CheckBox ch = new CheckBox();
      ch.IsChecked = true;
      ch.Name = S.Name;
      ((Grid)SeriesItem.Content).Children.Add(ch);
    }
  }
}
