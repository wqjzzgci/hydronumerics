using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Core
{
    public class UnitOptions
    {
        bool returnValuesInSiUnits = false;
        bool returnValuesInSpecifiedUnit = false;
        Unit returnUnit = null;
        double relaxationFactor = 0.0; 

        public bool ReturnValueInSiUnit
        {
            get { return returnValuesInSiUnits; }
        }

        public UnitOptions(bool returnValuesInSiUnits)
        {
            this.returnValuesInSiUnits = returnValuesInSiUnits;
            returnUnit = null; 
        }


    }
}
