using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroNumerics.Core;
using HydroNumerics.Nitrate.Model;


namespace HydroNumerics.Nitrate.View
{
  public class ViewModel:BaseViewModel
  {

    public ViewModel()
    {
      TheModel = new MainModel();
      TheModel.ReadConfiguration(@"F:\Oplandsmodel\NitrateModel\config_clgw122_Rerun11.xml");

      SubModels = new SmartCollection<BaseModel>();
      SubModels.AddRange(TheModel.SourceModels.Cast<BaseModel>());
      SubModels.AddRange(TheModel.InternalReductionModels.Cast<BaseModel>());
      SubModels.AddRange(TheModel.MainStreamRecutionModels.Cast<BaseModel>());
      TheModel.LoadCatchments();

    }


    private MainModel _TheModel;
    public MainModel TheModel
    {
      get { return _TheModel; }
      set
      {
        if (_TheModel != value)
        {
          _TheModel = value;
          RaisePropertyChanged("TheModel");
        }
      }
    }


    private SmartCollection<BaseModel> _SubModels;
    public SmartCollection<BaseModel> SubModels
    {
      get { return _SubModels; }
      set
      {
        if (_SubModels != value)
        {
          _SubModels = value;
          RaisePropertyChanged("SubModels");
        }
      }
    }
    
    


  }
}
