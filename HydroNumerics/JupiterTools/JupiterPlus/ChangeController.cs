using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Wells;
using HydroNumerics.Time.Core;

namespace HydroNumerics.JupiterTools.JupiterPlus
{
  public class ChangeController
  {
    public JupiterXLFastReader DataBaseConnection { get; set; }

    public string UserName { get; set; }
    public string ProjectName { get; set; }

    public void Dispose()
    {
      if (DataBaseConnection != null)
        DataBaseConnection.Dispose();
    }


    public ChangeDescription GetGenericPlantIntake()
    {
      ChangeDescription change = new ChangeDescription(JupiterTables.DRWPLANTINTAKE);
      change.User = UserName;
      change.Project = ProjectName;
      change.Action = TableAction.InsertRow;
      return change;
    }

    public ChangeDescription AddIntakeToPlant(PumpingIntake Intake, Plant plant)
    {
      ChangeDescription change = GetGenericPlantIntake();

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
      change.User = UserName;
      change.Project = ProjectName;

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

    public ChangeDescription ChangeDepthOnWell(IWell well, double NewValue)
    {
      ChangeDescription change = GetWellChange(well);
      change.ChangeValues.Add(new Change("DRILLDEPTH", NewValue.ToString(), well.Terrain.ToString()));
      return change;
    }

    public ChangeDescription GetRemoveWatlevel(IIntake intake, DateTime timeofmeasure)
    {
      ChangeDescription change = new ChangeDescription(JupiterTables.WATLEVEL);
      change.User = UserName;
      change.Project = ProjectName;

      change.Action = TableAction.DeleteRow;

      change.PrimaryKeys.Add("BOREHOLENO", intake.well.ID);

      int WATLEVELNO;
      if (DataBaseConnection.TryGetPrimaryID(intake, timeofmeasure, out WATLEVELNO))
        change.PrimaryKeys.Add("WATLEVELNO", WATLEVELNO.ToString());

      return change;
    }


    public ChangeDescription GetScreenChange(Screen screen)
    {
      ChangeDescription change = new ChangeDescription(JupiterTables.SCREEN);
      change.User = UserName;
      change.Project = ProjectName;

      change.Action = TableAction.EditValue;

      change.PrimaryKeys["BOREHOLENO"] = screen.Intake.well.ID;
      change.PrimaryKeys["SCREENNO"] = screen.Number.ToString();

      return change;
    }


    public ChangeDescription ChangeTopOnScreen(Screen screen, double NewValue)
    {
      ChangeDescription Cd = GetScreenChange(screen);
      Cd.ChangeValues.Add(new Change("TOP", NewValue.ToString(), screen.DepthToTop.ToString()));
      return Cd;
    }

    public void ChangeBottomOnScreen(ChangeDescription Cd, Screen screen, double NewValue)
    {
      Cd.ChangeValues.Add(new Change("BOTTOM", NewValue.ToString(), screen.DepthToBottom.ToString()));
    }

    public ChangeDescription ChangeBottomOnScreen(Screen screen, double NewValue)
    {
      ChangeDescription Cd = GetScreenChange(screen);
      ChangeBottomOnScreen(Cd, screen, NewValue);
      return Cd;
    }


    public ChangeDescription NewScreen(Screen screen)
    {
      ChangeDescription change = GetScreenChange(screen);
      change.Action = TableAction.InsertRow;

      change.ChangeValues.Add(new Change("TOP", screen.DepthToTop.ToString(), ""));
      change.ChangeValues.Add(new Change("BOTTOM", screen.DepthToBottom.ToString(), ""));
      change.ChangeValues.Add(new Change("INTAKENO", screen.Intake.IDNumber.ToString(), ""));
      return change;
    }



    public ChangeDescription RemoveIntakeFromPlant(PumpingIntake Intake, Plant plant)
    {
      ChangeDescription change = GetGenericPlantIntake();
      int id;

      change.Action = TableAction.DeleteRow;

      change.ChangeValues.Add(new Change("PLANTID", "", plant.IDNumber.ToString()));
      change.ChangeValues.Add(new Change("BOREHOLENO", "", Intake.Intake.well.ID));
      change.ChangeValues.Add(new Change("INTAKENO", "", Intake.Intake.IDNumber.ToString()));

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", "", Intake.StartNullable.Value.ToShortDateString()));

      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", "", Intake.EndNullable.Value.ToShortDateString()));

      if (DataBaseConnection.TryGetPrimaryID(Intake, plant, out id))
        change.PrimaryKeys["INTAKEPLANTID"] = id.ToString();

      return change;
    }


    public ChangeDescription ChangeStartDateOnPumpingIntake(PumpingIntake Intake, Plant plant, DateTime NewDate)
    {

      ChangeDescription change = GetGenericPlantIntake();

      int id;
      if (DataBaseConnection.TryGetPrimaryID(Intake, plant, out id))
        change.PrimaryKeys["INTAKEPLANTID"] = id.ToString();

      change.Action = TableAction.EditValue;

      if (Intake.StartNullable.HasValue)
        change.ChangeValues.Add(new Change("STARTDATE", NewDate.ToShortDateString(), Intake.StartNullable.Value.ToShortDateString()));
      else
        change.ChangeValues.Add(new Change("STARTDATE", NewDate.ToShortDateString(), ""));


      return change;
    }

    public ChangeDescription ChangeEndDateOnPumpingIntake(PumpingIntake Intake, Plant plant, DateTime NewDate)
    {
      ChangeDescription change = GetGenericPlantIntake();
      int id;
      if (DataBaseConnection.TryGetPrimaryID(Intake, plant, out id))
        change.PrimaryKeys["INTAKEPLANTID"] = id.ToString();

      change.Action = TableAction.EditValue;

      if (Intake.EndNullable.HasValue)
        change.ChangeValues.Add(new Change("ENDDATE", NewDate.ToShortDateString(), Intake.EndNullable.Value.ToShortDateString()));
      else
        change.ChangeValues.Add(new Change("ENDDATE", NewDate.ToShortDateString(), ""));
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

    /// <summary>
    /// Applies the change in the description to a plant or a well.
    /// Returns true if the change could be applied. 
    /// No checks on previous values or dates.
    /// </summary>
    /// <param name="plants"></param>
    /// <param name="wells"></param>
    /// <param name="cd"></param>
    /// <returns></returns>
    public bool ApplySingleChange(IPlantCollection plants, IWellCollection wells, ChangeDescription cd)
    {
      string wellid = "";
      int plantid;
      int intakeno = -1;

      IWell CurrentWell;

      bool succeded = false;

      switch (cd.Table)
      {
        case JupiterTables.BOREHOLE:
          wellid = cd.PrimaryKeys["BOREHOLENO"];
          if (wells.TryGetValue(wellid, out CurrentWell))
          {
            foreach (var c in cd.ChangeValues)
            {
              switch (c.Column.ToUpper())
              {
                case "XUTM":
                  CurrentWell.X = double.Parse(c.NewValue);
                  succeded = true;
                  break;
                case "YUTM":
                  CurrentWell.Y = double.Parse(c.NewValue);
                  succeded = true;
                  break;
                case "ELEVATION":
                  CurrentWell.Terrain = double.Parse(c.NewValue);
                  succeded = true;
                  break;
                default:
                  break;
              }
            }
          }
          break;
        case JupiterTables.SCREEN:
          wellid = cd.PrimaryKeys["BOREHOLENO"];
          if (wells.TryGetValue(wellid, out CurrentWell))
          {
            int screenNumber = int.Parse(cd.PrimaryKeys["SCREENNO"]);
            if (cd.Action == TableAction.EditValue)
            {
              var screen = CurrentWell.Intakes.SelectMany(var => var.Screens).FirstOrDefault(var2 => var2.Number == screenNumber);
              if (screen != null)
              {
                foreach (var cv in cd.ChangeValues)
                {
                  if (cv.Column == "TOP")
                    screen.DepthToTop = double.Parse(cv.NewValue);
                  else if (cv.Column == "BOTTOM")
                    screen.DepthToBottom = double.Parse(cv.NewValue);
                  succeded = true;
                }
              }
            }
            else if (cd.Action == TableAction.InsertRow)
            {
              intakeno = int.Parse(cd.ChangeValues.Single(var => var.Column == "INTAKENO").NewValue);
              IIntake CurrentIntake = CurrentWell.Intakes.Single(var => var.IDNumber == intakeno);
              if (CurrentIntake != null)
              {
                Screen sc = new Screen(CurrentIntake);
                sc.DepthToTop = double.Parse(cd.ChangeValues.Single(var => var.Column == "TOP").NewValue);
                sc.DepthToBottom = double.Parse(cd.ChangeValues.Single(var => var.Column == "BOTTOM").NewValue);
                succeded = true;
              }
            }
          }
          break;
        case JupiterTables.DRWPLANTINTAKE:
          if (cd.Action == TableAction.EditValue || cd.Action == TableAction.DeleteRow)
          {
            int tableid;
            if (int.TryParse(cd.PrimaryKeys.First().Value, out tableid))
            {
              if (DataBaseConnection.TryGetPlant(tableid, out plantid, out wellid, out intakeno))
                succeded = true;
            }
            else //No ID Change of a change
            {
              if (cd.Action == TableAction.EditValue)
              {
                if (int.TryParse(cd.ChangeValues[0].NewValue, out plantid))
                {
                  wellid = cd.ChangeValues[1].NewValue;
                  if (int.TryParse(cd.ChangeValues[2].NewValue, out intakeno))
                    succeded = true;
                }
              }
              else
                if (int.TryParse(cd.ChangeValues[0].OldValue, out plantid))
                {
                  wellid = cd.ChangeValues[1].OldValue;
                  if (int.TryParse(cd.ChangeValues[2].OldValue, out intakeno))
                    succeded = true;
                }

            }

            if (succeded)
            {
              var pi = plants[plantid].PumpingIntakes.SingleOrDefault(var => var.Intake.well.ID == wellid & var.Intake.IDNumber == intakeno);

              if (pi != null)
              {
                if (cd.Action == TableAction.DeleteRow)
                  plants[plantid].PumpingIntakes.Remove(pi);
                else
                {
                  var start = cd.ChangeValues.SingleOrDefault(var => var.Column == "STARTDATE");
                  if (start != null)
                    pi.StartNullable = DateTime.Parse(start.NewValue);
                  var end = cd.ChangeValues.SingleOrDefault(var => var.Column == "ENDDATE");
                  if (end != null)
                    pi.EndNullable = DateTime.Parse(end.NewValue);
                }
                succeded = true;
              }
            }
          }
          else //insertrow
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
                  var s = cd.ChangeValues.FirstOrDefault(var => var.Column == "STARTDATE");
                  if (s != null)
                    pi.StartNullable = DateTime.Parse(s.NewValue);
                  s = cd.ChangeValues.FirstOrDefault(var => var.Column == "ENDDATE");
                  if (s != null)
                    pi.EndNullable = DateTime.Parse(s.NewValue);
                  p.PumpingIntakes.Add(pi);
                  succeded = true;
                }
              }
            }
          }
          break;
        case JupiterTables.WATLEVEL:
          wellid = cd.PrimaryKeys["BOREHOLENO"];
          if (wells.TryGetValue(wellid, out CurrentWell))
          {
            DateTime TimeOfMeasure;
            int WatlevelNo = int.Parse(cd.PrimaryKeys["WATLEVELNO"]);
            if (DataBaseConnection.TryGetIntakeNoTimeOfMeas(CurrentWell, WatlevelNo, out intakeno, out TimeOfMeasure))
            {
              IIntake CurrentIntake = CurrentWell.Intakes.SingleOrDefault(var => var.IDNumber == intakeno);
              if (CurrentIntake != null)
              {
                var item = CurrentIntake.HeadObservations.Items.FirstOrDefault(var => var.Time == TimeOfMeasure);
                if (item != null)
                {
                  CurrentIntake.HeadObservations.Items.Remove(item);
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
