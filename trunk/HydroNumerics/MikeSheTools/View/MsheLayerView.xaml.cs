using System;
using System.IO;
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

using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.MikeSheTools.View
{
  /// <summary>
  /// Interaction logic for MsheLayerView.xaml
  /// </summary>
  public partial class MsheLayerView : UserControl
  {
    public MsheLayerView()
    {
      InitializeComponent();
//      listBox1.DataContext = this.DataContext;
    }

    public void TestMehtod()
    {

      DataGrid1.ItemsSource = ((HydroNumerics.MikeSheTools.ViewModel.LayersCollection)DataGrid1.DataContext).Layers;
    }

    private void OpenMikeSheFile(object sender, RoutedEventArgs e)
    {

      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "she files (*.she)|*.she|All files (*.*)|*.*";
      if (openFileDialog.ShowDialog().Value)
      {
        ((HydroNumerics.MikeSheTools.ViewModel.LayersCollection)DataContext).MikeSheFileName = openFileDialog.FileName;
      }
    }

    private void OpenGridCodeFile(object sender, RoutedEventArgs e)
    {

      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "dfs2 files (*.dfs2)|*.dfs2|All files (*.*)|*.*";
      if (openFileDialog.ShowDialog().Value)
      {
        ((HydroNumerics.MikeSheTools.ViewModel.Layer)((Button)sender).DataContext).GridCodesFileName = openFileDialog.FileName;
      }
    }

    private void DataGrid1_SelectedCellsChanged(object sender, Microsoft.Windows.Controls.SelectedCellsChangedEventArgs e)
    {
      var layer=((Layer)((Microsoft.Windows.Controls.DataGrid)sender).SelectedItem);
      if (layer!=null)
        listBox1.ItemsSource = layer.Wells;
    }
  }
}
