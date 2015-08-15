
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
namespace TransportDepot.Reports.Dispatch
{
  [ServiceContract]
  public interface IDispatchReports
  {
    [OperationContract]
    [WebGet(UriTemplate="moving-freight")]
    Stream GetMovingFreight();

    [OperationContract]
    [WebGet(UriTemplate = "driver-contacts")]
    Stream GetDriverContacts();
  }
}
