using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public interface IConnection
    {
        IWaterBody FromContainer
        {
            get;
            set;
        }

        IWaterBody ToContainer
        {
            get;
            set;
        }
    }
}
