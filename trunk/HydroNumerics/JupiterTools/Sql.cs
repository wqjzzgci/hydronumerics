using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using HydroNumerics.Wells;

namespace HydroNumerics.JupiterTools
{
  public class Sql
  {
    JupiterClassesDataContext jc;

    /// <summary>
    /// Constructor that can only be used on Jacobs stationary pc
    /// </summary>
    public Sql()
    {
      jc = new JupiterClassesDataContext();
    }

    public Sql(string connectionString)
    {
      jc = new JupiterClassesDataContext(connectionString);
 
      
    }


    /// <summary>
    /// Not Correc!
    /// </summary>
    /// <param name="KommuneNummer"></param>
    /// <returns></returns>
    public string GetKommuneNavn(int KommuneNummer)
    {

      var navn= from row in jc.CODEs
                 where row.CODE1 == KommuneNummer.ToString()
                 select row.SHORTTEXT;
      return navn.Single();
    }
 
    /// <summary>
    /// Gets the chemistry samples for a particular plant and compound
    /// </summary>
    /// <param name="CompoundNumber"></param>
    /// <param name="P"></param>
    /// <returns></returns>
    public void GetPlantChemistry(int[] CompoundNumber, Plant[] P)
    {
      var chem4 = from row in jc.DRWCHEMSAMPLEs
                  where P.Select(p=>p.IDNumber).Contains(row.PLANTID.Value) & row.SAMPLEDATE.HasValue
                  join row2 in (from row in jc.DRWCHEMANALYSIs where CompoundNumber.Contains(row.COMPOUNDNO) select row) 
                  on row.SAMPLEID equals row2.SAMPLEID
                  join row3 in jc.COMPOUNDLISTs on row2.COMPOUNDNO equals row3.COMPOUNDNO
                  join row4 in (from row in jc.CODEs where row.CODETYPE == 752 select row)
                  on row2.UNIT.Value.ToString() equals row4.CODE1
                  orderby row.SAMPLEDATE
                  select new 
                  {
                    PlantID = row.PLANTID,
                    Amount = row2.AMOUNT.Value,
                    CompoundName = row3.LONG_TEXT,
                    CompoundNo = row3.COMPOUNDNO,
                    Unit = row2.UNIT,
                    UnitString = row4.LONGTEXT,
                    SampleID = row.SAMPLEID,
                    SampleDate = row.SAMPLEDATE.Value,
                    Description = row.SAMPLESITE ?? "" 
                  };

      var v =chem4.ToArray();

      foreach(var p in P)
      {
        var chemp = from row in v where p.IDNumber == row.PlantID 
                  select new ChemistrySample
                  {
                    Amount = row.Amount,
                    CompoundName = row.CompoundName,
                    CompoundNo = row.CompoundNo,
                    Unit = row.Unit,
                    UnitString = row.UnitString,
                    SampleID = row.SampleID,
                    SampleDate = row.SampleDate,
                    Description = row.Description 
                  };

        p.Chemistry = chemp.ToArray();

        }
    }

    /// <summary>
    /// Gets the chemistry samples for particular well and compound
    /// </summary>
    /// <param name="CompoundNumber"></param>
    /// <param name="Well"></param>
    /// <returns></returns>
    public void GetWellChemistry(int[] CompoundNumber, JupiterWell[] Well)
    {

      var chem4 = from row in jc.GRWCHEMSAMPLEs
                  where Well.Select(p => p.ID).Contains(row.BOREHOLENO) & row.SAMPLEDATE.HasValue
                  join row2 in
                    (from row in jc.GRWCHEMANALYSIs where CompoundNumber.Contains(row.COMPOUNDNO) select row)
                  on row.SAMPLEID equals row2.SAMPLEID
                  join row3 in jc.COMPOUNDLISTs on row2.COMPOUNDNO equals row3.COMPOUNDNO
                  join row4 in
                    (from row in jc.CODEs where row.CODETYPE == 752 select row)
                  on row2.UNIT.Value.ToString() equals row4.CODE1
                  orderby row.SAMPLEDATE
                  select new
                  {
                    ID = row.BOREHOLENO,
                    Amount = row2.AMOUNT.Value,
                    CompoundName = row3.LONG_TEXT,
                    CompoundNo = row3.COMPOUNDNO,
                    Unit = row2.UNIT,
                    UnitString = row4.LONGTEXT,
                    SampleID = row.SAMPLEID,
                    SampleDate = row.SAMPLEDATE.Value,
                    Description = row.INTAKENO.ToString()
                  };

      var v = chem4.ToArray();

      foreach (var w in Well)
      {
        var chemp = from row in v
                    where w.ID == row.ID
                    select new ChemistrySample
                    {
                      Amount = row.Amount,
                      CompoundName = row.CompoundName,
                      CompoundNo = row.CompoundNo,
                      Unit = row.Unit,
                      UnitString = row.UnitString,
                      SampleID = row.SampleID,
                      SampleDate = row.SampleDate,
                      Description = row.Description
                    };

        w.ChemSamples.AddRange(chemp);

      }
    }


