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
    /// <summary>
    /// Pointer to the shape file
    /// </summary>
    protected IntPtr _shapePointer= IntPtr.Zero;

    /// <summary>
    /// The file name of the shape file
    /// </summary>
    protected string _fileName;

    
    public Shape()
    {
      ShapeLib.LoadNativeAssemblies();
    }


    private int _NoOfEntries;
    /// <summary>
    /// Gets the number of entries in the shape file
    /// </summary>
    public int NoOfEntries
    {
      get { return _NoOfEntries; }
      protected set
      {
        if (_NoOfEntries != value)
        {
          _NoOfEntries = value;
        }
      }
    }

    
    protected ProjNet.CoordinateSystems.ICoordinateSystem projection;

    /// <summary>
    /// Gets and sets the projection
    /// </summary>
    public virtual ICoordinateSystem Projection 
    { 
      get{return projection;}
      set{projection =value;}
    }

    /// <summary>
    /// Disposes the file
    /// </summary>
    public virtual void Dispose()
    {     
      if (_shapePointer!= IntPtr.Zero)
        ShapeLib.SHPClose(_shapePointer);
    }
  }
}
