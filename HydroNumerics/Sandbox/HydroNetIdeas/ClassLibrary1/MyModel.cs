using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public class MyModel
    {

        public void ConfigAndRun()
        {

            List<IWaterBody> waterBodies = new List<IWaterBody>();

            Stream stream = new Stream();

            stream.CurrentTime = stream.EarliestTime;
            IWaterPacket cleanWater = new CleanWaterPacket();
            stream.AddWaterPacket(cleanWater);


            
           

                        
        }

    }
}
