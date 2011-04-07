using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.Time.Core;


namespace HydroNumerics.JupiterTools
{
  public class SQLServerReader
  {
    JupiterXLTablesDataContext JXL = new JupiterXLTablesDataContext();

    /// <summary>
    /// Returns all wells with the postalnummer. Putting null in the array includes all wells without postal number
    /// </summary>
    /// <param name="PostalNumbers"></param>
    /// <returns></returns>
    public Dictionary<string, IWell> Wells(int?[] PostalNumbers)
    {
      Expression<Func<BOREHOLE, bool>> f;
      if (PostalNumbers.Contains(null)) //using contains on null did not work, therefore it was necessary to split the functions
        f = var=>PostalNumbers.Contains(var.BORHPOSTC) || !var.BORHPOSTC.HasValue;
      else
        f = var=>PostalNumbers.Contains(var.BORHPOSTC);
      return Wells(f);
    }

    /// <summary>
    /// Return all wells in the database
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, IWell> Wells()
    {
      //Always true
      Expression<Func<BOREHOLE, bool>> f = var => true;
      return Wells(f);
    }

    /// <summary>
    /// Returns the well with the ID. DGU nr. Must contain all spaces.
    /// </summary>
    /// <param name="WellID"></param>
    /// <returns></returns>
    public IWell Well(string WellID)
    {
      Expression<Func<BOREHOLE,bool>> f = var=> var.BOREHOLENO.Equals(WellID);
      return Wells(f).Values.FirstOrDefault();
    }


    /// <summary>
    /// Reads in wells from a Jupiter database based on the expression selector. 
    /// Only reads geographical information and location of Intakes and screen
    /// </summary>
    /// <param name="DataBaseFile"></param>
    private Dictionary<string, IWell> Wells(Expression<Func<BOREHOLE,bool>> Selector)
    {     
      //The query makes a left outer join on Boreholes, Intakes and Screens
      var query = from B in JXL.BOREHOLEs.Where(Selector)
                  join I in JXL.INTAKEs
                  on B.BOREHOLENO equals I.BOREHOLENO into sr
                  from x in sr.DefaultIfEmpty()
                  join S in JXL.SCREENs on B.BOREHOLENO equals S.BOREHOLENO into sr2
                  from xx in sr2.DefaultIfEmpty()
                  orderby B.BOREHOLENO, x.INTAKENO
                  select new
                  {
                    BOREHOLENO = B.BOREHOLENO,
                    INTAKENO = x.INTAKENO == null ? -1 : x.INTAKENO,
                    XUTM = B.XUTM,
                    YUTM = B.YUTM,
                    LOCATION = B.LOCATION,
                    ELEVATION = B.ELEVATION,
                    TOP = xx.TOP,
                    BOTTOM = xx.BOTTOM,
                    SCREENNO = xx.SCREENNO == null ? -1 : xx.SCREENNO
                  };

      Dictionary<string, IWell> Wells = new Dictionary<string, IWell>();
      JupiterWell CurrentWell = null;
      IIntake CurrentIntake;

      string BoreHoleID = "";
      //Loop the wells
      foreach (var Boring in query)
      {
        if (BoreHoleID != Boring.BOREHOLENO)
        {
          BoreHoleID = Boring.BOREHOLENO;
          CurrentWell = new JupiterWell(Boring.BOREHOLENO);
          Wells.Add(CurrentWell.ID, CurrentWell);

          if (Boring.XUTM.HasValue)
            CurrentWell.X = Boring.XUTM.Value;
          if (Boring.YUTM.HasValue)
            CurrentWell.Y = Boring.YUTM.Value;

          CurrentWell.Description = Boring.LOCATION;
          if (Boring.ELEVATION.HasValue)
            CurrentWell.Terrain = Boring.ELEVATION.Value;
        }
        //If we have intake data
        if (Boring.INTAKENO != -1)
        {
          CurrentIntake = CurrentWell.Intakes.FirstOrDefault(var => var.IDNumber == Boring.INTAKENO);
          if (CurrentIntake == null)
            CurrentIntake = CurrentWell.AddNewIntake(Boring.INTAKENO);

          //If we have screen data
          if (Boring.SCREENNO != -1)
          {
            Screen CurrentScreen = new Screen(CurrentIntake);
            if (Boring.TOP.HasValue)
              CurrentScreen.DepthToTop = Boring.TOP.Value;
            if (Boring.BOTTOM.HasValue)
              CurrentScreen.DepthToBottom = Boring.BOTTOM.Value;
            CurrentScreen.Number = Boring.SCREENNO;
          }
        }
      }
      return Wells;
    }

