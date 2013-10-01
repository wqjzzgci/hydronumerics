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
using System.Windows.Threading;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using Microsoft.Maps.MapControl.WPF;

using HydroNumerics.MikeSheTools.ViewModel;
using HydroNumerics.MikeSheTools.Mike11;
using HydroNumerics.Geometry;

namespace HydroNumerics.MikeSheTools.Mike11View
{
  /// <summary>
  /// Interaction logic for M11View.xaml
  /// </summary>
  public partial class M11View : Window
  {
    M11Setup m11 = new M11Setup();

    public M11View()
    {
      InitializeComponent();
      DataContext = m11;
      m11.PropertyChanged+=new System.ComponentModel.PropertyChangedEventHandler(m11_PropertyChanged);
    }

   

    /// <summary>
    /// Opens a sim 11 file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

      openFileDialog.Multiselect = false;
      openFileDialog.Filter = "sim 11 files | *.sim11";

      if (openFileDialog.ShowDialog().Value)
      {
        m11.ReadSetup(openFileDialog.FileName);
      }     
    }

    void m11_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "CurrentBranch")
      {
        MyMap.Center = new Location(m11.CurrentBranch.Line.Points.Average(p => ((XYPoint)p).Latitude), m11.CurrentBranch.Line.Points.Average(p => ((XYPoint)p).Longitude));
        RefreshMap();
        ObsGraph.Legend.Visibility = System.Windows.Visibility.Hidden;
        XsecsPlot.Legend.Visibility = System.Windows.Visibility.Hidden;
      }
    }

    private void RefreshMap()
    {
      MyMap.UpdateLayout();
      var c = MyMap.Center;
      c.Latitude += 0.00001;
      MyMap.SetView(c, MyMap.ZoomLevel);
    }


    /// <summary>
    /// Saves to shape
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
      if (dlg.ShowDialog() == true)
      {
        m11.WriteToShape(dlg.FileName);
      }
    }
    

    /// <summary>
    /// Opens the DEM source dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
      DEMSourceDialog dms = new DEMSourceDialog();
      dms.DataContext = m11.DEMConfig;
      dms.ShowDialog();
    }


    List<MapPolyline> ngraphs = new List<MapPolyline>();

    private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      m11.CurrentBranch = e.NewValue as M11Branch;
    }



    private void UpdateView()
    {
      


      if (m11 == null)
        return;

     // var g = new ElementMarkerPointsGraph(m11.CurrentBranch.Profile);
     // g.Marker = new CircleElementPointMarker();

     // var v = ObsGraph.AddLineGraph(m11.CurrentBranch.Profile, new Pen(Brushes.Green, 3), new CircleElementPointMarker
     //{
     //  Size = 10,
     //  Brush = Brushes.Red,
     //  Fill = Brushes.Orange,
     //}
     //        , null);

     // var bl = ObsGraph.AddLineGraph(m11.CurrentBranch.BottomProfileOffset, Colors.Blue, 3);


      //graphs.Add(v.LineGraph);
      //graphs.Add(v.MarkerGraph);
      //graphs.Add(bl);

      RecursiveAdd(m11.CurrentBranch);
    }


    private void RecursiveAdd(M11Branch b)
    {
      foreach (var c in b.UpstreamBranches)
      {

        //Microsoft.Research.DynamicDataDisplay.Charts.Shapes.PolylineEditor P = new Microsoft.Research.DynamicDataDisplay.Charts.Shapes.PolylineEditor();

        // var Polyline = new Microsoft.Research.DynamicDataDisplay.Charts.Shapes.ViewportPolyline();

        //PointCollection pc = new PointCollection(c.ProfileOffset.GetPoints());
        //var f = Polyline.Points.IsFrozen;
        //Polyline.Points = pc;
        //P.Polyline = Polyline;
        //ObsGraph.Children.Add(P);
        //graphs.Add(P);
       // c.ChainageOffset = b.ChainageOffset - b.Branch.ChainageEnd + c.Branch.DownStreamConnection.StartChainage;
       // var g = ObsGraph.AddLineGraph(c.ProfileOffset, new Pen(Brushes.Gray, 2), new CircleElementPointMarker
       // {
       //   Size = 5,
       //   Brush = Brushes.Red,
       //   Fill = Brushes.Orange,
          
       // }
       //, null);

       // var bl = ObsGraph.AddLineGraph(c.BottomProfileOffset, Colors.Black, 2);

        //graphs.Add(g.LineGraph);
        //graphs.Add(g.MarkerGraph);
        //graphs.Add(bl);
        RecursiveAdd(c);

      }
    }


    Dictionary<CrossSection, IPlotterElement> XsecsInPlotter = new Dictionary<CrossSection, IPlotterElement>();
    
    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      foreach (var xsec in e.RemovedItems.Cast<CrossSection>())
        m11.CurrentBranch.SelectedCrossSections.Remove(xsec);

      foreach (var xsec in e.AddedItems.Cast<CrossSection>())
        m11.CurrentBranch.SelectedCrossSections.Add(xsec);
    }

    /// <summary>
    /// Expands all TreeViewItems.
    /// Every time an item is expanded we need to do a non-blocking wait to give WPF a chance to create the
    /// next level of containers. We need to make sure all dispatcher operations at priority Background have
    /// been completed, so we wait for the priority that comes right after Background: ContextIdle.
    /// </summary>
    /// <param name="sender">The Button clicked.</param>
    /// <param name="e">Parameters associated to the Button click.</param>
    private void ExpandAll(object sender, RoutedEventArgs e)
    {
      ApplyActionToAllTreeViewItems(itemsControl =>
      {
        itemsControl.IsExpanded = true;
        WaitForPriority(DispatcherPriority.ContextIdle);
      },
          EndBranchTree);
    }

    /// <summary>
    /// Collapses all TreeViewItems.
    /// There is no need to give back control to WPF (by calling WaitForPriority) in this
    /// case because the TreeViewItems are all created.
    /// Also, any tree traversing order will work. Since we're not giving control back to 
    /// WPF as we collapse the TreeViewItems, WPF does not have a chance to clean them up. So, 
    /// even if we start closing them starting at the top of the tree, the children containers
    /// will be availabe.
    /// </summary>
    /// <param name="sender">The Button clicked.</param>
    /// <param name="e">Parameters associated to the Button click.</param>
    private void CollapseAll(object sender, RoutedEventArgs e)
    {
      ApplyActionToAllTreeViewItems(itemsControl => itemsControl.IsExpanded = false, EndBranchTree);
    }

    /// <summary>
    /// This method collapses only the top level TreeViewItems.
    /// So, if you expand several items, collapse all, and click on a top level TreeViewItem
    /// to expand it manually, the TreeView will remember which items were selected previously
    /// and will restore that state.
    /// </summary>
    /// <param name="sender">The Button clicked.</param>
    /// <param name="e">Parameters associated to the Button click.</param>
    private void CollapseTopLevel(object sender, RoutedEventArgs e)
    {
      // This iterates through the three top-level items only.
      foreach (var item in EndBranchTree.Items)
      {
        TreeViewItem tvi = EndBranchTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
        tvi.IsExpanded = false;
      }
    }

    /// <summary>
    /// Helper method that executes an action for every container in the hierarchy that starts with 
    /// the ItemsControl passed as a parameter. I'm passing an ItemsControl so that this method can be
    /// called with both TreeView and TreeViewItem as parameters.
    /// </summary>
    /// <param name="itemAction">Action to be executed for every item.</param>
    /// <param name="itemsControl">ItemsControl (TreeView or TreeViewItem) at the top of the hierarchy.</param>
    private void ApplyActionToAllTreeViewItems(Action<TreeViewItem> itemAction, ItemsControl itemsControl)
    {
      Stack<ItemsControl> itemsControlStack = new Stack<ItemsControl>();
      itemsControlStack.Push(itemsControl);

      while (itemsControlStack.Count != 0)
      {
        ItemsControl currentItem = itemsControlStack.Pop() as ItemsControl;
        TreeViewItem currentTreeViewItem = currentItem as TreeViewItem;
        if (currentTreeViewItem != null)
        {
          itemAction(currentTreeViewItem);
        }
        if (currentItem != null) // this handles the scenario where some TreeViewItems are already collapsed
        {
          foreach (object dataItem in currentItem.Items)
          {
            ItemsControl childElement = (ItemsControl)currentItem.ItemContainerGenerator.ContainerFromItem(dataItem);
            itemsControlStack.Push(childElement);
          }
        }
      }
    }


    internal static void WaitForPriority(DispatcherPriority priority)
    {
      DispatcherFrame frame = new DispatcherFrame();
      DispatcherOperation dispatcherOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, new DispatcherOperationCallback(ExitFrameOperation), frame);
      Dispatcher.PushFrame(frame);
      if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
      {
        dispatcherOperation.Abort();
      }
    }

    private static object ExitFrameOperation(object obj)
    {
      ((DispatcherFrame)obj).Continue = false;
      return null;
    }

   

    private void BranchGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      foreach (M11Branch b in e.RemovedItems)
        foreach (var CSC in b.CrossSections)
          m11.SelectedCrossSections.Remove((CSC));
      foreach (M11Branch b in e.AddedItems)
      {
        foreach (var CSC in b.CrossSections)
          m11.SelectedCrossSections.Add(CSC);
        m11.CurrentBranch = b;
      }

      m11.GetHeightsCommand.Execute(e.AddedItems);
    }

    private void DeSelectCrossSection(object sender, MouseButtonEventArgs e)
    {
      var tag = ((FrameworkElement)sender).Tag as CrossSection;

      if (tag != null)
        m11.CurrentBranch.SelectedCrossSections.Remove(tag);
      RefreshMap();
    }

    private void SelectCrossSection(object sender, MouseButtonEventArgs e)
    {
      var tag = ((FrameworkElement)sender).Tag as CrossSection;

      if (tag!=null)
        m11.CurrentBranch.SelectedCrossSections.Add(tag);
      RefreshMap();

    }

    private void SelectBranch(object sender, MouseButtonEventArgs e)
    {
      var tag = ((FrameworkElement)sender).Tag as M11Branch;

      m11.CurrentBranch = tag;
      e.Handled = true;

    }

  }
}
