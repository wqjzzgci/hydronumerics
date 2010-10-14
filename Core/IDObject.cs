using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
  [DataContract]
  public class IDObject
  {
    [DataMember]
    public int ID { get; set; }

    [DataMember]
    public string Name { get; set; }

  }
}
