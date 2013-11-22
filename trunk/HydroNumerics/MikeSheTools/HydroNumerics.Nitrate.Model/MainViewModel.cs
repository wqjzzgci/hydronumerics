using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroNumerics.Core.WPF;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class MainViewModel : BaseViewModel
  {




    public ObservableCollection<Catchment> EndCatchments { get; private set; }
    public Dictionary<int, Catchment> AllCatchments { get; private set; }
    public Dictionary<int, GridPoint> GridPoints { get; private set; }

    public ObservableCollection<Catchment> CurrentCatchments { get; private set; }


    public MainViewModel()
    {
      CurrentCatchments = new ObservableCollection<Catchment>();
    }

    private Catchment currentCatchment;

    public Catchment CurrentCatchment
    {
      get
      { return currentCatchment; }
      set
      {
        if (value != currentCatchment)
        {
          currentCatchment = value;
          NotifyPropertyChanged("CurrentCatchment");

          if (!CurrentCatchments.Contains(currentCatchment))
          {
            CurrentCatchments.Clear();
            RecursiveUpstreamAdd(GetNextDownstream(currentCatchment));
          }
        }
      }
    }


    private void RecursiveUpstreamAdd(Catchment c)
    {
      CurrentCatchments.Add(c);
      foreach (var ups in c.UpstreamConnections)
        RecursiveUpstreamAdd(ups);
    }

    private Catchment GetNextDownstream(Catchment c)
    {
      if (c.DownstreamConnection == null)
        return c;
      return GetNextDownstream(c.DownstreamConnection);
    }


    public void LoadGridPoints(string ShapeFileName)
    {
      GridPoints = new Dictionary<int, GridPoint>();
      using (Geometry.Shapes.ShapeReader sr = new Geometry.Shapes.ShapeReader(ShapeFileName))
      {
        foreach (var c in sr.GeoData)
        {
          GridPoint gp = new GridPoint();
          gp.Point = c.Geometry as XYPoint;
          gp.GridID = (int) c.Data["GRIDID"];
          GridPoints.Add(gp.GridID, gp);
        }
      }
    }

    List<Particle> Particles;

    public void LoadParticles(string ShapeFileName)
    {
      Particles = new List<Particle>();
      using (ShapeReader sr = new ShapeReader(ShapeFileName))
      {
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          double x = sr.Data.ReadDouble(i, "X-Reg");
          double y = sr.Data.ReadDouble(i, "Y-Reg");

          Particle p = new Particle();
          p.X = x;
          p.Y = y;
          p.StartX = sr.Data.ReadInt(i, "IX-Birth");
          p.StartY = sr.Data.ReadInt(i, "IY-Birth");
          p.TravelTime = sr.Data.ReadDouble(i, "TravelTime");
          Particles.Add(p);
        }
      }
    }

    public void CombineParticlesAndCatchments()
    {
      foreach (var c in AllCatchments.Values)
      {
        var b = c.Geometry.BoundingBox;
      }


      Parallel.ForEach(Particles, new ParallelOptions() { MaxDegreeOfParallelism = 7 },
        (p) =>
        {
          foreach (var c in AllCatchments.Values)
          {
            if (c.Geometry.Contains(p.X, p.Y))
            {
              c.Particles.Add(p);
              break;
            
            }
          }
        });
    }


    public void LoadCatchments(string ShapeFileName)
    {
      using (ShapeReader sr = new ShapeReader(ShapeFileName))
      {
        if (AllCatchments == null)
          AllCatchments = new Dictionary<int, Catchment>();

        foreach (var c in sr.GeoData)
        {
          Catchment ca = new Catchment((int)c.Data[0]);
          if (!AllCatchments.ContainsKey(ca.ID15))
            AllCatchments.Add(ca.ID15, ca);

          ca.Geometry = (XYPolygon) c.Geometry;
        }

        foreach (var c in sr.GeoData)
        {
          int catcid = ((int)c.Data[0]);
          int downid = ((int)c.Data[1]);
          Catchment DownStreamCatchment;
          if (AllCatchments.TryGetValue(downid, out DownStreamCatchment))
          {
            if (DownStreamCatchment != AllCatchments[catcid]) //Do not allow reference to self
            {
              AllCatchments[catcid].DownstreamConnection = DownStreamCatchment;
              DownStreamCatchment.UpstreamConnections.Add(AllCatchments[catcid]);
            }
          }
        }
        EndCatchments = new ObservableCollection<Catchment>(AllCatchments.Values.Where(c => c.DownstreamConnection == null));
      }
    }

  }
}

