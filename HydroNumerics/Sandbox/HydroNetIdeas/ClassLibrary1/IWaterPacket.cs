using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLibrary1
{
    public interface IWaterPacket
    {
        /// <summary>
        /// The water volume of the water packet
        /// </summary>
        double WaterVolume { get; }


        /// <summary>
        /// Start time for the period during which the water packet is added to the water body
        /// </summary>
        DateTime AddedFrom { get; }

        /// <summary>
        /// End time for the period during which the water packet is added to the water body
        /// </summary>
        DateTime AddedUntil { get; }

        /// <summary>
        /// Handle decay of species
        /// </summary>
        /// <param name="time"></param>
        void UpdateTo(DateTime time);
    }
}
