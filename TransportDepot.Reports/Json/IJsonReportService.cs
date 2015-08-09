
using System.ServiceModel;
using System.ServiceModel.Web;
using TransportDepot.Models.Reports;
using System.Collections.Generic;

namespace TransportDepot.Reports.Json
{
  [ServiceContract]
  public interface IJsonReportService
  {
    [OperationContract]
    [WebInvoke(Method = "POST", ResponseFormat=WebMessageFormat.Json,
      BodyStyle=WebMessageBodyStyle.WrappedRequest,
      UriTemplate="reports/report")]
    JsonReport GetReport(JsonReportRequest request);

    [OperationContract]
    [WebInvoke(Method="GET", UriTemplate="reports")]
    IEnumerable<JsonReportSpecs> GetReportSpecs();
  }
}
