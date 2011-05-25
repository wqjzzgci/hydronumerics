using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  [AttributeUsage(AttributeTargets.Property)]
  public class FixedFormatAttribute:Attribute 
  {
    public int Start{get; private set;}
    public int Width{get; private set;}
    public int Line{get; private set;}

    public FixedFormatAttribute(int Start, int Width)
    {
      this.Start = Start;
      this.Width = Width;
      this.Line = 0;
    }

    public FixedFormatAttribute(int Start, int Width, int Line)
    {
      this.Start = Start;
      this.Width = Width;
      this.Line = Line;
    }

  }
}
