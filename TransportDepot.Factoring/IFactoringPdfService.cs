
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
namespace TransportDepot.Factoring
{
  [ServiceContract(Namespace = "http://transportdepot.net/factoring")]
  interface IFactoringPdfService
  {
    [OperationContract]
    [WebGet(UriTemplate = "pdf?id={id}")]
    Stream GetSchedulePdf(int id);
  }
}
