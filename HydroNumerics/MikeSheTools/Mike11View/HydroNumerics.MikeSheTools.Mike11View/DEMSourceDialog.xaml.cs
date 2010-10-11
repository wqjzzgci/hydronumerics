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

namespace HydroNumerics.MikeSheTools.Mike11View
{
  /// <summary>
  /// Interaction logic for DEMSourceDialog.xaml
  /// </summary>
  public partial class DEMSourceDialog : Window
  {

    public DEMSourceDialog()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "dfs2 files | *.dfs2";

      if (openFileDialog.ShowDialog().Value)
      {
        Dfs2FileName.Text = openFileDialog.FileName;
      }     
    }

    private void SetSource()
    {
      if (DataContext != null)
      {
        DEMSourceConfiguration dem = DataContext as DEMSourceConfiguration;

        if (Web.IsChecked.Value)
          dem.DEMSource = SourceType.KMSWeb;
        else if (Oracle.IsChecked.Value)
          dem.DEMSource = SourceType.Oracle;
        else
          dem.DEMSource = SourceType.DFS2;
      }
    } 

    private void Web_Checked(object sender, RoutedEventArgs e)
    {
      SetSource(); 
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
      DEMSourceConfiguration dem = DataContext as DEMSourceConfiguration;

      double? height;

      try
      {
        dem.TryFindDemHeight(double.Parse(X.Text), double.Parse(Y.Text), out height);
        Height.Text = height.ToString();
        Status.Text = "";
      }
      catch (Exception ex)
      {
        Status.Text = ex.Message;
      }

    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
      this.Close();
    }
  }
}
