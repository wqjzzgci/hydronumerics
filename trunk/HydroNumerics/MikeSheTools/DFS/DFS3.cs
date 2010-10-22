using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero.DFS;


namespace HydroNumerics.MikeSheTools.DFS
{
  public class DFS3:DFS2DBase
  {
    //Static list holding all Matrices read from dfs2-files
    private static Dictionary<string, Dictionary<int, Dictionary<int, CacheEntry>>> SuperCache = new Dictionary<string, Dictionary<int, Dictionary<int, CacheEntry>>>();
    private static LinkedList<CacheEntry> AccessList = new LinkedList<CacheEntry>();
    public static int MaxEntriesInBuffer = 25;

    //DataBuffer. First on Item, then on timeStep. 
    private Dictionary<int, Dictionary<int, CacheEntry>> _bufferData;


        /// <summary>
    /// Creates a new DFS2 file from scratch
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="Title"></param>
    /// <param name="NumberOfItems"></param>
    public DFS3(string FileName, string Title, int NumberOfItems)
      : base(FileName, Title, NumberOfItems)
    {
      _spaceAxis = SpaceAxisType.F_EQ_AXIS_D3;
      _timeAxis = TimeAxisType.F_CAL_EQ_AXIS;
      BuildCache();
    }


    /// <summary>
    /// Provides read access to a .DFS3 file.
    /// </summary>
    /// <param name="DFSFileName"></param>
    public DFS3(string DFSFileName):base(DFSFileName)
    {
      BuildCache();
    }

    private void BuildCache()
    {
      if (!SuperCache.TryGetValue(AbsoluteFileName, out _bufferData))
      {
        _bufferData = new Dictionary<int, Dictionary<int, CacheEntry>>();
        SuperCache.Add(AbsoluteFileName, _bufferData);
      }
    }

    /// <summary>
    /// Returns a Matrix3D with the data for the TimeStep, Item
    /// TimeStep counts from 0, Item from 1.
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Item"></param>
    /// <returns></returns>
    public Matrix3d GetData(int TimeStep, int Item)
    {
      Dictionary<int, CacheEntry> _timeValues;
      CacheEntry cen;

      if (!_bufferData.TryGetValue(Item, out _timeValues))
      {
        _timeValues = new Dictionary<int, CacheEntry>();
        _bufferData.Add(Item, _timeValues);
      }
      if (!_timeValues.TryGetValue(TimeStep, out cen))
      {
        ReadItemTimeStep(TimeStep, Item);
        Matrix3d _data = new Matrix3d(_numberOfRows, _numberOfColumns, _numberOfLayers, dfsdata);
        cen = new CacheEntry(AbsoluteFileName, Item, TimeStep, _data);
        _timeValues.Add(TimeStep, cen);
        CheckBuffer();
      }
      else
        AccessList.Remove(cen);

      AccessList.AddLast(cen);
      return cen.Data3d;
    }
    /// <summary>
    /// NOTE: Cannot write data to a cell with a delete value in .dfs3-files written by MikeShe because it uses file compression
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <param name="Item"></param>
    /// <param name="Data"></param>
    public void SetData(int TimeStep, int Item, Matrix3d Data)
    {
            float[] fdata = new float[Data[0].ColumnCount * Data[0].RowCount*Data.LayerCount]; 
      int m = 0;
      for (int k = 0;k<Data.LayerCount;k++)
      for (int i = 0; i < Data[0].RowCount; i++)
        for (int j = 0; j < Data[0].ColumnCount; j++)
        {
          fdata[m] = (float) Data[k][i, j];
          m++;
        }
      WriteItemTimeStep(TimeStep, Item, fdata);

      //Now add to buffer
      Dictionary<int, CacheEntry> _timeValues;
      CacheEntry cen;

      if (!_bufferData.TryGetValue(Item, out _timeValues))
      {
        _timeValues = new Dictionary<int, CacheEntry>();
        _bufferData.Add(Item, _timeValues);
      }
      if (!_timeValues.TryGetValue(TimeStep, out cen))
      {
        cen = new CacheEntry(AbsoluteFileName, Item, TimeStep, Data);
        _timeValues.Add(TimeStep, cen);
        CheckBuffer();
      }
      else
        AccessList.Remove(cen);

      AccessList.AddLast(cen);


    }


    /// <summary>
    /// Removes the oldest Matrix from the dictionary if the Accesslist contains more than MaxNumberOfEntries
    /// </summary>
    private void CheckBuffer()
    {
      if (AccessList.Count > MaxEntriesInBuffer)
      {
        CacheEntry ToRemove = AccessList.First.Value;
        SuperCache[ToRemove.FileName][ToRemove.Item].Remove(ToRemove.TimeStep);
        AccessList.RemoveFirst();
      }
    }


    /// <summary>
    /// Gets and sets the number of Layers
    /// </summary>
    public int NumberOfLayers
    {
      get
      {
        return _numberOfLayers;
      }
      set
      {
        _numberOfLayers = value;
      }
    }

  }
}
