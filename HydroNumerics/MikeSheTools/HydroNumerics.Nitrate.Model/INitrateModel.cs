using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public interface INitrateModel
  {
    string Name { get; }
    bool Update { get; set; }
    bool Include { get; set; }
    void ReadConfiguration(System.Xml.Linq.XElement Configuration);
    void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments);
    event NewMessageEventhandler MessageChanged;

  }
}
