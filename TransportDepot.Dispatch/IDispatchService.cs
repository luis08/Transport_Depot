
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
    [OperationContract]
    [WebInvoke(Method = "GET", ResponseFormat= WebMessageFormat.Json, BodyStyle=WebMessageBodyStyle.Bare)]
    IEnumerable<MovingFreightTrip> GetMovingFreight();
    [OperationContract]
    [WebInvoke(Method = "POST")]
    void SendCompanySetup(CompanySetupRequest request);
    [OperationContract]
    [WebGet(UriTemplate = "driver-contacts")]
    IEnumerable<DriverContact> GetDriverContacts();
    [OperationContract]
    [WebGet(UriTemplate = "dispatch-queues")]
    IEnumerable<DispatchQueue> GetQueues();
    [OperationContract]
    [WebInvoke(UriTemplate = "dispatch-queues", Method = "POST")]
    void CreateQueue(DispatchQueue queue);
    [OperationContract]
    [WebInvoke(UriTemplate = "dispatch-queues", Method = "PUT")]
    void UpdateQueue(DispatchQueue queue);
    [OperationContract]
    [WebInvoke(UriTemplate = "dispatch-queues", Method = "DELETE")]
    void DeleteQueue(int id);
    [OperationContract]
    [WebGet(UriTemplate = "tractors-queued")]
    IEnumerable<QueuedTractor> GetQueuedTractors();
    [OperationContract]
    [WebInvoke(UriTemplate = "tractors-queued", Method = "POST")]
    void Queue(int queueId, IEnumerable<string> tractorIds);
    [OperationContract]
    [WebInvoke(UriTemplate = "tractors-queued", Method = "DELETE")]
    void DeQueue(IEnumerable<int> queuedTractorIds);
    [OperationContract]
    void UpdateQueuedTractor(QueuedTractor queuedTractor);
    [OperationContract]

    [WebInvoke(UriTemplate = "tractors-booked", Method = "POST")]
    void Book(string tractorId, string customerId);
    [OperationContract]
    [WebInvoke(UriTemplate = "tractors-booked", Method = "DELETE")]
    void DeleteBooking(IEnumerable<int> bookingIds);
  }
}
