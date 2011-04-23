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

namespace HydroNumerics.MikeSheTools.WellViewer
{
  /// <summary>
  /// Interaction logic for MsheLayerView.xaml
  /// </summary>
  public partial class MsheLayerView : UserControl
  {
    public MsheLayerView()
    {
      InitializeComponent();
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
      var items =((Microsoft.Windows.Controls.DataGrid)sender).SelectedItems;

      if (items.Count>1)
      {
        IEnumerable<HydroNumerics.Wells.IIntake> interwells = ((Layer)items[0]).Intakes;
        for (int i=1;i<items.Count;i++)
        {
          interwells = ((Layer)items[i]).Intakes.Intersect(interwells);
        }

        listBox1.ItemsSource = interwells;
      }
      else if (items.Count==1)
        listBox1.ItemsSource = ((Layer)items[0]).Intakes;

    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      ((LayersCollection)this.DataContext).BindAddedWells(); 
    }
  }
}
