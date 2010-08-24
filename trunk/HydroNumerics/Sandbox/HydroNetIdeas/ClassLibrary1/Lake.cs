using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class Lake : IWaterBody
    {
        #region IWaterBody Members


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

        public ISource Precipitation
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ISink Evaporation
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ISinkSource GroundWater
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

    }
}
