using System;
using System.Data;
using System.Data.OleDb;
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

    public void Dispose()
    {
      odb.Dispose();
    }

    /// <summary>
    /// Reads the water levels into the collection og wells
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
