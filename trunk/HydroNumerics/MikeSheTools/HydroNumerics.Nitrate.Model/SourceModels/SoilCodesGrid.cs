using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class SoilCodesGrid
  {
    public IntegerGrid GlobalGrid = new IntegerGrid();
    public IntegerGrid BornholmGrid = new IntegerGrid();

    public void BuildGrid(string ShapeSoilCodes)
    {
      using (ShapeReader sr = new ShapeReader(ShapeSoilCodes))
      {

        var Allpoints = sr.GeoData.ToList();

        var BornholmPoints = Allpoints.Where(g => int.Parse(g.Data["Model_Doma"].ToString()) == 7).ToList();
        var OtherPoints = Allpoints.Where(g => int.Parse(g.Data["Model_Doma"].ToString()) != 7).ToList();

        var bbox = XYGeometryTools.BoundingBox(OtherPoints.Select(f => (IXYPoint)f.Geometry));

        GlobalGrid.GridSize = 500.0;
        GlobalGrid.XOrigin = bbox.Points.Min(p => p.X);
        GlobalGrid.YOrigin = bbox.Points.Min(p => p.Y);
        GlobalGrid.NumberOfColumns = (int) (Math.Round((bbox.Points.Max(p => p.X) - GlobalGrid.XOrigin)/ GlobalGrid.GridSize,0))+1;
        GlobalGrid.NumberOfRows = (int)(Math.Round((bbox.Points.Max(p => p.Y) - GlobalGrid.YOrigin) / GlobalGrid.GridSize,0)+1);

        foreach (var g in OtherPoints)
        {
          GlobalGrid.Data[GlobalGrid.GetColumnIndex(((IXYPoint)g.Geometry).X), GlobalGrid.GetRowIndex(((IXYPoint)g.Geometry).Y)] = int.Parse(g.Data["GRIDID"].ToString());
        }

        bbox = XYGeometryTools.BoundingBox(BornholmPoints.Select(f => (IXYPoint)f.Geometry));
        BornholmGrid.GridSize = 250.0;
        BornholmGrid.XOrigin = bbox.Points.Min(p => p.X);
        BornholmGrid.YOrigin = bbox.Points.Min(p => p.Y);
        BornholmGrid.NumberOfColumns = (int)(Math.Round((bbox.Points.Max(p => p.X) - BornholmGrid.XOrigin) / BornholmGrid.GridSize, 0)) + 1;
        BornholmGrid.NumberOfRows = (int)(Math.Round((bbox.Points.Max(p => p.Y) - BornholmGrid.YOrigin) / BornholmGrid.GridSize, 0) + 1);

        foreach (var g in BornholmPoints)
        {
          int id = int.Parse(g.Data["GRIDID"].ToString());
          BornholmerID.Add(id,id);
          BornholmGrid.Data[BornholmGrid.GetColumnIndex(((IXYPoint)g.Geometry).X), BornholmGrid.GetRowIndex(((IXYPoint)g.Geometry).Y)] =id;
        }
      }
    }

    Dictionary<int, int> BornholmerID = new Dictionary<int, int>();

        private object Lock = new object();


        public Dictionary<int, List<int>> GetGridIdsWithInCatchment(IEnumerable<Catchment> Catchments)
        {
          Dictionary<int, List<int>> toreturn = new Dictionary<int, List<int>>();

            Parallel.ForEach(Catchments,
            (c) =>
          {
            var l =GlobalGrid.GetSubSet(c.Geometry).Select(t=>GlobalGrid.Data[t.Item1,t.Item2]).Where(v=>v!=0).ToList();
            l.AddRange(BornholmGrid.GetSubSet(c.Geometry).Select(t => BornholmGrid.Data[t.Item1, t.Item2]).Where(v => v != 0));
            lock(Lock)
              toreturn.Add(c.ID, l);
          });
          return toreturn;
        }

        /// <summary>
    /// Gets the id for a point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public int GetID(double X, double Y)
    {
      if (X<850000)
        return GlobalGrid.Data[GlobalGrid.GetColumnIndex(X), GlobalGrid.GetRowIndex(Y)];
      else
        return BornholmGrid.Data[BornholmGrid.GetColumnIndex(X), BornholmGrid.GetRowIndex(Y)];
    }


    public double GetArea(double X, double Y)
    {
      if (X < 850000)
        return Math.Pow(GlobalGrid.GridSize, 2);
      else
        return Math.Pow(BornholmGrid.GridSize, 2);
    }

    public double GetArea(int id)
    {
      if (BornholmerID.ContainsKey(id))
        return Math.Pow(BornholmGrid.GridSize, 2);
      else
        return Math.Pow(GlobalGrid.GridSize, 2);
    }

    /// <summary>
    /// Gets the id for a point
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public int GetID(IXYPoint point)
    {
      return GetID(point.X,point.Y);
    }


  }
}
