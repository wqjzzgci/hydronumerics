using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;

using HydroNumerics.Time.Core;

namespace HydroNumerics.Time.Web.Host
{
  [ServiceContract(Namespace = "")]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class TimeSeriesService
  {
    [OperationContract]
    public TimestampSeries GetTimeStampSeries()
    {
      TimestampSeries ts = new TimestampSeries();
      ts.Name = "Tidsserie1";
      ts.AddSiValue(new DateTime(2000, 1, 1), 3);
      ts.AddSiValue(new DateTime(2000, 1, 2), 4);
      ts.AddSiValue(new DateTime(2000, 1, 3), 6);
      ts.AddSiValue(new DateTime(2000, 1, 4), 11);
      return ts;
    }

    // Add more operations here and mark them with [OperationContract]
  }
}
