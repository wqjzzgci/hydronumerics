using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
  public struct Tuple<T1, T2>
  {
    private  T1 first;
    public T1 First
    {
      get { return first; }
      set { first = value; }
    }

    private T2 second;
    public T2 Second
    {
      get { return second; }
      set { second = value; }
    }

    public Tuple(T1 f, T2 s)
    {
      first = f;
      second = s;
    }

  }
}