    /// <summary>
    /// Adds chemistry information to the JupiterWells in the collection
    /// </summary>
    /// <param name="wells"></param>
    public void AddChemistry(Dictionary<string, IWell> wells)
    {
      var ChemQuery = from C in JXL.GRWCHEMSAMPLEs
                      join CO in JXL.GRWCHEMANALYSIs
                      on C.SAMPLEID equals CO.SAMPLEID into J1
                      from s in J1
                      join CL in JXL.COMPOUNDLISTs
                          on s.COMPOUNDNO equals CL.COMPOUNDNO into J2
                      from t in J2.DefaultIfEmpty()
                      orderby C.BOREHOLENO
                      select new
                      {
                        BOREHOLENO = C.BOREHOLENO,
                        SAMPLEDATE = C.SAMPLEDATE,
                        COMPOUNDNO = s.COMPOUNDNO,
                        AMOUNT = s.AMOUNT,
                        UNIT = s.UNIT,
                        LONG_TEXT = t.LONG_TEXT
                      };
      IWell CurrentWell = null;

      //This string is used because the query is ordered by well number and therefore we do not need to look up a well
      //for every result.
      string ID = "";

      foreach (var Chem in ChemQuery)
      {
        //If we have a new well find it.
        if (ID != Chem.BOREHOLENO)
        {
          ID = Chem.BOREHOLENO;
          wells.TryGetValue(ID, out CurrentWell);
        }

        if (CurrentWell != null & CurrentWell is JupiterWell)
        {
          ChemistrySample C = new ChemistrySample();
          if (Chem.SAMPLEDATE.HasValue)
            C.SampleDate = Chem.SAMPLEDATE.Value;
          C.CompoundNo = Chem.COMPOUNDNO;
          if (Chem.AMOUNT.HasValue)
            C.Amount = Chem.AMOUNT.Value;
          if (Chem.UNIT.HasValue)
            C.Unit = Chem.UNIT.Value;
          C.CompoundName = Chem.LONG_TEXT;
          ((JupiterWell)CurrentWell).ChemSamples.Add(C);
        }
      }
    }

    /// <summary>
    /// Reads in the lithology and adds it to the wells in the dictionary.
    /// Note the wells has to be JupiterWells
    /// </summary>
    /// <param name="wells"></param>
    public void AddLithology(Dictionary<string, IWell> wells)
    {
      //Makes a query for every well. The number has been found in a stopWatch test
      if (wells.Count < 2000)
      {
        foreach (KeyValuePair<string, IWell> KVP in wells)
        {
          var LithQuery = from L in JXL.LITHSAMPs
                          where L.BOREHOLENO == KVP.Key
                          select new LithResult()
                          {
                            BOREHOLENO = L.BOREHOLENO,
                            BOTTOM = L.BOTTOM,
                            TOP = L.TOP,
                            ROCKSYMBOL = L.ROCKSYMBOL,
                            ROCKTYPE = L.ROCKTYPE,
                            TOTALDESCR = L.TOTALDESCR
                          };
          foreach (var Lith in LithQuery)
            FillInLithology(KVP.Value, Lith);
        }
      }
      //Loads all the entries in the LithSamp-table.
      else
      {
        var LithQuery = from L in JXL.LITHSAMPs
                        orderby L.BOREHOLENO
                        select new LithResult()
                        {
                          BOREHOLENO = L.BOREHOLENO,
                          BOTTOM = L.BOTTOM,
                          TOP = L.TOP,
                          ROCKSYMBOL = L.ROCKSYMBOL,
                          ROCKTYPE = L.ROCKTYPE,
                          TOTALDESCR = L.TOTALDESCR
                        };


        IWell CurrentWell = null;

        //This string is used because the query is ordered by well number and therefore we do not need to look up a well
        //for every result.
        string ID = "";

        //Loop the lithology
        foreach (var Lith in LithQuery)
        {
          //If we have a new well find it.
          if (ID != Lith.BOREHOLENO)
          {
            ID = Lith.BOREHOLENO;
            wells.TryGetValue(ID, out CurrentWell);
          }
          FillInLithology(CurrentWell, Lith);
        }
      }
    }


