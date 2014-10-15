using System.ServiceModel;
using TransportDepot.Models.Factoring;

namespace TransportDepot.Factoring
{
  [ServiceContract(Namespace="http://transportdepot.net/factoring", Name="FactoringService")]
  interface IFactoringService
  {
    [OperationContract]
    ScheduleSetViewModel GetSchedules(ScheduleFilter filter);

    [OperationContract]
    InvoiceSet GetUnpostedInvoices(InvoiceFilter filter);

    [OperationContract]
    void DeleteSchedule(int id);

    [OperationContract(Name = "SoapSaveSchedule")]
    void SaveSchedule(ScheduleViewModel schedule);

    [OperationContract(Name = "SoapCreateSchedule")]
    int CreateSchedule(NewScheduleViewModel schedule);

    [OperationContract(Name="SoapGetSchedule")]
    ScheduleViewModel GetSchedule(int id);
  }
}
