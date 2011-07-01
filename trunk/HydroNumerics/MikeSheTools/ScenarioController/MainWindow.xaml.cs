using System;
using System.ComponentModel;
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
      //register for unhandled exceptions
      Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

      svm = new ScenarioViewModel();
      DataContext = svm;
      svm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(svm_PropertyChanged);
    }

    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      Exception ex = e.ExceptionObject as Exception;
      MessageBox.Show(ex.InnerException.Message + "\n" + ex.InnerException.Source + "\n" + ex.InnerException.StackTrace + "\n" + ex.InnerException.TargetSite);
    }

    void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
      MessageBox.Show(e.Exception.Message + "\n" + e.Exception.Source + "\n" + e.Exception.StackTrace + "\n" + e.Exception.TargetSite);
      e.Handled = true;
    }

    void svm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Runs")
        updateDataGrid();
    }

    void updateDataGrid()
    {

      List<ScenarioRun> dc = svm.Runs;

      if (dc != null)
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

        dataGrid2.Columns.Add(new DataGridCheckBoxColumn
        {
          Header = "Run this",
          Binding = new Binding("RunThis")
        });


        int index = 0;
        foreach (var column in dc.First().ParamValues)
        {
          dataGrid2.Columns.Add(new DataGridTextColumn
          {
            Header = column.Key.ShortName,
            Binding = new Binding(string.Format("ParamValues.Values[{0}]", index++))
          });

        }
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      FolderPickerLib.FolderPickerDialog fpd = new FolderPickerLib.FolderPickerDialog();

      if (fpd.ShowDialog().Value)
        Outputdir.Text = fpd.SelectedPath;

    }

    private void button3_Click(object sender, RoutedEventArgs e)
    {
            Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.ShowReadOnly = true;
      openFileDialog2.Multiselect = true;
      openFileDialog2.Title = "Select file names to copy after a scenario run";

      if (openFileDialog2.ShowDialog().Value)
      {
        IEditableCollectionViewAddNewItem items = listBox2.Items; 
        
        if (items.CanAddNewItem)
        foreach(var fi in openFileDialog2.FileNames)
          items.AddNewItem(fi);
      }
    }
  }
}
