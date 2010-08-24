using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public interface IWaterBody : IItem
    {
        DateTime CurrentTime
        {
            get;
            set;
        }

        DateTime EarliestTime
        {
            get;
            set;
        }

        DateTime LatestTime
        {
            get;
            set;
        }
    
    
        void AddWaterPacket(IWaterPacket water);

        void Update(DateTime time);
    }
}
