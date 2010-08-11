using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class LayersCollection:INotifyPropertyChanged
  {
    private Model _mshe;
    private int _wellsOutSideModel;

    public IEnumerable<IWell> Wells { get; set; }


    public ObservableCollection<Layer> Layers = new ObservableCollection<Layer>();


    public int WellsOutSideModel
    {
      get
      {
        return _wellsOutSideModel;
      }
      set
      {
        if (value != _wellsOutSideModel)
        {
          _wellsOutSideModel = value;
          NotifyPropertyChanged("WellsOutSideModel");
        }
      }
    }

    public LayersCollection()
    {
      Wells = new List<IWell>();
    }

    private void Load(string FileName)
    {
      if (_mshe != null)
        _mshe.Dispose();

      Layers.Clear();
      _wellsOutSideModel = 0;
      _mshe = new Model(FileName);

      for (int i = 0; i < _mshe.GridInfo.NumberOfLayers; i++)
      {
        Layers.Add(new Layer(i));
      }
      foreach (IWell W in Wells)
      {
        int col = _mshe.GridInfo.GetColumnIndex(W.X);
        int row = _mshe.GridInfo.GetRowIndex(W.Y);

        if (col > 0 & row > 0)
        {
          foreach (IIntake I in W.Intakes)
            foreach (Screen S in I.Screens)
            {
              int TopLayer = _mshe.GridInfo.GetLayer(col, row, S.TopAsKote);
              int BottomLayer = _mshe.GridInfo.GetLayer(col, row, S.BottomAsKote);

              for (int i = BottomLayer; i <= TopLayer; i++)
              {
                //ToDo check GridCode! Check if Column and Row can be used.
                Layers[i].Wells.Add(W);
              }
            }
        }
        else
          _wellsOutSideModel++;
      }
    }
    /// <summary>
    /// Gets and sets the MikeSheFileName
    /// </summary>
    public string MikeSheFileName
    {
      get
      {
        if (_mshe == null)
          return "";
        return _mshe.Files.SheFile;
      }
      set
      {
        Load(value);
        NotifyPropertyChanged("MikeSheFileName");
      }
    }






    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }


    #endregion
  }
}
