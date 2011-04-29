using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ChangesViewModel:BaseViewModel
  {

    private string lastuser
    {
      get
      {
        if (Changes.Count != 0)
          return Changes.Last().User;
        else
          return "UserName";
      }
    }

    private string lastproject
    {
      get
      {
        if (Changes.Count != 0)
          return Changes.Last().Project;
        else
          return "ProjectName";
      }
    }

    private ObservableCollection<ChangeDescription> changes;
    public ObservableCollection<ChangeDescription> Changes
    {
      get
      {
        return changes;
      }
     

    }


    private ChangeController changeController;
    public ChangeController ChangeController
    {
      get
      {
        return changeController;
      }
    }

    public ChangesViewModel(ChangeController ChangeController)
    {
      changeController = ChangeController;
      changes = new ObservableCollection<ChangeDescription>();

    }

    public ObservableCollection<string> DistinctUsers
    {
      get
      {
        ObservableCollection<string> users = new ObservableCollection<string>();
        foreach (string s in Changes.Select(var => var.User).Distinct())
          users.Add(s);
        return users;
      }
    }

    public ObservableCollection<string> DistinctProjects
    {
      get
      {
        ObservableCollection<string> users = new ObservableCollection<string>();

        foreach (string s in Changes.Select(var => var.Project).Distinct())
          users.Add(s);
        return users;
      }
    }  
  }
}
