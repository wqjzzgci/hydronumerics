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
      ListWells.SelectionChanged += new SelectionChangedEventHandler(ListWells_SelectionChanged);
     
    }

    void ListWells_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      IWell iw = e.AddedItems[0] as IWell;

      if (iw != null)
        DetailedWellView.DataContext = new WellViewModel(iw,jvm);
    }

 
    
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      jvm.ReadJupiter();
    }

  }
}
