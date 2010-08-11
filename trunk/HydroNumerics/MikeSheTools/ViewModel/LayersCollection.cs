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
    public Model MShe{get;private set;}
    public ObservableCollection<IWell> WellsOutSideModelDomain { get; private set; }
    public ObservableCollection<Screen> ScreensAboveTerrain { get; private set; }
    public ObservableCollection<Screen> ScreensBelowBottom { get; private set; }
    public ObservableCollection<Screen> ScreensWithMissingDepths { get; private set; }

    public IEnumerable<IWell> Wells { get; set; }
    public ObservableCollection<Layer> Layers { get; private set; }
    
    public LayersCollection()
    {
      Wells = new List<IWell>();
      Layers = new ObservableCollection<Layer>();
      WellsOutSideModelDomain = new ObservableCollection<IWell>();
      ScreensAboveTerrain = new ObservableCollection<Screen>();
      ScreensBelowBottom = new ObservableCollection<Screen>();
      ScreensWithMissingDepths = new ObservableCollection<Screen>();
    }

    private void Load(string FileName)
    {
      if (MShe != null)
        MShe.Dispose();

      Layers.Clear();
      WellsOutSideModelDomain.Clear();
      MShe = new Model(FileName);

      //Create the layers
      for (int i = 0; i < MShe.GridInfo.NumberOfLayers; i++)
      {
        Layers.Add(new Layer(i));
        Layers[i].PropertyChanged += new PropertyChangedEventHandler(LayersCollection_PropertyChanged);
      }

      Layers[MShe.GridInfo.NumberOfLayers-1].MoveUp = true;


      //Distribute the intakes
      foreach (IWell W in Wells)
      {
        int col;
        int row;

        if(!MShe.GridInfo.TryGetIndex(W.X,W.Y, out col, out row))
          WellsOutSideModelDomain.Add(W);
        else
        {
          //Well has no terrain. Use model topography
          if (W.Terrain < 0)
            W.Terrain = MShe.GridInfo.SurfaceTopography.Data[row, col];
          
          foreach (IIntake I in W.Intakes)
            foreach (Screen S in I.Screens)
            {
              //Missing screen info
              if (S.TopAsKote < -990 || S.BottomAsKote < -990 || S.DepthToBottom < -990 || S.DepthToTop <-990)
                ScreensWithMissingDepths.Add(S);
              else
              {
                int TopLayer = MShe.GridInfo.GetLayer(col, row, S.TopAsKote);
                int BottomLayer = MShe.GridInfo.GetLayer(col, row, S.BottomAsKote);

                //Above terrain
                if (BottomLayer == -1)
                  ScreensAboveTerrain.Add(S);
                //Below bottom
                else if (TopLayer == -2)
                  ScreensBelowBottom.Add(S);
                else
                {
                  BottomLayer = Math.Max(0, BottomLayer);
                  if (TopLayer == -1)
                    TopLayer = MShe.GridInfo.NumberOfLayers - 1;

                  for (int i = BottomLayer; i <= TopLayer; i++)
                  {
                    //Prevent adding the same intake twice if has two screens in the same layer
                    if (I.Screens.Count > 1)
                    {
                      if (!Layers[i].Intakes.Contains(I))
                        Layers[i].Intakes.Add(I);
                    }
                    else
                      Layers[i].Intakes.Add(I);
                  }
                }
              }
            }
        }
      }
    }

    void LayersCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IntakesAllowed")
      {

      }
    }
    /// <summary>
    /// Gets and sets the MikeSheFileName
    /// </summary>
    public string MikeSheFileName
    {
      get
      {
        if (MShe == null)
          return "";
        return MShe.Files.SheFile;
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
