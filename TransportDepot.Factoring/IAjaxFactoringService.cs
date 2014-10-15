using System.ServiceModel;
using System.ServiceModel.Web;
using TransportDepot.Models.Factoring;
using System.IO;

namespace TransportDepot.Factoring
{
  [ServiceContract(Namespace = "http://transportdepot.net/factoring")]
  interface IAjaxFactoringService
  {
    [OperationContract]
    [WebInvoke(Method="POST")]
    ScheduleViewModel GetSchedule(int id);

    [OperationContract]
    [WebInvoke(Method="POST")]
    void SaveSchedule(ScheduleViewModel schedule);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    int CreateSchedule(NewScheduleViewModel schedule);

    [OperationContract]
    [WebInvoke(Method = "POST")]
    InvoiceSet GetInvoices(InvoiceFilter filter);
  }
}
