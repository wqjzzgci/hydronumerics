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

using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.MikeSheTools.WellViewer
{
  /// <summary>
  /// Interaction logic for WellsOnPlantView.xaml
  /// </summary>
  public partial class WellsOnPlantView : Window
  {
    public WellsOnPlantView()
    {
      InitializeComponent();
      Loaded += new RoutedEventHandler(WellsOnPlantView_Loaded);
    }

    void WellsOnPlantView_Loaded(object sender, RoutedEventArgs e)
    {
      WellsOnPlantViewModel savm = DataContext as WellsOnPlantViewModel;
      if (savm != null)
        savm.RequestClose += () => { Close(); };

    }



    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      IIntake I = e.NewValue as IIntake;
      ((WellsOnPlantViewModel)DataContext).SelectedIntake = I;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      WellsOnPlantViewModel wpv = DataContext as WellsOnPlantViewModel;
      if (wpv != null)
        wpv.Cancel();
    }
  }
}
