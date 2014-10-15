using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace TransportDepot.Reports
{
  [ServiceContract(Namespace="http://transportdepot.net/reports/accounts-receivable")]
  interface IARReportService
  {
    [OperationContract]
    [WebGet(UriTemplate = "collections?ids={ids}")]
    Stream GetCollectionLettersForCustomers(string ids);

    [OperationContract]
    [WebGet(UriTemplate = "final-letters?ids={ids}")]
    Stream GetFinalLettersForCustomers(string ids);

    [OperationContract]
    [WebGet(UriTemplate = "collections-report?type={type}&sort={sort}")]
    Stream GetCollectionsReport(string type, string sort);

    [OperationContract]
    [WebGet(UriTemplate="collections-all")]
    Stream GetCollectionsReportAll();

    [OperationContract]
    [WebGet(UriTemplate = "collections-factored")]
    Stream GetCollectionsReportFactored();
    
    [OperationContract]
    [WebGet(UriTemplate = "collections-quick-pay")]
    Stream GetCollectionsReportQuickPay();
  }
}