    /// <summary>
    /// Reads the waterlevels for the particular well
    /// </summary>
    /// <param name="well"></param>
    public void Waterlevels(IWell well)
    {
      Dictionary<string, IWell> d = new Dictionary<string,IWell>();
      d.Add(well.ID,well);
      Waterlevels(d);
    }


    /// <summary>
    /// Reads in waterlevels and attach them to the wells in the dicitionary.
    /// </summary>
    /// <param name="wells"></param>
    public void Waterlevels(Dictionary<string, IWell> wells)
    {
      //Makes a query for every well. The number has been found in StopWatch test
      if (wells.Count < 4000)
      {
        foreach (KeyValuePair<string, IWell> KVP in wells)
        {
          //The query.
          var SingleQ = from l in JXL.WATLEVELs
                        where l.BOREHOLENO == KVP.Key
                        select new WatLevResult()
                        {
                          BOREHOLENO = l.BOREHOLENO,
                          INTAKENO = l.INTAKENO,
                          TIMEOFMEAS = l.TIMEOFMEAS,
                          WATLEVMSL = l.WATLEVMSL,
                          WATLEVGRSU = l.WATLEVGRSU
                        };
          foreach (WatLevResult WatLev in SingleQ)
            FillInWaterLevel(KVP.Value, WatLev);
        }
      }
      //Reads all water levels and loops them
      else
      {
        //The query.
        var q = from l in JXL.WATLEVELs
                orderby l.BOREHOLENO
                select new WatLevResult()
                {
                  BOREHOLENO = l.BOREHOLENO,
                  INTAKENO = l.INTAKENO,
                  TIMEOFMEAS = l.TIMEOFMEAS,
                  WATLEVMSL = l.WATLEVMSL,
                  WATLEVGRSU = l.WATLEVGRSU
                };

        IWell CurrentWell = null;

        //This string is used because the query is ordered by well number and therefore we do not need to look up a well
        //for every result.
        string ID = "";

        foreach (WatLevResult WatLev in q)
        {
          //If we have a new well find it.
          if (ID != WatLev.BOREHOLENO)
          {
            ID = WatLev.BOREHOLENO;
            wells.TryGetValue(ID, out CurrentWell);
          }
          FillInWaterLevel(CurrentWell, WatLev);
        }
      }
    }


    public Dictionary<int, Plant> ReadPlants()
    {
      //Always true
      Expression<Func<DRWPLANT, bool>> f = var => true;
      return ReadPlants(f);

    }

