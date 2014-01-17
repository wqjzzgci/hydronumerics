using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using HydroNumerics.Core;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Net;
using HydroNumerics.Geometry.Shapes;
using HydroNumerics.JupiterTools;
using HydroNumerics.Wells;

using HelixToolkit;

namespace HydroNumerics.View3d
{
  public class SiteViewModel:BaseViewModel
  {
    private GeoRefData area;
    private XYPolygon site;
    IWellCollection Wells;
    private List<JupiterWell> closestWells;

    public ObservableCollection<Sample> Samples { get; set; }

    public List<JupiterWell> ClosetsWells
    {
      get
      {
        if (closestWells == null)
        {
          closestWells = new List<JupiterWell>();
          foreach (var v in Wells)
          {
            if (XYGeometryTools.CalculatePointToPointDistance(site.Points.First(), v) < 100)
              closestWells.Add((JupiterWell)v);
          }

        }
        return closestWells;
      }
    }

    public SiteViewModel( GeoRefData Area, IWellCollection  Wells)
    {
      area = Area;
      site = area.Geometry as XYPolygon;
      this.Wells = Wells;

      DisplayName = Area.Data[0].ToString();
      Samples = new ObservableCollection<Sample>();
    }

    double? height;
    double? Height
    {
      get
      {
        if (!height.HasValue)
        {
          height = 40;
          HydroNumerics.Geometry.Net.KMSData.TryGetHeight(site.Points.First(), 32, out height);
        }
        return height;
      }
    }

    List<Visual3D> rep;
    public List<Visual3D> Representation3D
    {
      get
      {
        if (rep == null)
        {
          rep = new List<Visual3D>();
          var plant = site.Representation3D(site.Points.First(), Height.Value);
          rep.Add(plant);

          foreach (JupiterWell v in ClosetsWells)
            rep.AddRange(v.Representation3D(site.Points.First()));

//          var bmp = Map.GetImagery(site.Points.First(), 500, 500, 32);
          
          //BitmapImage bmp = new BitmapImage(new Uri(@"c:\temp\pict.jpg"));

          var ss = XYPolygon.GetSquare(5000*5000).Representation3D(new XYPoint(2500,2500), Height.Value-30);
        //  ((GeometryModel3D)ss.Content).Material = MaterialHelper.CreateMaterial(Colors.Blue, 0.6);
            ((GeometryModel3D)ss.Content).Material = MaterialHelper.CreateImageMaterial(@"c:\temp\pict.jpg");
          Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
              Sample s = new Sample();
              s.point = new Point3D(r.Next(0, 20), r.Next(0, 20), Height.Value - r.Next(0, 10));
              s.Compartment = "GAS";
              s.Compound = "PID";
              s.Value = 200 + 100 * r.NextDouble();
              s.SampleTime = new DateTime(2011, 6, 24);
              Samples.Add(s);
            }

            NotifyPropertyChanged("Samples");

          foreach (var v in Samples)
          {
            SphereVisual3D svd = new SphereVisual3D();
            svd.Center = v.point;
            svd.Radius = 0.4;

            if (v.Value < 250)
              svd.Material = MaterialHelper.CreateMaterial(Brushes.BlueViolet);
            else
              svd.Material = MaterialHelper.CreateMaterial(Brushes.Red);
            rep.Add(svd);
          }
          rep.Add(ss);
        }
        return rep;
      }
    }
  }
}
