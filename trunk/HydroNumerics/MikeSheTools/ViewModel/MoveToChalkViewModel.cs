using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core.WPF;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class MoveToChalkViewModel:BaseViewModel
  {

    public MoveToChalkViewModel(WellViewModel Wvm, ScreenViewModel sc)
    {
      Well = Wvm;
      screen = sc;
    }

    public WellViewModel Well { get; private set; }

    public ScreenViewModel screen { get; private set; }

    public double NewTop { get; set; }

    public double NewBottom { get; set; }

    public int NewLayer{get;set;}

    public void Move()
    {

      if (Well.StatusString != "")
        Well.StatusString += "\n";
      Well.StatusString += " Moved top of screen from " + screen.TopAsKote + " m b.g.s to " + NewTop + " m b.g.s.";

      screen.TopAsKote = NewTop;
      screen.BottomAsKote = NewBottom;
      screen.NewMsheLayer = NewLayer;
    }
  }
}
