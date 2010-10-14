using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;



namespace SilverlightApplication1.Web
{
  [ServiceContract(Name = "IPolicyRetriever")]
  public interface IPolicyRetriever
  {
    [OperationContract, WebGet(UriTemplate = "/clientaccesspolicy.xml")]
    Stream GetSilverlightPolicy();
  }
}
