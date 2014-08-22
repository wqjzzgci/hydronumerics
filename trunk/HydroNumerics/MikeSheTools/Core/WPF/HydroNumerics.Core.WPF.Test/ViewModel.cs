using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core.WPF.Test
{
  public class ViewModel:BaseViewModel
  {


    public ViewModel()
    {
      data = new ObservableCollection<DataObject>();
      data.Add(new DataObject() { X = 1.1, Y = 2.2 });
      data.Add(new DataObject() { X = 2.1, Y = 4.2 });
      data.Add(new DataObject() { X = 3.1, Y = 5.2 });
    }

    private ObservableCollection<DataObject> data;

    /// <summary>
    /// Gets and sets Data;
    /// </summary>
    public ObservableCollection<DataObject> Data
    {
      get { return data; }
      set
      {
        if (value != data)
        {
          data = value;
          RaisePropertyChanged("Data");
        }
      }
    }

    


    





  }
}
