using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.Core;


namespace Dfs3plotdfs0
{
  public class ExtractData
  {
    private Model mShe;

    public ExtractData(string FileName)
    {
      mShe = new Model(FileName);

      
    }

  }
}
