using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Geometry.Shapes
{
  public abstract class DBF:IDisposable
  {
    protected string _filename;
    protected IntPtr _dbfPointer;
    protected int _recordPointer;
    protected DataTable _data;
    protected Dictionary<string, DBFEntry> _columns;

    public DBF(string FileName)
    {
      _filename = FileName;
      ShapeLib.LoadNativeAssemblies();
    }

    public void SpoolBack()
    {
      _recordPointer = 0;
    }

    public Dictionary<string, DBFEntry> Columns
    {
      get { return _columns; }
    }

    public virtual void Dispose()
    {
      if (_data!=null)
        _data.Dispose();
      if (_dbfPointer!=IntPtr.Zero)
        ShapeLib.DBFClose(_dbfPointer);
    }

  }
}
