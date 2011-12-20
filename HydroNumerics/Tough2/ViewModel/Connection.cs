using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class Connection
  {
    public Element First { get; set; }
    public Element Second { get; set; }
    public int PermeabilityDirection { get; set; }
    public double Distance1 { get; set; }
    public double Distance2 { get; set; }
    public double Area { get; set; }
    public double? CosineAngle { get; set; }

    public List<FlowDataEntry> Flow { get; set; }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="ToCopy"></param>
    public Connection(Connection ToCopy)
    {
      First = ToCopy.First;
      Second = ToCopy.Second;
      PermeabilityDirection = ToCopy.PermeabilityDirection;
      Distance1 = ToCopy.Distance1;
      Distance2 = ToCopy.Distance2;
      Area = ToCopy.Area;
      CosineAngle = ToCopy.CosineAngle;
    }

    public Connection(string FromString, ElementCollection ElemeCollection)
    {
      First = ElemeCollection[FromString.Substring(0, 5)];
      Second = ElemeCollection[FromString.Substring(5, 5)];
      PermeabilityDirection = int.Parse(FromString.Substring(29, 1));
      Distance1 = double.Parse(FromString.Substring(30, 10));
      Distance2 = double.Parse(FromString.Substring(40, 10));
      Area = double.Parse(FromString.Substring(50, 10));

      if (FromString.Length > 60)
      {
        string temp = FromString.Substring(60, FromString.Length -60).Trim();
        if (temp != string.Empty)
          CosineAngle = double.Parse(temp);
      }
    }

    public override string ToString()
    {
      StringBuilder str = new StringBuilder(new string(' ', 100));
      str.Insert(0, First.Name);
      str.Insert(5, Second.Name);
      str.Insert(29, PermeabilityDirection.ToString());
      str.Insert(30, Distance1.ToString("0.0000E+00"));
      str.Insert(40, Distance2.ToString("0.0000E+00"));
      str.Insert(50, Area.ToString("0.0000E+00"));
      if (CosineAngle != null) 
        str.Insert(60, ((double)CosineAngle).ToString("0.0000E+00"));

      return str.ToString().Trim();
    }
  }

}
