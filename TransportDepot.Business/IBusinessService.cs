using System.ServiceModel;
using TransportDepot.Models.Business;
using System.ServiceModel.Web;

namespace TransportDepot.Business
{
  [ServiceContract(Namespace = "http://transportdepot.net/reports/business")]
  interface IBusinessService
  {
    [OperationContract]
    [WebInvoke(Method="POST", ResponseFormat=WebMessageFormat.Json)]
    Company GetCompany();
    
  }
}
