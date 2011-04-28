using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Wells;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ChangeController
  {
    JupiterXLFastReader dbConnection;

    private string lastuser
    {
      get
      {
        if (Changes.Count != 0)
          return Changes.Last().User;
        else
          return "UserName";
      }
    }

    private string lastproject
    {
      get
      {
        if (Changes.Count != 0)
          return Changes.Last().Project;
        else
          return "ProjectName";
      }
    }


    public ChangeController(string DataBaseFileName)
    {
      dbConnection = new JupiterXLFastReader(DataBaseFileName);
      Changes = new List<ChangeDescription>();
    }

    public List<ChangeDescription> Changes { get; private set; }


    public void Dispose()
    {
      dbConnection.Dispose();
    }

    private ChangeDescription GetDRWPLANTINTAKE()
    {
      ChangeDescription change = new ChangeDescription(JupiterTables.DRWPLANTINTAKE);
      change.User = lastuser;
      change.Project = lastproject;
      return change;

    }

    public ChangeDescription AddIntakeToPlant(PumpingIntake Intake, Plant plant)
    {
      ChangeDescription change = GetDRWPLANTINTAKE();
      change.Action = TableAction.InsertRow;

      change.ChangeValues.Add(new Change("PLANTID", plant.IDNumber.ToString(), ""));
      change.ChangeValues.Add(new Change("BOREHOLENO", Intake.Intake.well.ID, ""));
      change.ChangeValues.Add(new Change("INTAKENO", Intake.Intake.IDNumber.ToString(), ""));

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", Intake.StartNullable.Value.ToShortDateString(), ""));

      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", Intake.EndNullable.Value.ToShortDateString(), ""));


      return change;
    }



    public ChangeDescription RemoveIntakeFromPlant(PumpingIntake Intake, Plant plant)
    {
      ChangeDescription change = GetDRWPLANTINTAKE();

      int id = dbConnection.GetPrimaryID(Intake, plant);
      change.Action = TableAction.DeleteRow;

      change.ChangeValues.Add(new Change("PLANTID", "", plant.IDNumber.ToString()));
      change.ChangeValues.Add(new Change("BOREHOLENO", "", Intake.Intake.well.ID));
      change.ChangeValues.Add(new Change("INTAKENO", "", Intake.Intake.IDNumber.ToString()));

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", "", Intake.StartNullable.Value.ToShortDateString()));

      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", "", Intake.EndNullable.Value.ToShortDateString()));

      change.PrimaryKeys["INTAKEPLANTID"] = id.ToString();

      return change;
    }


    public ChangeDescription ChangeStartDateOnPumpingIntake(PumpingIntake Intake, Plant plant,  DateTime NewDate)
    {

      ChangeDescription change = GetDRWPLANTINTAKE();

      int id = dbConnection.GetPrimaryID(Intake, plant);
      change.Action = TableAction.EditValue;

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", NewDate.ToShortDateString(), Intake.StartNullable.Value.ToShortDateString()));
      else
        change.ChangeValues.Add(new Change("STARTDATE", NewDate.ToShortDateString(), ""));

      change.PrimaryKeys["INTAKEPLANTID"] = id.ToString();

      return change;
    }

    public ChangeDescription ChangeEndDateOnPumpingIntake(PumpingIntake Intake, Plant plant,  DateTime NewDate)
    {
      ChangeDescription change = GetDRWPLANTINTAKE();
      int id = dbConnection.GetPrimaryID(Intake, plant);
      change.Action = TableAction.EditValue;
 
      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", NewDate.ToShortDateString(), Intake.EndNullable.Value.ToShortDateString()));
      else
        change.ChangeValues.Add(new Change("ENDDATE", NewDate.ToShortDateString(), ""));

      change.PrimaryKeys["INTAKEPLANTID"] = id.ToString();

      return change;
    }


    public void ApplySingleChange(Plant p, IWellCollection wells, ChangeDescription cd)
    {
      switch (cd.Table)
      {
        case JupiterTables.BOREHOLE:
          break;
        case JupiterTables.SCREEN:
          break;
        case JupiterTables.DRWPLANTINTAKE:
          switch (cd.Action)
          {
            case TableAction.EditValue:
              break;
            case TableAction.DeleteRow:
              break;
            case TableAction.InsertRow:
              string wellid = cd.ChangeValues.First(var => var.Column == "BOREHOLENO").NewValue;
              int intakeno = int.Parse(cd.ChangeValues.First(var => var.Column == "INTAKENO").NewValue);
              PumpingIntake pi = new PumpingIntake(wells[wellid].Intakes.First(var => var.IDNumber == intakeno), p);
              var s = cd.ChangeValues.First(var => var.Column == "STARTDATE");
              if (s != null)
                pi.StartNullable = DateTime.Parse(s.NewValue);
              s = cd.ChangeValues.First(var => var.Column == "ENDDATE");
              if (s != null)
                pi.StartNullable = DateTime.Parse(s.NewValue);
              p.PumpingIntakes.Add(pi);
              break;
            default:
              break;
          }


          break;
        default:
          break;
      }


    }


  }
}
