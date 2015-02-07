
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using TransportDepot.Models.Factoring;
using System.IO;
using TransportDepot.Utilities.Csv;
namespace TransportDepot.AccountsReceivable
{
  [ServiceContract]
  public interface IApexPaymentsService
  {
    [OperationContract]
    ApexPaymentBatch ParsePayments(string path);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    void SavePayments(IEnumerable<FactoringPayment> payments);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<string> GetExistingPayments(IEnumerable<FactoringPayment> payments);

    [OperationContract]
    [WebInvoke(Method = "POST",
      UriTemplate = "?paymentsCsv={fileName}",
      ResponseFormat=WebMessageFormat.Json)]
    ApexPaymentBatch ReadCsv(string fileName, Stream stream);
  }
}
