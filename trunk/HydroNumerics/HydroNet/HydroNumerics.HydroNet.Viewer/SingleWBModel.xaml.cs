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

using HydroNumerics.HydroNet.Core;
using HydroNumerics.HydroNet.ViewModel;

namespace HydroNumerics.HydroNet.View
{
  /// <summary>
  /// Interaction logic for SingleWBModel.xaml
  /// </summary>
  public partial class SingleWBModel : Window
  {
    public SingleWBModel()
    {
      InitializeComponent();
   
    
    }

    /// <summary>
    /// Loads a model
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "xml files | *.xml";

      if (openFileDialog.ShowDialog().Value)
      {
        ModelList.Items.Add(new WaterBodyViewModel((AbstractWaterBody)ModelFactory.GetModel(openFileDialog.FileName)._waterBodies.First()));
        DataContext = ModelFactory.GetModel(openFileDialog.FileName);
      }     
    }

    /// <summary>
    /// Runs the model
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      Model M = DataContext as Model;

      M.SetState("Initial",StartTime.SelectedDate.Value,new WaterPacket(1));
      ((Model)DataContext).MoveInTime(EndTime.SelectedDate.Value, TimeSpan.FromDays(1));

      foreach( System.Windows.Controls.DataVisualization.Charting.LineSeries ls in WaterView.OutputChart.Series)
        ls.Refresh();

     

    }

    /// <summary>
    /// Saves the model
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
      saveFileDialog.Filter = "xml files | *.xml";
      if (saveFileDialog.ShowDialog().Value)
      {
        Model M = DataContext as Model;
        M.Save(saveFileDialog.FileName);
      }
    }
  }
}
