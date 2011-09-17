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
using System.Windows.Shapes;
using System.Windows.Navigation;

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
    private string docfile;

    public static RoutedUICommand AddRemoveWells = new RoutedUICommand("Add/Remove intakes", "AddRemoveWells", typeof(JupView));
    public static RoutedUICommand EditScreensCommand = new RoutedUICommand("Edit screens", "EditScreens", typeof(JupView));
    public static RoutedUICommand AddProjectCommand = new RoutedUICommand("Add project", "AddProject", typeof(JupView));
    public static RoutedUICommand RemoveProjectCommand = new RoutedUICommand("Remove project", "RemoveProject", typeof(JupView));
    public static RoutedUICommand AddUserCommand = new RoutedUICommand("Add user", "AddUser", typeof(JupView));
    public static RoutedUICommand RemoveUserCommand = new RoutedUICommand("Remove user", "RemoveUser", typeof(JupView));
    public static RoutedUICommand RemoveSelectedChanges = new RoutedUICommand("Remove selected changes", "RemoveSelectedChanges", typeof(JupView));
    public static RoutedUICommand EditChangeDesription = new RoutedUICommand("Edit description of selected changes", "EditSelectedChangeDescription", typeof(JupView));
    public static RoutedUICommand ShowDocumentation = new RoutedUICommand("Show documentation", "ShowDocumentationChangeDescription", typeof(JupView));
    public static RoutedUICommand ShowAbout = new RoutedUICommand("About", "ShowAboutBox", typeof(JupView));
    public static RoutedUICommand SavePermits = new RoutedUICommand("Save extraction permits", "SavePermit", typeof(JupView));

    public JupView()
    {
      DataContext = jvm;

      jvm.CVM.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(jvm_PropertyChanged);
      InitializeComponent();

      CommandBinding cb = new CommandBinding(AddRemoveWells, AddRemoveWellsExecute, AddRemoveWellsCanExecute);
      this.CommandBindings.Add(cb);

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

      CommandBinding cb9 = new CommandBinding(EditChangeDesription, EditSelectedChangeExecute, EditSelectedChangeCanExecute);
      this.CommandBindings.Add(cb9);

      CommandBinding cb10 = new CommandBinding(ShowDocumentation, LaunchHelpFile, CanExecuteLaunchHelpFile);
      this.CommandBindings.Add(cb10);

      CommandBinding cb11 = new CommandBinding(ShowAbout, ShowAboutBox, CanShowAboutBox);
      this.CommandBindings.Add(cb11);

      CommandBinding cb12 = new CommandBinding(SavePermits, SavePermitCommand, CanSavePermits);
      this.CommandBindings.Add(cb12);


      //register for unhandled exceptions
      Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

      string fullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

      //get the folder that's in
      string theDirectory = System.IO.Directory.GetParent(System.IO.Path.GetDirectoryName(fullPath)).FullName;

      
      docfile = System.IO.Path.Combine(theDirectory, @"documentation\WellViewer.pdf");

    }

    void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      Exception ex = e.ExceptionObject as Exception;
      MessageBox.Show(ex.InnerException.Message + "\n" + ex.InnerException.Source + "\n" + ex.InnerException.StackTrace + "\n" + ex.InnerException.TargetSite); 
    }

    void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    { 
      MessageBox.Show(e.Exception.Message+ "\n" + e.Exception.Source + "\n" + e.Exception.StackTrace + "\n" + e.Exception.TargetSite);
      e.Handled = true;
    }

    void jvm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName.Equals("NewChange"))
      {
        ChangeMetaDataDialog cmd = new ChangeMetaDataDialog();
        cmd.DataContext = jvm.CVM.CurrentChange;
        cmd.ShowDialog();
      }
    }

    private void ShowAboutBox(object sender, ExecutedRoutedEventArgs e)
    {
      AboutWellViewer aWv = new AboutWellViewer();
      aWv.ShowDialog();
      e.Handled = true;
    }

    private void CanShowAboutBox(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = true;
    }


    private void SavePermitCommand(object sender, ExecutedRoutedEventArgs e)
    {
      WritePermits wp = new WritePermits();
      wp.DataContext = jvm;
      wp.ShowDialog();
      e.Handled = true;
    }

    private void CanSavePermits(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = jvm.SaveExtractionsCommand.CanExecute(null);
    }


    private void LaunchHelpFile(object sender, ExecutedRoutedEventArgs e)
    {
      Process P = new Process();
      P.StartInfo = new ProcessStartInfo(docfile);
      P.Start();
      e.Handled = true;
    }

    private void CanExecuteLaunchHelpFile(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = System.IO.File.Exists(docfile);
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
      WellsOnPlantViewModel vpm = new WellsOnPlantViewModel(jvm.AllWells.Values, List.SelectedItem as PlantViewModel, jvm.CVM);
      wpv.DataContext = vpm;
      wpv.ShowDialog();
      e.Handled = true;
    }

    private void EditWellCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = false;
      if (TheTabs.SelectedIndex == 1)
        if (ListWells.SelectedItem as WellViewModel != null)
        e.CanExecute = true;
      if (TheTabs.SelectedIndex == 0)
        if (List.SelectedItem as WellViewModel != null)
          e.CanExecute = true;
    }


    private void EditScreensExecute(object sender, ExecutedRoutedEventArgs e)
    {
      ScreenAdder EWV = new ScreenAdder();
      WellViewModel wm;

      if (TheTabs.SelectedIndex == 0)
        wm = List.SelectedItem as WellViewModel;
      else
        wm = ListWells.SelectedItem as WellViewModel;

      if (wm != null)
      {
        ScreenAdderViewModel sc =new ScreenAdderViewModel(wm);

        EWV.DataContext = sc;
        EWV.ShowDialog();
      }
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

    private void EditSelectedChangeCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = false;
      if (ChangesGrid.SelectedItems != null)
       if (ChangesGrid.SelectedItems.Count ==1)
          e.CanExecute = true;
    }



    private void EditSelectedChangeExecute(object sender, ExecutedRoutedEventArgs e)
    {
      ChangeMetaDataDialog cmd = new ChangeMetaDataDialog();
      cmd.DataContext = ChangesGrid.SelectedItem;
      cmd.ShowDialog();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }



    
  }
}
