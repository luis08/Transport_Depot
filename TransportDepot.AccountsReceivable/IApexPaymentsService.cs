
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
    IEnumerable<FactoringPayment> ParsePayments(string path);

    [OperationContract]
    void SavePayments(IEnumerable<FactoringPayment> payments);

    [OperationContract]
    [WebInvoke(Method = "POST",
      UriTemplate = "?paymentsCsv={fileName}",
      ResponseFormat=WebMessageFormat.Json)]
    IEnumerable<FactoringPayment> ReadCsv(string fileName, Stream stream);
  }
}
