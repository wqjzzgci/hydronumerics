using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace DataConnectorWCF
{
    [ServiceContract]
    public interface ITile
    {

        /// <summary>
        /// ITile - Tile Interface
        /// </summary>
        /// <param name="table">string table - DB table to query</param>
        /// <param name="quadkey">string quadkey</param>
        /// <param name="thematicstr">string thematicstr - true || false</param>
        /// <returns>Stream - tile png</returns>
        [OperationContract]
        [WebGet( UriTemplate="{table}/{quadkey}/{thematicstr}" )]

        Stream GetTile(string table, string quadkey, string thematicstr);
    }
}
