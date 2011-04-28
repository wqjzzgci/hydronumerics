using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;

using HydroNumerics.JupiterTools.JupiterPlus;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ChangeDescriptionViewModel : BaseViewModel
  {

    public ChangeDescription changeDescription {get; private set;}


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

  

    public ChangeDescriptionViewModel(ChangeDescription changeDescription, IList<ICollection<string>> validComments)
    {
      this.changeDescription = changeDescription;

      ValidComments = validComments;

      if (ValidComments != null)
      {
        foreach (var v in ValidComments)
          changeDescription.Comments.Add(v.First());
      }
      changeDescription.Comments.Add("");
    }


    /// <summary>
    /// Gets the collection of valid comments. This is to be used to restrain the comments to a list of values
    /// </summary>
    public IList<ICollection<string>> ValidComments { get; private set; }


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
        return changeDescription.Comments[0];
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
        return changeDescription.Comments[1];
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

