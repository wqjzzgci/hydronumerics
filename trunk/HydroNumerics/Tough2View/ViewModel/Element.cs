using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class Element
  {
    [FixedFormat(0,5)]
    public string Name { get; set; }

    [FixedFormat(18,5)]
    public int Material { get; set; }
    [FixedFormat(10,10)]
    public double Volume { get; set; }
    [FixedFormat(50, 10)]
    public double? X { get; set; }
    [FixedFormat(60, 10)]
    public double? Y { get; set; }
    [FixedFormat(70, 10)]
    public double? Z { get; set; }

    public double Porosity { get; set; }
    public Rock rock { get; set; }

    //The fluids and their saturation
    public Dictionary<IFluid, double> Fluids {get;private set;}

    public double[] PrimaryVaribles { get; set; }
    public int? PrimaryVariablesIndex { get; set; }

    public Dictionary<TimeSpan, Dictionary<string, double>> PrintData { get; private set; }

    public List<TSEntry> TimeData{get;set;}

    public Element(string FromString)
    {
      PrintData = new Dictionary<TimeSpan, Dictionary<string, double>>();
      Name = FromString.Substring(0, 5);
      Material = int.Parse(FromString.Substring(18, 2));
      if (FromString.Length>20)
        Volume = double.Parse(FromString.Substring(20, 10));
      if (FromString.Length > 70)
      {
        X = double.Parse(FromString.Substring(50, 10));

        //No y-value in radial coordinates
        double y;
        if (double.TryParse(FromString.Substring(60, 10),out y))
          Y = y;
        Z = double.Parse(FromString.Substring(70, 10));
      }
    }

    public override string ToString()
    {
      StringBuilder str = new StringBuilder(new string(' ', 100));
      str.Insert(0, Name);
      str.Insert(18, Material.ToString("d2"));
      str.Insert(20, Volume.ToString("0.0000E+00"));
      if (X.HasValue)
        str.Insert(50, X.Value.ToString("0.0000E+00"));
      if(Y.HasValue)
        str.Insert(60, Y.Value.ToString("0.0000E+00"));
  if (Z.HasValue)
      str.Insert(70, Z.Value.ToString("0.0000E+00"));
      return str.ToString().Trim();
    }
  }
}
