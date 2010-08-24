using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class Stream : IWaterBody
    {
        #region IWaterBody Members

        public DateTime CurrentTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime EarliestTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime LatestTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void AddWaterPacket(IWaterPacket water)
        {
            throw new NotImplementedException();
        }

        public void Update(DateTime time)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IItem Members

        public string ID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
