
using TransportDepot.Models.Dispatch;
using System.Collections.Generic;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;
namespace TransportDepot.Dispatch
{
  [ServiceContract]
  public interface IDispatchService
  {
    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<DispatcherCommission> GetUnpaidCommissions();
    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<DateTime> GetAllCommissionDates();
    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<DateTime> GetCommissionDates(string dispatcherId);
    [OperationContract]
    [WebInvoke(Method="POST")]
    IEnumerable<DispatcherCommission> GetCommissions(DispatcherCommissionDate request);
    [OperationContract]
    [WebInvoke(Method = "POST")]
    IEnumerable<Dispatcher> GetDispatchers();

  }
}
