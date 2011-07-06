using System;
using System.Diagnostics;
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
using HydroNumerics.MikeSheTools.DFS;


namespace Dfs2TxtMerger
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    DFSBase DFS;

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.dfs2; *.dfs3)|*.dfs2; *.dfs3";
      openFileDialog2.Title = "Select a dfs file with grid codes";

      if (openFileDialog2.ShowDialog().Value)
      {
        DFS2File.Text = openFileDialog2.FileName;
        
      }
    }

    private void button2_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog2 = new Microsoft.Win32.OpenFileDialog();
      openFileDialog2.Filter = "Known file types (*.txt; *.asc)|*.txt; *.asc";
      openFileDialog2.Title = "Select a txt file with grid codes and data";

      if (openFileDialog2.ShowDialog().Value)
      {
        TxtFile.Text = openFileDialog2.FileName;
      }
    }

    private void button4_Click(object sender, RoutedEventArgs e)
    {
      string ext = System.IO.Path.GetExtension(DFS2File.Text);

      Microsoft.Win32.SaveFileDialog SaveFileDialog = new Microsoft.Win32.SaveFileDialog();
      SaveFileDialog.Filter = string.Format("Known file types (*{0})|*{0}",ext);
      SaveFileDialog.Title = "Merge into dfs-file";

      if (SaveFileDialog.ShowDialog().HasValue)
      {
        DFS = DfsFileFactory.OpenFile(DFS2File.Text); 
        var dfsout = DfsFileFactory.CreateFile(SaveFileDialog.FileName, 1);

        using (System.IO.StreamReader sr = new System.IO.StreamReader(TxtFile.Text))
        {
          dfsout.CopyFromTemplate(DFS);

          dfsout.FirstItem.EumItem = DHI.Generic.MikeZero.eumItem.eumIPrecipitationRate;
          dfsout.FirstItem.EumUnit = DHI.Generic.MikeZero.eumUnit.eumUmmPerDay;
          dfsout.TimeStep = TimeControl.Value;
            dfsout.TimeOfFirstTimestep = datePicker1.DateTimeSelected;
  
          sr.ReadLine();

          var gridcodes = DFS.ReadItemTimeStep(0, 1);
          Dictionary<int, List<int>> GridcodesIndex = new Dictionary<int, List<int>>();

          for (int i = 0; i<gridcodes.Count(); i++) 
          {
            if (gridcodes[i] != DFS.DeleteValue)
            {
              List<int> grids;
              if (!GridcodesIndex.TryGetValue((int)gridcodes[i], out grids))
              {
                grids = new List<int>();
                GridcodesIndex.Add((int)gridcodes[i], grids);
              }
              grids.Add(i);
              gridcodes[i] = (float)DFS.DeleteValue;
            }

          }

          var splits = sr.ReadLine().Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
          List<int> gridcodestxt = new List<int>(splits.Select(var => (int)float.Parse(var)));

          int tscounter=0;
          while (!sr.EndOfStream)
          {
            splits = sr.ReadLine().Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < splits.Count(); j++)
            {
              List<int> grids;

              if (GridcodesIndex.TryGetValue(gridcodestxt[j], out grids))
              {
                float val = float.Parse(splits[j]);
                foreach (var k in grids)
                  gridcodes[k] = val;
              }
            }

            dfsout.WriteItemTimeStep(tscounter, 1, gridcodes);
            tscounter++;
          }
        }

        DFS.Dispose();
        dfsout.Dispose();
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Process p = new Process();
      p.StartInfo.FileName  = "klimagrid_10km_sst.dfs2";
      p.Start();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      Process p = new Process();
      p.StartInfo.FileName = "sample.txt";
      p.Start();

    }
  }
}
