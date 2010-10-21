using System;
using System.Runtime.InteropServices;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;
using DHI.Generic.MikeZero.DFS;

namespace HydroNumerics.MikeSheTools.DFS
{
  public class Item
  {
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
    private extern static int dfsGetItemInfo_(IntPtr ItemPointer, ref int ItemType, ref IntPtr Name, ref IntPtr Unit, ref int DataType);


    public IntPtr ItemPointer { get; private set; }
    public string Name { get; set; }
    public eumItem EumItem {get; set;}
    public string EumUnit {get;set;}


    internal Item(IntPtr ItemPointer)
    {
            int item_type = 0;
      int data_type = 0;
      IntPtr name = new IntPtr();
      IntPtr Eum = new IntPtr();

      this.ItemPointer = ItemPointer;
              Status= dfsGetItemInfo_(ItemPointer, ref item_type, ref name, ref Eum, ref data_type);
        Name = (Marshal.PtrToStringAnsi(name));
        EumUnit = (Marshal.PtrToStringAnsi(Eum));
      EumItem = (eumItem)item_type;
    }

        private int _status;


    public int Status
    {
      get { return _status; }
      set {
        _status = value;
        if (_status != 0)
        {
          string error = "fjel";
        }
      }
    }

  }
}
