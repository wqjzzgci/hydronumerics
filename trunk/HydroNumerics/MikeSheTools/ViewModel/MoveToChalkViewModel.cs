using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class MoveToChalkViewModel:BaseViewModel
  {

    public MoveToChalkViewModel(WellViewModel Wvm, Screen sc)
    {
      Well = Wvm;
      screen = sc;
    }

    public WellViewModel Well { get; private set; }

    public Screen screen { get; private set; }

    public double NewTop { get; set; }

    public double NewBottom { get; set; }

    public void Move()
    {

      if (Well.StatusString != "")
        Well.StatusString += "\n";
      Well.StatusString += "Moved top of screen from " + screen.DepthToTop + " m b.g.s to " + NewTop + " m b.g.s to chalk layer";

      screen.DepthToTop = NewTop;
      screen.DepthToBottom = NewBottom;
    }
  }
}
