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
  public class JupiterWell:Well 
  {
    private List<Lithology> _lithSamples;
    private List<ChemistrySample> _chemSamples;
      
      /// <summary>
      /// Adds a new JupiterIntake to the well
      /// </summary>
      /// <param name="IDNumber"></param>
      /// <returns></returns>
    public override IIntake AddNewIntake(int IDNumber)
    {
      JupiterIntake JI = new JupiterIntake(this, IDNumber);
      _intakes.Add(JI);
      return JI;
    }

    private IIntake AddNewIntake(IIntake intake)
    {
      JupiterIntake Ji = new JupiterIntake(this, intake);
      _intakes.Add(Ji);
      return Ji;
    }

    [DataMember]
    private JupiterIntake[] IntakesA
    {
      get
      {
        JupiterIntake[] arr = new JupiterIntake[_intakes.Count];
        for (int i = 0; i < _intakes.Count; i++)
        {
          arr[i] = (JupiterIntake)_intakes[i];
        }
        return arr;
      }
    }

    [DataMember]
    public List<Lithology> LithSamples
    {
      get 
      {
          if (_lithSamples == null)
              _lithSamples = new List<Lithology>();
          return _lithSamples; }
    }

    [DataMember]
    public List<ChemistrySample> ChemSamples
    {
      get 
      {
          if (_chemSamples == null)
              _chemSamples = new List<ChemistrySample>();
          return _chemSamples; }
    }



        #region Constructors
  
    public JupiterWell(string ID)
      : base(ID)
    {
    }

    public JupiterWell(string ID, double UTMX, double UTMY)
      : base(ID, UTMX, UTMY)
    {
    }

    /// <summary>
    /// Constructs a new JupiterWell based on the other Well. Also construct new JupiterIntakes;
    /// </summary>
    /// <param name="Well"></param>
    public JupiterWell(IWell Well):base(Well.ID,Well.X,Well.Y)
    {
      this.Description = Well.Description;
      this.Terrain = Well.Terrain;

      foreach (IIntake I in Well.Intakes)
      {
         this.AddNewIntake(I);
      }
    }

    #endregion


  }
}
