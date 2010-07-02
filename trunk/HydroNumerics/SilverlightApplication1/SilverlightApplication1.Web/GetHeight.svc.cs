using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

namespace SilverlightApplication1.Web
{
  [ServiceContract(Namespace = "")]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class GetHeight
  {
    [OperationContract]
    public double DoWork(double Latitude, double Longitude)
    {
      return HydroNumerics.Geometry.Net.KMSData.GetHeight(Latitude, Longitude);
    }

    // Add more operations here and mark them with [OperationContract]
  }
}
