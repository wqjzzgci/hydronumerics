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

using HydroNumerics.HydroNet.ViewModel;



namespace HydroNumerics.HydroNet.View
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1()
    {
      ModelViewModel mp = new ModelViewModel(new HydroNumerics.HydroNet.Core.Model(@"c:\temp\setup.xml"));
      this.DataContext = mp;
      InitializeComponent();


      //dataGrid1.ItemsSource = mp.WaterBodies;
    }

    private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {

    }
  }
}
