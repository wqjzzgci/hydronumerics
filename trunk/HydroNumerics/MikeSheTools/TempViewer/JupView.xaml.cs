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
     
    }

 
    
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      jvm.ReadJupiter();
    }

    private void LogWindow_SourceUpdated(object sender, DataTransferEventArgs e)
    {
      Scroller.UpdateLayout();
      Scroller.ScrollToBottom();
    }

    private void MenuItem_Click_1(object sender, RoutedEventArgs e)
    {
      UserAndProjectName usp = new UserAndProjectName();
     
      bool? Result =usp.ShowDialog();
      if (Result.HasValue)
        if (Result.Value)
          jvm.SaveChanges(usp.UserName.Text, usp.ProjectName.Text);
    }

    private void MenuItem_Click_2(object sender, RoutedEventArgs e)
    {
      jvm.ImportChanges();
    }
  }
}
