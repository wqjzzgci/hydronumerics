using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using System.Text;
using MikeSheWrapper.JupiterTools;
using MikeSheWrapper.Tools;

namespace SilverlightApplication1.Web
{
  [ServiceContract(Namespace = "")]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class WellWrapper
  {
    
    [OperationContract]
    public JupiterWell GetX()
    {
      SQLServerReader sr = new SQLServerReader();
      Dictionary<string, IWell> wells = sr.Wells();

      return (JupiterWell)wells.Values.First(var => var.Intakes.FirstOrDefault() != null & var.Intakes.FirstOrDefault().Observations.Count > 2);
    }

    // Add more operations here and mark them with [OperationContract]
  }
}
