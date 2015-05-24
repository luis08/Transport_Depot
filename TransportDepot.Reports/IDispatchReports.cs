
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
namespace TransportDepot.Reports.Dispatch
{
  public interface IDispatchReports
  {
    [OperationContract]
    [WebGet(UriTemplate = "/movingFreight")]
    Stream GetMovingFreight();
  }
}
