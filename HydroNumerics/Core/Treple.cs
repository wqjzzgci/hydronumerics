using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
  public struct Treple<T1, T2, T3>
  {
    private T1 first;
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

    private readonly T3 third;
    public T3 Third
    {
      get { return third;}
    }

    public Treple(T1 f, T2 s, T3 t)
    {
      first = f;
      second = s;
      third = t;
    }

  }
}
