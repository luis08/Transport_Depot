
using System.ServiceModel;
using System.ServiceModel.Web;
using TransportDepot.Models.Reports;

namespace TransportDepot.Reports.Json
{
  [ServiceContract(Namespace = "http://transportdepot.net/reports")]
  public interface IJsonReportService
  {
    [OperationContract]
    [WebInvoke(Method = "POST", ResponseFormat= WebMessageFormat.Json)]
    JsonReport GetReport(JsonReportRequest request);
  }
}
