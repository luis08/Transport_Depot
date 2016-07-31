using System;
using System.ServiceModel;
using System.ServiceModel.Web;
namespace TransportDepot.Utilities
{
  [ServiceContract]
  interface IWorkarounds
  {
    [OperationContract]
    [WebGet]
    bool IsReadyToOpen();
    [OperationContract]
    [WebGet]
    bool IsReadyToWork();
    [OperationContract]
    [WebInvoke(Method="POST")]
    void PrepareToOpen();
    [OperationContract]
    [WebInvoke(Method = "POST")]
    void PrepareToWork();
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate="/driver?driverId={driverId}&firstName={firstName}")]
    void AppendDriver(string driverId, string firstName);
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "/tractor?id={id}")]
    void AppendTractor(string id);
  }
}
