using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using HydroNumerics.Wells;

namespace HydroNumerics.JupiterTools
{
  [DataContract]
  public class JupiterIntake:Intake,IEquatable<JupiterIntake>, IEqualityComparer<JupiterIntake>
  {
    public DataRow Data { get; set; }
    public string RefPoint { get; set; }

    public JupiterIntake()
    {
    }

    internal JupiterIntake(IWell Well, int IDNumber)
    {
      this.well = Well;
      this.IDNumber = IDNumber;
    }

    /// <summary>
    /// Property to expose screens for use in webservice.
    /// </summary>
    [DataMember]
    private Screen[] ScreensForWeb
    {
      get
      {
        return Screens.ToArray();
      }
    }

    /// <summary>
    /// Returns true if there is no complete screen for this intake
    /// </summary>
    public bool MissingData
    {
      get
      {
        return this.Screens.Count == 0 || Screens.Any(var => var.MissingData);
      }
    }

   
    internal JupiterIntake(JupiterWell Well, IIntake Intake):this(Well, Intake.IDNumber)
    {

      HeadObservations = new HydroNumerics.Time.Core.TimestampSeries(Intake.HeadObservations);
      Extractions = new Time.Core.TimespanSeries(Intake.Extractions);

      foreach (Screen SB in Intake.Screens)
      {
        Screen SBClone = new Screen(this);
        SBClone.DepthToBottom = SB.DepthToBottom;
        SBClone.DepthToTop = SB.DepthToTop;
        SBClone.Number = SB.Number;
      }

      if (Intake is JupiterIntake)
        Data = ((JupiterIntake)Intake).Data;
    }


    #region IEquatable<JupiterIntake> Members

    public bool Equals(JupiterIntake other)
    {
      return base.Equals(other);
    }

    #endregion

    #region IEqualityComparer<JupiterIntake> Members

    public bool Equals(JupiterIntake x, JupiterIntake y)
    {
      return base.Equals(x,y);
    }

    public int GetHashCode(JupiterIntake obj)
    {
      return base.GetHashCode(obj);
    }

    #endregion
  }
}
