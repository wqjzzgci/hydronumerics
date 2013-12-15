using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HydroNumerics.Geometry
{
  public class IntegerGrid:BaseGrid
  {
    private int[,] data;

    public int[,] Data
    {
      get
      {
        if (data == null)
          data = new int[NumberOfColumns, NumberOfRows];
        return data;
      }
    }
  }


}
