using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using HydroNumerics.OpenMI.Sdk.Wrapper;

namespace HydroNumerics.HydroNet.OpenMI
{
    public class LinkableComponent : LinkableEngine
    {

        protected override void SetEngineApiAccess()
        {
            this._engineApiAccess = new EngineWrapper();
        }
    }
}
