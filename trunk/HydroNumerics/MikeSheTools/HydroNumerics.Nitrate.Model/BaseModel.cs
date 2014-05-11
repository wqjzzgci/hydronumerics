using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public delegate void NewMessageEventhandler(INitrateModel sender, string Message);

  public class BaseModel : BaseViewModel,INitrateModel
  {

    protected XElement Configuration;


    public BaseModel()
    {

    }


    public virtual void ReadConfiguration(XElement Configuration)
    {
      this.Configuration = Configuration;
      Update = Configuration.SafeParseBool("Update") ?? _Update;
      Include = Configuration.SafeParseBool("Include") ?? _Include;
      Name = Configuration.SafeParseString("Name");

      MultiplicationPar = Configuration.SafeParseDouble("MultiplicationPar") ?? _MultiplicationPar;
      AdditionPar = Configuration.SafeParseDouble("AdditionPar") ?? _AdditionPar;

      NewMessage("Configuration read.");

    }

    public virtual void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {

    }

    public virtual void DebugPrint(string Directory, Dictionary<int, Catchment> Catchments)
    {


    }


    public event NewMessageEventhandler MessageChanged;

    /// <summary>
    /// Add a new message to the log and sends it to all listeners
    /// </summary>
    /// <param name="Message"></param>
    protected void NewMessage(string Message)
    {
      _Log.AppendLine(Message);
      NotifyPropertyChanged("Log");
      if (MessageChanged != null)
      {
        MessageChanged(this, Message);
      }
    }

    private StringBuilder _Log = new StringBuilder();

    /// <summary>
    /// Gets the log
    /// </summary>
    public string Log
    {
      get { return _Log.ToString(); }
    }
    



    private bool _Update= true;
    /// <summary>
    /// Set to false if the model should not update input data but run using values from previous simulation.
    /// Default is true.
    /// </summary>
    public bool Update
    {
      get { return _Update; }
      set
      {
        if (_Update != value)
        {
          _Update = value;
          NotifyPropertyChanged("Update");
        }
      }
    }


    private bool _Include=true;
    /// <summary>
    /// Set to false if the model should not be included in the simulation. Default is false
    /// </summary>
    public bool Include
    {
      get { return _Include; }
      set
      {
        if (_Include != value)
        {
          _Include = value;
          NotifyPropertyChanged("Include");
        }
      }
    }
    


    private double _MultiplicationPar=1.0;
    public double MultiplicationPar
    {
      get { return _MultiplicationPar; }
      set
      {
        if (_MultiplicationPar != value)
        {
          _MultiplicationPar = value;
          NotifyPropertyChanged("MultiplicationPar");
        }
      }
    }

    private double _AdditionPar=0;
    public double AdditionPar
    {
      get { return _AdditionPar; }
      set
      {
        if (_AdditionPar != value)
        {
          _AdditionPar = value;
          NotifyPropertyChanged("AdditionPar");
        }
      }
    }
    
    



  }
}
