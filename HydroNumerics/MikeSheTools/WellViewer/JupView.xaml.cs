﻿using System;
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
using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.MikeSheTools.WellViewer
{
  /// <summary>
  /// Interaction logic for JupView.xaml
  /// </summary>
  public partial class JupView : Window
  {
    private JupiterViewModel jvm = new JupiterViewModel();

    public static RoutedUICommand AddRemoveWells = new RoutedUICommand("Add/Remove wells", "AddRemoveWells", typeof(JupView));

    public JupView()
    {
      DataContext = jvm;
      InitializeComponent();
      DetailedPlantView.ZoomToTimeScale();

      CommandBinding cb = new CommandBinding(AddRemoveWells, AddRemoveWellsExecute, AddRemoveWellsCanExecute);
      this.CommandBindings.Add(cb);

    }

    private void AddRemoveWellsCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (List.SelectedItem as PlantViewModel != null)
        e.CanExecute = true;
      else
        e.CanExecute = false;     
    }

    private void AddRemoveWellsExecute(object sender, ExecutedRoutedEventArgs e)
    {
      WellsOnPlantView wpv = new WellsOnPlantView();

      WellsOnPlantViewModel vpm = new WellsOnPlantViewModel(jvm.AllWells, List.SelectedItem as PlantViewModel, jvm.JupChanges);
      wpv.DataContext = vpm;
      wpv.ShowDialog();
      e.Handled = true;
    }


 
    
   

    private void LogWindow_SourceUpdated(object sender, DataTransferEventArgs e)
    {
      Scroller.UpdateLayout();
      Scroller.ScrollToBottom();
    }


  }
}