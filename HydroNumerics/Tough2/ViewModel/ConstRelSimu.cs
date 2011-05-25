using System;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace HydroNumerics.Tough2.ViewModel
{
  public enum EOS
  {
    t2eco2m,
    t2eco2n,
    t2voc
  }


  public class ConstRelSimu
  {
    public bool Succeded { get; private set; }

    public ConstRelSimu(string RockBlock, EOS eos, string Executable)
    {
      Rocks r = new Rocks();
      

      StreamReader sew = new StreamReader(new MemoryStream(ASCIIEncoding.ASCII.GetBytes(RockBlock)));
      
      while(!sew.ReadLine().StartsWith("ROCKS"));

      r.ReadFromStream(sew);

      RunSimu(r, eos, Executable); 
    }


    public ConstRelSimu(Model M, EOS eos)
    {
      RunSimu(M.Rocks, eos, M.simu.Executable);
    }


    private void RunSimu(Rocks rock, EOS eos, string Executable)
    {
      Model DummySim = new Model();

      switch (eos)
      {
        case EOS.t2eco2m:
          DummySim = new Model(@"C:\Jacob\Udvikling\NewT2voc\DotNetT2VOC\RelPermTemp\eco2m.txt");
          break;
        case EOS.t2eco2n:
          break;
        case EOS.t2voc:
          break;
        default:
          break;
      }

      DummySim.Rocks = rock;

      DummySim.FileContent = "DummyTitle \n" + rock.ToString() + DummySim.FileContent;

      DummySim.simu.Executable = Executable;

      WaterRelativePermeability = new Dictionary<Rock, IEnumerable<Point>>();
      GasCO2RelativePermeability = new Dictionary<Rock, IEnumerable<Point>>();
      LiquidCO2RelativePermeability = new Dictionary<Rock, IEnumerable<Point>>();
      LiquidCO2ThreePhaseRelativePermeability = new Dictionary<Rock, IEnumerable<Point3D>>();
      GasCO2ThreePhaseRelativePermeability = new Dictionary<Rock, IEnumerable<Point3D>>();

        for (int j = 0; j < rock.Count;j++ )
        {
          foreach (var el in DummySim.Elements)
            el.Material = j+1;
          DummySim.SaveMesh();
          DummySim.simu.Run(false);
          DummySim.SaveOutput("Output.txt");
      
      if (DummySim.simu.si.ExitInfo != CauseOfStop.ConvergenceFailure)
      {
        Succeded = true;
          List<Point> _waterrel = new List<Point>();
          List<Point> _gasrel = new List<Point>();
          List<Point> _liqrel = new List<Point>();
          List<Point3D> _intrel = new List<Point3D>();
          List<Point3D> _gas3rel = new List<Point3D>();

          var res = DummySim.Results.Vectors[0];
          for (int i = 0; i < DummySim.Elements.Count; i++)
          {
            _waterrel.Add(new Point(res["SAQ"][i], res["K(AQ)"][i]));
            
            if (res["SLIQ"][i]==0)
              _gasrel.Add(new Point(res["SGAS"][i], res["K(GAS)"][i]));

            if (res["SGAS"][i] == 0)
              _liqrel.Add(new Point(res["SLIQ"][i], res["K(LIQ.)"][i]));

  
            _intrel.Add(new Point3D(res["SAQ"][i], res["SLIQ"][i], res["K(LIQ.)"][i]));
            _gas3rel.Add(new Point3D(res["SAQ"][i], res["SGAS"][i], res["K(GAS)"][i]));
          }
          WaterRelativePermeability.Add(rock[j], _waterrel.Distinct(new PointComparerByX()).OrderBy(var => var.X));
          GasCO2RelativePermeability.Add(rock[j], _gasrel.Distinct(new PointComparerByX()).OrderBy(var => var.X));
          LiquidCO2RelativePermeability.Add(rock[j], _liqrel.Distinct(new PointComparerByX()).OrderBy(var => var.X));

          LiquidCO2ThreePhaseRelativePermeability.Add(rock[j], _intrel);
          GasCO2ThreePhaseRelativePermeability.Add(rock[j],_gas3rel);
        }
      else
        this.Succeded = false;
    }
  }
    /// <summary>
    /// Gets the water relative permeability as function of water saturation for each rock.
    /// </summary>
    public Dictionary<Rock, IEnumerable<Point>> WaterRelativePermeability { get; private set; }

    /// <summary>
    /// Gets the non wetting relative permeability as function of saturation for each rock.
    /// </summary>
    public Dictionary<Rock, IEnumerable<Point>> GasCO2RelativePermeability { get; private set; }

    public Dictionary<Rock, IEnumerable<Point>> LiquidCO2RelativePermeability { get; private set; }


    public Dictionary<Rock, IEnumerable<Point3D>> GasCO2ThreePhaseRelativePermeability { get; private set; }

    public Dictionary<Rock, IEnumerable<Point3D>> LiquidCO2ThreePhaseRelativePermeability { get; private set; }

  }

  public class PointComparerByX : IEqualityComparer<Point>
  {
    public bool Equals(Point x, Point y)
    {
      if (x.X  == y.X)
        return true;
      return false;
    }

    public int GetHashCode(Point a)
    {
      return (a.X.GetHashCode());
    }
  }
}
