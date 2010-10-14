using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.JupiterPlus
{
  public class Change<T>
  {
    public string User { get; set; }
    public string Project { get; set; }
    public DateTime Date { get; set; }
    public T NewValue { get; set; }
    public T OldValue { get; set; }
  }
}
