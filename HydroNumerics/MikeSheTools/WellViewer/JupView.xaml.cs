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
    public static RoutedUICommand EditWellCommand = new RoutedUICommand("Edit well", "EditWell", typeof(JupView));
    public static RoutedUICommand EditScreensCommand = new RoutedUICommand("Edit screens", "EditScreens", typeof(JupView));
    public static RoutedUICommand AddProjectCommand = new RoutedUICommand("Add project", "AddProject", typeof(JupView));
    public static RoutedUICommand RemoveProjectCommand = new RoutedUICommand("Remove project", "RemoveProject", typeof(JupView));
    public static RoutedUICommand AddUserCommand = new RoutedUICommand("Add user", "AddUser", typeof(JupView));
    public static RoutedUICommand RemoveUserCommand = new RoutedUICommand("Remove user", "RemoveUser", typeof(JupView));
    public static RoutedUICommand RemoveSelectedChanges = new RoutedUICommand("Remove selected changes", "RemoveSelectedChanges", typeof(JupView));

    public JupView()
    {
      DataContext = jvm;
      InitializeComponent();

      CommandBinding cb = new CommandBinding(AddRemoveWells, AddRemoveWellsExecute, AddRemoveWellsCanExecute);
      this.CommandBindings.Add(cb);

      CommandBinding cb2 = new CommandBinding(EditWellCommand, EditWellExecute, EditWellCanExecute);
      this.CommandBindings.Add(cb2);

      CommandBinding cb3 = new CommandBinding(EditScreensCommand, EditScreensExecute, EditWellCanExecute);
      this.CommandBindings.Add(cb3);

      CommandBinding cb4 = new CommandBinding(AddUserCommand, AddUserExecute, AddUserCanExecute);
      this.CommandBindings.Add(cb4);

      CommandBinding cb5 = new CommandBinding(RemoveUserCommand, RemoveUserExecute, RemoveUserCanExecute);
      this.CommandBindings.Add(cb5);

      CommandBinding cb6 = new CommandBinding(AddProjectCommand, AddProjectExecute, AddProjectCanExecute);
      this.CommandBindings.Add(cb6);

      CommandBinding cb7 = new CommandBinding(RemoveProjectCommand, RemoveProjectExecute, RemoveProjectCanExecute);
      this.CommandBindings.Add(cb7);

      CommandBinding cb8 = new CommandBinding(RemoveSelectedChanges, RemoveSelectedChangesExecute, RemoveSelectedChangesCanExecute);
      this.CommandBindings.Add(cb8);

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
      WellsOnPlantViewModel vpm = new WellsOnPlantViewModel(jvm.AllWells, List.SelectedItem as PlantViewModel, jvm.CVM);
      wpv.DataContext = vpm;
      wpv.ShowDialog();
      e.Handled = true;
    }

    private void EditWellCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (ListWells.SelectedItem as WellViewModel != null)
        e.CanExecute = true;
      else
        e.CanExecute = false;
    }


    private void EditScreensExecute(object sender, ExecutedRoutedEventArgs e)
    {
      ScreenAdder EWV = new ScreenAdder();
      EWV.DataContext = ListWells.SelectedItem;
      EWV.ShowDialog();
      e.Handled = true;
    }


    private void EditWellExecute(object sender, ExecutedRoutedEventArgs e)
    {
      EditWellView EWV = new EditWellView();
      EWV.DataContext = ListWells.SelectedItem;
      EWV.ShowDialog();
      e.Handled = true;
    }

    private void AddUserCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = false;
      if (AllUsers.SelectedItem != null)
        if ( !SelectedUsers.Items.Contains(AllUsers.SelectedItem))
          e.CanExecute = true;
    }

    private void AddUserExecute(object sender, ExecutedRoutedEventArgs e)
    {
      jvm.CVM.SelectedUsers.Add(AllUsers.SelectedItem as string);
        e.Handled = true;
    }

    private void RemoveUserCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;
      if (SelectedUsers.SelectedItem != null)
          e.CanExecute = true;
    }

    private void RemoveUserExecute(object sender, ExecutedRoutedEventArgs e)
    {
     jvm.CVM.SelectedUsers.Remove(SelectedUsers.SelectedItem as string); 
      e.Handled = true;
    }

    private void AddProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;
      if (AllProjects.SelectedItem != null)
        if (!SelectedProjects.Items.Contains(AllProjects.SelectedItem))
          e.CanExecute = true;
    }

    private void AddProjectExecute(object sender, ExecutedRoutedEventArgs e)
    {
      jvm.CVM.SelectedProjects.Add(AllProjects.SelectedItem as string);
      e.Handled = true;
    }

    private void RemoveProjectCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;
      if (SelectedProjects.SelectedItem != null)
        e.CanExecute = true;
    }

    private void RemoveProjectExecute(object sender, ExecutedRoutedEventArgs e)
    {
      jvm.CVM.SelectedProjects.Remove(SelectedProjects.SelectedItem as string);
      e.Handled = true;
    }

    private void RemoveSelectedChangesCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (ChangesGrid.SelectedItems != null)
        e.CanExecute = true;
      else
        e.CanExecute = false;
    }

    private void RemoveSelectedChangesExecute(object sender, ExecutedRoutedEventArgs e)
    {

      jvm.CVM.RemoveChanges(ChangesGrid.SelectedItems.Cast<ChangeDescriptionViewModel>());
    }

    private void LogWindow_SourceUpdated(object sender, DataTransferEventArgs e)
    {
      Scroller.UpdateLayout();
      Scroller.ScrollToBottom();
    }

  }
}
