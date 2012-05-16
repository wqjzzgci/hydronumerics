using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero.DFS;

using HydroNumerics.Geometry;

namespace HydroNumerics.MikeSheTools.DFS
{
  public abstract class DFS2DBase:DFSBase,IGrid 
  {

    /// <summary>
    /// Small abstract class adding 2d information to a dfs-file. Used as base for dfs2 and dfs3
    /// </summary>
    /// <param name="DFSFileName"></param>
    public DFS2DBase(string DFSFileName)
      : base(DFSFileName)
    {
      OriginAtCenter = true;
    }

    public DFS2DBase(string DFSFileName, int NumberOfItems)
      : base(DFSFileName, NumberOfItems)
    {
      //Create the header
      _headerPointer = DfsDLLWrapper.dfsHeaderCreate(FileType.EqtimeFixedspaceAllitems, "Title", "HydroNumerics", 1, NumberOfItems, StatType.NoStat);
      _timeAxis = TimeAxisType.CalendarEquidistant;
    }

    public DFS2DBase(string DFSFileName, int NumberOfItems, IGrid Grid):this(DFSFileName, NumberOfItems)
    {
      CopyGrid(Grid);
    }


    public DFS2DBase(string DFSFileName, DFS2DBase DFSTemplate)
      : this(DFSFileName, DFSTemplate.NumberOfItems)
    {
      CopyItemInfo(DFSTemplate);
      CopyFromTemplate(DFSTemplate);
    }

    public override void CopyFromTemplate(DFSBase DFSTemplate1)
    {
      base.CopyFromTemplate(DFSTemplate1);
      DFS2DBase DFSTemplate = DFSTemplate1 as DFS2DBase;
      CopyGrid(DFSTemplate);
    }


    private void CopyGrid(IGrid Grid)
    {
      this.NumberOfColumns = Grid.NumberOfColumns;
      this.NumberOfRows = Grid.NumberOfRows;
      this.GridSize = Grid.GridSize;
      this.XOrigin = Grid.XOrigin;
      this.YOrigin = Grid.YOrigin;
      this.Orientation = Grid.Orientation;

    }

    /// <summary>
    /// Gets the Column index for this coordinate. Lower left is (0,0). 
    /// Returns -1 if UTMY is left of the grid and -2 if it is right.
    /// </summary>
    /// <param name="UTMY"></param>
    /// <returns></returns>
    public int GetColumnIndex(double UTMX)
    {
      //Calculate as a double to prevent overflow errors when casting 
      double ColumnD = Math.Max(-1, Math.Floor((UTMX - (XOrigin - GridSize / 2)) / GridSize));

      if (ColumnD > _numberOfColumns)
        return -2;
      return (int)ColumnD;
    }

    /// <summary>
    /// Gets the Row index for this coordinate. Lower left is (0,0). 
    /// Returns -1 if UTMY is below the grid and -2 if it is above.
    /// </summary>
    /// <param name="UTMY"></param>
    /// <returns></returns>
    public int GetRowIndex(double UTMY)
    {
      //Calculate as a double to prevent overflow errors when casting 
      double RowD = Math.Max(-1, Math.Floor((UTMY - (YOrigin - GridSize / 2)) / GridSize));

      if (RowD > _numberOfRows)
        return -2;
      return (int)RowD;
    }

    /// <summary>
    /// Gets and sets the number of columns. Not possible to change this setting on existing file
    /// </summary>
    public int NumberOfColumns
    {
      get
      {
        return _numberOfColumns;
      }
      set
      {
        _numberOfColumns = value;
      }
    }

    /// <summary>
    /// Gets and sets the number of rows. Not possible to change this setting on existing file
    /// </summary>
    public int NumberOfRows
    {
      get
      {
        return _numberOfRows;
      }
      set
      {
        _numberOfRows = value;
      }
    }


    /// <summary>
    /// Gets and sets the x-coordinate of the grid the center of the lower left
    /// Remember that MikeShe does not use the center
    /// </summary>
    public double XOrigin
    {
      get
      {
        return _xOrigin;
      }
      set
      {
        if (_xOrigin != value)
        {
          _xOrigin = value;
          WriteGeoInfo();
        }
      }
    }


    /// <summary>
    /// Gets and sets the Y-coordinate of the grid the center of the lower left
    /// Remember that MikeShe does not use the center but the outer boundary
    /// </summary>
    public double YOrigin
    {
      get
      {
        return _yOrigin;
      }
      set
      {
        if (_yOrigin != value)
        {
          _yOrigin = value;
          WriteGeoInfo();
        }
      }
    }

    public bool OriginAtCenter { get; set; }

    /// <summary>
    /// Gets and sets the orientation of the grid
    /// </summary>
    public double Orientation
    {
      get { return _orientation; }
      set
      {
        if (_orientation != value)
        {
          _orientation = value;
          WriteGeoInfo();
        }
      }
    }


    /// <summary>
    /// Gets the grid size.
    /// </summary>
    public double GridSize 
    {
      get
      {
        return _gridSize;
      }
      set
      {
        _gridSize = value;
      }
    }

    /// <summary>
    /// Gets and sets the size of a time step
    /// </summary>
    public new TimeSpan TimeStep
    {
      get
      {
        return _timeStep;
      }
      set
      {
        _timeStep = value;
        WriteTime();
      }
    }

  }
}
