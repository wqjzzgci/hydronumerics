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
using System.Windows.Shapes;

using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.MikeSheTools.WellViewer
{
  /// <summary>
  /// Interaction logic for WritePermits.xaml
  /// </summary>
  public partial class WritePermits : Window
  {
    JupiterViewModel JVM;


    public WritePermits()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(WritePermits_DataContextChanged);

    }

    void WritePermits_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      JVM = DataContext as JupiterViewModel;
      if (JVM != null)
      {
        Distyear.Text = JVM.SelectionEndTime.Year.ToString();
        StartYear.Text = JVM.SelectionStartTime.Year.ToString();
        EndYear.Text = JVM.SelectionEndTime.Year.ToString();
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (JVM != null)
      {
        int distyear = int.Parse(Distyear.Text);
        int startyear = int.Parse(StartYear.Text);
        int endyear = int.Parse(EndYear.Text);
        JVM.SaveExtractionPermits(distyear, startyear, endyear);
      }
      this.Close();
    }
  }
}