    /// <summary>
    /// Gets the plant with all wells that are not "P" in USE
    /// </summary>
    /// <param name="Selector"></param>
    /// <returns></returns>
    public Dictionary<int, Plant> ReadPlants(Expression<Func<HydroNumerics.JupiterTools.Linq2Sql.DRWPLANT, bool>> Selector)
    {

      List<Plant> Plants = new List<Plant>();
      Dictionary<int, Plant> DPlants = new Dictionary<int, Plant>();
      IWell CurrentWell = null;
      Plant CurrentPlant = null;
      List<System.Tuple<int, Plant>> SubPlants = new List<System.Tuple<int, Plant>>();

      var PlantQuery = from V1 in jc.DRWPLANTs.Where(Selector)
                       join V2 in jc.DRWPLANTINTAKEs on V1.PLANTID equals V2.PLANTID
                       join V4 in jc.BOREHOLEs on V2.BOREHOLENO equals V4.BOREHOLENO
                       orderby V1.PLANTID
                       select new 
                       {
                         PLANTID = V1.PLANTID,
                         PLANTNAME = V1.PLANTNAME,
                         PLANTADDRESS = V1.PLANTADDRESS,
                         PLANTPOSTALCODE = V1.PLANTPOSTALCODE,
                         PLANTXUTM = V1.XUTM,
                         PLANTYUTM = V1.YUTM,
                         ACTIVE = V1.ACTIVE,

                         PERMITDATE = V1.PERMITDATE,
                         PERMITEXPIREDATE = V1.PERMITEXPIREDATE,
                         PERMITAMOUNT = V1.PERMITAMOUNT,
                         SUPPLANT = V1.SUPPLANT,
                         BOREHOLENO = V2.BOREHOLENO,
                         USE = V4.USE,
                         BX = V4.XUTM,
                         BY = V4.YUTM,
                         INTAKENO = V2.INTAKENO,
                         STARTDATE = V2.STARTDATE,
                         ENDDATE = V2.ENDDATE,
                       };
      int PID = -1;
      foreach (var PlantJoinWell in PlantQuery)
      {
        if (PID != PlantJoinWell.PLANTID)
        {
          PID = PlantJoinWell.PLANTID;
          CurrentPlant = new Plant(PlantJoinWell.PLANTID);
          DPlants.Add(PlantJoinWell.PLANTID, CurrentPlant);
          CurrentPlant.Name = PlantJoinWell.PLANTNAME;
          CurrentPlant.Address = PlantJoinWell.PLANTADDRESS;
          CurrentPlant.Active = PlantJoinWell.ACTIVE ?? -1;

            CurrentPlant.X = PlantJoinWell.PLANTXUTM ?? 0;
            CurrentPlant.Y = PlantJoinWell.PLANTYUTM ?? 0;

            CurrentPlant.PostalCode = PlantJoinWell.PLANTPOSTALCODE ?? 0;

          if (PlantJoinWell.PERMITDATE.HasValue)
            CurrentPlant.PermitDate = PlantJoinWell.PERMITDATE.Value;

          if (PlantJoinWell.PERMITEXPIREDATE.HasValue)
            CurrentPlant.PermitExpiryDate = PlantJoinWell.PERMITEXPIREDATE.Value;

          if (PlantJoinWell.PERMITAMOUNT.HasValue)
            CurrentPlant.Permit = PlantJoinWell.PERMITAMOUNT.Value;

          if (PlantJoinWell.SUPPLANT.HasValue)
            SubPlants.Add(new System.Tuple<int, Plant>(PlantJoinWell.SUPPLANT.Value, CurrentPlant));
        }
        if (PlantJoinWell.USE != "P")
        {
          CurrentWell = CurrentPlant.PumpingWells.FirstOrDefault(var => var.ID.Equals(PlantJoinWell.BOREHOLENO));
          if (CurrentWell == null)
            CurrentWell = new JupiterWell(PlantJoinWell.BOREHOLENO);

          CurrentWell.X = PlantJoinWell.BX ?? 0;
          CurrentWell.Y = PlantJoinWell.BY ?? 0;
          IIntake I = CurrentWell.AddNewIntake(PlantJoinWell.INTAKENO.Value);
          PumpingIntake CurrentPumpingIntake = new PumpingIntake(I, CurrentPlant);
          CurrentPlant.PumpingIntakes.Add(CurrentPumpingIntake);

          CurrentPumpingIntake.StartNullable = PlantJoinWell.STARTDATE;
          CurrentPumpingIntake.EndNullable = PlantJoinWell.ENDDATE;
        }

      }

      //Now attach the subplants
      foreach (System.Tuple<int, Plant> KVP in SubPlants)
      {
        Plant Upper;
        if (DPlants.TryGetValue(KVP.Item1, out Upper))
        {
          Upper.SubPlants.Add(KVP.Item2);
          foreach (PumpingIntake PI in KVP.Item2.PumpingIntakes)
          {
            PumpingIntake d = Upper.PumpingIntakes.FirstOrDefault(var => var.Intake.well.ID == PI.Intake.well.ID);
            //Remove pumping intakes from upper plant if they are attached to lower plants.
            if (d != null)
              Upper.PumpingIntakes.Remove(d);
          }
        }
      }

      var extQuery = from V1 in jc.DRWPLANTs.Where(Selector)
                     join v2 in jc.WRRCATCHMENTs on V1.PLANTID equals v2.PLANTID
                     orderby v2.STARTDATE
                     select new
                     {
                       PLANTID = V1.PLANTID,
                       AMOUNT = v2.AMOUNT,
                       STARTDATE = v2.STARTDATE,
                       ENDDATE = v2.ENDDATE,
                       SURFACEWATERVOLUME = v2.SURFACEWATERVOLUME
                     };


      foreach (var Ext in extQuery)
      {
          if (DPlants.TryGetValue(Ext.PLANTID, out CurrentPlant))
          {
            if (Ext.AMOUNT.HasValue)
              CurrentPlant.Extractions.AddSiValue(Ext.STARTDATE, Ext.ENDDATE, Ext.AMOUNT.Value);
            if (Ext.SURFACEWATERVOLUME.HasValue)
              CurrentPlant.SurfaceWaterExtrations.AddSiValue(Ext.STARTDATE, Ext.ENDDATE, Ext.SURFACEWATERVOLUME.Value);
          }
      }
      return DPlants;
    }
  }
}
