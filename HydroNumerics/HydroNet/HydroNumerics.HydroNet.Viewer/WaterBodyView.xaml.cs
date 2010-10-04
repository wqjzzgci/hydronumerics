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
      DataContextChanged += new DependencyPropertyChangedEventHandler(VedstedCalibration_DataContextChanged);

    }

    void VedstedCalibration_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue.GetType().Equals(typeof(WaterBodyViewModel)))
      {
        WaterBodyViewModel wbm = DataContext as WaterBodyViewModel;

        WBChart.Series.Clear();
        PieSeries PS = new PieSeries();
        PS.ItemsSource = wbm.WaterBalanceComponents;
        PS.DependentValuePath = "Value";
        PS.IndependentValuePath = "Key";
        this.WBChart.Series.Add(PS);

        WBChart.MouseEnter += new MouseEventHandler(WBChart_MouseEnter);

      }
    }

    void WBChart_MouseEnter(object sender, MouseEventArgs e)
    {
      WBChart.UpdateLayout();
    } 

  }
}
