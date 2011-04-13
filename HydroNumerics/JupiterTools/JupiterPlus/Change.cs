using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class Change
  {

    public Change(string Column, string NewValue, string OldValue)
    {
      this.Column = Column;
      this.NewValue = NewValue;
      this.OldValue = OldValue;
    }


    public string Column { get; set; }
    public string NewValue { get; set; }
    public string OldValue { get; set; }
  }
}
