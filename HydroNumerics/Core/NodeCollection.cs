using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using System.Collections.ObjectModel;

namespace HydroNumerics.Core
{
  [DataContract]
  public class NodeCollection<T>:BaseViewModel
  {

    private T _Item;
    [DataMember]
    public T Item
    {
      get { return _Item; }
      set
      {
//        if (_Item != value)
        {
          _Item = value;
          RaisePropertyChanged("Item");
        }
      }
    }
    

    private ObservableCollection<NodeCollection<T>> _Children = new ObservableCollection<NodeCollection<T>>();
    [DataMember]
    public ObservableCollection<NodeCollection<T>> Children
    {
      get { return _Children; }
      set
      {
        if (_Children != value)
        {
          _Children = value;
          RaisePropertyChanged("Children");
        }
      }
    }
    
  }
}
