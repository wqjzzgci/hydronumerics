using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class CleanWaterPacket : IWaterPacket
    {
        #region IWaterPacket Members

        public void MoveInTime(TimeSpan timespan)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
