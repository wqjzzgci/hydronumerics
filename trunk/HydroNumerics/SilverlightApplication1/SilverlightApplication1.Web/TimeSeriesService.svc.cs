using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;

namespace SilverlightApplication1.Web
{
  [ServiceContract(Namespace = "")]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class TimeSeriesService
  {
    [OperationContract]
    public TimespanSeries DoWork()
    {
      Model m = ModelFactory.GetModel(@"C:\Users\Jacob\Work\HydroNumerics\HydroNet\TestData\setup.xml");

      return ((Lake)m._waterBodies.First()).Output.Evaporation;

    }

    // Add more operations here and mark them with [OperationContract]
  }
}
