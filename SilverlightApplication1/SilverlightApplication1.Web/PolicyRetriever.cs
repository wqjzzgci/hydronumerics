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
  public class PolicyRetriever : IPolicyRetriever
  {


    #region IPolicyRetriever Members

    public Stream GetSilverlightPolicy()
    {
      WebOperationContext.Current.OutgoingResponse.ContentType = "application/xml";

      string result = @"<?xml version=""1.0"" encoding=""utf-8""?>
<access-policy>
    <cross-domain-access>
        <policy>
            <allow-from http-request-headers=""*"">
                <domain uri=""*""/>
            </allow-from>
            <grant-to>
                <resource path=""/"" include-subpaths=""true""/>
            </grant-to>
        </policy>
    </cross-domain-access>
</access-policy>";

      return new MemoryStream(Encoding.UTF8.GetBytes(result));
    }

    #endregion

  }
}