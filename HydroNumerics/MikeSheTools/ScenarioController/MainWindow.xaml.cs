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

using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.MikeSheTools.ScenarioController
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    ScenarioViewModel svm;
    public MainWindow()
    {
      InitializeComponent();

      svm = new ScenarioViewModel();
      DataContext = svm;
      svm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(svm_PropertyChanged);
    }

    void svm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Runs")
        updateDataGrid();
    }

    void updateDataGrid()
    {

      List<ScenarioRun> dc = svm.Runs;
 
      if (dc!=null)
      {
        
      dataGrid2.Columns.Clear();

        dataGrid2.Columns.Add(new DataGridTextColumn
        {
            Header = "Number",
            Binding = new Binding("Number")
        });

        dataGrid2.Columns.Add(new DataGridTextColumn
        {
          Header = "Status",
          Binding = new Binding("IsRunning")
        });


        int index = 0;
        foreach (var column in dc.First().ParamValues)
    {

          
        dataGrid2.Columns.Add(new DataGridTextColumn
        {
            Header = column.Key.DisplayName,
            Binding = new Binding(string.Format("ParamValues.Values[{0}]", index++))
        });

    }
}
    }
  }
}
