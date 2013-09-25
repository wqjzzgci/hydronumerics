using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.Time.Core;

namespace HydroNumerics.JupiterTools
{
  public class Reader:IDisposable
  {

    private JupiterXL JXL;



    public Reader(string DataBaseFile)
    {
      JXL = new JupiterXL(DataBaseFile);        

    }

    /// <summary>
    /// Disposes the dataset and closes the connection to the database.
    /// </summary>
    public void Dispose()
    {
      JXL.Dispose();
    }

    /// <summary>
    /// Read in water levels from a Jupiter access database. 
    /// Entries with blank dates of waterlevels are skipped.
    /// </summary>
    /// <param name="DataBaseFile"></param>
    /// <param name="CreateWells"></param>
    public void Waterlevels(IWellCollection Wells, bool OnlyRo)
    {
        JXL.ReadWaterLevels(OnlyRo);

        foreach (var WatLev in JXL.WATLEVEL)
        {
            //Find the well in the dictionary
            if (Wells.Contains(WatLev.BOREHOLENO))
            {
              IIntake I = Wells[WatLev.BOREHOLENO].Intakes.FirstOrDefault(var => var.IDNumber == WatLev.INTAKENO);
                if (I != null)
                    FillInWaterLevel(I, WatLev);
            }
        }
        JXL.WATLEVEL.Clear();

        foreach (Well W in Wells)
          foreach (Intake I in W.Intakes)
            I.HeadObservations.Sort();
    }
    

    /// <summary>
    /// Put the observation in the well
    /// </summary>
    /// <param name="CurrentWell"></param>
    /// <param name="WatLev"></param>
    private void FillInWaterLevel(IIntake CurrentIntake, JupiterXL.WATLEVELRow WatLev)
    {
      string Description;
      if (WatLev.IsSITUATIONNull())
        Description = "Unknown";
      else
      {
        if (WatLev.SITUATION == 0)
          Description = "Ro";
        else
          Description = "Drift";
      }

      if (!WatLev.IsTIMEOFMEASNull())
        if (!WatLev.IsWATLEVMSLNull())
          CurrentIntake.HeadObservations.Items.Add(new TimestampValue(WatLev.TIMEOFMEAS, WatLev.WATLEVMSL,Description));
        else if (!WatLev.IsWATLEVGRSUNull())
          CurrentIntake.HeadObservations.Items.Add(new TimestampValue(WatLev.TIMEOFMEAS, CurrentIntake.well.Terrain - WatLev.WATLEVGRSU,Description));

    }


