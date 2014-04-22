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
    public event NewMessageEventhandler MessageChanged;

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
    


    protected XElement Configuration;

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
    

    public BaseModel()
    {

    }


    public virtual void ReadConfiguration(XElement Configuration)
    {
      this.Configuration = Configuration;
      Update = Configuration.SafeParseBool("Update") ?? _Update;
      Include = Configuration.SafeParseBool("Include") ?? _Include;
      Name = Configuration.SafeParseString("Name");

      NewMessage("Configuration read.");

    }

    public virtual void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {

    }



  }
}
