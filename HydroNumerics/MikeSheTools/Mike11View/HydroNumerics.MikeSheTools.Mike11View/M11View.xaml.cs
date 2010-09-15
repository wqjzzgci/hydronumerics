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

namespace HydroNumerics.MikeSheTools.View
{
  /// <summary>
  /// Interaction logic for M11View.xaml
  /// </summary>
  public partial class M11View : Window
  {
    M11ViewModel m11 = new M11ViewModel();

    public M11View()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      m11.Sim11FileName = @"C:\Users\Jacob\Work\HydroNumerics\MikeSheTools\TestData\Mike11\Novomr6_release2009.sim11";
    }
  }
}
