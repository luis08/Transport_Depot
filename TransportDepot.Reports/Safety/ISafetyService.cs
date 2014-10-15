
using System.ServiceModel;
using System.IO;
using TransportDepot.Safety;
using System.ServiceModel.Web;
namespace TransportDepot.Reports.Safety
{
  [ServiceContract(Namespace = "http://transportdepot.net/reports/safety")]
  interface ISafetyService
  {
    [OperationContract]
    [WebGet(UriTemplate="DriverSafetyReport")]
    Stream GetDriverSafety();

    [OperationContract]
    [WebGet(UriTemplate = "TractorSafetyReport")]
    Stream GetTractorSafety();

    [OperationContract]
    [WebGet(UriTemplate = "TrailerSafetyReport")]
    Stream GetTrailerSafety();
  }
}