    /// <summary>
    /// Read Extractions.
    /// </summary>
    /// <param name="Plants"></param>
    /// <param name="Wells"></param>
    private Dictionary<int, Plant> ReadPlants(Expression<Func<DRWPLANT, bool>> Selector)
    {
      List<Plant> Plants = new List<Plant>();
      Dictionary<int, Plant> DPlants = new Dictionary<int, Plant>();
      IWell CurrentWell = null;
      IIntake CurrentIntake = null;
      Plant CurrentPlant = null;
      List<Tuple<int, Plant>> SubPlants = new List<Tuple<int, Plant>>();

      var PlantQuery = from V1 in JXL.DRWPLANTs.Where(Selector)
                       join V2 in JXL.DRWPLANTINTAKEs on V1.PLANTID equals V2.PLANTID into J1
                       from V3 in J1
                       orderby V1.PLANTID
                       select new
                       {
                         PLANTID = V1.PLANTID,
                         PLANTNAME = V1.PLANTNAME,
                         PLANTADDRESS = V1.PLANTADDRESS,
                         PLANTPOSTALCODE = V1.PLANTPOSTALCODE,
                         PERMITDATE = V1.PERMITDATE,
                         PERMITEXPIREDATE = V1.PERMITEXPIREDATE,
                         PERMITAMOUNT = V1.PERMITAMOUNT,
                         SUPPLANT = V1.SUPPLANT,
                         BOREHOLENO = V3.BOREHOLENO,
                         INTAKENO = V3.INTAKENO,
                         STARTDATE = V3.STARTDATE,
                         ENDDATE = V3.ENDDATE,
                       };
      int PID = -1;
      foreach (var Anlaeg in PlantQuery)
      {
        if (PID != Anlaeg.PLANTID)
        {
          PID = Anlaeg.PLANTID;
          CurrentPlant = new Plant(Anlaeg.PLANTID);
          DPlants.Add(Anlaeg.PLANTID, CurrentPlant);
          CurrentPlant.Name = Anlaeg.PLANTNAME;
          CurrentPlant.Address = Anlaeg.PLANTADDRESS;

          if (Anlaeg.PLANTPOSTALCODE.HasValue)
            CurrentPlant.PostalCode = Anlaeg.PLANTPOSTALCODE.Value;

          if (Anlaeg.PERMITDATE.HasValue)
            CurrentPlant.PermitDate = Anlaeg.PERMITDATE.Value;

          if (Anlaeg.PERMITEXPIREDATE.HasValue)
            CurrentPlant.PermitExpiryDate = Anlaeg.PERMITEXPIREDATE.Value;

          if (Anlaeg.PERMITAMOUNT.HasValue)
            CurrentPlant.Permit = Anlaeg.PERMITAMOUNT.Value;

          if (Anlaeg.SUPPLANT.HasValue)
            SubPlants.Add(new Tuple<int, Plant>(Anlaeg.SUPPLANT.Value, CurrentPlant));
        }
        CurrentWell = CurrentPlant.PumpingWells.FirstOrDefault(var => var.ID.Equals(Anlaeg.BOREHOLENO));
        if (CurrentWell == null)
          CurrentWell = new JupiterWell(Anlaeg.BOREHOLENO);

        IIntake I = CurrentWell.AddNewIntake(Anlaeg.INTAKENO.Value);
        PumpingIntake CurrentPumpingIntake = new PumpingIntake(I, CurrentPlant);
        CurrentPlant.PumpingIntakes.Add(CurrentPumpingIntake);

        CurrentPumpingIntake.Start = Anlaeg.STARTDATE ?? DateTime.MinValue;
        CurrentPumpingIntake.End = Anlaeg.ENDDATE ?? DateTime.MaxValue;

      }

      //Now attach the subplants
      foreach (Tuple<int, Plant> KVP in SubPlants)
      {
        Plant Upper;
        if (DPlants.TryGetValue(KVP.First, out Upper))
        {
          Upper.SubPlants.Add(KVP.Second);
          foreach (PumpingIntake PI in KVP.Second.PumpingIntakes)
          {
            PumpingIntake d = Upper.PumpingIntakes.FirstOrDefault(var => var.Intake.well.ID == PI.Intake.well.ID);
            //Remove pumping intakes from upper plant if they are attached to lower plants.
            if (d != null)
              Upper.PumpingIntakes.Remove(d);
          }
        }
      }

      foreach (var Ext in JXL.WRRCATCHMENTs)
      {
        if (Ext.PLANTID.HasValue)
          if (DPlants.TryGetValue(Ext.PLANTID.Value, out CurrentPlant))
          {
            if (Ext.AMOUNT.HasValue)
              CurrentPlant.Extractions.AddSiValue(Ext.STARTDATE, Ext.ENDDATE, Ext.AMOUNT.Value);
            if (Ext.SURFACEWATERVOLUME.HasValue)
              CurrentPlant.SurfaceWaterExtrations.AddSiValue(Ext.STARTDATE, Ext.ENDDATE, Ext.SURFACEWATERVOLUME.Value);
          }
      }
      return DPlants;
    }




