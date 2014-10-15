
using System.ServiceModel;
using System.Collections.Generic;
using TransportDepot.Models.DB;
using TransportDepot.Models.AR;
using System.ServiceModel.Web;
namespace TransportDepot.AccountsReceivable
{
  [ServiceContract(Namespace = "http://transportdepot.net/accounts-receivable")]
  interface IAccountsReceivableService
  {
    [OperationContract]
    IEnumerable<CustomerAging> GetAging();

    [OperationContract]
    IEnumerable<CustomerAging> GetAgingForCustomers(IEnumerable<string> ids);

    [OperationContract]
    IEnumerable<CustomerAgingSummary> GetAgingSummaryForCustomers(IEnumerable<string> ids);

    [OperationContract]
    IEnumerable<CollectionLetter> GetCollectionLetters(IEnumerable<string> clientIds);

    [OperationContract]
    [WebGet(UriTemplate="aging-summary")]
    IEnumerable<Models.AR.CustomerAgingSummary> GetAgingSummary();

    [OperationContract]
    [WebInvoke(Method="POST", ResponseFormat=WebMessageFormat.Json)]
    IEnumerable<Models.AR.CustomerAgingSummary> GetOverdueAgingSummary();

    [OperationContract]
    [WebInvoke(Method="POST")]
    IEnumerable<Models.AR.CustomerAgingSummary> GetAgingSummaryExceedingDays(int dayCount);

    [OperationContract]
    IEnumerable<CollectionsReportCustomer> GetAgingDetails(int type);

    [OperationContract]
    IEnumerable<CollectionsReportCustomer> GetAllAgingDetails();
  }
}
