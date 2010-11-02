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
    /// <summary>
    /// Gets and set the ID
    /// </summary>
    [DataMember]
    public int ID { get; set; }

    /// <summary>
    /// Gets and sets the name
    /// </summary>
    [DataMember]
    public string Name { get; set; }

  }
}
