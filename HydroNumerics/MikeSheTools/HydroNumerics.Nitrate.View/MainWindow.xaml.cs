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
    MainViewModel mvm;
    public MainWindow()
    {
      InitializeComponent();
      mvm = new MainViewModel();
      mvm.LoadCatchments(@"D:\DK_information\id15_NSTmodel\id15_NSTmodel.shp");
      mvm.LoadParticles(@"D:\DK_information\DK_data\Data from MIKE SHE WQ\PTReg_Extraction_1_20131016_dk4.shp");
      mvm.CombineParticlesAndCatchments();
      DataContext = mvm;


    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
        mvm.CurrentCatchment = e.AddedItems[0] as Catchment;
    }
  }
}
