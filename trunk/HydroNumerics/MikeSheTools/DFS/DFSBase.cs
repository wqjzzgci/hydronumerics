using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using DHI.Generic.MikeZero.DFS;

namespace HydroNumerics.MikeSheTools.DFS
{

  /// <summary>
  /// Abstract class that handles all direct access to .dfs-files. Uses static methods from DFSWrapper in 
  /// DHI.Generic.MikeZero.DFS.dll as well as direct calls into the ufs.dll
  /// </summary>
  public abstract class DFSBase
  {
    #region Calls directly into ufs.dll because the wrapped call does not work on vista due to something with strings.
    const string UFSDll = "ufs.dll";  // Name of dll. Should be in path

    /// <summary>
    /// Call directly into ufs.dll because the wrapped call does not work on vista due to something with strings.
    /// </summary>
    /// <param name="ItemPointer"></param>
    /// <param name="ItemType"></param>
    /// <param name="Name"></param>
    /// <param name="Unit"></param>
    /// <param name="DataType"></param>
    /// <returns></returns>
    [DllImport(UFSDll, CharSet = CharSet.None, CallingConvention = CallingConvention.StdCall)]
    internal extern static int dfsGetItemInfo_(IntPtr ItemPointer, ref int ItemType, ref IntPtr Name, ref IntPtr Unit, ref int DataType);


    /// <summary>
    /// Call directly into ufs.dll because the wrapped call does not work on vista due to something with strings.
    /// </summary>
    /// <param name="HeaderPointer"></param>
    /// <param name="Projection"></param>
    /// <param name="longitude"></param>
    /// <param name="Latitude"></param>
    /// <param name="Orientation"></param>
    /// <returns></returns>
    [DllImport(UFSDll, CharSet = CharSet.None, CallingConvention = CallingConvention.StdCall)]
    internal extern static int dfsGetGeoInfoUTMProj(IntPtr HeaderPointer, ref IntPtr Projection, ref double longitude, ref double Latitude, ref double Orientation);

    #endregion

    private int _currentTimeStep = -1;
    private int _currentItem = -1;
    protected IntPtr _filePointer = IntPtr.Zero;
    protected IntPtr _headerPointer = IntPtr.Zero;
    protected bool _initializedForWriting = false;
    private string _filename;
    private DateTime _firstTimeStep;
    protected TimeSpan _timeStep = TimeSpan.Zero;

    protected TimeAxisType _timeAxis;
    protected SpaceAxisType _spaceAxis;

    protected string AbsoluteFileName;

    protected float[] dfsdata; //Buffer used to fill data into

    protected int _numberOfLayers = 1;
    protected int _numberOfColumns = 1;
    protected int _numberOfRows = 1;

    protected double _xOrigin=0;
    protected double _yOrigin=0;
    protected double _orientation = 0;
    protected double _gridSize=0;


    private int _status;

    #region Constructors

    /// <summary>
    /// Creates a new .dfs file
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="Title"></param>
    /// <param name="NumberOfItems"></param>
    public DFSBase(string DFSFileName, string Title, int NumberOfItems)
    {
      _filename = DFSFileName;
      AbsoluteFileName = Path.GetFullPath(DFSFileName);

      //Create the header
      LastStatus = DFSWrapper.dfsHeaderCreate(1, Title, "HydroNumerics", 1, NumberOfItems, 1, ref _headerPointer);
      Items = new Item[NumberOfItems];

      //Gets the pointers to the items
      for (int i = 0; i < NumberOfItems; i++)
      {
        Items[i] = new Item(DFSWrapper.dfsItemD(_headerPointer, i + 1), this);
      }
      _initializedForWriting = true;
    }

