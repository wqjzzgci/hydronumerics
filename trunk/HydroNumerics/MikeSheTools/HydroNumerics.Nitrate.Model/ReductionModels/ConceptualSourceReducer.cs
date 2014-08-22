using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class ConceptualSourceReducer:BaseModel,ISink
  {
    private string SourceModelName;
    private SafeFile ShapeFile;
    Dictionary<int, double> Reduction = new Dictionary<int, double>();

    public ConceptualSourceReducer()
    { }


    public override void ReadConfiguration(System.Xml.Linq.XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      if (Update)
      {
        SourceModelName = Configuration.SafeParseString("SourceModelName").ToLower();
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
        for(int i =0;i<sr.Data.NoOfEntries;i++)
        {
          Reduction.Add(sr.Data.ReadInt(i, ShapeFile.ColumnNames[0]), sr.Data.ReadDouble(i, ShapeFile.ColumnNames[1]));
        }
      }
    }


    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      double sourcevalue = (double)c.StateVariables.Rows.Find(new object[] { c.ID, CurrentTime })[SourceModelName];

        if(Reduction.ContainsKey(c.ID))
          return (Reduction[c.ID] * sourcevalue * MultiplicationPar + AdditionPar) / (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0); ;
      return 0;
    }
  }
}
