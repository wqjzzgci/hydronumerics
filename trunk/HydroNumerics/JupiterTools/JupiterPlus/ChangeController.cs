using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ChangeController
  {
    JupiterXLFastReader dbConnection;

    public ChangeController(string DataBaseFileName)
    {
      dbConnection = new JupiterXLFastReader(DataBaseFileName);
    }


    public void Dispose()
    {
      dbConnection.Dispose();
    }

    public  void AddIntakeToPlant(PumpingIntake Intake, Plant plant, ChangeDescription change)
    {
      change.Action = TableAction.InsertRow;
      change.Table = JupiterTables.DRWPLANTINTAKE;

      change.ChangeValues.Add(new Change("PLANTID", plant.IDNumber.ToString(), ""));
      change.ChangeValues.Add(new Change("BOREHOLENO", Intake.Intake.well.ID, ""));
      change.ChangeValues.Add(new Change("INTAKENO", Intake.Intake.IDNumber.ToString(), ""));

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", Intake.StartNullable.Value.ToShortDateString(), ""));

      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", Intake.EndNullable.Value.ToShortDateString(), ""));
    }



    public void RemoveIntakeFromPlant(PumpingIntake Intake, Plant plant, ChangeDescription change)
    {
      int id = dbConnection.GetPrimaryID(Intake, plant);
      change.Action = TableAction.DeleteRow;
      change.Table = JupiterTables.DRWPLANTINTAKE;

      change.ChangeValues.Add(new Change("PLANTID", "", plant.IDNumber.ToString()));
      change.ChangeValues.Add(new Change("BOREHOLENO", "", Intake.Intake.well.ID));
      change.ChangeValues.Add(new Change("INTAKENO", "", Intake.Intake.IDNumber.ToString()));

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", "", Intake.StartNullable.Value.ToShortDateString()));

      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", "", Intake.EndNullable.Value.ToShortDateString()));

      change.PrimaryKeys.Add("INTAKEPLANTID", id.ToString());
    }


    public  void ChangeStartDateOnPumpingIntake(PumpingIntake Intake, Plant plant, ChangeDescription change, DateTime NewDate)
    {
      int id = dbConnection.GetPrimaryID(Intake, plant);
      change.Action = TableAction.EditValue;
      change.Table = JupiterTables.DRWPLANTINTAKE;

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", NewDate.ToShortDateString(), Intake.StartNullable.Value.ToShortDateString()));
      else
        change.ChangeValues.Add(new Change("STARTDATE", NewDate.ToShortDateString(), ""));

      change.PrimaryKeys.Add("INTAKEPLANTID", id.ToString());

    }

    public  void ChangeEndDateOnPumpingIntake(PumpingIntake Intake, Plant plant, ChangeDescription change, DateTime NewDate)
    {
      int id = dbConnection.GetPrimaryID(Intake, plant);
      change.Action = TableAction.EditValue;
      change.Table = JupiterTables.DRWPLANTINTAKE;

      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", NewDate.ToShortDateString(), Intake.EndNullable.Value.ToShortDateString()));
      else
        change.ChangeValues.Add(new Change("ENDDATE", NewDate.ToShortDateString(), ""));

      change.PrimaryKeys.Add("INTAKEPLANTID", id.ToString());
    }
  }
}