    /// <summary>
    /// Opens an existing dfs-file
    /// </summary>
    /// <param name="DFSFileName"></param>
    public DFSBase(string DFSFileName)
    {
      _filename = DFSFileName;
      AbsoluteFileName = Path.GetFullPath(DFSFileName);

      LastStatus = DFSWrapper.dfsFileRead(DFSFileName, ref _headerPointer, ref _filePointer);
      if (LastStatus != 0)
        return; //Not a valid file. 

      int nitems = DFSWrapper.dfsGetNoOfItems(_headerPointer);
      Items = new Item[nitems];

      //Gets the pointers and create the items items
      for (int i = 1; i <= nitems; i++)
      {
        Items[i - 1] = new Item(DFSWrapper.dfsItemD(_headerPointer, i), this);
      }

      string eum_unit = "";
      int unit = 0;
      int data_type = 0;
      int item_type = 0;

      float x = 0;
      float y = 0;
      float z = 0;

      float dx = 0;
      float dy = 0;
      float dz = 0;

      IntPtr name = new IntPtr();

      //Reads the projection
      LastStatus = dfsGetGeoInfoUTMProj(_headerPointer, ref name, ref _xOrigin, ref _yOrigin, ref _orientation);

      //Reads the space axis
      _spaceAxis = (SpaceAxisType)DFSWrapper.dfsGetItemAxisType(FirstItem.ItemPointer);

      //Now read axes info dependent on the type of axis
      switch (_spaceAxis)
      {
        case SpaceAxisType.F_EQ_AXIS_D0:
          break;
        case SpaceAxisType.F_EQ_AXIS_D1:
          break;
        case SpaceAxisType.F_EQ_AXIS_D2:       //DFS2 from MikeShe
          LastStatus = DFSWrapper.dfsGetItemAxisEqD2(FirstItem.ItemPointer, ref item_type, ref eum_unit, ref _numberOfColumns, ref _numberOfRows, ref x, ref y, ref dx, ref dy);
          break;
        case SpaceAxisType.F_EQ_AXIS_D3: //DFS3 from MikeShe
          LastStatus = DFSWrapper.dfsGetItemAxisEqD3(FirstItem.ItemPointer, ref item_type, ref eum_unit, ref _numberOfColumns, ref _numberOfRows, ref _numberOfLayers, ref x, ref y, ref z, ref dx, ref dy, ref dz);
          break;
        case SpaceAxisType.F_EQ_AXIS_D4:
          break;
        case SpaceAxisType.F_NEQ_AXIS_D1:
          IntPtr coords = new IntPtr();
          LastStatus = DFSWrapper.dfsGetItemAxisNeqD1(FirstItem.ItemPointer, ref unit, ref eum_unit, ref data_type, ref coords);
          break;
        case SpaceAxisType.F_NEQ_AXIS_D2:
          break;
        case SpaceAxisType.F_NEQ_AXIS_D3:
          break;
        case SpaceAxisType.F_TVAR_AXIS_D1:
          break;
        case SpaceAxisType.F_TVAR_AXIS_D2:
          break;
        case SpaceAxisType.F_TVAR_AXIS_D3:
          break;
        case SpaceAxisType.F_UNDEFINED_SAXIS:
          break;
        default:
          break;
      }

      _gridSize = dx;

      //Prepares an array of floats to recieve the data
      dfsdata = new float[_numberOfColumns * _numberOfRows * _numberOfLayers];

      //Now look at time axis
      _timeAxis = (TimeAxisType) DFSWrapper.dfsGetTimeAxisType(_headerPointer);
      string startdate = "";
      string starttime = "";
      double tstart = 0;
      double tstep = 0;
      int nt = 0;
      int tindex = 0;

      switch (_timeAxis)
      {
        case TimeAxisType.F_CAL_EQ_AXIS: //Dfs2 and dfs3 always here
          LastStatus = DFSWrapper.dfsGetEqCalendarAxis(_headerPointer, ref startdate, ref starttime, ref unit, ref eum_unit, ref tstart, ref tstep, ref nt, ref tindex);
          if (unit == 1400)
            _timeStep = TimeSpan.FromSeconds(tstep);
          else if (unit == 1402)
            _timeStep = TimeSpan.FromHours(tstep);
          break;
        case TimeAxisType.F_CAL_NEQ_AXIS://Only dfs0 can have varying time steps
          LastStatus = DFSWrapper.dfsGetNeqCalendarAxis(_headerPointer, ref startdate, ref starttime, ref unit, ref eum_unit, ref tstart, ref tstep, ref nt, ref tindex);
          break;
        case TimeAxisType.F_TM_EQ_AXIS:
          break;
        case TimeAxisType.F_TM_NEQ_AXIS:
          break;
        case TimeAxisType.F_UNDEFINED_TAXIS:
          break;
        default:
          break;
      }


      NumberOfTimeSteps = nt;
      TimeSteps = new DateTime[NumberOfTimeSteps];

      if (startdate != null & starttime != null)
      {
        _firstTimeStep = DateTime.Parse(startdate).Add(TimeSpan.Parse(starttime));
        TimeSteps[0] = _firstTimeStep;
      }

      for (int i = 1; i < nt; i++)
      {
        if (_timeAxis == TimeAxisType.F_CAL_NEQ_AXIS)
        {
          if (unit == 1400)
            TimeSteps[i] = _firstTimeStep.AddSeconds(ReadItemTimeStep(i, 1));
          else if (unit == 1402)
            TimeSteps[i] = _firstTimeStep.AddHours(ReadItemTimeStep(i, 1));
        }
        else
          TimeSteps[i] = TimeSteps[i - 1].Add(_timeStep);
      }
    }

