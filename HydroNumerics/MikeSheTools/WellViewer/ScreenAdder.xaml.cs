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
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class ScreenAdder : Window
  {
    public ScreenAdder()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(ScreenAdder_Loaded);
    }

    void ScreenAdder_Loaded(object sender, RoutedEventArgs e)
    {
      ScreenAdderViewModel savm = DataContext as ScreenAdderViewModel;
      if (savm !=null)
        savm.RequestClose += () => { Close(); };
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      
      ScreenAdderViewModel savm = DataContext as ScreenAdderViewModel;
      if (savm != null)
        savm.Cancel();
    }
  }
}
