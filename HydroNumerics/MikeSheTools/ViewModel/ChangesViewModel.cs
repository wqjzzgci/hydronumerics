using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

using HydroNumerics.Wells;
using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ChangesViewModel:BaseViewModel
  {

    public string LastUser
    {
      get
      {
        if (Changes.Count != 0)
          return Changes.Last().User;
        else
          return "UserName";
      }
    }

    public string LastProject
    {
      get
      {
        if (Changes.Count != 0)
          return Changes.Last().Project;
        else
          return "ProjectName";
      }
    }

    // The collection of changes
    private ObservableCollection<ChangeDescriptionViewModel> Changes = new ObservableCollection<ChangeDescriptionViewModel>();

    /// <summary>
    /// Adds a new change
    /// </summary>
    /// <param name="CDVM"></param>
    public void AddChange(ChangeDescriptionViewModel CDVM)
    {
      Changes.Add(CDVM);
    }

    /// <summary>
    /// Gets the collection of selected changes
    /// </summary>
    public ObservableCollection<ChangeDescriptionViewModel> SelectedChanges 
    {
      get
      {
        return Changes;
      }
    }

    
    private ChangeController changeController;
    public ChangeController ChangeController
    {
      get
      {
        if (changeController == null)
          ChangeController = new ChangeController();
        return changeController;
      }
      private set
      {
        if (changeController != value)
        {
          changeController = value;
          NotifyPropertyChanged("ChangeController");
        }
      }

    }

    public IPlantCollection Plants {get;set;}
    public IWellCollection Wells { get; set; }

    public ChangesViewModel()
    {
      Changes = new ObservableCollection<ChangeDescriptionViewModel>();
      this.Plants = Plants;
      this.Wells = Wells;
    }


    public IEnumerable<string> DistinctUsers
    {
      get
      {
        return Changes.Select(var => var.User).Distinct();
      }
    }

    public IEnumerable<string> DistinctProjects
    {
      get
      {
        return Changes.Select(var => var.Project).Distinct();
      }
    }

    #region Commands
    RelayCommand saveCommand;
    RelayCommand loadCommand;
    RelayCommand applyCommand;

    /// <summary>
    /// Gets the command that saves to an xml-file
    /// </summary>
    public ICommand SaveCommand
    {
      get
      {
        if (saveCommand == null)
        {
          saveCommand = new RelayCommand(param => Save(), param => CanSave);
        }
        return saveCommand;
      }
    }

    /// <summary>
    /// Gets the command that saves to an xml-file
    /// </summary>
    public ICommand LoadCommand
    {
      get
      {
        if (loadCommand == null)
        {
          loadCommand = new RelayCommand(param => Load(), param => CanLoad);
        }
        return loadCommand;
      }
    }

    /// <summary>
    /// Gets the command that saves to an xml-file
    /// </summary>
    public ICommand ApplyCommand
    {
      get
      {
        if (applyCommand == null)
        {
          applyCommand = new RelayCommand(param => Apply(), param => CanApply);
        }
        return applyCommand;
      }
    }

    private bool CanSave
    {
      get
      {
        return (Changes != null && Changes.Count > 0);
      }
    }

    private void Save()
    {
      Microsoft.Win32.SaveFileDialog savedialog = new Microsoft.Win32.SaveFileDialog();
      savedialog.Filter = "(*.xml)|*.xml";

      if (savedialog.ShowDialog().Value)
      {
        ChangeController.SaveToFile(SelectedChanges.Select(var => var.changeDescription), savedialog.FileName);
      }
    }

    private bool CanLoad
    {
      get
      {
        return true;
      }
    }

    private void Load()
    {
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
      openFileDialog.Filter = "Known file types (*.xml)|*.xml";
      openFileDialog.Title = "Select an XML file with changes in JupiterPlus format";

      if (openFileDialog.ShowDialog().Value)
      {
        foreach (var c in ChangeController.LoadFromFile(openFileDialog.FileName))
          Changes.Add(new ChangeDescriptionViewModel(c));
        NotifyPropertyChanged("DistinctUsers");
        NotifyPropertyChanged("DistinctProjects");
      }
    }

    private bool CanApply
    {
      get
      {
        return (SelectedChanges.Any(var => var.IsApplied == false));
      }
    }

    private void Apply()
    {
      foreach (var v in SelectedChanges.Where(var=>var.IsApplied ==false))
      {
        v.IsApplied = ChangeController.ApplySingleChange(Plants, Wells, v.changeDescription);
      }
    }

    #endregion

  }
}
