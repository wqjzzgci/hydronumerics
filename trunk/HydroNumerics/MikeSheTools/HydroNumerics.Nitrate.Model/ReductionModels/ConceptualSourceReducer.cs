using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroNumerics.Nitrate.Model.ReductionModels
{
  public class ConceptualSourceReducer:BaseModel,ISink
  {
    private string SourceModelName;
    Dictionary<int, double> Reduction = new Dictionary<int, double>();

    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      base.Initialize(Start, End, Catchments);
    }

    public override void ReadConfiguration(System.Xml.Linq.XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      SourceModelName=Configuration.SafeParseString("SourceModel").ToLower();

    }

    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      var smodel = c.SourceModels.FirstOrDefault(s => s.Name.ToLower() == SourceModelName);
      if (smodel!=null)
      {
        return Reduction[c.ID] * smodel.GetValue(c, CurrentTime);
      }
      return 0;
    }
  }
}
