using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public interface IWaterBody
    {
        String Id { get; }

        String Name { get; }
        
        DateTime CurrentTime
        {
            get;
        }

        DateTime ValidFrom
        {
            get;
        }

        DateTime ValidTo
        {
            get;
        }

        IWaterBody DownstreamWaterBody { get; set; }

        List<IWaterPacket> WaterPackets {get;}

        void InitializeAt(DateTime time);
        void UpdateTo(DateTime time);
    }
}
