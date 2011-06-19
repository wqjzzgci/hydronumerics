using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Wells;
using HydroNumerics.Time.Core;

namespace HydroNumerics.JupiterTools
{
  public class JupiterXLFastReader
  {
    OleDbConnection odb;

    public JupiterXLFastReader(string DataBaseFileName)
    {
      string ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFileName + ";Persist Security Info=False";
      odb = new OleDbConnection(ConnectionString);
      odb.Open();
    }


    public bool TryGetPlant(int PKey, out int PlantId, out string BoreHoleID, out int IntakeNo)
    {
      PlantId = -1;
      BoreHoleID = "";
      IntakeNo = -1;
      string sql = "select PLANTID, BOREHOLENO, INTAKENO from DRWPLANTINTAKE where INTAKEPLANTID = " + PKey;
      OleDbCommand command = new OleDbCommand(sql, odb);
      OleDbDataReader reader2;
      reader2 = command.ExecuteReader();
      reader2.Read();
      if (!reader2.HasRows)
        return false;

      PlantId = reader2.GetInt32(0);
      BoreHoleID = reader2.GetString(1);
      IntakeNo = reader2.GetInt32(2);
      return true;
    }

    /// <summary>
    /// Returns true if the row is found. The LatestDate can still be null
    /// </summary>
    /// <param name="Table"></param>
    /// <param name="PrimaryKeys"></param>
    /// <param name="LatestDate"></param>
    /// <returns></returns>
    public bool TryGetLatestDate(JupiterTables Table, Dictionary<string, string> PrimaryKeys, out DateTime? LatestDate)
    {
      string sql = "select INSERTDATE, UPDATEDATE from " + Table.ToString();

      switch (Table)
      {
        case JupiterTables.BOREHOLE:
          sql = sql + " WHERE " + PrimaryKeys.First().Key + " = '" + PrimaryKeys.First().Value + "'";
          break;
        case JupiterTables.SCREEN:
          sql = sql + " WHERE BOREHOLENO = '" + PrimaryKeys["BOREHOLENO"] + "' and SCREENNO = " + PrimaryKeys["SCREENNO"];
          break;
        case JupiterTables.DRWPLANTINTAKE:
          sql = sql + " WHERE " + PrimaryKeys.First().Key + " = " + PrimaryKeys.First().Value + "";
          break;
        default:
          break;
      }

      OleDbCommand command = new OleDbCommand(sql, odb);
      OleDbDataReader reader2;
      reader2 = command.ExecuteReader();
      reader2.Read();
      LatestDate = null;

      if (!reader2.HasRows)
        return false;

      DateTime UpdateDate;
     
      if (!reader2.IsDBNull(0))
      {
        LatestDate = reader2.GetDateTime(0);
      }
      if (!reader2.IsDBNull(1))
      {
          UpdateDate = reader2.GetDateTime(1);
        if (LatestDate.HasValue)
        {
          if (LatestDate.Value.CompareTo(UpdateDate)<0)
            LatestDate = UpdateDate;
        }
        else
          LatestDate = UpdateDate;
      }
      return true;
    }

    public bool TryGetPrimaryID(IIntake I, DateTime TimeOfMeasure, out int WATLEVELNO)
    {
      WATLEVELNO = -1;

      OleDbCommand command = new OleDbCommand("select WATLEVELNO from WATLEVEL where BOREHOLENO ='" + I.well.ID + "' and INTAKENO = " + I.IDNumber + " and TIMEOFMEAS = @mydate", odb);
  
      OleDbParameter myParam = new OleDbParameter();
      myParam.ParameterName = "@mydate";
      myParam.DbType = DbType.DateTime;
      myParam.Value = TimeOfMeasure;
      command.Parameters.Add(myParam);
      OleDbDataReader reader2;
      try
      {
        reader2 = command.ExecuteReader();
      }
      catch (OleDbException E)
      {
        throw new Exception("Make sure that the database is in JupiterXL-format, Access 2000");
      }

      reader2.Read();

      if (!reader2.HasRows)
        return false;
      else
      {
        WATLEVELNO = reader2.GetInt32(0);
        return true;
      }

    }

    /// <summary>
    /// Returns the primary id for the row in the drwplantintake table. Still sensitive to sql-injection.
    /// Will not yet return correct number if intake is there twice with different dates
    /// </summary>
    /// <param name="Intake"></param>
    /// <param name="plant"></param>
    /// <returns></returns>
    public bool TryGetPrimaryID(PumpingIntake Intake, Plant plant, out int ID)
    {
      ID = -1;
      string sql = "select IntakeplantId from DRWPLANTINTAKE where PLANTID=" + plant.IDNumber.ToString() + " and BOREHOLENO ='" + Intake.Intake.well.ID + "' and INTAKENO = " + Intake.Intake.IDNumber.ToString();

      OleDbCommand command = new OleDbCommand(sql, odb);
      OleDbDataReader reader2;
      try
      {
        reader2 = command.ExecuteReader();
      }
      catch (OleDbException E)
      {
        throw new Exception("Make sure that the database is in JupiterXL-format, Access 2000");
      }

      reader2.Read();

      if (!reader2.HasRows)
        return false;
      else
      {
        ID = reader2.GetInt32(0);
        return true;
      }
    }

