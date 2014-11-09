using System.IO;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using TransportDepot.Utilities.Csv;

namespace TransportDepot.Utilities
{
  [ServiceContract]
  public interface ICsvService
  {
    [OperationContract]
    IEnumerable<string[]> ReadCsv(Stream csv);

    [OperationContract]
    IEnumerable<string[]> ReadCsvRequest(CsvRequest request);


    [OperationContract(Name = "ReadCsvFromPath")]
    IEnumerable<string[]> ReadCsv(string path);

    [OperationContract(Name = "ReadCsvFromUri")]
    [WebInvoke(Method = "POST",
      UriTemplate = "/ReadCsv?fileName={fileName}")]
    IEnumerable<string[]> ReadCsvFromUri(string fileName, Stream stream);
  }
}
