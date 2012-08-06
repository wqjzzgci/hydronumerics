using HydroNumerics.JupiterTools.JupiterXLTableAdapters;

namespace HydroNumerics.JupiterTools
{
  public partial class JupiterXL
  {
    private string ConnectionString;

    public JupiterXL(string DataBaseFileName):this()
    {
      ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFileName + ";Persist Security Info=False";
      //Appears to make it more threadsafe
      this.EnforceConstraints = false;
    }


    public void ReadWellsOnly()
    {
      //Read in boreholes through table adapter
      BOREHOLETableAdapter BTA = new BOREHOLETableAdapter();
      BTA.Connection.ConnectionString = ConnectionString;
      BTA.Fill(BOREHOLE);
      BTA.Dispose();
    }

    /// <summary>
    /// Reads intakes and casings table
    /// </summary>
    public void ReadIntakes()
    {
      //Read in Intakes through table adapter
      INTAKETableAdapter ITA = new INTAKETableAdapter();
      ITA.Connection.ConnectionString = ConnectionString;
        ITA.Fill(INTAKE);
        ITA.Dispose();

        CASINGTableAdapter CTA = new CASINGTableAdapter();
        CTA.Connection.ConnectionString = ConnectionString;
        CTA.Fill(CASING);
        CTA.Dispose();
    }

    public void ReadScreens()
    {
      //Read in Screens throug the table adapter
      SCREENTableAdapter STA = new SCREENTableAdapter();
      STA.Connection.ConnectionString = ConnectionString;
      STA.Fill(SCREEN);
      STA.Dispose();
    }

 
    /// <summary>
    /// Read the data from the Lithsample table.
    /// </summary>
    /// <param name="DataBaseFileName"></param>
    public void ReadInLithology()
    {
      LITHSAMPTableAdapter LTA = new LITHSAMPTableAdapter();
      LTA.Connection.ConnectionString = ConnectionString;
      LTA.FillByOnlyRock(LITHSAMP);
      LTA.Dispose();
    }

    /// <summary>
    /// Reads in groundwater chemistry
    /// </summary>
    /// <param name="DataBaseFileName"></param>
    public void ReadInChemistrySamples()
    {
      GRWCHEMSAMPLETableAdapter GSA = new GRWCHEMSAMPLETableAdapter();
      GSA.Connection.ConnectionString = ConnectionString;
      GSA.Fill(GRWCHEMSAMPLE);
      GSA.Dispose();

      COMPOUNDLISTTableAdapter CTA = new COMPOUNDLISTTableAdapter();
      CTA.Connection.ConnectionString = ConnectionString;
      CTA.Fill(COMPOUNDLIST);
      CTA.Dispose();

      GRWCHEMANALYSISTableAdapter GTA = new GRWCHEMANALYSISTableAdapter();
      GTA.Connection.ConnectionString = ConnectionString;
      GTA.Fill(GRWCHEMANALYSIS);
      GTA.Dispose();
    }


    /// <summary>
    /// Reads in the waterlevels from the database using the FillByNovana method. 
    /// Only necessary fields are read.
    /// </summary>
    /// <param name="DataBaseFileName"></param>
    public void ReadWaterLevels( bool OnlyRo)
    {
      WATLEVELTableAdapter WTA = new WATLEVELTableAdapter();
      WTA.Connection.ConnectionString = ConnectionString;
      if (OnlyRo)
          WTA.FillByNovanaOnlyRo(WATLEVEL);
      else
          WTA.FillByNovana(WATLEVEL);
      WTA.Dispose(); 
    }

    /// <summary>
    /// Read in plants and  related intakes
    /// Tables DRWPLANT, DRWPLANTINTAKE are filled.
    /// </summary>
    /// <param name="DataBaseFileName"></param>
    public void ReadPlantData()
    {
      DRWPLANTTableAdapter DTA = new DRWPLANTTableAdapter();
      DTA.Connection.ConnectionString = ConnectionString;
      DTA.Fill(DRWPLANT);
      DTA.Dispose();

      DRWPLANTINTAKETableAdapter DTIA = new DRWPLANTINTAKETableAdapter();
      DTIA.Connection.ConnectionString = ConnectionString;
      DTIA.Fill(DRWPLANTINTAKE);
      DTIA.Dispose();

      DRWPLANTCOMPANYTYPETableAdapter dca = new DRWPLANTCOMPANYTYPETableAdapter();
      dca.Connection.ConnectionString = ConnectionString;
      dca.Fill(DRWPLANTCOMPANYTYPE);
      dca.Dispose();

    }

    public void ReadExtractionTables()
    {
      WRRCATCHMENTTableAdapter WTA = new WRRCATCHMENTTableAdapter();
      WTA.Connection.ConnectionString = ConnectionString;
      WTA.Fill(WRRCATCHMENT);
      WTA.Dispose();

      INTAKECATCHMENTTableAdapter ITA = new INTAKECATCHMENTTableAdapter();
      ITA.Connection.ConnectionString = ConnectionString;
      ITA.Fill(INTAKECATCHMENT);
      ITA.Dispose();
    }
  }
}



