using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Wells;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ChangeController
  {
    public JupiterXLFastReader DataBaseConnection { get; set; }


    public ChangeController()
    {

    }


    public void Dispose()
    {
      if (DataBaseConnection != null)
        DataBaseConnection.Dispose();
    }

    private ChangeDescription GetDRWPLANTINTAKE()
    {
      ChangeDescription change = new ChangeDescription(JupiterTables.DRWPLANTINTAKE);
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

    private ChangeDescription GetWellChange(IWell well)
    {
      ChangeDescription change = new ChangeDescription(JupiterTables.BOREHOLE);
      change.Action = TableAction.EditValue;

      change.PrimaryKeys["BOREHOLENO"] = well.ID;

      return change;
    }

    public ChangeDescription ChangeXOnWell(IWell well, double NewValue)
    {
      ChangeDescription change = GetWellChange(well);
      change.ChangeValues.Add(new Change("XUTM", NewValue.ToString(), well.X.ToString()));
      return change;
    }

    public ChangeDescription ChangeYOnWell(IWell well, double NewValue)
    {
      ChangeDescription change = GetWellChange(well);
      change.ChangeValues.Add(new Change("YUTM", NewValue.ToString(), well.Y.ToString()));
      return change;
    }

    public ChangeDescription ChangeTerrainOnWell(IWell well, double NewValue)
    {
      ChangeDescription change = GetWellChange(well);
      change.ChangeValues.Add(new Change("ELEVATION", NewValue.ToString(), well.Terrain.ToString()));
      return change;
    }

    private ChangeDescription GetScreenChange(Screen screen)
    {
      ChangeDescription change = new ChangeDescription(JupiterTables.SCREEN);
      change.Action = TableAction.EditValue;

      change.PrimaryKeys["BOREHOLENO"] = screen.Intake.well.ID;
      change.PrimaryKeys["SCREENNO"] = screen.Number.ToString();

      return change;
    }


    public ChangeDescription ChangeTopOnScreen(Screen screen, double NewValue)
    {
      ChangeDescription change = GetScreenChange(screen);
      change.ChangeValues.Add(new Change("TOP", NewValue.ToString(), screen.DepthToTop.ToString()));
      return change;
    }

    public ChangeDescription ChangeBottomOnScreen(Screen screen, double NewValue)
    {
      ChangeDescription change = GetScreenChange(screen);
      change.ChangeValues.Add(new Change("BOTTOM", NewValue.ToString(), screen.DepthToBottom.ToString()));
      return change;
    }

    public ChangeDescription NewScreen(IIntake intake, double top, double bottom)
    {
      ChangeDescription change = new ChangeDescription(JupiterTables.SCREEN);
      change.Action = TableAction.EditValue;

      change.PrimaryKeys["BOREHOLENO"] = intake.well.ID;
      change.PrimaryKeys["SCREENNO"] = (intake.well.Intakes.Max(var1 => var1.Screens.Max(var => var.Number)) + 1).ToString();

      change.ChangeValues.Add(new Change("TOP", top.ToString(), ""));
      change.ChangeValues.Add(new Change("BOTTOM", bottom.ToString(), ""));
      change.ChangeValues.Add(new Change("INTAKENO", intake.IDNumber.ToString(), ""));
      return change;
    }



    public ChangeDescription RemoveIntakeFromPlant(PumpingIntake Intake, Plant plant)
    {
      ChangeDescription change = GetDRWPLANTINTAKE();

      int id = DataBaseConnection.GetPrimaryID(Intake, plant);
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


    public ChangeDescription ChangeStartDateOnPumpingIntake(PumpingIntake Intake, Plant plant, DateTime NewDate)
    {

      ChangeDescription change = GetDRWPLANTINTAKE();

      int id = DataBaseConnection.GetPrimaryID(Intake, plant);
      change.Action = TableAction.EditValue;

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", NewDate.ToShortDateString(), Intake.StartNullable.Value.ToShortDateString()));
      else
        change.ChangeValues.Add(new Change("STARTDATE", NewDate.ToShortDateString(), ""));

      change.PrimaryKeys["INTAKEPLANTID"] = id.ToString();

      return change;
    }

    public ChangeDescription ChangeEndDateOnPumpingIntake(PumpingIntake Intake, Plant plant, DateTime NewDate)
    {
      ChangeDescription change = GetDRWPLANTINTAKE();
      int id = DataBaseConnection.GetPrimaryID(Intake, plant);
      change.Action = TableAction.EditValue;

      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", NewDate.ToShortDateString(), Intake.EndNullable.Value.ToShortDateString()));
      else
        change.ChangeValues.Add(new Change("ENDDATE", NewDate.ToShortDateString(), ""));

      change.PrimaryKeys["INTAKEPLANTID"] = id.ToString();

      return change;
    }


    /// <summary>
    /// Loads all changes from a file
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public IEnumerable<ChangeDescription> LoadFromFile(string FileName)
    {
      XDocument _changes = XDocument.Load(FileName);
      XElement cc = new XElement("Changes");

      foreach (var c in _changes.Element("Changes").Elements())
      {
        yield return new ChangeDescription(c);
      }
    }


    /// <summary>
    /// Saves the changes to an xml-file.
    /// </summary>
    /// <param name="Changes"></param>
    /// <param name="FileName"></param>
    public void SaveToFile(IEnumerable<ChangeDescription> Changes, string FileName)
    {
      XDocument _changes = new XDocument();
      XElement cc = new XElement("Changes");

      foreach (var c in Changes)
      {
        XElement cx = c.ToXML();
        cc.Add(cx);
      }

      _changes.Add(cc);
      _changes.Save(FileName);
    }


    public bool ApplySingleChange(IPlantCollection plants, IWellCollection wells, ChangeDescription cd)
    {
      string wellid;
      int plantid;
      int intakeno;

      bool succeded = false;

      switch (cd.Table)
      {
        case JupiterTables.BOREHOLE:
          wellid = cd.PrimaryKeys["BOREHOLENO"];
          foreach (var c in cd.ChangeValues)
          {
            switch (c.Column.ToUpper())
            {
              case "XUTM":
                wells[wellid].X = double.Parse(c.NewValue);
                break;
              case "YUTM":
                wells[wellid].Y = double.Parse(c.NewValue);
                break;
              case "ELEVATION":
                wells[wellid].Terrain = double.Parse(c.NewValue);
                break;
              default:
                break;
            }
          }
          break;
        case JupiterTables.SCREEN:
          break;
        case JupiterTables.DRWPLANTINTAKE:
          if (cd.Action == TableAction.EditValue || cd.Action == TableAction.DeleteRow)
          {

            int tableid = int.Parse(cd.PrimaryKeys.First().Value);
            if (DataBaseConnection.TryGetPlant(tableid, out plantid, out wellid, out intakeno))
            {
              var pi = plants[plantid].PumpingIntakes.Single(var => var.Intake.well.ID == wellid & var.Intake.IDNumber == intakeno);

              if (cd.Action == TableAction.DeleteRow)
                plants[plantid].PumpingIntakes.Remove(pi);
              else
              {
                var start = cd.ChangeValues.SingleOrDefault(var => var.Column == "STARTDATE");
                if (start != null)
                  pi.StartNullable = DateTime.Parse(start.NewValue);
                var end = cd.ChangeValues.SingleOrDefault(var => var.Column == "ENDDATE");
                if (end != null)
                  pi.End = DateTime.Parse(end.NewValue);
              }
            }
          }
          else
          {
            plantid = int.Parse(cd.ChangeValues.First(var => var.Column == "PLANTID").NewValue);
            Plant p;
            if (plants.TryGetValue(plantid, out p))
            {
              wellid = cd.ChangeValues.First(var => var.Column == "BOREHOLENO").NewValue;
              IWell w;
              if (wells.TryGetValue(wellid, out w))
              {
                intakeno = int.Parse(cd.ChangeValues.First(var => var.Column == "INTAKENO").NewValue);
                IIntake I = w.Intakes.First(var => var.IDNumber == intakeno);
                if (I != null)
                {
                  PumpingIntake pi = new PumpingIntake(I, p);
                  var s = cd.ChangeValues.First(var => var.Column == "STARTDATE");
                  if (s != null)
                    pi.StartNullable = DateTime.Parse(s.NewValue);
                  s = cd.ChangeValues.First(var => var.Column == "ENDDATE");
                  if (s != null)
                    pi.StartNullable = DateTime.Parse(s.NewValue);
                  p.PumpingIntakes.Add(pi);
                  succeded = true;
                }
              }
            }
          }
          break;

        default:
          break;
      }
      return succeded;
    }
  }
}
