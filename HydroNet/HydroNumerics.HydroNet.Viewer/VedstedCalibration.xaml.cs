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
  public partial class VedstedCalibration : UserControl
  {
    public VedstedCalibration()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(VedstedCalibration_DataContextChanged);


    }

    void VedstedCalibration_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (e.NewValue.GetType().Equals(typeof(DemoViewModel)))
      {
        this.SourceChart.Series.Clear();
        LineSeries Ls = new LineSeries();
        Ls.ItemsSource = ((DemoViewModel)DataContext).Discharge.Items;
        Ls.DependentValuePath = "Value";
        Ls.IndependentValuePath = "Time";
        Ls.Title = ((DemoViewModel)DataContext).Discharge.Name;
        SourceChart.Series.Add(Ls);

        LineSeries Ls2 = new LineSeries();
        Ls2.ItemsSource = ((DemoViewModel)DataContext).Outflow.Items;
        Ls2.DependentValuePath = "Value";
        Ls2.IndependentValuePath = "EndTime";
        Ls2.Title = ((DemoViewModel)DataContext).Outflow.Name;
        SourceChart.Series.Add(Ls2);


        this.concchart.Series.Clear();

        LineSeries LS3 = new LineSeries();
        LS3.ItemsSource = ((DemoViewModel)DataContext).ChlorideConc.Items;
        LS3.DependentValuePath = "Value";
        LS3.IndependentValuePath = "EndTime";
        LS3.Title = ((DemoViewModel)DataContext).ChlorideConc.Name;
        this.concchart.Series.Add(LS3);

        LineSeries LS4 = new LineSeries();
        LS4.ItemsSource = ((DemoViewModel)DataContext).IsotopeConc.Items;
        LS4.DependentValuePath = "Value";
        LS4.IndependentValuePath = "EndTime";
        LS4.Title = ((DemoViewModel)DataContext).IsotopeConc.Name;
        this.concchart.Series.Add(LS4);


        WBChart.Series.Clear();
        PieSeries PS = new PieSeries();
        PS.ItemsSource = ((DemoViewModel)DataContext).WaterBalanceComponents;
        PS.DependentValuePath = "Value";
        PS.IndependentValuePath = "Key";
        this.WBChart.Series.Add(PS);
        concchart.MouseEnter += new MouseEventHandler(concchart_MouseEnter);

        WBChart.MouseEnter += new MouseEventHandler(WBChart_MouseEnter);

        SourceChart.MouseEnter += new MouseEventHandler(SourceChart_MouseEnter);
      }
    }

    void SourceChart_MouseEnter(object sender, MouseEventArgs e)
    {
      SourceChart.UpdateLayout();
    }
    void WBChart_MouseEnter(object sender, MouseEventArgs e)
    {
      WBChart.UpdateLayout();
    }

    void concchart_MouseEnter(object sender, MouseEventArgs e)
    {
      concchart.UpdateLayout();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      ((DemoViewModel)DataContext).Run();
      //
      

      //WBChart.Refresh();

    }
  }
}