    #endregion

    #region Read methods

    /// <summary>
    /// Returns the zero-based index of the TimeStep closest to the TimeStamp. If the timestamp falls exactly between two timestep the smallest is returned.
    /// If the TimeStamp is before the first timestep 0 is returned.
    /// If the TimeStamp is after the last timestep the index of the last timestep is returned
    /// </summary>
    /// <param name="TimeStamp"></param>
    /// <returns></returns>
    public int GetTimeStep(DateTime TimeStamp)
    {
      if (TimeStamp < _firstTimeStep || NumberOfTimeSteps == 1)
        return 0;
      int TimeStep;
      //fixed timestep
      if (_timeAxis== TimeAxisType.F_CAL_EQ_AXIS)
        TimeStep = (int)Math.Round(TimeStamp.Subtract(_firstTimeStep).TotalSeconds / _timeStep.TotalSeconds, 0);
      //Variabale timestep
      else
      {
        //Last timestep is known
        if (TimeStamp >= TimeSteps[TimeSteps.Length - 1])
          return TimeSteps.Length - 1;

        int i = 1;
        //Loop the timesteps
        while (TimeStamp > TimeSteps[i])
        {
          i++;
        }
        //Check if last one was actually closer
        if (TimeSteps[i].Subtract(TimeStamp) < TimeStamp.Subtract(TimeSteps[i - 1]))
          return i;
        else
          return i - 1;
      }
      return Math.Min(NumberOfTimeSteps, TimeStep);
    }

    /// <summary>
    /// Reads data for the TimeStep and Item if necessary and fills them into the buffer.
    /// Time steps counts from 0 and Item from 1.
    /// In case of nonequidistant time (only dfs0) it returns the timestep as double
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Item"></param>
    protected double ReadItemTimeStep(int TimeStep, int Item)
    {
      double time = 0;

      if (TimeStep != _currentTimeStep || Item != _currentItem)
      {
        _currentTimeStep = TimeStep;
        _currentItem = Item;
        //Spools to the correct Item and TimeStep
        LastStatus = DFSWrapper.dfsFindItemDynamic(_headerPointer, _filePointer, TimeStep, Item);
        if (LastStatus != 0)
          throw new Exception("Could not find TimeStep number: " + TimeStep + " and Item number: " + Item);

        //Reads the data
        LastStatus = DFSWrapper.dfsReadItemTimeStep(_headerPointer, _filePointer, ref time, dfsdata);
        if (LastStatus != 0)
          throw new Exception("Error in file: " + _filename + " reading timestep number: " + this._currentTimeStep);
      }
      return time;
    }

    #endregion

    #region Write methods

    /// <summary>
    /// Writes data for the TimeStep and Item
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Item"></param>
    protected void WriteItemTimeStep(int TimeStep, int Item, float[] data)
    {
      if (!_initializedForWriting)
        InitializeForWriting();

      if (_filePointer == IntPtr.Zero)
        CreateFile();

      //Spools to the correct Item and TimeStep. Does this fail when at the end of file?
      LastStatus = DFSWrapper.dfsFindItemDynamic(_headerPointer, _filePointer, TimeStep, Item);
      //      if (ok != 0)
      //          throw new Exception("Could not find TimeStep number: " + TimeStep + " and Item number: " + Item);

      double time = 50;

      //Writes the data
      LastStatus = DFSWrapper.dfsWriteItemTimeStep(_headerPointer, _filePointer, time, data);
      if (LastStatus != 0)
        throw new Exception("Error writing timestep number: " + _currentTimeStep);
    }

    /// <summary>
    /// Opens the file for writing. First closes the file since it has already been opened for reading
    /// </summary>
    protected void InitializeForWriting()
    {
      Dispose(false);
      LastStatus = DFSWrapper.dfsFileEdit(_filename, ref _headerPointer, ref _filePointer);
      if (LastStatus != 0)
        throw new Exception("Error in initializing file : " + _filename + " for writing");
      _initializedForWriting = true;
    }

