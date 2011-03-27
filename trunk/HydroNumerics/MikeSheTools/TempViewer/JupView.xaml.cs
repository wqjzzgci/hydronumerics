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

using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.View;
using HydroNumerics.MikeSheTools.ViewModel;

namespace TempViewer
{
  /// <summary>
  /// Interaction logic for JupView.xaml
  /// </summary>
  public partial class JupView : Window
  {
    private JupiterViewModel jvm = new JupiterViewModel();

    public JupView()
    {
      InitializeComponent();
      DataContext = jvm;
      List.SelectionChanged += new SelectionChangedEventHandler(List_SelectionChanged);
     
    }

    void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RbPlants.IsChecked.Value)
        {}
        else if (RbWells.IsChecked.Value)
        {
          if (List.SelectedItem != null)
            DetailedView.DataContext = new WellViewModel((IWell)List.SelectedItem);
          else
            DetailedView.DataContext = null;
        }
    }
 
    
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      jvm.ReadJupiter();
      RadioButton_Checked(null, null);
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
      if (DataContext != null)
      {
        if (RbPlants.IsChecked.Value)
          List.ItemsSource = jvm.Plants.OrderBy(var => var.Name);
        else if (RbWells.IsChecked.Value)
        {
          FilterBox.Content = new NumberOfObsFilter();
          List.ItemsSource = jvm.SortedAndFilteredWells;
          DetailedView.Content = new WellView();
        }
      }

    }
  }
}