    /// <summary>
    /// Read Extractions.
    /// The boolean set dates indicates whether the dates read from the DRWPLANTINTAKE table should be used as Pumpingstart
    /// and pumpingstop.
    /// </summary>
    /// <param name="Plants"></param>
    /// <param name="Wells"></param>
    public IPlantCollection ReadPlants(IWellCollection Wells)
    {
      List<Plant> Plants = new List<Plant>();
      IPlantCollection DPlants = new IPlantCollection();

      JXL.ReadPlantData();

      IIntake CurrentIntake = null;
      Plant CurrentPlant;

      List<Tuple<int, Plant>> SubPlants = new List<Tuple<int, Plant>>();


      foreach (var Anlaeg in JXL.DRWPLANT)
      {
        CurrentPlant = new Plant(Anlaeg.PLANTID);
        DPlants.Add(CurrentPlant);

        if (Anlaeg.IsPLANTNAMENull())
          CurrentPlant.Name = "<no name in database>";
        else
          CurrentPlant.Name = Anlaeg.PLANTNAME;

        CurrentPlant.Address = Anlaeg.PLANTADDRESS;

        CurrentPlant.PostalCode = Anlaeg.PLANTPOSTALCODE;

        if (!Anlaeg.IsSUPPLANTNull())
          CurrentPlant.SuperiorPlantNumber = Anlaeg.SUPPLANT;

         CurrentPlant.NewCommuneNumber = Anlaeg.MUNICIPALITYNO2007;
         CurrentPlant.OldCommuneNumber = Anlaeg.MUNICIPALITYNO;


        var cmp =Anlaeg.GetDRWPLANTCOMPANYTYPERows().LastOrDefault();
        if(cmp !=null)
         CurrentPlant.CompanyType = cmp.COMPANYTYPE;

        if (!Anlaeg.IsPERMITDATENull())
          CurrentPlant.PermitDate = Anlaeg.PERMITDATE;

        if (!Anlaeg.IsPERMITEXPIREDATENull())
          CurrentPlant.PermitExpiryDate = Anlaeg.PERMITEXPIREDATE;

        if (Anlaeg.IsPERMITAMOUNTNull())
          CurrentPlant.Permit = 0;
        else
          CurrentPlant.Permit = Anlaeg.PERMITAMOUNT;

        if (!Anlaeg.IsSUPPLANTNull())
          SubPlants.Add(new Tuple<int, Plant>(Anlaeg.SUPPLANT, CurrentPlant));

        if (!Anlaeg.IsXUTMNull())
          CurrentPlant.X = Anlaeg.XUTM;

        if (!Anlaeg.IsYUTMNull())
          CurrentPlant.Y = Anlaeg.YUTM;


        //Loop the intakes. Only add intakes from wells already in table
        foreach (var IntakeData in Anlaeg.GetDRWPLANTINTAKERows())
        {
          if (Wells.Contains(IntakeData.BOREHOLENO))
          {
            JupiterWell jw = Wells[IntakeData.BOREHOLENO] as JupiterWell;
            CurrentIntake = jw.Intakes.FirstOrDefault(var => var.IDNumber == IntakeData.INTAKENO);
            if (CurrentIntake != null)
            {
              PumpingIntake CurrentPumpingIntake = new PumpingIntake(CurrentIntake, CurrentPlant);
              CurrentPlant.PumpingIntakes.Add(CurrentPumpingIntake);

              if (!IntakeData.IsSTARTDATENull())
                CurrentPumpingIntake.StartNullable = IntakeData.STARTDATE;
              else if (jw.StartDate.HasValue)
                CurrentPumpingIntake.StartNullable = jw.StartDate;
              else if (CurrentIntake.Screens.Where(var => var.StartDate.HasValue).Count() != 0)
                CurrentPumpingIntake.StartNullable = CurrentIntake.Screens.Where(var => var.StartDate.HasValue).Min(var => var.StartDate);

              if (!IntakeData.IsENDDATENull())
                CurrentPumpingIntake.EndNullable = IntakeData.ENDDATE;
              else if (jw.EndDate.HasValue)
                CurrentPumpingIntake.EndNullable = jw.EndDate;
              else if (CurrentIntake.Screens.Where(var => var.EndDate.HasValue).Count() != 0)
                CurrentPumpingIntake.EndNullable = CurrentIntake.Screens.Where(var => var.EndDate.HasValue).Max(var => var.EndDate);
            }
          }
        }
      }
      //Now attach the subplants
      foreach (Tuple<int, Plant> KVP in SubPlants)
      {
        Plant Upper;
        if (DPlants.TryGetValue(KVP.Item1, out Upper))
        {
          if (Upper == KVP.Item2)
          {
            string l = "what";
          }
          else
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
      }

      JXL.DRWPLANT.Dispose();
      JXL.DRWPLANTINTAKE.Dispose();
      return DPlants;
    }


    public int FillInExtractionWithCount(IPlantCollection Plants)
    {
      JXL.ReadExtractionTables();

      Plant CurrentPlant;
      //Loop the extractions
      foreach (var Ext in JXL.WRRCATCHMENT)
      {
        if (Plants.TryGetValue(Ext.PLANTID, out CurrentPlant))
        {
          if (!Ext.IsAMOUNTNull())
            CurrentPlant.Extractions.AddSiValue(Ext.STARTDATE,Ext.ENDDATE, Ext.AMOUNT);
          if (!Ext.IsSURFACEWATERVOLUMENull())
            CurrentPlant.SurfaceWaterExtrations.AddSiValue(Ext.STARTDATE, Ext.ENDDATE, Ext.SURFACEWATERVOLUME);
        }
      }


      //In ribe amt extractions are in another table

      foreach (var IntExt in JXL.INTAKECATCHMENT)
      {
        if (Plants.TryGetValue(IntExt.DRWPLANTINTAKERow.PLANTID, out CurrentPlant))
        {
          //It would be possible to store this on the intake instead of the plant.
          //We are throwing away information!

          if (!IntExt.IsVOLUMENull())
          {
            CurrentPlant.Extractions.AddSiValue(IntExt.STARTDATE, IntExt.ENDDATE, IntExt.VOLUME);
            //if (IntExt.ENDDATE.Year != IntExt.STARTDATE.Year)
            //  throw new Exception("Volume cover period longer than 1 year)");

            //var E = CurrentPlant.Extractions.Items.FirstOrDefault(var => var.StartTime.Year == IntExt.ENDDATE.Year);
            //if (E == null)
            //  CurrentPlant.Extractions.AddSiValue (new TimeSeriesEntry(IntExt.ENDDATE, IntExt.VOLUME));
            //else
            //  E.Value += IntExt.VOLUME;
          }
        }
      }
      int toreturn = JXL.INTAKECATCHMENT.Count + JXL.WRRCATCHMENT.Count;
      JXL.INTAKECATCHMENT.Dispose();
      JXL.WRRCATCHMENT.Dispose();
      return toreturn;
    }



    public IWellCollection ReadWellsInSteps()
    {
      string[] NotExtractionPurpose = new string[] { "A", "G", "I", "J", "L", "R", "U", "M", "P"};
      string[] ExtractionUse = new string[]{"C","V","VA","VD","VH","VI","VM","VP","VV"};
      string[] NotExtractionUse = new string[] { "A", "G", "I", "J", "L", "R", "U", "M", "P"};

      IWellCollection Wells = new IWellCollection();
      JupiterWell CurrentWell;
      JupiterIntake CurrentIntake;

      #region Borehole
      JXL.ReadWellsOnly();
      //Loop the wells
      foreach (var Boring in JXL.BOREHOLE)
      {
        CurrentWell = new JupiterWell(Boring.BOREHOLENO);
        Wells.Add(CurrentWell);

        if (!Boring.IsXUTMNull())
            CurrentWell.X = Boring.XUTM;
          else //If no x set x to 0!
            CurrentWell.X = 0;

          if (!Boring.IsYUTMNull())
            CurrentWell.Y = Boring.YUTM;
          else
            CurrentWell.Y = 0;

          CurrentWell.Description = Boring.LOCATION;
          if (Boring.ELEVATION ==-999 & Boring.CTRPELEVA!=-999)
            CurrentWell.Terrain = Boring.CTRPELEVA;
          else
            CurrentWell.Terrain = Boring.ELEVATION;

        if (!Boring.IsDRILLDEPTHNull())
          CurrentWell.Depth = Boring.DRILLDEPTH;

          CurrentWell.UsedForExtraction = true;

          CurrentWell.Use = Boring.USE;
          CurrentWell.Purpose = Boring.PURPOSE;

        //Hvis USE er noget andet end indvinding
          if (NotExtractionUse.Contains(Boring.USE.ToUpper()))
            CurrentWell.UsedForExtraction = false;

        //Hvis den er oprettet med et andet formål og USE ikke er sat til indvinding er det ikke en indvindingsboring
          if (NotExtractionPurpose.Contains(Boring.PURPOSE.ToUpper()) & !ExtractionUse.Contains(Boring.USE.ToUpper()))
            CurrentWell.UsedForExtraction = false;

          if (!Boring.IsDRILENDATENull())
            CurrentWell.StartDate = Boring.DRILENDATE;
          if (!Boring.IsABANDONDATNull())
            CurrentWell.EndDate = Boring.ABANDONDAT;

      }
      JXL.BOREHOLE.Clear();
      #endregion

      #region Intakes
      //Intakes
      JXL.ReadIntakes();
      foreach (var Intake in JXL.INTAKE)
      {
        if (Wells.Contains(Intake.BOREHOLENO))
        {
          JupiterIntake I = Wells[Intake.BOREHOLENO].AddNewIntake(Intake.INTAKENO) as JupiterIntake;
          if (I != null)
          {
            if (!Intake.IsSTRINGNONull())
            {
              I.StringNo = Intake.STRINGNO;
              I.ResRock = Intake.RESERVOIRROCK;
            }
          }
        }
      }

      foreach( var Casing in JXL.CASING)
      {
        if (Wells.Contains(Casing.BOREHOLENO))
        {
          if (!Casing.IsSTRINGNONull())
          {
            IIntake I = Wells[Casing.BOREHOLENO].Intakes.FirstOrDefault(var => ((JupiterIntake)var).StringNo == Casing.STRINGNO);
            if (I != null)
              if (!Casing.IsBOTTOMNull())
                I.Depth = Casing.BOTTOM;
          }
        }
      }
      JXL.INTAKE.Clear();
      JXL.CASING.Clear();
#endregion

      #region Screens
      //Screens
      JXL.ReadScreens();
      foreach (var screen in JXL.SCREEN)
      {
        if (Wells.Contains(screen.BOREHOLENO))
        {
          CurrentIntake = Wells[screen.BOREHOLENO].Intakes.FirstOrDefault(var => var.IDNumber == screen.INTAKENO) as JupiterIntake;
          if (CurrentIntake != null)
          {
            Screen CurrentScreen = new Screen(CurrentIntake);
            if (!screen.IsTOPNull())
              CurrentScreen.DepthToTop = screen.TOP;
            if (!screen.IsBOTTOMNull())
              CurrentScreen.DepthToBottom = screen.BOTTOM;
            CurrentScreen.Number = screen.SCREENNO;

            if (!screen.IsSTARTDATENull())
              CurrentScreen.StartDate = screen.STARTDATE;

            if (!screen.IsENDDATENull())
              CurrentScreen.EndDate = screen.ENDDATE;
          }
        }
      }
      JXL.SCREEN.Clear();
      #endregion

      return Wells;
    }

    /// <summary>
    /// Reads the lithology and assign to all the JupiterWells in the collection
    /// </summary>
    /// <param name="Wells"></param>
    public void ReadLithology(IWellCollection Wells)
    {
      JupiterWell CurrentWell;
      JXL.ReadInLithology();
      CurrentWell = Wells.FirstOrDefault() as JupiterWell;

      //Loop the lithology
      foreach (var Lith in JXL.LITHSAMP)
      {
        if (CurrentWell.ID == Lith.BOREHOLENO)
        {
          Lithology L = new Lithology();
          L.Bottom = Lith.BOTTOM;
          L.Top = Lith.TOP;
          L.RockSymbol = Lith.ROCKSYMBOL;
          L.RockType = Lith.ROCKTYPE;
          L.TotalDescription = Lith.TOTALDESCR;
          CurrentWell.LithSamples.Add(L);
        }
        else
        {
          if (Wells.Contains(Lith.BOREHOLENO))
          {
            CurrentWell = Wells[Lith.BOREHOLENO] as JupiterWell;
            Lithology L = new Lithology();
            L.Bottom = Lith.BOTTOM;
            L.Top = Lith.TOP;
            L.RockSymbol = Lith.ROCKSYMBOL;
            L.RockType = Lith.ROCKTYPE;
            L.TotalDescription = Lith.TOTALDESCR;
            CurrentWell.LithSamples.Add(L);
          }
        }
      }
      JXL.LITHSAMP.Clear();
    }

    /// <summary>
    /// Reads all water levels and assign them to the wells in the collection
    /// </summary>
    /// <param name="Wells"></param>
    public void ReadWaterLevels(IWellCollection Wells)
    {
      IWell CurrentWell = Wells.FirstOrDefault();
      IIntake CurrentIntake;

      JXL.ReadWaterLevels(false);

      foreach (var WatLev in JXL.WATLEVEL)
      {
        if (CurrentWell.ID == WatLev.BOREHOLENO)
        {
          CurrentIntake = CurrentWell.Intakes.FirstOrDefault(var => var.IDNumber == WatLev.INTAKENO) as JupiterIntake;
          if (CurrentIntake is JupiterIntake)
            ((JupiterIntake)CurrentIntake).RefPoint = WatLev.REFPOINT;
          FillInWaterLevel(CurrentIntake, WatLev);
        }
        else
        {
          if (Wells.Contains(WatLev.BOREHOLENO))
          {
            CurrentIntake = Wells[WatLev.BOREHOLENO].Intakes.FirstOrDefault(var => var.IDNumber == WatLev.INTAKENO) as JupiterIntake;
            if (CurrentIntake is JupiterIntake)
              ((JupiterIntake)CurrentIntake).RefPoint = WatLev.REFPOINT;
            FillInWaterLevel(CurrentIntake, WatLev);
          }
        }
      }
      JXL.WATLEVEL.Clear();
    }

  }
}
