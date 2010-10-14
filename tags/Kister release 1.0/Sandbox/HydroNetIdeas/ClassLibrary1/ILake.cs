using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public interface ILake : IWaterBody
    {
        double WaterLevel
        {
            get;
        }
    }
}
