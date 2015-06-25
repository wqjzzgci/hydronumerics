using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class ConceptualSourceReducer : BaseModel, ISink
  {
    public string SourceModelName { get; set; }
    private SafeFile ShapeFile;
    public Dictionary<int, double> Reduction = new Dictionary<int, double>();

    public ConceptualSourceReducer()
    { }


    public override void ReadConfiguration(System.Xml.Linq.XElement Configuration)
    {
      base.ReadConfiguration(Configuration);
      //Needs the source model name even if it does not update for the reduction map
      SourceModelName = (Configuration.SafeParseString("SourceModelName") ?? "").ToLower();

      if (Update)
      {
        ShapeFile = new SafeFile() { FileName = Configuration.SafeParseString("ShapeFileName") };
        ShapeFile.ColumnNames.Add(Configuration.SafeParseString("IDColumn"));
        ShapeFile.ColumnNames.Add(Configuration.SafeParseString("ValueColumn"));
      }
    }

    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      base.Initialize(Start, End, Catchments);

      using (ShapeReader sr = new ShapeReader(ShapeFile.FileName))
      {
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          Reduction.Add(sr.Data.ReadInt(i, ShapeFile.ColumnNames[0]), sr.Data.ReadDouble(i, ShapeFile.ColumnNames[1]));
        }
      }
    }


    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      double sourcevalue;
      if(string.IsNullOrEmpty(SourceModelName))
        sourcevalue = CurrentMass;
      else
      {
        sourcevalue = (double)c.StateVariables.Rows.Find(new object[] { c.ID, CurrentTime })[SourceModelName];
      }
      sourcevalue /= (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0);

      double red = 0;
      if (Reduction.ContainsKey(c.ID))
        red = (Reduction[c.ID] * sourcevalue);

      red = red * MultiplicationPar + AdditionPar;

      if (MultiplicationFactors != null)
        if (MultiplicationFactors.ContainsKey(c.ID))
          red *= MultiplicationFactors[c.ID];

      if (AdditionFactors != null)
        if (AdditionFactors.ContainsKey(c.ID))
          red += AdditionFactors[c.ID];

      return red;
    }


    public override void DebugPrint(string Directory, Dictionary<int, Catchment> Catchments)
    {

      if (Reduction.Count == 0)
        return;

      DataTable dt = new DataTable();
      dt.Columns.Add("ID15", typeof(int));
      dt.Columns.Add("RedFactor", typeof(double));

      using (ShapeWriter sw = new ShapeWriter(Path.Combine(Directory, Name + "_factors")) { Projection = MainModel.projection })
      {
        foreach (var kvp in Reduction)
        {
          if (Catchments.ContainsKey(kvp.Key))
          {
            GeoRefData gd = new GeoRefData() { Geometry = Catchments[kvp.Key].Geometry };
            gd.Data = dt.NewRow();
            gd.Data[0] = kvp.Key;
            gd.Data["RedFactor"] = kvp.Value;
            sw.Write(gd);
          }
        }
      }
    }
  }
}

