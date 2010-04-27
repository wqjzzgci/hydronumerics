using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class StreamViewModel : WaterBodyViewModel,INotifyPropertyChanged 
  {

    private Stream _stream;

    public StreamViewModel(Stream stream):base(stream)
    {
      _stream = stream;
    }

    /// <summary>
    /// Gets and sets the width
    /// </summary>
    public double Width
    {
      get
      {
        return _stream.Width;
      }
      set
      {
        if (value != _stream.Width)
        {
          _stream.Width = value;
          NotifyPropertyChanged("Width");
          NotifyPropertyChanged("Volume");
        }
      }
    }

    /// <summary>
    /// Gets and sets the length
    /// </summary>
    public double Length
    {
      get
      {
        return _stream.Length;
      }
      set
      {
        _stream.Length = value;
        NotifyPropertyChanged("Length");
        NotifyPropertyChanged("Volume");
      }
    }

    /// <summary>
    /// Gets and sets the depth
    /// </summary>
    public double Depth
    {
      get
      {
        return _stream.Depth;
      }
      set
      {
        _stream.Depth = value;
        NotifyPropertyChanged("Depth");
        NotifyPropertyChanged("Volume");
      }
    }

   
  }
}
