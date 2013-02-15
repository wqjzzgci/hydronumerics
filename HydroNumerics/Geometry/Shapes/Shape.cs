using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ProjNet.CoordinateSystems;

namespace HydroNumerics.Geometry.Shapes
{
  public abstract class Shape:IDisposable
  {
    protected IntPtr _shapePointer= IntPtr.Zero;
    protected int _recordPointer = 0;
    protected int _noOfEntries;
    protected string _fileName;

    public Shape()
    {

    }

    public virtual ICoordinateSystem Projection { get; private set; }

    public virtual void Dispose()
    {     
      if (_shapePointer!= IntPtr.Zero)
        ShapeLib.SHPClose(_shapePointer);
    }
  }
}
