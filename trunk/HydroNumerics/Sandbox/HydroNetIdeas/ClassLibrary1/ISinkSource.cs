using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public interface IBoundary
    {
        bool IsSink { get; }
        bool IsSource { get; }

    }
}