    public void Dispose()
    {
      odb.Dispose();
    }


    public bool TryGetIntakeNoTimeOfMeas(IWell w, int WatLevelNo, out int IntakeNo, out DateTime TimeOfMeasure)
    {
      IntakeNo = -1;
      TimeOfMeasure = DateTime.MinValue;

      string sql = "SELECT INTAKENO, TIMEOFMEAS, WATLEVGRSU, WATLEVMSL, SITUATION FROM WATLEVEL where BOREHOLENO ='" + w.ID + "' and WATLEVELNO = "+ WatLevelNo;
      OleDbCommand command = new OleDbCommand(sql, odb);
      OleDbDataReader reader2;
      try
      {
        reader2 = command.ExecuteReader();
      }
      catch (OleDbException E)
      {
        throw new Exception("Make sure that the database is in JupiterXL-format, Access 2000");
      }

      reader2.Read();

      if (!reader2.HasRows)
        return false;
      else
      {
        IntakeNo = reader2.GetInt32(0);
        TimeOfMeasure = reader2.GetDateTime(1);
        return true;
      }
    }

    /// <summary>
    /// Reads the water levels into the collection of wells
    /// </summary>
    /// <param name="Wells"></param>
    public int ReadWaterLevels(IWellCollection Wells)
    {
      string sql = "SELECT BOREHOLENO, INTAKENO, TIMEOFMEAS, WATLEVGRSU, WATLEVMSL, REFPOINT, SITUATION FROM WATLEVEL";

      OleDbCommand command = new OleDbCommand(sql, odb);
      OleDbDataReader reader2;
      try
      {
        reader2 = command.ExecuteReader();
      }
      catch (OleDbException E)
      {
        throw new Exception("Make sure that the database is in JupiterXL-format, Access 2000");
      }

      IWell CurrentWell = null;
      IIntake CurrentIntake = null;

      //Get the ordinals
      int BoreHoleOrdinal = reader2.GetOrdinal("BOREHOLENO");
      int IntakeOrdinal = reader2.GetOrdinal("INTAKENO");
      int TimeOrdinal = reader2.GetOrdinal("TIMEOFMEAS");
      int WatLevGroundOrdinal = reader2.GetOrdinal("WATLEVGRSU");
      int WaterLevKoteOrdinal = reader2.GetOrdinal("WATLEVMSL");
      int RefPointOrdinal = reader2.GetOrdinal("REFPOINT");
      int SituationOrdinal = reader2.GetOrdinal("SITUATION");

      string previousWellID="";

      int k = 0;

      //Now loop the data
      while (reader2.Read())
      {
        k++;
        string WellID = reader2.GetString(BoreHoleOrdinal);

        if (previousWellID != WellID)
        {
          Wells.TryGetValue(WellID, out CurrentWell);
          previousWellID = WellID;
          CurrentIntake = null;
        }

        if (CurrentWell != null)
        {
          int IntakeNo = reader2.GetInt32(IntakeOrdinal);
          
          if (CurrentIntake == null || CurrentIntake.IDNumber != IntakeNo)
          {
            CurrentIntake = CurrentWell.Intakes.FirstOrDefault(var => var.IDNumber == IntakeNo);
            if (CurrentIntake is JupiterIntake)
            {
              if(!reader2.IsDBNull(RefPointOrdinal))
                ((JupiterIntake)CurrentIntake).RefPoint = reader2.GetString(RefPointOrdinal);
            }
          }

          if (CurrentIntake != null)
          {
            string Description;

            if (reader2.IsDBNull(SituationOrdinal))
              Description = "Unknown";
            else
            {
              if (reader2.GetInt32(SituationOrdinal) == 0)
                Description = "Ro";
              else
                Description = "Drift";
            }

            if (!reader2.IsDBNull(TimeOrdinal)) //No time data
              if (!reader2.IsDBNull(WaterLevKoteOrdinal)) //No kote data
                CurrentIntake.HeadObservations.Items.Add(new TimestampValue(reader2.GetDateTime(TimeOrdinal), reader2.GetDouble(WaterLevKoteOrdinal), Description));
              else if (!reader2.IsDBNull(WatLevGroundOrdinal)) //No ground data
                CurrentIntake.HeadObservations.Items.Add(new TimestampValue(reader2.GetDateTime(TimeOrdinal), CurrentIntake.well.Terrain - reader2.GetDouble(WatLevGroundOrdinal), Description));
          }
        }
      }
      reader2.Close();
      return k;
    }

  }
}
