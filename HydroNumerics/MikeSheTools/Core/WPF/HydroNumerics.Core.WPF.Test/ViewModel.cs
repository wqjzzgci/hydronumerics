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
      data = new ObservableCollection<Tuple<double, double>>();
      data.Add(new Tuple<double, double>(1.1, 2.2));
      data.Add(new Tuple<double, double>(2.1, 2.2));
      data.Add(new Tuple<double, double>(3.1, 2.2));
    }

    private ObservableCollection<Tuple<double,double>> data;

    /// <summary>
    /// Gets and sets Data;
    /// </summary>
    public ObservableCollection<Tuple<double, double>> Data
    {
      get { return data; }
      set
      {
        if (value != data)
        {
          data = value;
          NotifyPropertyChanged("Data");
        }
      }
    }

    


    





  }
}
