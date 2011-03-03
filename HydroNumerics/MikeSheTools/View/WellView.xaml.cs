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

using HydroNumerics.MikeSheTools.ViewModel;
using HydroNumerics.Time.Core;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.View
{
  /// <summary>
  /// Interaction logic for WellView.xaml
  /// </summary>
  public partial class WellView : UserControl
  {
    public WellView()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(WellView_DataContextChanged);

    }

    void WellView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      WellViewModel wm = (WellViewModel)e.NewValue;

      wm.XHistory = new System.Collections.ObjectModel.ObservableCollection<Change<double>>();
      wm.XHistory.Add(new Change<double>());
      wm.XHistory[0].Date = DateTime.Now;
      wm.XHistory[0].User = "jag";
      wm.XHistory[0].Project = "Sømod";
      wm.XHistory[0].OldValue = -99;
      wm.XHistory[0].NewValue = wm.X;

      if (wm.XHistory.Count() > 0)
        XHistory.Visibility = Visibility.Visible;
      else
        XHistory.Visibility = Visibility.Hidden;


      foreach (TimestampSeries ts in wm.Observations)
      {
        LineSeries LS = new LineSeries();
        LS.ItemsSource = ts.Items;
        LS.DependentValuePath = "Value";
        LS.IndependentValuePath = "Time";
        LS.Title = ts.Name;
      }
    }
  }
}
