using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;

using HydroNumerics.Core.WPF;
using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ChangeDescriptionViewModel : BaseViewModel
  {

    public ChangeDescription changeDescription {get; set;}

    private bool isDirty = true;

    //Returns true if the Change needs to be saved
    public bool IsDirty
    {
      get
      {
        return isDirty;
      }
      set
      {
        if (isDirty != value)
        {
          isDirty = value;
          NotifyPropertyChanged("IsDirty");
        }
      }
    }


    private bool isApplied = false;
    public bool IsApplied
    {
      get
      {
        return isApplied;
      }
      set
      {
        if (isApplied != value)
        {
          isApplied = value;
          NotifyPropertyChanged("IsApplied");
        }
      }
    }

    private bool dateOk = true;
    public bool IsDateOk
    {
      get
      {
        return dateOk;
      }
      set
      {
        if (dateOk != value)
        {
          dateOk = value;
          NotifyPropertyChanged("DateOk");
        }
      }
    }

    private bool isFoundInJupiter = true;
    public bool IsFoundInJupiter
    {
      get
      {
        return isFoundInJupiter;
      }
      set
      {
        if (isFoundInJupiter != value)
        {
          isFoundInJupiter = value;
          NotifyPropertyChanged("IsFoundInJupiter");
        }
      }
    }

    private bool isOldValueTheSame = true;
    public bool IsOldValueTheSame
    {
      get
      {
        return isOldValueTheSame;
      }
      set
      {
        if (isOldValueTheSame != value)
        {
          isOldValueTheSame = value;
          NotifyPropertyChanged("IsOldValueTheSame");
        }
      }
    }


    private string description;
    /// <summary>
    /// Gets and sets a description of this change
    /// </summary>
    public string Description
    {
      get
      {
        return description;
      }
      set
      {
        if (description != value)
        {
          description = value;
          NotifyPropertyChanged("Description");
        }
      }
    }


    public ChangeDescriptionViewModel(ChangeDescription changeDescription)
    {
      this.changeDescription = changeDescription;

        foreach (var v in ValidComments)
          changeDescription.Comments.Add(v.First());

      //Make sure there is always a comment since the last one is used as free comment
        changeDescription.Comments.Add("");
    }


    /// <summary>
    /// Gets the collection of valid comments. This is to be used to restrain the comments to a list of values
    /// </summary>
    public IList<ICollection<string>> ValidComments 
    { 
      get
      {
        return JupiterTools.JupiterPlus.ValidComments.GetValidComments(changeDescription);
      } 
    }

    /// <summary>
    /// Gets and sets the comment that is not constrained
    /// </summary>
    public string FreeComment
    {
      get
      {
        return changeDescription.Comments.Last();
      }
      set
      {
        
        if (changeDescription.Comments.Last() != value)
        {
          changeDescription.Comments[changeDescription.Comments.Count -1] = value;
          NotifyPropertyChanged("FreeComment");
        }
      }
    }

    /// <summary>
    /// Gets and sets the first fixed comment. Should only be used if there are fixed comments
    /// </summary>
    public string FirstFixedComment
    {
      get
      {
        if (changeDescription.Comments.Count > 0)
          return changeDescription.Comments[0];
        else
          return "";
      }
      set
      {

        if (changeDescription.Comments[0] != value)
        {
          changeDescription.Comments[0] = value;
          NotifyPropertyChanged("FirstFixedComment");
        }
      }
    }

    /// <summary>
    /// Gets and sets the first fixed comment. Should only be used if there are fixed comments
    /// </summary>
    public string SecondFixedComment
    {
      get
      {
        if (changeDescription.Comments.Count > 1)
          return changeDescription.Comments[1];
        else
          return "";
      }
      set
      {

        if (changeDescription.Comments[1] != value)
        {
          changeDescription.Comments[1] = value;
          NotifyPropertyChanged("SecondFixedComment");
        }
      }
    }

    /// <summary>
    /// Gets and sets the first fixed comment. Should only be used if there are fixed comments
    /// </summary>
    public string ThirdFixedComment
    {
      get
      {
        if (changeDescription.Comments.Count > 2)
          return changeDescription.Comments[2];
        else
          return "";
      }
      set
      {

        if (changeDescription.Comments[2] != value)
        {
          changeDescription.Comments[2] = value;
          NotifyPropertyChanged("SecondFixedComment");
        }
      }
    }


    

    /// <summary>
    /// Gets and sets the User
    /// </summary>
    public string User
    {
      get
      {
        return changeDescription.User;
      }
      set
      {
        if (value != changeDescription.User)
        {
          changeDescription.User = value;
          NotifyPropertyChanged("User");
        }
      }
    }

    /// <summary>
    /// Gets and sets the Project
    /// </summary>
    public string Project
    {
      get
      {
        return changeDescription.Project;
      }
      set
      {
        if (value != changeDescription.Project)
        {
          changeDescription.Project = value;
          NotifyPropertyChanged("Project");
        }
      }
    }

    /// <summary>
    /// Gets and sets the Date
    /// </summary>
    public DateTime Date
    {
      get
      {
        return changeDescription.Date;
      }
      set
      {
        if (value != changeDescription.Date)
        {
          changeDescription.Date = value;
          NotifyPropertyChanged("Date");
        }
      }
    }
  
  
  }

}

