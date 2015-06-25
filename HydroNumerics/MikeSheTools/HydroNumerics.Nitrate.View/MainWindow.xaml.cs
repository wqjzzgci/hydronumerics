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

using HydroNumerics.Nitrate.Model;

namespace HydroNumerics.Nitrate.View
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    MainModel mvm;
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new ViewModel();

      mvm = ((ViewModel)DataContext).TheModel;
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
        mvm.CurrentCatchment = e.AddedItems[0] as Catchment;
    }
  }
}
