using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class WaterPacket : IWaterPacket
    {
        #region IWaterPacket Members

        public void Initialize(DateTime time)
        {
        }

        public double WaterVolume
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DateTime AddedFrom
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public DateTime AddedUntil
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void UpdateTo(DateTime time)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
