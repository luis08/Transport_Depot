
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using TransportDepot.Models.Factoring;
using System.IO;
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
      UriTemplate = "?apexCsv={fileName}")]
    IEnumerable<FactoringPayment> ReadCsv(string fileName, Stream stream);
  }
}