    private void CreateFile()
    {
      WriteGeoInfo();
      WriteTime();
      foreach (Item I in Items)
      {
        WriteItemInfo(I);
        if (_spaceAxis== SpaceAxisType.F_EQ_AXIS_D2)
          LastStatus = DFSWrapper.dfsSetItemAxisEqD2(I.ItemPointer, 1000, _numberOfColumns, _numberOfRows, 0, 0, (float)_gridSize, (float)_gridSize);
        else if (_spaceAxis == SpaceAxisType.F_EQ_AXIS_D3)
          LastStatus = DFSWrapper.dfsSetItemAxisEqD3(I.ItemPointer, 1000, _numberOfColumns, _numberOfRows, _numberOfLayers, 0, 0, 0, (float)_gridSize, (float)_gridSize, (float)_gridSize);
      }
      LastStatus = DFSWrapper.dfsFileCreate(FileName, _headerPointer, ref _filePointer);
    }

    protected void WriteGeoInfo()
    {
      if (!_initializedForWriting)
        InitializeForWriting();
      LastStatus = DFSWrapper.dfsSetGeoInfoUTMProj(_headerPointer, "NON-UTM", _xOrigin, _yOrigin, _orientation);
    }

    internal void WriteItemInfo(Item I)
    {
      if (!_initializedForWriting)
        InitializeForWriting();
      LastStatus = DFSWrapper.dfsSetItemInfo(_headerPointer, I.ItemPointer, (int)I.EumItem, I.Name, (int) I.EumUnit, 1);
    }

    /// <summary>
    /// Writes timestep and starttime
    /// Because it is called twice
    /// </summary>
    protected void WriteTime()
    {
      if (!_initializedForWriting)
        InitializeForWriting();
      LastStatus = DFSWrapper.dfsSetEqCalendarAxis(_headerPointer, _firstTimeStep.ToString("yyyy-MM-dd"), _firstTimeStep.ToString("hh:mm:ss"), 1400, 0, _timeStep.TotalSeconds, 0);
    }

    #endregion

    #region Properties
    /// <summary>
    /// Gets the items
    /// </summary>
    public Item[] Items { get; private set; }

    /// <summary>
    /// Gets the status code from the last call by DFSWrapper
    /// </summary>
    protected int LastStatus
    {
      get { return _status; }
      set
      {
        _status = value;
        if (_status != 0)
        {
          string error = "fejl";
        }
      }
    }

    /// <summary>
    /// Gets the first item. There should always be at least one item
    /// </summary>
    public Item FirstItem
    {
      get
      {
        return Items[0];
      }
    }


    /// <summary>
    /// Gets an array with the timesteps.
    /// </summary>
    public DateTime[] TimeSteps { get; private set; }

    /// <summary>
    /// Gets and sets the date and time of the first time step.
    /// </summary>
    public DateTime TimeOfFirstTimestep
    {
      get
      {
        return _firstTimeStep;
      }
      set
      {
        _firstTimeStep = value;
        WriteTime();
      }
    }

    /// <summary>
    /// Gets the size of a time step
    /// </summary>
    public TimeSpan TimeStep
    {
      get
      {
        return _timeStep;
      }
    }


    /// <summary>
    /// Gets the DeleteValue from the DFS-file
    /// </summary>
    public double DeleteValue
    {
      get
      {
        return DFSWrapper.dfsGetDeleteValFloat(_headerPointer);
      }
    }


    /// <summary>
    /// Gets the FileName
    /// </summary>
    public string FileName
    {
      get
      {
        return _filename;
      }
    }


    /// <summary>
    /// Gets the number of timesteps
    /// </summary>
    public int NumberOfTimeSteps
    {
      get;
      protected set;
    }


    #endregion

    #region Dispose methods

    /// <summary>
    /// Override of the Dispose method in DFSFileInfo which probably does not account for finalization
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        dfsdata = null;
      }
      LastStatus = DFSWrapper.dfsFileClose(_headerPointer, ref _filePointer);
    }

    /// <summary>
    /// Destructor called when the object is garbage collected.
    /// </summary>
    ~DFSBase()
    {
      // Simply call Dispose(false).
      Dispose(false);
    }

    #endregion

  }
}