    #region Private Methods
    /// <summary>
    /// Fills in the water level into an observationEntry in the correct Intake
    /// If the well is null or the intake is not there, nothing is done.
    /// </summary>
    /// <param name="CurrentWell"></param>
    /// <param name="WatLev"></param>
    private void FillInWaterLevel(IWell CurrentWell, WatLevResult WatLev)
    {
      IIntake CurrentIntake;

      if (CurrentWell != null)
      {
        //Gets the intake
        CurrentIntake = CurrentWell.Intakes.FirstOrDefault(var => var.IDNumber == WatLev.INTAKENO);
        if (CurrentIntake != null)
        {
          //checks that there is a data
          if (WatLev.TIMEOFMEAS.HasValue)
            //First use the WATLEVMSL if it is there
            if (WatLev.WATLEVMSL.HasValue)
              CurrentIntake.HeadObservations.Items.Add(new TimestampValue(WatLev.TIMEOFMEAS.Value, WatLev.WATLEVMSL.Value));
            //then use the WATLEVGRSU. This relates the water table to well terrain.
            else if (WatLev.WATLEVGRSU.HasValue)
              CurrentIntake.HeadObservations.Items.Add(new TimestampValue(WatLev.TIMEOFMEAS.Value, CurrentIntake.well.Terrain - WatLev.WATLEVGRSU.Value));
        }
      }
    }

    /// <summary>
    /// Fills in the lithology in the current well if the well is a JupiterWell
    /// </summary>
    /// <param name="CurrentWell"></param>
    /// <param name="Lith"></param>
    private void FillInLithology(IWell CurrentWell, LithResult Lith)
    {
      if (CurrentWell != null & CurrentWell is JupiterWell)
      {
        Lithology L = new Lithology();
        if (Lith.BOTTOM.HasValue)
          L.Bottom = Lith.BOTTOM.Value;
        if (Lith.TOP.HasValue)
          L.Top = Lith.TOP.Value;
        L.RockSymbol = Lith.ROCKSYMBOL;
        L.RockType = Lith.ROCKTYPE;
        L.TotalDescription = Lith.TOTALDESCR;
        ((JupiterWell)CurrentWell).LithSamples.Add(L);
      }
    }
    #endregion

    #region Private result classes
    /// <summary>
    /// Small class to hold the results of the queries into the WATLEVEL table.
    /// </summary>
    private class WatLevResult
    {
      public string BOREHOLENO { get; set; }
      public int? INTAKENO { get; set; }
      public DateTime? TIMEOFMEAS { get; set; }
      public double? WATLEVMSL { get; set; }
      public double? WATLEVGRSU { get; set; }
    }

    /// <summary>
    /// Small class to hold results from queries into the LITHSAMP table
    /// </summary>
    private class LithResult
    {
      public string BOREHOLENO { get; set; }
      public double? BOTTOM { get; set; }
      public double? TOP { get; set; }
      public string ROCKSYMBOL { get; set; }
      public string ROCKTYPE { get; set; }
      public string TOTALDESCR { get; set; }
    }
    #endregion
  }
}
