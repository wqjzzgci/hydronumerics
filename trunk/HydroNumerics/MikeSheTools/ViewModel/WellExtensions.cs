using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public static class WellExtensions
  {

    /// <summary>
    /// Returns true if the well has missing data.
    /// x,y==0 or intakes with missing screens
    /// </summary>
    public static bool HasMissingData(this IWell well)
    {
      return well.X == 0 || well.Y == 0 || well.Intakes.Count() == 0 || well.HasScreenErrors();
    }

    private static bool HasScreenErrors(this IWell well)
    {
      return well.Intakes.Sum(var => var.Screens.Count) == 0 || well.Intakes.SelectMany(var => var.Screens).Any(var => var.HasMissingData());
    }


    public static bool CanFixErrors(this IWell well)
    {      
      bool canfix=false;
      if (well.Intakes.Count()>0)
      if (well.HasScreenErrors())
        if (well.Intakes.Any(var=>var.Depth.HasValue) || well.Depth.HasValue)
          canfix = true;
        
        return canfix;
    }

    public static double DefaultScreenLength=2;

    public static string FixErrors(this IWell well)
    {
      StringBuilder Returnstring = new StringBuilder();
      
      if (well.CanFixErrors())
      {
        var screens = well.Intakes.SelectMany(var => var.Screens);
        if (screens.Count() == 0)
        {
          //First intake with depth
          var I = well.Intakes.FirstOrDefault(var => var.Depth.HasValue);
          if (I != null)
          {
            Screen sc = new Screen(I);
            sc.DepthToTop = I.Depth.Value;
            if (well.Depth.HasValue)
              sc.DepthToBottom = well.Depth.Value;
            else
              sc.DepthToBottom = sc.DepthToTop + DefaultScreenLength;
            return "Added new screen at the bottom of Intake number " + I.IDNumber +".";
          }
          else
          {
            Screen sc = new Screen(well.Intakes.First());
            sc.DepthToBottom = well.Depth.Value;
            sc.DepthToTop = Math.Max(0, sc.DepthToBottom.Value - DefaultScreenLength);
            return "Added new screen to Intake number " + well.Intakes.First().IDNumber + " at the bottom of the well.";
          }
        }
        else
        {
          foreach (var sc in screens)
          {
            if (!sc.DepthToBottom.HasValue)
            {
              if (sc.DepthToTop.HasValue)
              {
                sc.DepthToBottom = sc.DepthToTop + DefaultScreenLength;
                Returnstring.AppendLine(String.Format("Bottom of screen number {0} in Intake number {1} was set from top of screen.", sc.Number, sc.Intake.IDNumber));
              }
              else if (well.Depth.HasValue)
              {
                sc.DepthToBottom = well.Depth;
                Returnstring.AppendLine(String.Format("Bottom of screen number {0} in Intake number {1} was set to bottom of well.", sc.Number, sc.Intake.IDNumber));
              }
              else
              {
                sc.DepthToBottom = sc.Intake.Depth;
                Returnstring.AppendLine(String.Format("Bottom of screen number {0} in Intake number {1} was set to bottom of Intake.", sc.Number, sc.Intake.IDNumber));
              }
            }
            if (!sc.DepthToTop.HasValue)
            {
              if (sc.Intake.Depth.HasValue)
              sc.DepthToTop = Math.Max(0, sc.DepthToBottom.Value - DefaultScreenLength);
              Returnstring.AppendLine(String.Format("Top of screen number {0} in Intake number {1} was set from bottom of screen.", sc.Number, sc.Intake.IDNumber));
            }
          }
          return Returnstring.ToString();
        }
      }
      return "Could not fix error";
    }




    /// <summary>
    /// Returns true if one of the depths is missing
    /// </summary>
    public static bool HasMissingData(this Screen _screen)
    {
        return !_screen.DepthToBottom.HasValue || !_screen.DepthToTop.HasValue;
    }

  }
}
