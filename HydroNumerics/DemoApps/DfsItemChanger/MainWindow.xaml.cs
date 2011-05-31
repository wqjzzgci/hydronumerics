using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using HydroNumerics.MikeSheTools.DFS;
using DHI.Generic.MikeZero;
using DHI.Generic.MikeZero.DFS;

namespace DfsItemChanger
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    public ObservableCollection<string> FileNames { get; private set; }

    public MainWindow()
    {
      InitializeComponent();

      FileNames = new ObservableCollection<string>();
      Files.ItemsSource = FileNames;

      List<eumQuantity> eums = new List<eumQuantity>();

      foreach (eumItem v in Enum.GetValues(typeof(eumItem)))
       eums.Add(new eumQuantity(v));


      Items.ItemsSource = eums;
      Items.DisplayMemberPath = "ItemDescription";
      Items.SelectionChanged += new SelectionChangedEventHandler(Items_SelectionChanged);

      Type.ItemsSource = Enum.GetNames(typeof(DataValueType));
    }

    void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var Item = Items.SelectedValue as eumQuantity;
        if (Item != null)
        {
          Units.ItemsSource = Item.AllowedUnitsForItem();
        }
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {

      eumQuantity eq = Items.SelectedValue as eumQuantity;
      
      if(eq!=null) 
        eq.Unit = (eumUnit)Enum.Parse(typeof(eumUnit),Units.SelectedItem.ToString());
      

      foreach (var f in FileNames)
      {
        DFSBase dfs = HydroNumerics.MikeSheTools.DFS.DfsFileFactory.OpenFile(f);
        foreach (var i in dfs.Items)
        {
          if (eq != null)
          {
            i.EumItem = eq.Item;
            i.EumUnit = eq.Unit;
          }
          if (Type.SelectedItem != null) 
            i.ValueType =(DataValueType)Enum.Parse(typeof(DataValueType),Type.SelectedItem.ToString());
        }
        dfs.Dispose();
      }

      this.Close();
    }

    private void button3_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.dfs0,*.dfs2,*.dfs3)|*.dfs0;*.dfs2;*.dfs3";
      openFileDialog2.Multiselect = true;
      openFileDialog2.Title = "Select a DFS-file";

      if (openFileDialog2.ShowDialog().Value)
      {
        foreach(var s in openFileDialog2.FileNames)
        FileNames.Add(s);
      }

    }
  }
}
